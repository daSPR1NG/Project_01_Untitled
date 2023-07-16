using UnityEngine;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding.Utilities
{
    public static class GUIStyles
    {
        public static GUIStyle GetCenteredHeaderStyle( float height, float fontRatio, float fontSizePadding, EditorColor color )
        {
            GUIStyle style = new( "box" )
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = ( int ) ( fontRatio >= 1 ? fontSizePadding : fontSizePadding * fontRatio ),

                border = DefaultRectOffset(),
                fixedHeight = height,
                stretchWidth = true,
            };

            style.normal.textColor = Color.white;
            style.normal.background = CreateColorPixel( GetColor( color ) );

            return style;
        }

        public static GUIStyle GetLineStyle( float height, Texture2D texture2D )
        {
            GUIStyle lineStyle = new( "box" )
            {
                stretchWidth = true,
                border = DefaultRectOffset(),
                fixedHeight = height
            };
            lineStyle.normal.background = texture2D;

            return lineStyle;
        }
    }
}