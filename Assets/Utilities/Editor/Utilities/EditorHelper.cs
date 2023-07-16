using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace dnSR_Coding.Utilities
{
    public enum EditorColor
    {
        Clear,
        White,
        Grey,
        Black,
    }

    ///<summary> EditorHelper description <summary>
    public class EditorHelper
    {
        public static Color GetColor( EditorColor color )
        {
            switch ( color )
            {
                case EditorColor.Clear:
                    return new Color32( 0, 0, 0, 0 );
                case EditorColor.White:
                    return new Color32( 255, 255, 255, 255 );
                case EditorColor.Grey:
                    return new Color32( 128, 128, 128, 255 );
                case EditorColor.Black:
                    return new Color32( 0, 0, 0, 255 );
                default: 
                    return new Color32( 0, 0, 0, 255 );
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
    }
}