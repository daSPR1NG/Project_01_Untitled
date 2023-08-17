using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> 
    /// Redraws the overridden Header attribute (found at CstmHeaderAttribute) styling it.
    ///<summary>
    [CustomPropertyDrawer( typeof( CstmHeaderAttribute ), true )]
    public class CstmHeaderAttributeDrawer : DecoratorDrawer
    {
        private bool _hasUnderline = false;
        private HorizontalLineAttribute _horizontalLineAttribute = null;

        ///<inheritdoc/>
        public override void OnGUI( Rect position )
        {
            CstmHeaderAttribute headerAttribute = ( attribute as CstmHeaderAttribute );
            _hasUnderline = ( bool ) ( headerAttribute?.HasUnderline );

            if ( _hasUnderline )
            {
                position.yMin -= EditorGUIUtility.singleLineHeight * 0.5f;
            }

            GUIStyle style = new GUIStyle( EditorStyles.boldLabel )
            {
                richText = true
            };

            GUIContent label = new(
               $"<color=lightblue><size=13>{headerAttribute?.Header}</size></color>" );

            GUI.Label( EditorGUI.IndentedRect( position ), label, style );

            if ( _hasUnderline )
            {
                _horizontalLineAttribute = new HorizontalLineAttribute( 1, 4, EditorColor.Grey );

                HorizontalLineAttributeDrawer.DrawLine(
                    position: new Rect(
                            position.x,
                            position.y + GetHeight() - _horizontalLineAttribute.Height - _horizontalLineAttribute.YOffset,
                            0, 0 ),
                    GetInspectorWidthBasedOnIndentation( position ),
                    _horizontalLineAttribute.Height,
                    _horizontalLineAttribute.YOffset,
                    _horizontalLineAttribute.Color );
            }
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight * 1.5f;

            float totalHeightWithUnderline = _horizontalLineAttribute.IsNull() ?
                0 : singleLineHeight + _horizontalLineAttribute.Height + _horizontalLineAttribute.YOffset;

            return _hasUnderline && !_horizontalLineAttribute.IsNull() ?
                totalHeightWithUnderline : singleLineHeight;
        }
    }
}