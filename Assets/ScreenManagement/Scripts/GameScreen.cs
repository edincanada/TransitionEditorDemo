
using UnityEngine;

namespace XLib.ScreenMgmt
{
   public class GameScreen : BaseGameScreen
   {  [SerializeField]
      bool _canBeCached = false;

      [SerializeField]
      string _screenName = null;

      override public bool CanBeCached { get { return _canBeCached; } }

      override public string ScreenName { get { return _screenName; } }
   }
}