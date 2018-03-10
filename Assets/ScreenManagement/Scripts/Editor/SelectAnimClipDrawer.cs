
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using XLib.ScreenMgmt;

namespace XLib.EditorScripts.ScreenMgmt.Transitions
{
   using Util;

   [CustomPropertyDrawer(typeof(SelectAnimClipAttribute))]
   public class SelectAnimClipDrawer : PropertyDrawer
   {  List<string> _clipNamesList;

      void _InitializeOptions(SerializedProperty pProperty)
      {  Animator _animator = (pProperty.serializedObject.targetObject as Component).gameObject.GetComponent<Animator>();
         AnimatorController animCtrl = _animator.runtimeAnimatorController as AnimatorController;

         if (_animator != null)
         {  if (_clipNamesList == null)
               _clipNamesList = new List<string>(100);

            _clipNamesList.Clear();

            animCtrl = _animator.runtimeAnimatorController as AnimatorController;

            if (animCtrl != null)
            {  AnimatorControllerLayer[] layers = animCtrl.layers;

               foreach (var layer in layers)
                  foreach (var state in layer.stateMachine.states)
                     _clipNamesList.Add(state.state.name);
            }
            else
            {  //Logging a message.
               Debug.Log("no animator controller");
            }
         }
         else
         {  //Logging message.
            Debug.Log("no animator");
         }
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {  _InitializeOptions(property);

         if (_clipNamesList != null && _clipNamesList.Count > 0)
         {  using (EditorGUIPropertyBlock.BeginProperty(position, label, property))
            {  position = EditorGUI.PrefixLabel(position, label);
               int selected = _clipNamesList.IndexOf(property.stringValue);

               if (selected < 0 || selected > _clipNamesList.Count)
                  selected = 0;

               selected = EditorGUI.Popup(position, selected, _clipNamesList.ToArray());
               property.stringValue = _clipNamesList[selected];
            }
         }
         else
         {  //Default behaviour
            EditorGUI.PropertyField(position, property, label, true);
         }
      }
   }
}