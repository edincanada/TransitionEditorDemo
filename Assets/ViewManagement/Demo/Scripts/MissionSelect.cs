using System;
using UnityEngine;
using UnityEngine.UI;

namespace XLib.Demo
{
   using UnityEvents = UnityEngine.EventSystems;
   using UnityUI = UnityEngine.UI;
   public class MissionSelect : MonoBehaviour
   {  [SerializeField]
      UnityUI.Image _missionImage = default;
      [SerializeField]
      UnityUI.Text _missionText = default;
      [SerializeField]
      MissionComponent[] _missions = default;

      protected virtual void OnEnable()
      {  foreach (var mission in _missions)
         {  if (mission != null)
               mission.OnMissionButtonClicked += onMissionButtonClickedHandler;
         }
      }

      virtual protected void OnDisable()
      {  foreach (var mission in _missions)
         {
            if (mission != null)
               mission.OnMissionButtonClicked -= onMissionButtonClickedHandler;
         }
      }

      private void onMissionButtonClickedHandler(MissionComponent pMission, UnityEvents.PointerEventData pData)
      {  foreach (var mission in _missions)
         {  if (mission != null)
               mission.SetToggle(false);
         }

         pMission.SetToggle(true);
         _missionImage.sprite = pMission.MissionSprite;
         _missionText.text = pMission.MissionText;
      }
   }
}