using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> HorizontalLineAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( HorizontalLineAttribute ), true )]
    public class HorizontalLineAttributeDrawer : DecoratorDrawer
    {
        private float _lineHeight = 0;
        private float _yOffset = 0;
        private EditorColor _color = EditorColor.White;

        public override void OnGUI( Rect position )
        {
            HorizontalLineAttribute horizontalLineAttribute = attribute as HorizontalLineAttribute;

            _lineHeight = ( float ) ( horizontalLineAttribute?.Height );
            _yOffset = ( float ) ( horizontalLineAttribute?.YOffset );
            _color = ( EditorColor ) ( horizontalLineAttribute?.Color );

            DrawLine( position, GetInspectorWidthBasedOnIndentation( position ), _lineHeight, _yOffset, _color );
        }

        public static void DrawLine( Rect position, float width, float height, float yOffset, EditorColor color )
        {
            Rect rect = EditorGUI.IndentedRect( position );
            rect.y = position.y + yOffset;

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = GUIStyles.GetLineStyle( height, CreateBackground( color ) );
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