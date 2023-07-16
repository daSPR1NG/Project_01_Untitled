using UnityEngine;
using UnityEditor;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding.Utilities
{
    ///<summary> HorizontalLineAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( HorizontalLineAttribute ), true )]
    public class HorizontalLineAttributeDrawer : DecoratorDrawer
    {
        private float _lineHeight = 0;
        private float _yOffset = 0;
        private Color _color = Color.white;

        public override void OnGUI( Rect position )
        {
            HorizontalLineAttribute horizontalLineAttribute = attribute as HorizontalLineAttribute;

            _lineHeight = ( float ) ( horizontalLineAttribute?.Height );
            _yOffset = ( float ) ( horizontalLineAttribute?.YOffset );
            _color = GetColor( ( EditorColor ) ( horizontalLineAttribute?.Color ) );

            DrawLine( position, GetInspectorWidth_BasedOnPositionX( position ), _lineHeight, _yOffset, _color );
        }

        public static void DrawLine( Rect position, float width, float height, float yOffset, Color color )
        {
            Rect rect = EditorGUI.IndentedRect( position );
            rect.y = position.y + yOffset;

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = GUIStyles.GetLineStyle( height, CreateColorPixel( color ) );
                style.Draw(
                    new Rect( rect.x, rect.y, width, style.fixedHeight ),
                    false, false, false, false );
            }
        }

        ///<inheritdoc/>
        public override float GetHeight() {
            return _lineHeight + _yOffset * 2;
        }
    }
}