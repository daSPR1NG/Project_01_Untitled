using System;
using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary> HorizontalLineAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = false )]
    public class HorizontalLineAttribute : PropertyAttribute
    {
        private const float DEFAULT_HEIGHT = .25f;
        private const float DEFAULT_Y_OFFSET = 4f;
        private const EditorColor DEFAULT_COLOR = EditorColor.Clear;

        public float Height { get; private set; }
        public float YOffset { get; private set; }
        public EditorColor Color { get; private set; }

        public HorizontalLineAttribute ( 
            float height = DEFAULT_HEIGHT, 
            float yOffset = DEFAULT_Y_OFFSET,
            EditorColor color = DEFAULT_COLOR )
        {
            Height = height;
            YOffset = yOffset;
            Color = color;
        }

        public HorizontalLineAttribute(
            float height = DEFAULT_HEIGHT,
            EditorColor color = DEFAULT_COLOR )
        {
            Height = height;
            YOffset = DEFAULT_Y_OFFSET;
            Color = color;
        }
    }
}