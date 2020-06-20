using UnityEngine;

namespace XLib.Demo
{
   using UnityEvents = UnityEngine.EventSystems;
   using UnityUI = UnityEngine.UI;

   using ButtonClickAction = System.Action<MissionComponent, UnityEngine.EventSystems.PointerEventData>;

   [RequireComponent(typeof(UnityUI.Image))]
   [RequireComponent(typeof(UnityUI.Button))]
   public class MissionComponent : MonoBehaviour, UnityEvents.IPointerClickHandler
   {
      [SerializeField]
      Sprite _missionSprite = default;

      [SerializeField]
      Sprite _spriteWhenSelected = default;

      [SerializeField]
      string _missionText = default;

      Sprite _defaultSprite;
      UnityUI.Image _image;

      public Sprite MissionSprite { get { return _missionSprite; } }

      public string MissionText { get { return _missionText; } }

      public ButtonClickAction OnMissionButtonClicked;

      virtual protected void Start()
      {  _image = gameObject.GetComponent<UnityUI.Image>();
         if (_image != null)
            _defaultSprite = _image.sprite;
      }

      public void SetToggle(bool pToggle)
      {  if (pToggle)
            _image.sprite = _spriteWhenSelected;
         else
            _image.sprite = _defaultSprite;
      }

      public void OnPointerClick(UnityEvents.PointerEventData eventData)
      {  if (OnMissionButtonClicked != null)
            OnMissionButtonClicked(this, eventData);
      }
   }
}