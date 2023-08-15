using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    public static class GUIStyles
    {
        public static GUIStyle GetCenteredHeaderStyle( float height, float fontRatio, float fontSizePadding, EditorColor color )
        {
            GUIStyle style = new()
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = ( int ) ( fontRatio >= 1 ? fontSizePadding : fontSizePadding * fontRatio ),

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