
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.ScreenMgmt
{
   using Transitions;

   public class StringKeyDictionary : Dictionary<string, System.Object> { }

   public interface IGameScreen
   {  string ScreenName { get; }
      bool CanBeCached { get; }

      IEnumerable WillAppear(StringKeyDictionary pInfo);
      InTransition TransitionIn(string pName , StringKeyDictionary pInfo = null , bool pSkip = false );
      IEnumerable DidAppear(StringKeyDictionary pInfo);

      IEnumerable WillDisappear(StringKeyDictionary pInfo);
      OutTransition TransitionOut(string pName , StringKeyDictionary pInfo = null , bool pSkip = false );
      IEnumerable DidDisappear(StringKeyDictionary pInfo);

      GameObject gameObject { get; }
   }
}