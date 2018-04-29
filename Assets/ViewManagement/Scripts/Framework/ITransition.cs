
using System.Collections;

namespace XLib.ViewMgmt.Transitions
{
   using Util;

   public interface ITransition : IEnumerable
   {  string TransitionName { get; }
      IEnumerator DoTransition(StringKeyDictionary pInfo);
      void Rewind();
      IEnumerator GetEnumerator(StringKeyDictionary pInfo);
   }

   public interface InTransition : ITransition { bool BeginsInTheBackground { get; } }
   public interface OutTransition : ITransition  { }
}
