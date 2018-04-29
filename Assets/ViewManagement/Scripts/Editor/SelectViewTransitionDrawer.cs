
using UnityEngine;
using UnityEditor;

using XLib.ViewMgmt;
using XLib.ViewMgmt.Transitions;

namespace XLib.EditorScripts.ViewMgmt
{
   using Transitions;
   using Util;

   [CustomPropertyDrawer(typeof(SelectViewTransitionAttribute))]
   public class SelectViewTransitionDrawer : PropertyDrawer
   {  string[] _transitionNames;

      void _InitializeOptions()
      {  ViewManager viewMgr = Object.FindObjectOfType<ViewManager>();

         if (viewMgr == null)
         {  //Initialize
            _transitionNames = null;
         }
         else
         {  ViewTransition[] transitions = viewMgr.GetComponents<ViewTransition>();

            if (_transitionNames == null || _transitionNames.Length != transitions.Length)
            {  _transitionNames = new string[transitions.Length + 1];
               _transitionNames[0] = "None";
            }

            int ii = 1;
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

               if (selected > 0)
                  property.stringValue = _transitionNames[selected];
               else
                  property.stringValue = "";
            }
         }
         else
         {  //Default
            EditorGUI.PropertyField(position, property, label, true);
         }
      }
   }
}