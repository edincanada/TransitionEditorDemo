
using UnityEngine;

namespace XLib.Demo
{
   using UnityEngine.EventSystems;
   using UnityEngine.UI;

   public class ChangeUITextColorOnPointerDown : MonoBehaviour,
                                                 IPointerDownHandler,
                                                 IPointerUpHandler,
                                                 IPointerExitHandler,
                                                 IPointerEnterHandler
   {  [SerializeField]
      Text _text = default;

      [SerializeField]
      Color _color = default;

      [SerializeField]
      bool _undoOnPointerUp = default;

      [SerializeField]
      bool _applyAsDelta = default;

      Color _originalColor;

      private void _doColorChange()
      {  if (_text != null)
         {  if (_applyAsDelta)
               _text.color += _color;
            else
               _text.color = _color;
         }
      }

      private void _undoColorChange() { _text.color = _originalColor; }

      public void OnPointerDown (PointerEventData eventData)
      {  _originalColor = _text.color;
         _doColorChange();
      }

      public void OnPointerUp (PointerEventData eventData)
      {  if (eventData.pointerPress == this.gameObject && _undoOnPointerUp)
            _undoColorChange();
      }

      public void OnPointerExit (PointerEventData eventData)
      {  if (eventData.pointerPress == this.gameObject && _undoOnPointerUp)
            _undoColorChange();
      }

      public void OnPointerEnter (PointerEventData eventData)
      {  if (eventData.pointerPress == this.gameObject && _undoOnPointerUp)
            _doColorChange();
      }
   }
}