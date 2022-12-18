using UnityEngine;
using UnityEditor;

namespace dnSR_Coding
{
    ///<summary> ValidationDrawer description <summary>
    [CustomPropertyDrawer(typeof(ValidationAttribute), true)]
    public class ValidationDrawer : PropertyDrawer
    {
        const int k_boxPadding = 10;
        const float k_padding = 10f;
        const float k_offset = 20f;

        float m_height = 10f;
        float m_helpBoxHeight = 0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if ( property.objectReferenceValue == null )
            {
                ValidationAttribute attr = ( ValidationAttribute ) attribute;

                position.height = m_helpBoxHeight + ( k_padding * .75f );
                position.y += k_padding * .5f;
                EditorGUI.HelpBox( position, attr.Text, MessageType.Error );

                position.height = m_height - ( k_padding * .75f );
                EditorGUI.DrawRect( position, new Color( 1f, .2f, .2f, .2f ) );

                position.y += m_helpBoxHeight + ( k_padding * .75f );
                position.height = base.GetPropertyHeight( property, label );
                EditorGUI.PropertyField( position, property, new GUIContent( property.displayName ));
            }
            else EditorGUI.PropertyField( position, property, new GUIContent( property.displayName ) );

            EditorUtility.SetDirty( property.serializedObject.targetObject );
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            if ( property.objectReferenceValue == null )
            {
                ValidationAttribute attr = ( ValidationAttribute ) attribute;

                GUIStyle style = EditorStyles.helpBox;
                style.alignment = TextAnchor.MiddleLeft;
                style.wordWrap = true;
                style.padding = new RectOffset( k_boxPadding, k_boxPadding, k_boxPadding, k_boxPadding );
                style.fontSize = 12;

                m_helpBoxHeight = style.CalcHeight( new GUIContent( attr.Text ), Screen.width );

                m_height = m_helpBoxHeight + base.GetPropertyHeight( property, label ) + k_offset;

                return m_height;
            }
            else return base.GetPropertyHeight( property, label );
        }
    }
}