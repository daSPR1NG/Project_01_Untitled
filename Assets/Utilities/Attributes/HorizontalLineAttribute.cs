using System;
using UnityEngine;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding.Utilities
{
    ///<summary> HorizontalLineAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
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