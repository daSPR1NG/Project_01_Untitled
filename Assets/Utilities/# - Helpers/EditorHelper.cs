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
#if UNITY_EDITOR
        public const int DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET = 2;
        internal const float PROPERTY_DEFAULT_FIELD_WIDTH = 50f;

        public const string RESET_ICON = "Assets/Utilities/Editor/Icons/ResetIcon.png";
        internal const string FOCUS_ON_ICON = "Assets/Utilities/Editor/Icons/FocusOnIcon.png";
        internal const string OPEN_IN_FOLDER_ICON = "Assets/Utilities/Editor/Icons/OpenInFolderIcon.png";

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
        /// <param name="position"> Represents the totalRect passed by OnGUI. </param>
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

        /// <summary>
        /// This is used to create a proper zone in which we can draw what we want - 
        /// (It represents the right zone).
        /// </summary>
        /// <param name="totalRect">This is the total zone of the field.</param>
        /// <returns></returns>
        public static Rect GetFieldContainer( Rect totalRect )
        {
            float offsetFromLabel = totalRect.xMin + EditorGUIUtility.labelWidth + DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;

            float containerTotalWidth = totalRect.width - EditorGUIUtility.labelWidth - DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;

            Rect container = GetTotalRectForElements( totalRect, offsetFromLabel, containerTotalWidth );

            return container;
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

        public static Texture2D CreateBackgroundWithGradient(
            int width,
            int height,
            Color leftColor,
            Color rightColor )
        {
            Texture2D texture2D = new Texture2D( width, height, TextureFormat.ARGB32, false );
            texture2D.hideFlags = HideFlags.HideAndDontSave;

            Color32 [] colors = new Color32 [ width * height ];

            for ( int i = 0; i < width; i++ )
            {
                Color color = Color.Lerp( leftColor, rightColor, i / ( float ) ( width - 1 ) );

                for ( int j = 0; j < height; j++ )
                {
                    colors [ j * width + i ] = color;
                }
            }

            texture2D.SetPixels32( colors );
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.Apply();

            return texture2D;
        }

        public static bool CreateEditorButton(
            Rect rect,
            GUIContent content,
            GUIStyle style,
            Color color = default,
            System.Action onClickingButton = null )
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

        public static bool CreateEditorBackground(
            Rect rect,
            GUIStyle style,
            Color color = default )
        {
            GUI.backgroundColor = color;

            EditorGUI.BeginDisabledGroup( true );

            if ( GUI.Button( rect, GUIContent.none, style ) )
            {
                return true;
            }

            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = Color.white;

            return false;
        }

        public static void CreateHelpBox(
            Rect position, string message,
            UnityEditor.MessageType messageType,
            out Rect helpBox )
        {
            Vector2 initialIconSize = EditorGUIUtility.GetIconSize();

            GUIContent content = new GUIContent( ' ' + message );
            helpBox = new Rect(
                    position.xMin,
                    position.y,
                    position.width,
                    position.height );

            int fontSize = 12;

            EditorGUIUtility.SetIconSize( new Vector2( fontSize + 2, fontSize + 2 ) );

            GUIStyle style = EditorStyles.helpBox;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = fontSize;

            EditorGUI.HelpBox( helpBox, content.text, messageType );

            EditorGUIUtility.SetIconSize( initialIconSize );
        }

        #endregion

        #region Global Methods

        public static void ShowInExplorer( string assetPath )
        {
            try
            {
                EditorUtility.RevealInFinder( assetPath );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't reveal target file {e.StackTrace} {e.Message}" );
                throw;
            }
        }

        public static void OpenInVisualStudio( string assetPath, int lineNumber = 0 )
        {
            try
            {
                AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( assetPath ), lineNumber );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't open target file in VS: {e.StackTrace} {e.Message}" );
                throw;
            }
        }

        public static void PingAsset( string assetPath )
        {
            try
            {
                EditorGUIUtility.PingObject( AssetDatabase.LoadMainAssetAtPath( assetPath ) );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't open target file {e.StackTrace} {e.Message}" );
                throw;
            }
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
#endif
    }
}