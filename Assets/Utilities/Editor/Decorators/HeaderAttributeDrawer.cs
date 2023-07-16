using UnityEngine;
using UnityEditor;
using static dnSR_Coding.Utilities.EditorHelper;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> 
    /// Redraws the overridden Header attribute (found at HeaderAttribute) styling it.
    ///<summary>
    [CustomPropertyDrawer( typeof( HeaderAttribute ), true )]
    public class HeaderAttributeDrawer : DecoratorDrawer
    {
        private bool _hasUnderline = false;
        private HorizontalLineAttribute _horizontalLineAttribute = null;

        ///<inheritdoc/>
        public override void OnGUI( Rect position )
        {
            HeaderAttribute headerAttribute = ( attribute as HeaderAttribute );
            _hasUnderline = ( bool ) ( headerAttribute?.HasUnderline );

            if ( _hasUnderline )
            {
                position.yMin -= EditorGUIUtility.singleLineHeight * 0.5f;
            }

            GUIStyle style = new GUIStyle( EditorStyles.boldLabel ) {
                richText = true 
            };

            GUIContent label = new(
               $"<color=lightblue><size=13>{headerAttribute?.Header}</size></color>" );

            GUI.Label( EditorGUI.IndentedRect( position ), label, style );

            if ( _hasUnderline ) {
                _horizontalLineAttribute = new HorizontalLineAttribute( 1, 4, EditorColor.Grey );

                HorizontalLineAttributeDrawer.DrawLine( 
                    position: new Rect(
                            position.x,
                            position.y + GetHeight() - _horizontalLineAttribute.Height - _horizontalLineAttribute.YOffset,
                            0, 0 ),
                    width: GetInspectorWidth_BasedOnIndentation( position ),
                    height: _horizontalLineAttribute.Height, 
                    yOffset: _horizontalLineAttribute.YOffset,
                    color: GetColor( _horizontalLineAttribute.Color ) );
            }
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight * 1.5f;

            float totalHeightWithUnderline = _horizontalLineAttribute.IsNull<HorizontalLineAttribute>() ? 
                0 : singleLineHeight + _horizontalLineAttribute.Height + _horizontalLineAttribute.YOffset;

            return _hasUnderline && !_horizontalLineAttribute.IsNull<HorizontalLineAttribute>() ? 
                totalHeightWithUnderline : singleLineHeight;
        }
    }
}