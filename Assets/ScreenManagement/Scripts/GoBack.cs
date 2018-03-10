
using UnityEngine;

namespace XLib.ScreenMgmt
{
   public class GoBack : MonoBehaviour
   {  [SerializeField]
      bool _goBackOnEnable = false;

      [SerializeField]
      int _count = 0;


      [SerializeField]
      [SelectScreenTransition]
      string _transition = null;
      ScreenManager _screenManager;

      virtual protected void OnEnable()
      {  _screenManager = SingletonUnityObject<ScreenManager>.Instance;
         if (_goBackOnEnable)
            DoGoBack();
      }

      public void DoGoBack() { _screenManager.GoBack(_count, null, _transition); }
   }
}