using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class SingletonUnityObject<T> where T : UnityEngine.Object
{  static private T _instance ;
   static public T Instance
   {  get
	   {  if (_instance == null)
            _instance = UnityEngine.Object.FindObjectOfType<T> ();

		   return _instance;
	   }
   }
}
