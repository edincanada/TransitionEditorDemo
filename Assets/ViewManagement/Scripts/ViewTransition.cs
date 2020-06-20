
using UnityEngine;

namespace XLib.ViewMgmt.Transitions
{
   public class ViewTransition : MonoBehaviour, IViewTransition
   {  public bool Simultaneous { get { return _simultaneous; } }

      [SerializeField]
      private bool _simultaneous = default;

      [SerializeField]
      private string _transitionName = default;

      [SerializeField]
      private string _transitionInName = default;

      [SerializeField]
      private string _transitionOutName = default;

      public string TransitionInName { get { return _transitionInName; } }

      public string TransitionOutName { get { return _transitionOutName; } }

      public string TransitionName { get { return _transitionName; } }
   }
}