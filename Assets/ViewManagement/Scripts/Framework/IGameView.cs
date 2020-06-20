
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ViewMgmt
{
   using Transitions;
   using Util;

   public interface IGameView
   {  string ViewName { get; }
      bool CanBeCached { get; }

      IEnumerable WillAppear(StringKeyDictionary pInfo);
      TransitionIntro TransitionIntro(string pName , StringKeyDictionary pInfo = null , bool pSkip = false );
      IEnumerable DidAppear(StringKeyDictionary pInfo);

      IEnumerable WillDisappear(StringKeyDictionary pInfo);
      TransitionOutro TransitionOutro(string pName , StringKeyDictionary pInfo = null , bool pSkip = false );
      IEnumerable DidDisappear(StringKeyDictionary pInfo);

      GameObject gameObject { get; }
   }
}