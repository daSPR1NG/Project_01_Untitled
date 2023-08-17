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
                padding = new RectOffset( 10, 10, 2, 2 ),

                fixedHeight = height,
                normal = CreateStyleWithBackground( GUI.skin.box.ToString(), color ).normal
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

        public static GUIStyle CreateStyleWithBackground( string s, EditorColor color )
        {
            GUIStyle style = new( s ) { };

            style.normal.textColor = Color.white;
            style.normal.background = CreateBackground( color );

            return style;
        }
    }
}