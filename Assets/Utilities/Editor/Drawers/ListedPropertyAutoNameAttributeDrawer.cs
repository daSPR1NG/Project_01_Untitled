using UnityEngine;
using UnityEditor;
using System.Linq;

namespace dnSR_Coding.Attributes.Drawer
{
    ///<summary> HeaderDrawer description <summary>
    [CustomPropertyDrawer( typeof( ListedPropertyAutoNameAttribute ), true )]
    public class ListedPropertyAutoNameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( rect, label, property );
            try
            {
                var path = property.propertyPath;
                int pos = int.Parse( path.Split( '[' ).LastOrDefault().TrimEnd( ']' ) );
                EditorGUI.PropertyField( rect, property, new GUIContent( ( ( ListedPropertyAutoNameAttribute ) attribute ).Names [ pos ] ), true );
            }
            catch
            {
                EditorGUI.PropertyField( rect, property, label, true );
            }
            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }

}