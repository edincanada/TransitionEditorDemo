
using System.Collections;
using UnityEngine;

namespace XLib.ScreenMgmt.Transitions
{
   abstract public class Transition : MonoBehaviour, IEnumerable
   {  abstract public string TransitionName { get; }

      virtual public IEnumerator DoTransition(StringKeyDictionary pInfo) { yield break; }

      virtual public void Rewind() { }

      public IEnumerator GetEnumerator() { return DoTransition(null); }

      public IEnumerator GetEnumerator(StringKeyDictionary pInfo) { return DoTransition(pInfo); }
   }
}
