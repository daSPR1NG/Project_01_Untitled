using UnityEngine;
using UnityEditor;
using System.Linq;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> LabeledArrayDrawer description <summary>
    [CustomPropertyDrawer(typeof( LabeledArrayAttribute ), true)]
    public class LabeledArrayAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( rect, label, property );
            try
            {
                var path = property.propertyPath;
                int pos = int.Parse( path.Split( '[' ).LastOrDefault().TrimEnd( ']' ) );
                EditorGUI.PropertyField( rect, property, new GUIContent( ( ( LabeledArrayAttribute ) attribute ).names [ pos ] ), true );
            }
            catch
            {
                EditorGUI.PropertyField( rect, property, label, true );
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}