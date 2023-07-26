using UnityEngine;
using UnityEditor;

namespace dnSR_Coding
{
    ///<summary> ReadOnlyAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( ReadOnlyAttribute ), true )]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            GUI.enabled = false;
            EditorGUI.PropertyField( position, property, label, true );
            GUI.enabled = true;
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}