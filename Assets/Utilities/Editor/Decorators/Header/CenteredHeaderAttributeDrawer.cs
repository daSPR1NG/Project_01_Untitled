using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> CenteredHeaderAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( CenteredHeaderAttribute ), true )]
    public class CenteredHeaderAttributeDrawer : DecoratorDrawer
    {
        private const float MIN_WIDTH = 300f;
        private const float HEIGHT_OFFSET = 4f;
        private const int PADDING_HEIGHT_OFFSET = 1;

        private string _header;
        private float _height;
        private float _yMinPos;
        private TextAnchor _textAnchor;
        private EditorColor _leftDecorationColor;

        public override void OnGUI( Rect position )
        {
            CenteredHeaderAttribute centeredHeaderAttribute = attribute as CenteredHeaderAttribute;

            _header = centeredHeaderAttribute?.Header.ToUpper();
            _height = ( float ) ( centeredHeaderAttribute?.Height );
            _textAnchor = centeredHeaderAttribute.TextAnchor;
            _leftDecorationColor = centeredHeaderAttribute.LeftDecorationColor;

            _yMinPos = centeredHeaderAttribute.YMinPos;
            position.yMin += _yMinPos;

            float fontRatio = EditorGUIUtility.currentViewWidth / MIN_WIDTH;            float fontSizePadding = _height - ( _height / 4 ) - ( PADDING_HEIGHT_OFFSET * 2 );

            Rect rect = EditorGUI.IndentedRect( position );

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle headerBGStyle = GUIStyles.GetHeaderWithBackground(
                    _height + HEIGHT_OFFSET,
                    fontRatio, fontSizePadding,
                    EditorColor.Black,
                    _textAnchor );

                DrawLeftDecoration(
                    ref rect,
                    headerBGStyle,
                    _leftDecorationColor,
                    out float leftDecorationWidth );

                headerBGStyle.Draw(
                    new Rect(
                        rect.x + leftDecorationWidth, rect.y,
                     GetInspectorWidthBasedOnIndentation( position ) 
                     - GetXOffset_BasedOnIndentation( rect )
                     + 2 
                     - ( IsIndented() ? 0 : leftDecorationWidth ),
                     headerBGStyle.fixedHeight ),
                    new GUIContent( _header ),
                    false, false, false, false );
            }
        }

        private void DrawLeftDecoration( ref Rect rect, GUIStyle style, EditorColor color, out float decorationWidth )
        {
            float height = _height + HEIGHT_OFFSET;
            decorationWidth = 5;

            GUIStyle leftDecoration = GUIStyles.GetHeaderWithBackground(
                height,
                0, 0,
                color );

            leftDecoration.Draw(
                new Rect(
                    rect.x, rect.y,
                 decorationWidth,
                 style.fixedHeight ),
                GUIContent.none,
                false, false, false, false );
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            return _height + ( _yMinPos + PADDING_HEIGHT_OFFSET ) * 2;
        }
    }
}