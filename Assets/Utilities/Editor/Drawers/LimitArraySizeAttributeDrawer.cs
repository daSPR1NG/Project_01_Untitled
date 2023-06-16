using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using UnityEditor;
using dnSR_Coding.Attributes;

namespace dnSR_Coding
{
    [CustomPropertyDrawer( typeof( LimitArraySizeAttribute ), true )]
    public class LimitArraySizeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label )
        {
            LimitArraySizeAttribute element = attribute as LimitArraySizeAttribute;
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}