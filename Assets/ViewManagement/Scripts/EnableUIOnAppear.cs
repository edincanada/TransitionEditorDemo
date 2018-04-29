
using UnityEngine;
using UnityEngine.UI;

namespace XLib.ViewMgmt
{
   using Util;
   public class EnableUIOnAppear : MonoBehaviour, IDidAppear
   {  public void DidAppear(StringKeyDictionary pInfo)
      {  foreach (var uiControl in this.gameObject.GetComponentsInChildren<Selectable>(true))
         {  //Enable
            uiControl.enabled = true;
         }
      }
   }
}
