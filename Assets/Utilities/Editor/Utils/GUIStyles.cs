using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    public static class GUIStyles
    {
        public static GUIStyle GetHeaderWithBackground(
            float height,
            float fontRatio,
            float fontSizePadding,
            EditorColor color,
            TextAnchor textAnchor = TextAnchor.MiddleCenter )
        {
            GUIStyle style = new()
            {
                alignment = textAnchor,
                fontStyle = FontStyle.Bold,
                fontSize = ( int ) ( fontRatio >= 1
                    ? fontSizePadding
                    : fontSizePadding * fontRatio ),
                padding = new RectOffset( 8, 8, 2, 2 ),

                fixedHeight = height,
                normal = CreateBackgroundFromStyle( GUI.skin.box.ToString(), color ).normal
            };

            return style;
        }

        public static GUIStyle GetLineStyle( float height, Texture2D texture2D )
        {
            GUIStyle lineStyle = new( "box" )
            {
                border = GetDefaultRectOffset(),
                fixedHeight = height
            };
            lineStyle.normal.background = texture2D;

            return lineStyle;
        }

        public static GUIStyle CreateBackgroundFromStyle( string styleName, EditorColor color )
        {
            GUIStyle style = new( styleName ) { };

            style.normal.textColor = Color.white;
            style.normal.background = EditorHelper.CreateBackground( color );

            return style;
        }
    }
}