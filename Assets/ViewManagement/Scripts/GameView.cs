
using UnityEngine;

namespace XLib.ViewMgmt
{
   public class GameView : BaseGameView
   {  [SerializeField]
      bool _canBeCached = false;

      [SerializeField]
      string _viewName = null;

      override public bool CanBeCached { get { return _canBeCached; } }

      override public string ViewName { get { return _viewName; } }
   }
}