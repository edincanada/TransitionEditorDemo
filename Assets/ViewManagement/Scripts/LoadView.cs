using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ViewMgmt
{
   using Util;
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
      ViewManager _viewManager = default;

      virtual protected void OnEnable ( )
      {  if (_loadOnEnable)
		      DoLoad();
      }

      protected void DoLoadImpl(StringKeyDictionary pDict, bool pWaitIfBusy)
      {  //Load the view from the manager
         _viewManager.LoadView(_viewName, pDict, _transitionName, pWaitIfBusy, _replaceCurrent);
      }

      virtual public void DoLoad ( ) { DoLoadImpl(null, false); }
   }
}