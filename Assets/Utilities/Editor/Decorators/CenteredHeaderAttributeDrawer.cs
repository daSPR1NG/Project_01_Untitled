using UnityEngine;
using UnityEditor;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding.Utilities
{
    ///<summary> CenteredHeaderAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( CenteredHeaderAttribute ), true )]
    public class CenteredHeaderAttributeDrawer : DecoratorDrawer
    {
        private string _header;
        private float _height;
        private bool _isIndented;

        public override void OnGUI( Rect position )
        {
            CenteredHeaderAttribute centeredHeaderAttribute = attribute as CenteredHeaderAttribute;

            _header = centeredHeaderAttribute?.Header;
            _height = ( float ) ( centeredHeaderAttribute?.Height );
            _isIndented = ( bool ) ( centeredHeaderAttribute?.IsIndented );

            position.yMin += 5 * 2;

            float fontRatio = EditorGUIUtility.currentViewWidth / 450;            float fontSizePadding = _height - ( _height / 4 );            

            Rect rect = _isIndented ? EditorGUI.IndentedRect( position ) : position;

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle style = GUIStyles.GetCenteredHeaderStyle( _height, fontRatio, fontSizePadding, EditorColor.Grey );

                style.Draw( 
                    new Rect(
                        rect.x, rect.y,
                        _isIndented ? 
                        GetInspectorWidth_BasedOnIndentation( position ) : GetInspectorWidth_BasedOnPositionX( position ), 
                        style.fixedHeight ),
                    new GUIContent( _header ), 0 );
            }
        }

        ///<inheritdoc/>
        public override float GetHeight() {
            return _height + ( 5 * 2 ) + 5;
        }
    }
}