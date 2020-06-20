
using System;
using System.Collections;
using UnityEngine;

namespace XLib.ViewMgmt
{
   using Transitions;
   using Util;

   abstract public class BaseGameView : MonoBehaviour, IGameView
   {  abstract public bool CanBeCached { get ;  }
      abstract public string ViewName { get ; }

      protected TransitionIntro[] TransitionsIn;
      protected TransitionOutro[] TransitionsOut;

      virtual protected void OnEnable()
      {  TransitionsIn = GetComponents<TransitionIntro>();
         TransitionsOut = GetComponents<TransitionOutro>();
      }

      virtual public IEnumerable DidAppear(StringKeyDictionary pInfo) { yield break; }

      virtual public IEnumerable DidDisappear(StringKeyDictionary pInfo) { yield break; }

      public TransitionIntro TransitionIntro(string pName, StringKeyDictionary pInfo, bool pSkip = false)
      {  if (!String.IsNullOrEmpty(pName))
            foreach (var trans in TransitionsIn)
               if (trans.TransitionName == pName)
                  return trans;

         return null;
      }

      public TransitionOutro TransitionOutro(string pName, StringKeyDictionary pInfo, bool pSkip = false)
      {  if (!String.IsNullOrEmpty(pName))
            foreach (var trans in TransitionsOut)
               if (trans.TransitionName == pName)
                  return trans;

         return null;
      }

      virtual public IEnumerable WillAppear(StringKeyDictionary pInfo) { yield break; }

      virtual public IEnumerable WillDisappear(StringKeyDictionary pInfo) { yield break; }
   }
}