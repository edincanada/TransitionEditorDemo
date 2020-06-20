
using System.Collections;
using UnityEngine;

using UnityEngine.Assertions;

namespace XLib.ViewMgmt.Transitions
{
   using Util;
   using Exception = System.Exception;

   public abstract class BaseTransitionAnimClip : Transition
   {  static readonly private string NO_ANIMATOR_ERROR_FORMAT = "Game object '{0}' uses a transition anim clip but does not have an animator";
      static readonly private string ANIMATOR_MISSING_EXCEPTION_FORMAT = "Transition failed. Game object '{0}' has no animator";
      Animator _animator;

      [SerializeField]
      string _transitionName = null;

      [SerializeField]
      [SelectAnimClip]
      string _animClip = null;

      public Animator TransitionAnimator { get { return _animator; } }

      public string AnimClip { get { return _animClip; } }

      override public string TransitionName { get { return _transitionName; } }

      virtual protected void OnEnable()
      {  _animator = GetComponent<Animator>();
         Assert.IsNotNull(_animator, System.String.Format(NO_ANIMATOR_ERROR_FORMAT, gameObject.name));
         Debug.Assert(_animator != null, System.String.Format(NO_ANIMATOR_ERROR_FORMAT, gameObject.name));
         System.Diagnostics.Debug.Assert(_animator != null, System.String.Format(NO_ANIMATOR_ERROR_FORMAT, gameObject.name));
      }

      override public IEnumerator DoTransition (StringKeyDictionary pInfo)
      {  Assert.IsNotNull(_animator, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));
         Debug.Assert(_animator != null, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));
         System.Diagnostics.Debug.Assert(_animator != null, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));

         _animator.speed = 1f;
         _animator.Play(_animClip, -1, 0.0f);

         while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return null;
      }

      override public void Rewind ( )
      {  Assert.IsNotNull(_animator, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));
         Debug.Assert(_animator != null, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));
         System.Diagnostics.Debug.Assert(_animator != null, System.String.Format(ANIMATOR_MISSING_EXCEPTION_FORMAT, gameObject.name));
         _animator.Play(_animClip, -1, 0.0f);
         _animator.Update(0f);
         _animator.speed = 0;
      }

      public void moveViewToBackground() { gameObject.transform.SetAsFirstSibling(); }
   }
}
