using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.Demo
{
   using UnityEvents = UnityEngine.EventSystems;
   using UnityUI = UnityEngine.UI;
   public class ToggleButton : MonoBehaviour
   {  private bool _toggled;

      [SerializeField]
      Sprite _toggledOffSprite = default;

      Sprite _originalSprite;

      [SerializeField]
      UnityUI.Text _textComponent = default;

      UnityUI.Image _buttonImage;

      virtual protected void Start()
      {  _toggled = true;
         SetToggle(_toggled);
         _buttonImage = this.gameObject.GetComponent<UnityUI.Image>();
         if (_buttonImage)
            _originalSprite = _buttonImage.sprite;
      }

      public void SetToggle(bool pToggle)
      {  if (pToggle)
         {  if (_textComponent)
               _textComponent.enabled = true;
            if (_buttonImage)
               _buttonImage.sprite = _originalSprite;
         }
         else
         {  if (_textComponent)
               _textComponent.enabled = false;
            if (_buttonImage)
               _buttonImage.sprite = _toggledOffSprite;
         }

         _toggled = pToggle;
      }

      public void Toggle() { SetToggle(!_toggled); }
   }
}