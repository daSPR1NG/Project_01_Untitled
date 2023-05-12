using UnityEditor;
using UnityEngine;

namespace dnSR_Coding.Attributes.Drawer
{
    ///<summary> HeaderDrawer description <summary>
    [CustomPropertyDrawer( typeof( NamedArrayElementAttribute ), true )]
    public class NamedArrayElementAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label )
        {
            NamedArrayElementAttribute element = attribute as NamedArrayElementAttribute;

            string path = property.propertyPath;
            int indexOfNum = path.LastIndexOf( '[' ) + 1;
            int index = System.Convert.ToInt32( path.Substring( indexOfNum, path.LastIndexOf( ']' ) - indexOfNum ) );

            if ( element.ExcludeFirstIndex ) { index += 1; }

            if ( index >= element.Names.Length )
            {
                property.stringValue = "Over length limit => won't be named".ToUpper();
                return; 
            }

            property.stringValue = element.Names [ index ];
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}