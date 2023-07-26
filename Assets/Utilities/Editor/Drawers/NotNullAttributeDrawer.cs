using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding
{
    ///<summary> NotNullAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( NotNullAttribute ), true )]
    public class NotNullAttributeDrawer : PropertyDrawer
    {
        private const int DEFAULT_RECT_OFFSET = 4;
        private const float ERROR_MESSAGE_RECT_RATIO = 2;
        private const float PROPERTY_RECT_RATIO = 2;

        private Rect _errorMessageRect, _propertyRect;

        private float _contentsOffset;
        private Vector2 _contentSize;
        private GUIContent _errorMessageContent = null;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            if ( property.objectReferenceValue.IsNull() )
            {
                _contentsOffset = _contentSize.y * 1.5f + DEFAULT_RECT_OFFSET / 2;
                Rect backgroundRect = IsIndented() ? EditorGUI.IndentedRect( position ) : position;

                _errorMessageContent = new GUIContent()
                {
                    text = $" Null Reference Exception",
                };

                _contentSize = GetGUIContentSize( _errorMessageContent );

                DrawErrorBox( backgroundRect, _errorMessageContent );
                DrawPropertyBackground( backgroundRect );
                DrawNullPropertyField( property, backgroundRect );
                return;
            }

            EditorGUI.PropertyField( position, property, label, true );
        }

        private void DrawErrorBox( Rect rect, GUIContent content )
        {
            _errorMessageRect = new Rect( 
                rect.x,
                rect.y,
                rect.width,
                rect.height / ERROR_MESSAGE_RECT_RATIO );

            EditorGUI.HelpBox( _errorMessageRect, content.text, MessageType.Error );
        }

        private void DrawPropertyBackground( Rect rect )
        {
            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = new GUIStyle();

                style.normal.background = CreateColorPixel( GetColor( EditorColor.Red ).WithAlpha( .35f ) );

                _propertyRect = new Rect(
                    rect.x,
                    rect.y + _contentsOffset,
                    rect.width,
                    rect.height / PROPERTY_RECT_RATIO );

                style.Draw( _propertyRect, GUIContent.none, false, false, false, false );
            }
        }

        private void DrawNullPropertyField( SerializedProperty property, Rect rect )
        {
            //EditorStyles.label.normal.textColor = Color.red;

            EditorGUI.PropertyField(
                new Rect(
                    rect.x,
                    rect.y + _contentsOffset + DEFAULT_RECT_OFFSET / 2,
                    rect.width - 2,
                    rect.height / PROPERTY_RECT_RATIO ),
                property,
                new GUIContent( $"{property.displayName} is Null" ),
                true );

            //EditorStyles.label.normal.textColor = Color.white;
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
            return EditorGUI.GetPropertyHeight( property, label ) + _contentsOffset + DEFAULT_RECT_OFFSET;
        }
    }
}