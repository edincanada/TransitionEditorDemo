
using System.Collections;
using UnityEngine;

namespace XLib.ScreenMgmt.Transitions
{
   public class InTransitionAnimClip : Transition, InTransition
   {  Animator _animator;

      [SerializeField]
      string _transitionName = null;

      [SerializeField]
      [SelectAnimClip]
      string _animClip = null;

      [SerializeField]
      [SelectAnimClip]
      string _idleClip = null;

      [SerializeField]
      bool _beginsInTheBackround = false;

      override public string TransitionName { get { return _transitionName ; } }

      public bool BeginsInTheBackground  {  get  { return _beginsInTheBackround; } }

      virtual protected void OnEnable() { _animator = GetComponent<Animator>(); }

      override public IEnumerator DoTransition(StringKeyDictionary pInfo)
      {  _animator.speed = 1.0f;
         _animator.Play(_animClip, -1, 0.0f);
         while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && !_animator.IsInTransition(0))
            yield return null;

         _animator.Play(_idleClip, -1, 0.0f);
	      _animator.speed = 0.0f;
         yield return null;
      }
   }
}
