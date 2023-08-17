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

        public override void OnGUI( Rect position )
        {
            CenteredHeaderAttribute centeredHeaderAttribute = attribute as CenteredHeaderAttribute;

            _header = centeredHeaderAttribute?.Header.ToUpper();
            _height = ( float ) ( centeredHeaderAttribute?.Height );
            _textAnchor = centeredHeaderAttribute.TextAnchor;

            _yMinPos = centeredHeaderAttribute.YMinPos;
            position.yMin += _yMinPos;

            float fontRatio = EditorGUIUtility.currentViewWidth / MIN_WIDTH;            float fontSizePadding = _height - ( _height / 4 ) - ( PADDING_HEIGHT_OFFSET * 2 );

            Rect rect = EditorGUI.IndentedRect( position );

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = GUIStyles.GetHeaderWithBackground(
                    _height + HEIGHT_OFFSET,
                    fontRatio, fontSizePadding,
                    EditorColor.Black,
                    _textAnchor );

                float leftDecorationWidth = 5;
                GUIStyle leftDecoration = GUIStyles.GetHeaderWithBackground(
                    _height + HEIGHT_OFFSET,
                    0, 0,
                    EditorColor.Red );

                leftDecoration.Draw(
                    new Rect(
                        rect.x, rect.y,
                     leftDecorationWidth,
                     style.fixedHeight ),
                    GUIContent.none,
                    false, false, false, false );

                style.Draw(
                    new Rect(
                        rect.x + leftDecorationWidth, rect.y,
                     GetInspectorWidthBasedOnIndentation( position ) - leftDecorationWidth,
                     style.fixedHeight ),
                    new GUIContent( _header ),
                    false, false, false, false );
            }
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            return _height + ( _yMinPos + PADDING_HEIGHT_OFFSET ) * 2;
        }
    }
}