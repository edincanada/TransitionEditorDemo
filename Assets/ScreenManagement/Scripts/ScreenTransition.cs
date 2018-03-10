
using UnityEngine;

namespace XLib.ScreenMgmt.Transitions
{

   public class ScreenTransition : MonoBehaviour, IScreenTransition
   {  public bool Simultaneous { get { return _simultaneous; } }

      [SerializeField]
      private bool _simultaneous = false;

      [SerializeField]
      private string _transitionName = null;

      [SerializeField]
      private string _transitionInName = null;

      [SerializeField]
      private string _transitionOutName = null;

      public string TransitionInName { get { return _transitionInName; } }

      public string TransitionOutName { get { return _transitionOutName; } }

      public string TransitionName { get { return _transitionName; } }

   }
}