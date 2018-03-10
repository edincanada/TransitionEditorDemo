
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ScreenMgmt.Transitions
{
   public class OutTransitionAnimClip : Transition , OutTransition
   {  Animator _animator;

      [SerializeField]
      string _transitionName = null;

      [SerializeField]
      [SelectAnimClip]
      string _animClip = null;

      override public string TransitionName { get { return _transitionName; } }

      virtual protected void OnEnable() { _animator = GetComponent<Animator>(); }

      override public IEnumerator DoTransition(StringKeyDictionary pInfo)
      {  _animator.speed = 1f;
         _animator.Play(_animClip, -1, 0.0f);

         while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
      }

      override public void Rewind()
      {  _animator.Play(_animClip, -1, 0.0f);
         _animator.Update(0f);
         _animator.speed = 0;
      }
   }
}
