
using UnityEngine;
using UnityEditor;

namespace XLib.EditorScripts.Util
{
   sealed public class EditorGUIPropertyBlock : System.IDisposable
   {  static private EditorGUIPropertyBlock _instance;

      private EditorGUIPropertyBlock() { }

      static public EditorGUIPropertyBlock BeginProperty(Rect position, GUIContent label, SerializedProperty property)
      {  if (_instance == null)
            _instance = new EditorGUIPropertyBlock();

         EditorGUI.BeginProperty(position, label, property);

         return _instance;
      }

      public void Dispose() { EditorGUI.EndProperty(); }
   }
}