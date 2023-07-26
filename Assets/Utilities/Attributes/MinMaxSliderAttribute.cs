using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> MinMaxSliderAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public MinMaxSliderAttribute ( float min, float max )
        {
            Min = min;
            Max = max;
        }
    }
}