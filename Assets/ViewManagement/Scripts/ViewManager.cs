
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ViewMgmt
{
   using Transitions;
   using Util;
   using Exception = System.Exception;

   class ViewCache : Dictionary<string, IList<IGameView>> , ISingleton
   {  static readonly private string ALREADY_CACHED_WARNING_FORMAT = "View for caching: {0} is already cached";
      static readonly private string CANT_CACHE_NULL_VIEW_ERROR = "Null View can't be cached";

      private void _AddOrSetIfNull(string pViewName, List<IGameView> pCacheList)
      {  // If the key does not exist or the value for this key is null,
         // Set/Add the value key pair.

         if (!ContainsKey(pViewName))
            Add(pViewName, pCacheList);
         else if (this[pViewName] == null)
            this[pViewName] = pCacheList;
      }

      public void Add (IGameView pView)
	   {  try { DebugUtil.LogAssert(pView != null, CANT_CACHE_NULL_VIEW_ERROR); }
         catch (Exception e) { throw new Exception(e.Message); }

         _AddOrSetIfNull(pView.ViewName, new List<IGameView>());
            //If the view is already cached. Warning. Else, cache it.
			if (this[pView.ViewName].Contains(pView))
            Debug.LogWarning(String.Format(ALREADY_CACHED_WARNING_FORMAT, pView.ViewName));
			else
			   this[pView.ViewName].Insert(0, pView);
	   }
   }

   public class GameViewDictionary : Dictionary<string, IGameView> { }

   public class ViewManager : MonoBehaviour
   {  static readonly private string NO_VIEW_EXCEPTION = "No prefab or cached view available for: " ;
      static readonly private string NO_VIEW_TO_DISPLAY_WARNING = "There are no views left to display";
      static readonly private string NOT_ENOUGH_VIEWS_TO_GO_BACK_ERROR_FORMAT = "Failed to go back {0} views. There aren't enough views in the stack";
      static readonly private string INTRANSITION_NOT_FOUND_WARNING_FORMAT = "InTransition {0} not found";
      static readonly private string OUTTRANSITION_NOT_FOUND_WARNING_FORMAT = "OutTransition {0} not found";
      static readonly private int VIEW_STACK_DEFAULT_START_SIZE = 10;
      static readonly private int TRANSITION_LIST_DEFAULT_START_SIZE = 50;
      private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

      GameObjectDictionary _viewPrefabs = new GameObjectDictionary();
      ViewCache _viewCache = new ViewCache();
      List<IGameView> _viewStack = new List<IGameView>(VIEW_STACK_DEFAULT_START_SIZE);

      [SerializeField] GameObject _activeViewStackGameObject = null;
      [SerializeField] GameObject _cacheGameObject = null;

      Dictionary<string, IViewTransition> _viewTransitions = new Dictionary<string, IViewTransition>(TRANSITION_LIST_DEFAULT_START_SIZE);
      bool _initialized = false;
      CoroutineLock _coroutineLock = new CoroutineLock();

      private WaitForEndOfFrame WaitForEndOfFrameInstance
	   {  get
		   {  if (_waitForEndOfFrame == null )
			      _waitForEndOfFrame = new WaitForEndOfFrame();

            return _waitForEndOfFrame;
		   }
	   }

      private void _InitializeTransitionsDictionary()
      {  IViewTransition[] transitions = this.GetComponents<IViewTransition>();

         if (_viewTransitions == null)
            _viewTransitions = new Dictionary<string, IViewTransition>(transitions.Length);

         _viewTransitions.Clear();

         foreach (var transition in transitions)
            _viewTransitions.Add(transition.TransitionName, transition);
      }

      private void _InitializeViewCache()
      {  if (_viewCache == null)
            _viewCache = new ViewCache();

         _viewCache.Clear();

         Transform cacheTransform = _cacheGameObject.transform;
         int cacheCount = cacheTransform.childCount;
         for (int ii = 0; ii < cacheCount; ii++)
         {
            GameObject cachedViewGameObject = cacheTransform.GetChild(ii).gameObject;
            IGameView view = cachedViewGameObject.GetComponent<IGameView>();

            if (view != null)
               _viewCache.Add(view);
         }
      }

      virtual protected void OnEnable()
      {  if (_viewPrefabs == null)
            _viewPrefabs = new GameObjectDictionary();

         _viewPrefabs.Clear();

         if (_viewStack == null)
            _viewStack = new List<IGameView>(VIEW_STACK_DEFAULT_START_SIZE);

         _viewStack.Clear();
	      _InitializeViewCache();

         if (_activeViewStackGameObject != null)
         {  while (_activeViewStackGameObject.transform.childCount > 0 )
            {  //Disable the children of the view stack object and move them under the cache game object
               Transform currentChild = _activeViewStackGameObject.transform.GetChild(0);

               if (currentChild.gameObject.activeSelf)
                  currentChild.gameObject.SetActive(false);

               currentChild.SetParent(_cacheGameObject.transform, false);
			      //add it to the cache dictionary
			      _viewCache.Add(currentChild.gameObject.GetComponent<IGameView>());
            }
         }

         _InitializeTransitionsDictionary();
         _initialized = true;
      }

      IViewTransition _GetTransition(string pName)
      {  if (!String.IsNullOrEmpty(pName) && _viewTransitions != null && _viewTransitions.ContainsKey(pName))
            return _viewTransitions[pName];

         return null;
      }

      IGameView _GetByIndexOrNull(int pIndex , List<IGameView> pList)
      {  if (pIndex < pList.Count)
            return pList[pIndex];

         return null;
      }

      GameObject _GetCachedOrCreateNew(string pView)
      {  IList<IGameView> cachedViews;
         IGameView cachedView = null;
         GameObject cachedViewGameObject = null ;
         if (!String.IsNullOrEmpty(pView) && _viewCache.ContainsKey(pView))
         {  cachedViews = _viewCache[pView];
            if (cachedViews != null && cachedViews.Count > 0)
            {  cachedView = cachedViews[0];
               cachedViewGameObject = cachedView.gameObject;
               cachedViews.RemoveAt(0);
            }
         }

         if (cachedViewGameObject == null &&
               !String.IsNullOrEmpty(pView) &&
               _viewPrefabs.ContainsKey(pView) &&
               _viewPrefabs[pView] != null)
         {
            cachedViewGameObject = GameObject.Instantiate<GameObject>(_viewPrefabs[pView]);
         }

         return cachedViewGameObject;
      }

      public IGameView CurrentView  { get { return _GetByIndexOrNull(0, _viewStack); } }

      public void LoadView(string pViewName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false, bool pReplaceCurrent = false)
      {  //Call load view as a coroutine
         StartCoroutine(_LoadView(pViewName, pInfo, pTransitionName, pWaitIfBusy, pReplaceCurrent).GetEnumerator());
      }

	   IEnumerable _RunTransitions (IGameView pLeaving, IGameView pEntering, IViewTransition pTransition, StringKeyDictionary pInfo)
      {
	      if (pTransition != null)
         {  //get transitions.
            ITransition transitionIn = null;
            ITransition transitionOut = null;

            if (pEntering != null)
               transitionIn = pEntering.TransitionIn(pTransition.TransitionInName, pInfo);

            if (pLeaving != null)
               transitionOut = pLeaving.TransitionOut(pTransition.TransitionOutName, pInfo);

            //run transitions.
            if (pTransition.Simultaneous && transitionIn != null && transitionOut != null)
            {  //Run both at the same time
               var startOut = StartCoroutine(transitionOut.GetEnumerator(pInfo));
               var startIn = StartCoroutine(transitionIn.GetEnumerator(pInfo));

               yield return startOut;
               yield return startIn;
            }
            else
            {  if (transitionOut != null)
               {  var enumeratorOut = transitionOut.GetEnumerator(pInfo);
                  if (enumeratorOut != null)
                     while (enumeratorOut.MoveNext())
                        yield return enumeratorOut.Current;
               }
               else if (!String.IsNullOrEmpty(pTransition.TransitionInName))
               {  //Log a warning
                  Debug.LogWarningFormat(INTRANSITION_NOT_FOUND_WARNING_FORMAT, pTransition.TransitionInName);
               }

               if (transitionIn != null)
               {  var enumeratorIn = transitionIn.GetEnumerator(pInfo);
                  if (enumeratorIn != null)
                     while (enumeratorIn.MoveNext())
                        yield return enumeratorIn.Current;
               }
               else if (!String.IsNullOrEmpty(pTransition.TransitionOutName))
               {  //Log a warning
                  Debug.LogWarningFormat(OUTTRANSITION_NOT_FOUND_WARNING_FORMAT, pTransition.TransitionInName);
               }
            }
         }
      }

      IEnumerable _LoadView(string pViewName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false, bool pReplaceCurrent = false)
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
            IGameView current = CurrentView;

            //get reference to next.
            GameObject nextViewGameObject = _GetCachedOrCreateNew(pViewName);

            try { DebugUtil.LogAssert(this, nextViewGameObject != null, NO_VIEW_EXCEPTION + pViewName); }
            catch (Exception e) { throw new Exception(e.Message); }

            IGameView next = nextViewGameObject.GetComponent<IGameView>();

            //Enable next.
            nextViewGameObject.transform.SetParent(_activeViewStackGameObject.transform, false);
            nextViewGameObject.SetActive(true);

            IViewTransition transition = this._GetTransition(pTransitionName);
            InTransition inTransition = null;

            if (transition != null)
               inTransition = next.TransitionIn(transition.TransitionInName);

            //Check if the new view must come from the background
            if (inTransition != null && inTransition.BeginsInTheBackground)
               nextViewGameObject.transform.SetAsFirstSibling();

            //run willDisappear on current.
            if (current != null)
            {  //Run WillDisappear handlers
               IWillDisappear[] disappearHandlers = current.gameObject.GetComponentsInChildren<IWillDisappear>(false);
               foreach (var handler in disappearHandlers)
                  handler.WillDisappear(pInfo);

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
            IDidAppear[] appearHandlers = nextViewGameObject.GetComponentsInChildren<IDidAppear>(false);
            foreach (var handler in appearHandlers)
               handler.DidAppear(pInfo);

            //run DidAppear on next.
            foreach (var it in next.DidAppear(pInfo))
               yield return it;

            //run DidDisappear on current.
            if (current != null)
            {  foreach (var it in current.DidDisappear(pInfo))
                  yield return it;

               //Rewind the out transition of the view that disappeared
               if (current.TransitionOut(transition.TransitionOutName) != null)
                  current.TransitionOut(transition.TransitionOutName).Rewind();
            }

            //disable current.
            if (current != null)
            {  current.gameObject.SetActive(false);

               if (pReplaceCurrent)
               {  _cacheView(current, true);
                  current = null;
               }
            }
            //add next to the top of the view stack.
            nextViewGameObject.transform.SetAsLastSibling();
            _viewStack.Insert(0, next);
         }
      }

      private void _cacheView(IGameView pView, bool pRemoveFromCacheList)
      {  pView.gameObject.transform.SetParent(_cacheGameObject.transform, false);
         _viewCache.Add(pView);
         if (pRemoveFromCacheList)
            _viewStack.Remove(pView);
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

            try { DebugUtil.LogAssertFormat(this, pGoBack <= _viewStack.Count, NOT_ENOUGH_VIEWS_TO_GO_BACK_ERROR_FORMAT, pGoBack); }
            catch (Exception e) { throw new Exception(e.Message); }

            //get rid of views in the middle.
            for (int ii = 1; ii < pGoBack; ii++)
            {  //add it to the cache dictionary.
               //do not remove it from the view stack yet. It's done in bulk
               if (_viewStack[ii].CanBeCached)
               {  //Cache it
                  _cacheView(_viewStack[ii], false);
               }
               else
               {  GameObject.Destroy(_viewStack[ii].gameObject);
                  _viewStack[ii] = null;
               }
            }

            //Bulk remove the views from the stack
            if (pGoBack > 1)
               _viewStack.RemoveRange(1, pGoBack - 1);

            //Warning if the stack will be left empty
            if (_viewStack.Count == 1)
               Debug.LogWarning(NO_VIEW_TO_DISPLAY_WARNING);

            IGameView current = CurrentView;
            IGameView next = _GetByIndexOrNull(1, _viewStack);

            IViewTransition transition = this._GetTransition(pTransitionName);
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
                  handler.WillDisappear(pInfo);

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
                  handler.DidAppear(pInfo);

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
            {  GameObject currentViewGameObject = current.gameObject;
               currentViewGameObject.SetActive(false);

               if (current.CanBeCached)
               {  //add it to the cache dictionary.
                  _cacheView(current, true);
               }
               else
               {  GameObject.Destroy(currentViewGameObject);
                  _viewStack.RemoveAt(0);
                  current = null;
                  currentViewGameObject = null;
               }
            }

         }
      }

	   public void GoBack (int pGoBack = 1, StringKeyDictionary pInfo = null , string pTransitionName = null, bool pWaitIfBusy = false)
      {  //Call go back as a coroutine
	      StartCoroutine(_GoBack(pGoBack, pInfo, pTransitionName, pWaitIfBusy).GetEnumerator());
      }

      public void GoBackTo (string pViewName, StringKeyDictionary pInfo = null, string pTransitionName = null, bool pWaitIfBusy = false)
      {  bool found = false;
         int viewStackLength = _viewStack.Count;
         int viewCount = 0;

         while (!found && viewCount < viewStackLength)
         {  if (_viewStack[viewCount].ViewName == pViewName)
               found = true;
            else
               viewCount++;
         }

         GoBack(viewCount, pInfo, pTransitionName, pWaitIfBusy);
      }
   }
}
