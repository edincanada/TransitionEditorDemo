
using UnityEngine;
using UnityEditor;

using XLib.ScreenMgmt;
using XLib.ScreenMgmt.Transitions;

namespace XLib.EditorScripts.ScreenMgmt
{
   using Transitions;
   using Util;

   [CustomPropertyDrawer(typeof(SelectScreenTransitionAttribute))]
   public class SelectScreenTransitionDrawer : PropertyDrawer
   {  string[] _transitionNames;

      void _InitializeOptions()
      {  ScreenManager screenMgr = Object.FindObjectOfType<ScreenManager>();

         if (screenMgr == null)
         {  //Initialize
            _transitionNames = null;
         }
         else
         {  ScreenTransition[] transitions = screenMgr.GetComponents<ScreenTransition>();

            if (_transitionNames == null || _transitionNames.Length != transitions.Length)
               _transitionNames = new string[transitions.Length];

            int ii = 0;
            foreach (var transition in transitions)
            {  _transitionNames[ii] = transition.TransitionName;
               ii++;
            }
         }
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
      {  _InitializeOptions();

         if (_transitionNames != null && _transitionNames.Length > 0)
         {  using (EditorGUIPropertyBlock.BeginProperty(position, label, property))
            {  position = EditorGUI.PrefixLabel(position, label);
               int selected = System.Array.IndexOf(_transitionNames, property.stringValue);

               if (selected < 0 || selected > _transitionNames.Length)
                  selected = 0;

               selected = EditorGUI.Popup(position, selected, _transitionNames);
               property.stringValue = _transitionNames[selected];
            }
         }
         else
         {  //Default
            EditorGUI.PropertyField(position, property, label, true);
         }
      }
   }
}