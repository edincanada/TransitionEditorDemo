using UnityEngine;

namespace XLib.Util
{
   using UnityEngine.Assertions;
   using System.Diagnostics;
   static public class DebugUtil
   {  static DebugUtil()
      {
#if UNITY_ASSERTIONS
            Assert.raiseExceptions = true;
#endif
      }


      [Conditional("UNITY_ASSERTIONS"),
       Conditional("UNITY_EDITOR"),
       Conditional("DEBUG"),
       Conditional("DEVELOPMENT_BUILD")]
      static public void LogAssertFormat(UnityEngine.Object pContext, bool pCondition, string pMessage, params System.Object[] pArgs)
      {  //Format and then call LogAssert
         LogAssert(pContext, pCondition, System.String.Format(pMessage, pArgs));
      }

      [Conditional("UNITY_ASSERTIONS"),
       Conditional("UNITY_EDITOR"),
       Conditional("DEBUG"),
       Conditional("DEVELOPMENT_BUILD")]
      static public void LogAssertFormat(bool pCondition, string pMessage, params System.Object[] pArgs)
      {  //Format and then call LogAssert
         LogAssert(null, pCondition, System.String.Format(pMessage, pArgs));
      }

      [Conditional("UNITY_ASSERTIONS"),
       Conditional("UNITY_EDITOR"),
       Conditional("DEBUG"),
       Conditional("DEVELOPMENT_BUILD")]
      static public void LogAssert(bool pCondition, string pMessage)
      {
         LogAssert(null, pCondition, pMessage);
      }

      [Conditional("UNITY_ASSERTIONS"),
       Conditional("UNITY_EDITOR"),
       Conditional("DEBUG"),
       Conditional("DEVELOPMENT_BUILD")]
      static public void LogAssert(UnityEngine.Object pContext, bool pCondition, string pMessage)
      {
#if UNITY_ASSERTIONS
         if (pContext != null)
            UnityEngine.Debug.Assert(pCondition, pMessage, pContext);
         else
            UnityEngine.Debug.Assert(pCondition, pMessage);

         Assert.IsTrue(pCondition, pMessage);
#else
         if (!pCondition)
         {  if (pContext != null)
               UnityEngine.Debug.LogError(pMessage, pContext);
            else
               UnityEngine.Debug.LogError(pMessage);
         }
#if UNITY_EDITOR
         if (!pCondition)
             Debug.Break();
#elif DEBUG || DEVELOPMENT_BUILD
         if (!pCondition)
               throw new System.Exception(pMessage);
#endif
#endif



      }
   }
}