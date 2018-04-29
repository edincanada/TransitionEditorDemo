
using UnityEngine;

namespace XLib.ViewMgmt
{
   public class GoBack : MonoBehaviour
   {  [SerializeField]
      bool _goBackOnEnable = false;

      [SerializeField]
      int _count = 0;


      [SerializeField]
      [SelectViewTransition]
      string _transition = null;

      [SerializeField]
      ViewManager _viewManager;

      virtual protected void OnEnable()
      {  if (_goBackOnEnable)
            DoGoBack();
      }

      public void DoGoBack() { _viewManager.GoBack(_count, null, _transition); }
   }
}