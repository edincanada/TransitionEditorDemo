
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ScreenMgmt
{
   using Transitions;

   class ScreenCache : Dictionary<string, IList<IGameScreen>>
   {  static readonly private string ALREADY_CACHED_WARNING_FORMAT = "Screen for caching: {0} is already cached";
      static readonly private string CANT_CACHE_NULL_SCREEN_ERROR = "Null Screen can't be cached";

      private void _AddOrSetIfNull(string pScreenName, List<IGameScreen> pCacheList)
      {  // If the key does not exist or the value for this key is null,
         // Set/Add the value key pair.

         if (!ContainsKey(pScreenName))
            Add(pScreenName, pCacheList);
         else if (this[pScreenName] == null)
            this[pScreenName] = pCacheList;
      }

      public void Add (IGameScreen pScreen)
	   {  if (pScreen == null)
		   {  //error. cant cache null.
            throw new System.Exception(CANT_CACHE_NULL_SCREEN_ERROR);
		   }
		   else
		   {   _AddOrSetIfNull(pScreen.ScreenName, new List<IGameScreen>());

               //If the screen is already cached. Warning. Else, cache it.
			   if (this[pScreen.ScreenName].Contains(pScreen))
               Debug.LogWarning(String.Format(ALREADY_CACHED_WARNING_FORMAT, pScreen.ScreenName));
			   else
			      this[pScreen.ScreenName].Insert(0, pScreen);
         }
	   }
   }

   public class GameScreenDictionary : Dictionary<string, IGameScreen> { }

   public class GameObjectDictionary : Dictionary<string, GameObject> { }

   public class ScreenManager : MonoBehaviour
   {  static readonly private string NO_SCREEN_EXCEPTION = "No prefab or cached screen available for: " ;
      static readonly private string NO_SCREEN_TO_DISPLAY_WARNING = "There are no screens left to display";
      static readonly private string NOT_ENOUGH_SCREENS_TO_GO_BACK_ERROR_FORMAT = "Failed to go back {0} screens. There aren't enough screens in the stack";

      static readonly private int SCREEN_STACK_DEFAULT_START_SIZE = 10;
      static private WaitForEndOfFrame _waitForEndOfFrame;

      GameObjectDictionary _screenPrefabs;
      ScreenCache _screenCache;
      List<IGameScreen> _screenStack;

      [SerializeField] GameObject _activeScreenStackGameObject = null;
      [SerializeField] GameObject _cacheGameObject = null;

      Dictionary<string, IScreenTransition> _screenTransitions;
      bool _initialized = false;
      CoroutineLock _coroutineLock = new CoroutineLock();

      static private WaitForEndOfFrame WaitForEndOfFrameInstance
	   {  get
		   {  if (_waitForEndOfFrame == null )
			      _waitForEndOfFrame = new WaitForEndOfFrame();

            return _waitForEndOfFrame;
		   }
	   }

      private void _InitializeTransitionsDictionary()
      {  IScreenTransition[] transitions = this.GetComponents<IScreenTransition>();

         if (_screenTransitions == null)
            _screenTransitions = new Dictionary<string, IScreenTransition>(transitions.Length);

         _screenTransitions.Clear();

         foreach (var transition in transitions)
            _screenTransitions.Add(transition.TransitionName, transition);
      }

      private void _InitializeScreenCache()
      {  if (_screenCache == null)
            _screenCache = new ScreenCache();

         _screenCache.Clear();

         Transform cacheTransform = _cacheGameObject.transform;
         int cacheCount = cacheTransform.childCount;
         for (int ii = 0; ii < cacheCount; ii++)
         {  GameObject cachedScreenGameObject = cacheTransform.GetChild(ii).gameObject;
            IGameScreen screen = cachedScreenGameObject.GetComponent<IGameScreen>();

            if (screen != null)
               _screenCache.Add(screen);

         }
      }

      virtual protected void OnEnable()
      {  if (_screenPrefabs == null)
            _screenPrefabs = new GameObjectDictionary();

         _screenPrefabs.Clear();

         if (_screenStack == null)
            _screenStack = new List<IGameScreen>(SCREEN_STACK_DEFAULT_START_SIZE);

         _screenStack.Clear();
	      _InitializeScreenCache();

         if (_activeScreenStackGameObject != null)
         {  while (_activeScreenStackGameObject.transform.childCount > 0 )
            { //Disable the children of the screen stack object and move them under the cache game object
               Transform currentChild = _activeScreenStackGameObject.transform.GetChild(0);

               if (currentChild.gameObject.activeSelf)
                  currentChild.gameObject.SetActive(false);

               currentChild.SetParent(_cacheGameObject.transform, false);
			      //add it to the cache dictionary
			      _screenCache.Add(currentChild.gameObject.GetComponent<IGameScreen>());
            }
         }

         _InitializeTransitionsDictionary();
         _initialized = true;
      }

      IScreenTransition _GetTransition(string pName)
      {  if (!String.IsNullOrEmpty(pName) && _screenTransitions != null && _screenTransitions.ContainsKey(pName))
            return _screenTransitions[pName];

         return null;
      }

      IGameScreen _GetByIndexOrNull(int pIndex , List<IGameScreen> pList)
      {  if (pIndex < pList.Count)
            return pList[pIndex];

         return null;
      }

      GameObject _GetCachedOrCreateNew(string pScreen)
      {  IList<IGameScreen> cachedScreens;
         IGameScreen cachedScreen = null;
         GameObject cachedScreenGameObject = null ;
         if (!String.IsNullOrEmpty(pScreen) && _screenCache.ContainsKey(pScreen))
         {  cachedScreens = _screenCache[pScreen];
            if (cachedScreens != null && cachedScreens.Count > 0)
            {  cachedScreen = cachedScreens[0];
               cachedScreenGameObject = cachedScreen.gameObject;
               cachedScreens.RemoveAt(0);
            }
         }

         if (cachedScreenGameObject == null &&
               !String.IsNullOrEmpty(pScreen) &&
               _screenPrefabs.ContainsKey(pScreen) &&
               _screenPrefabs[pScreen] != null)
         {
            cachedScreenGameObject = GameObject.Instantiate<GameObject>(_screenPrefabs[pScreen]);
         }

         return cachedScreenGameObject;
      }

      public IGameScreen CurrentScreen  { get { return _GetByIndexOrNull(0, _screenStack); } }

      public void LoadScreen(string pScreenName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false, bool pReplaceCurrent = false)
      {  //Call load screen as a coroutine
         StartCoroutine(_LoadScreen(pScreenName, pInfo, pTransitionName, pWaitIfBusy, pReplaceCurrent).GetEnumerator());
      }

	   IEnumerable _RunTransitions (IGameScreen pLeaving, IGameScreen pEntering, IScreenTransition pTransition, StringKeyDictionary pInfo)
      {
	      if (pTransition != null)
         {  //get transitions.
            IEnumerable transitionIn = null;
            IEnumerable transitionOut = null;

            if (pEntering != null)
               transitionIn = pEntering.TransitionIn(pTransition.TransitionInName, pInfo);

            if (pLeaving != null)
               transitionOut = pLeaving.TransitionOut(pTransition.TransitionOutName, pInfo);

            //run transitions.
            if (pTransition.Simultaneous && transitionIn != null && transitionOut != null)
            {  //Run both at the same time
               var startOut = StartCoroutine(transitionOut.GetEnumerator());
               var startIn = StartCoroutine(transitionIn.GetEnumerator());

               yield return startOut;
               yield return startIn;
            }
            else
            {  //Run them in sequence
               if (transitionOut != null)
                  foreach (var it in transitionOut)
                     yield return it;

               if (transitionIn != null)
                  foreach (var it in transitionIn)
                     yield return it;
            }
         }
      }

      IEnumerable _LoadScreen(string pScreenName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false, bool pReplaceCurrent = false)
      {  while (!_initialized)
            yield return null;

         if (pWaitIfBusy)
         {  while (_coroutineLock.IsLocked)
               yield return null;
         }
         else if (_coroutineLock.IsLocked)
         {  //Exit. Don't wait
            yield break;
         }

         using (_coroutineLock.Lock())
         {  //keep reference to current.
            IGameScreen current = CurrentScreen;

            //get reference to next.
            GameObject nextScreenGameObject = _GetCachedOrCreateNew(pScreenName);

            if (nextScreenGameObject == null)
               throw new System.Exception(NO_SCREEN_EXCEPTION + pScreenName);

            IGameScreen next = nextScreenGameObject.GetComponent<IGameScreen>();

            //Enable next.
            nextScreenGameObject.transform.SetParent(_activeScreenStackGameObject.transform, false);
            nextScreenGameObject.SetActive(true);

            IScreenTransition transition = this._GetTransition(pTransitionName);
            InTransition inTransition = null;

            if (transition != null)
               inTransition = next.TransitionIn(transition.TransitionInName);

            //Check if the new screen must come from the background
            if (inTransition != null && inTransition.BeginsInTheBackground)
               nextScreenGameObject.transform.SetAsFirstSibling();

            //run willDisappear on current.
            if (current != null)
            {  //Run WillDisappear handlers
               IWillDisappear[] disappearHandlers = current.gameObject.GetComponentsInChildren<IWillDisappear>(false);
               foreach (var handler in disappearHandlers)
                  handler.WillDisappear();

               foreach (var it in current.WillDisappear(pInfo))
                  yield return it;
            }

            //run WillAppear on next.
            foreach (var it in next.WillAppear(pInfo))
               yield return it;

            //Run transitions
            foreach (var step in _RunTransitions(current, next, transition, pInfo))
               yield return step;

            //Run DidAppear handlers
            IDidAppear[] appearHandlers = nextScreenGameObject.GetComponentsInChildren<IDidAppear>(false);
            foreach (var handler in appearHandlers)
               handler.DidAppear();

            //run DidAppear on next.
            foreach (var it in next.DidAppear(pInfo))
               yield return it;

            //run DidDisappear on current.
            if (current != null)
            {  foreach (var it in current.DidDisappear(pInfo))
                  yield return it;

               //Rewind the out transition of the screen that disappeared
               if (current.TransitionOut(transition.TransitionOutName) != null)
                  current.TransitionOut(transition.TransitionOutName).Rewind();
            }

            //disable current.
            if (current != null)
            {  current.gameObject.SetActive(false);

               if (pReplaceCurrent)
               {  _cacheScreen(current, true);
                  current = null;
               }
            }
            //add next to the top of the screen stack.
            nextScreenGameObject.transform.SetAsLastSibling();
            _screenStack.Insert(0, next);
         }
      }

      private void _cacheScreen(IGameScreen pScreen, bool pRemoveFromCacheList)
      {  pScreen.gameObject.transform.SetParent(_cacheGameObject.transform, false);
         _screenCache.Add(pScreen);
         if (pRemoveFromCacheList)
            _screenStack.Remove(pScreen);
      }

      public IEnumerable _GoBack (int pGoBack = 1, StringKeyDictionary pInfo = null , string pTransitionName = null, bool pWaitIfBusy = false)
      {  while (!_initialized)
            yield return null;

         if (pWaitIfBusy)
         {  while (_coroutineLock.IsLocked)
               yield return null;
         }
         else if (_coroutineLock.IsLocked)
         { //Exit. Don't wait
            yield break;
         }

         using (_coroutineLock.Lock())
         {  if (pGoBack < 1)
            {  //Do Nothing. Exit
               yield break;
            }
            else if (pGoBack > _screenStack.Count)
            {  //Error if removing more screens than available.
               throw new System.Exception(NOT_ENOUGH_SCREENS_TO_GO_BACK_ERROR_FORMAT);
            }
            else
            {  //get rid of screens in the middle.
               for (int ii = 1; ii < pGoBack; ii++)
               {  //add it to the cache dictionary.
                  //do not remove it from the screen stack yet. It's done in bulk
                  if (_screenStack[ii].CanBeCached)
                  {  //Cache it
                     _cacheScreen(_screenStack[ii], false);
                  }
                  else
                  {  GameObject.Destroy(_screenStack[ii].gameObject);
                     _screenStack[ii] = null;
                  }
               }

               //Bulk remove the screens from the stack
               if (pGoBack > 1)
                  _screenStack.RemoveRange(1, pGoBack - 1);

               //Warning if the stack will be left empty
               if (_screenStack.Count == 1)
                  Debug.LogWarning(NO_SCREEN_TO_DISPLAY_WARNING);

               IGameScreen current = CurrentScreen;
               IGameScreen next = _GetByIndexOrNull(1, _screenStack);

               IScreenTransition transition = this._GetTransition(pTransitionName);
               if (next != null)
               {  next.gameObject.SetActive(true);
                  InTransition inTransition;
                  if (transition != null)
                  {  inTransition = next.TransitionIn(transition.TransitionInName);
                     if (inTransition != null && !inTransition.BeginsInTheBackground)
                        next.gameObject.transform.SetAsLastSibling();
                  }
               }

               if (current != null)
               {  IWillDisappear[] disappearHandlers = current.gameObject.GetComponentsInChildren<IWillDisappear>(false);

                  foreach (var handler in disappearHandlers)
                     handler.WillDisappear();

                  //current willDisappear.
                  foreach (var it in current.WillDisappear(pInfo))
                     yield return it;
               }

               //previous willAppear.
               if (next != null)
                  foreach (var it in next.WillAppear(pInfo))
                     yield return it;

               //run transitions
               foreach (var step in _RunTransitions(current, next, transition, pInfo))
                  yield return step;

               //previous didAppear
               if (next != null)
               {  IDidAppear[] appearHandlers = next.gameObject.GetComponentsInChildren<IDidAppear>(false);

                  foreach (var handler in appearHandlers)
                     handler.DidAppear();

                  foreach (var it in next.DidAppear(pInfo))
                     yield return it;
               }

               //current didDisappear
               if (current != null)
               {  foreach (var it in current.DidDisappear(pInfo))
                     yield return it;

                  if (current.TransitionOut(transition.TransitionOutName) != null)
                     current.TransitionOut(transition.TransitionOutName).Rewind();
               }

               //current goes to the cache.
               if (current != null)
               {  GameObject currentScreenGameObject = current.gameObject;
                  currentScreenGameObject.SetActive(false);

                  if (current.CanBeCached)
                  {  //add it to the cache dictionary.
                     _cacheScreen(current, true);
                  }
                  else
                  {  GameObject.Destroy(currentScreenGameObject);
                     _screenStack.RemoveAt(0);
                     current = null;
                     currentScreenGameObject = null;
                  }
               }
            }
         }
      }

	   public void GoBack (int pGoBack = 1, StringKeyDictionary pInfo = null , string pTransitionName = null, bool pWaitIfBusy = false)
      {  //Call go back as a coroutine
	      StartCoroutine(_GoBack(pGoBack, pInfo, pTransitionName, pWaitIfBusy).GetEnumerator());
      }

      public void GoBackTo (string pScreenName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false)
      {  bool found = false;
         int screenStackLength = _screenStack.Count;
         int screenCount = 0;

         while (!found && screenCount < screenStackLength)
         {  if (_screenStack[screenCount].ScreenName == pScreenName)
               found = true;
            else
               screenCount++;
         }

         GoBack(screenCount, pInfo, pTransitionName, pWaitIfBusy);
      }
   }
}
