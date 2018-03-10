using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ScreenMgmt
{
   public class LoadScreen : MonoBehaviour
   {  [SerializeField]
      bool _loadOnEnable = false;

      [SerializeField]
      string _screenName = null;

      [SerializeField]
      [SelectScreenTransition]
      string _transitionName = null;

      [SerializeField]
      bool _replaceCurrent = false;

      ScreenManager _screenManager;

      virtual protected void OnEnable ( )
      {  _screenManager = SingletonUnityObject<ScreenManager>.Instance;
	      if (_loadOnEnable)
		      DoLoad();
      }

      public void DoLoad ( )
      {  //Load the screen from the manager
	      _screenManager.LoadScreen(_screenName, null, _transitionName, false, _replaceCurrent);
      }
   }
}