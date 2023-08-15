using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding.Utilities.Editor
{
    [CustomPropertyDrawer( typeof( ScriptableObject ), true )]
    public class ScriptableObjectPropertyDrawer : PropertyDrawer
    {
        // Static foldout dictionary
        private static readonly Dictionary<System.Type, bool> foldoutByType = new();

        // Cached scriptable object _editor
        private UnityEditor.Editor _editor = null;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            // Draw label
            EditorGUI.PropertyField( position, property, label, true );

            // Draw foldout arrow
            bool foldout = false;
            if ( !property.objectReferenceValue.IsNull<SerializedProperty>() )
            {
                // Store foldout values in a dictionary per object Type
                bool foldoutExists = foldoutByType.TryGetValue( property.objectReferenceValue.GetType(), out foldout );
                foldout = EditorGUI.Foldout( position, foldout, GUIContent.none );
                if ( foldoutExists )
                {
                    foldoutByType [ property.objectReferenceValue.GetType() ] = foldout;
                }
                else
                {
                    foldoutByType.Add( property.objectReferenceValue.GetType(), foldout );
                }
            }

            // Draw foldout properties
            if ( foldout )
            {
                // Make child fields be indented
                EditorGUI.indentLevel++;

                // Draw object properties
                if ( !_editor )
                {
                    UnityEditor.Editor.CreateCachedEditor( property.objectReferenceValue, null, ref _editor );
                }

                _editor.OnInspectorGUI();

                // Set indent back to what it was
                EditorGUI.indentLevel--;
            }
        }
    }
}