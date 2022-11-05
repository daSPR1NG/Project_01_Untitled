#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace dnSR_Coding.Utilities
{
    ///<summary> HeaderDrawer description <summary>
    [CustomPropertyDrawer( typeof( TitleAttribute ), true )]
    public class TitleDrawer : DecoratorDrawer
    {
        float _yOffset;

        ///<inheritdoc/>
        public override void OnGUI( Rect position )
        {
            if ( attribute is not TitleAttribute headerAttribute ) return;

            position = EditorGUI.IndentedRect( position );
            position.yMin += EditorGUIUtility.singleLineHeight * ( headerAttribute.TextHeightIncrease - 0.5f );

            GUIStyle style = new ( EditorStyles.label ) { richText = true };

            GUIContent label = new (
               $"<color={headerAttribute.ColorString}>" +
               $"<size={headerAttribute.FontSize + headerAttribute.TextHeightIncrease}>" +
               $"<b>{headerAttribute.Header}</b></size></color>" );

            _yOffset = style.fontSize + headerAttribute.TextHeightIncrease;

            Vector2 textSize = style.CalcSize( label );

            Rect labelRect = new ( position.xMin, position.yMin, textSize.x, position.height );

            EditorGUI.LabelField( labelRect, label, style );
        }

        ///<inheritdoc/>
        public override float GetHeight()
        {
            TitleAttribute headerAttribute = TitleAttribute;
            return EditorGUIUtility.singleLineHeight + ( headerAttribute.FontSize /*/ 5*/ ) /*+ .5f*//*( headerAttribute?.TextHeightIncrease + 0.5f ?? 0 )*/;
        }

        TitleAttribute TitleAttribute => ( TitleAttribute ) attribute;
    }
}
#endif