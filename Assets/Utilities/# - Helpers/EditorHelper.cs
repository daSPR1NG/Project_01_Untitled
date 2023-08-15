using UnityEditor;
using UnityEngine;

namespace dnSR_Coding.Utilities.Helpers
{
    public enum EditorColor
    {
        Clear,
        White,
        Grey,
        Black,
        Orange,
        Red,
        Yellow,
        Green,
        Blue,
    }

    ///<summary> EditorHelper description <summary>
    public class EditorHelper
    {
        public const int DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET = 2;
        private const float PROPERTY_DEFAULT_FIELD_WIDTH = 50f;

        public const string FOCUS_ON_ICON = "Assets/Utilities/Editor/Icons/FocusOnIcon.png";
        public const string OPEN_IN_FOLDER_ICON = "Assets/Utilities/Editor/Icons/OpenInFolderIcon.png";

        #region Dimensions Utils - Width

        public static float GetInspectorWidthBasedOnIndentation( Rect position )
        {
            return IsIndented() ?
                GetInspectorWidth_BasedOnIndentation( position ) : GetInspectorWidth_BasedOnPositionX( position );
        }

        private static float GetInspectorWidth_BasedOnIndentation( Rect position )
        {
            return GetInspectorWidth_BasedOnPositionX( position ) - ( position.min.x * EditorGUI.indentLevel ) / 3f;
        }
        private static float GetInspectorWidth_BasedOnPositionX( Rect position )
        {
            return position.xMax - position.xMin;
        }

        public static float GetDefaultPropertyFieldWidth()
        {
            return PROPERTY_DEFAULT_FIELD_WIDTH;
        }

        public static float GetCurrentViewWidth( float xOffset = 0 )
        {
            return EditorGUIUtility.currentViewWidth - xOffset - 4;
        }

        #endregion

        #region Dimensions Utils - Offset(s)

        /// <summary>
        /// Use this function to get the value of the indent
        /// </summary>
        /// <param name="sourceRect"></param>
        /// <returns> The offset value corresponding to the indent level. </returns>
        public static float GetXOffset_BasedOnIndentation( Rect sourceRect )
        {
            Rect indentRect = EditorGUI.IndentedRect( sourceRect );
            float xOffset = indentRect.x - sourceRect.x;

            return xOffset;
        }

        #endregion

        #region Elements Utils - Get

        public static Color GetColor( EditorColor color )
        {
            switch ( color )
            {
                case EditorColor.Clear: return new Color32( 0, 0, 0, 0 );

                case EditorColor.White: return Helper.GetColorFromHexCode( "#f3f5f0" );

                case EditorColor.Grey: return Helper.GetColorFromHexCode( "#808080" );

                case EditorColor.Black: return Helper.GetColorFromHexCode( "#2f2e2e" );

                case EditorColor.Orange: return Helper.GetColorFromHexCode( "#ffae00" );

                case EditorColor.Red: return Helper.GetColorFromHexCode( "#b32424" );

                case EditorColor.Yellow: return Helper.GetColorFromHexCode( "#ffff19" );

                case EditorColor.Green: return Helper.GetColorFromHexCode( "#61c928" );

                case EditorColor.Blue: return Helper.GetColorFromHexCode( "#286ec9" );

                default: return new Color32( 0, 0, 0, 255 );
            }
        }

        public static RectOffset GetDefaultRectOffset()
        {
            return new RectOffset( 0, 0, 0, 0 );
        }

        /// <summary>
        /// Use this function to get a container where to put elements in it, eg: buttons, fields, etc...
        /// </summary>
        /// <param name="position"> Represents the position passed by OnGUI. </param>
        /// <param name="xOffset"> Represents the offset from the left => label posiiton. </param>
        /// <param name="totalWidth"> Represents the total width used by the property. </param>
        /// <returns> A rect where you can add elements in it.</returns>
        public static Rect GetTotalRectForElements(
            Rect position,
            float xOffset,
            float totalWidth )
        {
            Rect rect = new Rect(
                xOffset,
                position.y,
                totalWidth,
                position.height
                );

            return rect;
        }

        public static Vector2 GetGUIContentSize( GUIStyle style, GUIContent content )
        {
            return style.CalcSize( content );
        }

        #endregion

        #region Elements Utils - Creation

        public static Texture2D CreateBackground( EditorColor color )
        {
            Color c = GetColor( color );

            Texture2D texture = new Texture2D( 1, 1 );
            texture.SetPixel( 0, 0, c );
            texture.Apply();
            return texture;
        }

        public static bool CreateEditorButton( Rect rect, GUIContent content, GUIStyle style, Color color = default, System.Action onClickingButton = null )
        {
            GUI.backgroundColor = color;

            if ( GUI.Button( rect, content, style ) )
            {
                onClickingButton?.Invoke();
                return true;
            }

            GUI.backgroundColor = Color.white;

            return false;
        }


        #endregion

        public static bool IsIndented()
        {
            return EditorGUI.indentLevel >= 1;
        }

        public static float GetIndentedLevel( int offset = 0 )
        {
            return EditorGUI.indentLevel - offset;
        }
    }
}