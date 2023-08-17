using UnityEngine;
using UnityEditor;

namespace dnSR_Coding
{
    ///<summary> TestFoldAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( TestFoldAttribute ), true )]
    public class TestFoldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            Debug.Log( property.serializedObject );
            Debug.Log( property.serializedObject.targetObject );
            Debug.Log( property.serializedObject.targetObject.name );

            Debug.Log( property.serializedObject.GetType().GetCustomAttributes(true).Length );

            foreach ( var item in property.serializedObject.GetType().GetCustomAttributes( true ) )
            {

                Debug.Log( item.ToString() );
            }
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}