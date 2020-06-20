
using UnityEngine;

using UnityEngine.Assertions;

namespace XLib.ViewMgmt
{
   public class GoBack : MonoBehaviour
   {
      static readonly private string VIEW_MANAGER_NULL_EXCEPTION = "_viewManager not set to an instance of a VIewManager object";

      [SerializeField]
      bool _goBackOnEnable = false;

      [SerializeField]
      int _count = 0;


      [SerializeField]
      [SelectViewTransition]
      string _transition = default;

      [SerializeField]
      ViewManager _viewManager = default;

      virtual protected void OnEnable()
      {  if (_goBackOnEnable)
            DoGoBack();
      }

      public void DoGoBack()
      {
         Assert.IsTrue(_viewManager, VIEW_MANAGER_NULL_EXCEPTION);
         Debug.Assert(_viewManager != null, VIEW_MANAGER_NULL_EXCEPTION);
         System.Diagnostics.Debug.Assert(_viewManager != null, VIEW_MANAGER_NULL_EXCEPTION);

         _viewManager.GoBack(_count, null, _transition);
      }
   }
}