
using UnityEngine;
using UnityEngine.UI;

namespace XLib.ScreenMgmt
{
   public class DisableUIOnDisappear : MonoBehaviour, IWillDisappear
   {  public void WillDisappear()
      {  foreach (var uiControl in this.gameObject.GetComponentsInChildren<Selectable>(true))
         {  //Disable
            uiControl.enabled = false;
         }
      }
   }
}