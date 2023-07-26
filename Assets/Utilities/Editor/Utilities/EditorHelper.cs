using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace dnSR_Coding.Utilities
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

        public static float GetInspectorWidth_BasedOnIndentation( Rect position ) {
            return GetInspectorWidth_BasedOnPositionX( position ) - ( position.min.x * EditorGUI.indentLevel ) / 3f;
        }

        public static float GetInspectorWidth_BasedOnPositionX( Rect position ) {
            return position.max.x - position.min.x;
        }

        public static RectOffset DefaultRectOffset() {
            return new RectOffset( 0, 0, 0, 0 );
        }

        public static Texture2D CreateColorPixel( Color color )
        {
            Texture2D texture = new Texture2D( 1, 1 );
            texture.SetPixel( 0, 0, color );
            texture.Apply();
            return texture;
        }

        public static bool IsIndented() {
            return EditorGUI.indentLevel >= 1;
        }

        public static Vector2 GetGUIContentSize( GUIContent content )
        {
            GUIStyle style = new();
            return style.CalcSize( content );
        }
    }

    public class InspectorUtility
    {
        public static InspectorMode GetInspectorMode()
        {
            InspectorMode inspectorMode;
            
            System.Type type = Assembly.GetAssembly( typeof( UnityEditor.Editor ) ).GetType( "UnityEditor.InspectorWindow" );
            FieldInfo field = type.GetField( "m_InspectorMode", BindingFlags.NonPublic | BindingFlags.Instance );

            inspectorMode = ( InspectorMode ) field.GetValue( EditorWindow.GetWindow( type ) );

            return inspectorMode;
        }
    }
}