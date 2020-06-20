using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.Demo
{
   using ViewMgmt.Transitions;
   using Util;
   using UnityUI = UnityEngine.UI;
   public class BorrowBackgroundInTransitionAnimClip : InTransitionAnimClip
   {  [SerializeField]
      UnityUI.Image _backgroundComponent = default;

      override public IEnumerator DoTransition(StringKeyDictionary pInfo)
      {  _backgroundComponent.sprite = pInfo[LoadOptionsView.BACKGROUND_SPRITE_KEY] as Sprite;
         return base.DoTransition(pInfo);
      }
   }
}