using UnityEngine;
using UnityEditor;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding.Utilities
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
        private bool _isIndented;

        public override void OnGUI( Rect position )
        {
            CenteredHeaderAttribute centeredHeaderAttribute = attribute as CenteredHeaderAttribute;

            _header = centeredHeaderAttribute?.Header;
            _height = ( float ) ( centeredHeaderAttribute?.Height );
            _isIndented = ( bool ) ( centeredHeaderAttribute?.IsIndented );

            position.yMin += 10;

            float fontRatio = EditorGUIUtility.currentViewWidth / MIN_WIDTH;            float fontSizePadding = _height - ( _height / 4 ) - PADDING_HEIGHT_OFFSET;

            Rect rect = _isIndented ? EditorGUI.IndentedRect( position ) : position;

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = GUIStyles.GetCenteredHeaderStyle( 
                    _height + HEIGHT_OFFSET, 
                    fontRatio, fontSizePadding, 
                    EditorColor.Black );

                style.Draw( 
                    new Rect(
                        rect.x, rect.y,
                        _isIndented ? 
                        GetInspectorWidth_BasedOnIndentation( position ) : GetInspectorWidth_BasedOnPositionX( position ), 
                        style.fixedHeight ),
                    new GUIContent( _header ),
                    false, false, false, false);
            }
        }

        ///<inheritdoc/>
        public override float GetHeight() {
            return _height + EditorGUIUtility.singleLineHeight + PADDING_HEIGHT_OFFSET;
        }
    }
}