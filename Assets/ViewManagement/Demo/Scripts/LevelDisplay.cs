using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLib.Demo
{
   using UnityUI = UnityEngine.UI;
   using UnityEvents = UnityEngine.EventSystems;
   using System;

   public class LevelDisplay : MonoBehaviour
   {
      [SerializeField]
      string[] _levels = default;

      [SerializeField]
      int _currentIndex = default;

      [SerializeField]
      UnityUI.Button _levelUpButton = default;

      [SerializeField]
      UnityUI.Button _levelDownButton = default;

      UnityUI.Text _textComponent;

      virtual protected void OnEnable()
      {  if (_levelUpButton)
            _levelUpButton.onClick.AddListener(_OnLevelUp);

         if (_levelDownButton)
            _levelDownButton.onClick.AddListener(_OnLevelDown);

         _textComponent = this.GetComponent<UnityUI.Text>();

         _SetText();
      }

      private void _SetText()
      {  if (_currentIndex > _levels.Length - 1)
            _currentIndex = _levels.Length - 1;
         else if (_currentIndex < 0)
            _currentIndex = 0;

         if (_textComponent)
            _textComponent.text = _levels[_currentIndex];
      }

      virtual protected void OnDisable()
      {  if (_levelUpButton)
            _levelUpButton.onClick.RemoveListener(_OnLevelUp);

         if (_levelDownButton)
            _levelDownButton.onClick.RemoveListener(_OnLevelDown);
      }

      private void _OnLevelUp()
      {  _currentIndex ++;
         _SetText();
      }

      private void _OnLevelDown()
      {  _currentIndex --;
         _SetText();
      } 
   }
}
