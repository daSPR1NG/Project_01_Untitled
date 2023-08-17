using UnityEngine;
using UnityEditor;

namespace dnSR_Coding
{
    ///<summary> LabelRenamerAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( LabelRenamerAttribute ), true )]
    public class LabelRenamerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            LabelRenamerAttribute abbreviationAttribute = attribute as LabelRenamerAttribute;

            label.text = abbreviationAttribute.Abbreviation;

            EditorGUI.PropertyField( position, property, label );
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}