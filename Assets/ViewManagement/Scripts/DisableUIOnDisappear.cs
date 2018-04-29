
using UnityEngine;
using UnityEngine.UI;

namespace XLib.ViewMgmt
{
   using Util;
   public class DisableUIOnDisappear : MonoBehaviour, IWillDisappear
   {  public void WillDisappear(StringKeyDictionary pInfo)
      {  foreach (var uiControl in this.gameObject.GetComponentsInChildren<Selectable>(true))
         {  //Disable
            uiControl.enabled = false;
         }
      }
   }
}