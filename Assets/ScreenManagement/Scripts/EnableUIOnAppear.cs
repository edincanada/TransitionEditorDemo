
using UnityEngine;
using UnityEngine.UI;

namespace XLib.ScreenMgmt
{
   public class EnableUIOnAppear : MonoBehaviour, IDidAppear
   {  public void DidAppear()
      {  foreach (var uiControl in this.gameObject.GetComponentsInChildren<Selectable>(true))
         {  //Enable
            uiControl.enabled = true;
         }
      }
   }
}
