
using System.Collections;
using UnityEngine;

namespace XLib.ViewMgmt.Transitions
{
   using Util;

   public class InTransitionAnimClip : BaseTransitionAnimClip, InTransition
   {  [SerializeField]
      [SelectAnimClip]
      string _idleClip = null;

      [SerializeField]
      bool _beginsInTheBackround = false;

      public string IdleClip { get { return _idleClip; } }

      public bool BeginsInTheBackground  {  get  { return _beginsInTheBackround; } }

      override public IEnumerator DoTransition(StringKeyDictionary pInfo)
      {  IEnumerator doTransition = base.DoTransition(pInfo);
         while(doTransition.MoveNext())
            yield return doTransition.Current;


         TransitionAnimator.Play(_idleClip, -1, 0.0f);
         TransitionAnimator.speed = 0.0f;
         yield return null;
      }
   }
}
