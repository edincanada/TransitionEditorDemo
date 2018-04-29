using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ViewMgmt
{
   public class LoadView : MonoBehaviour
   {  [SerializeField]
      bool _loadOnEnable = false;

      [SerializeField]
      string _viewName = null;

      [SerializeField]
      [SelectViewTransition]
      string _transitionName = null;

      [SerializeField]
      bool _replaceCurrent = false;

      [SerializeField]
      ViewManager _viewManager;

      virtual protected void OnEnable ( )
      {  if (_loadOnEnable)
		      DoLoad();
      }

      public void DoLoad ( )
      {  //Load the view from the manager
	      _viewManager.LoadView(_viewName, null, _transitionName, false, _replaceCurrent);
      }
   }
}