using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Attributes;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> MinMaxSliderAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( MinMaxSliderAttribute ), true )]
    public class MinMaxSliderAttributeDrawer : PropertyDrawer
    {
        private float _minValue = 0;
        private float _maxValue = 0;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            MinMaxSliderAttribute minMaxSliderAttribute = attribute as MinMaxSliderAttribute;

            float valueFieldWidth = GetDefaultPropertyFieldWidth() + GetXOffset_BasedOnIndentation( position );
            float xOffsetFromLabel = EditorGUIUtility.labelWidth - GetXOffset_BasedOnIndentation( position );

            float sliderBaseWidth = GetInspectorWidthBasedOnIndentation( position );
            float sliderPadding = 5;
            float sliderXOffsetFromLabel = xOffsetFromLabel + valueFieldWidth + sliderPadding + DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;
            float sliderSizeWithNoIndent = sliderBaseWidth - EditorGUIUtility.labelWidth - ( ( GetDefaultPropertyFieldWidth() + sliderPadding ) * 2 ) - DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;

            GUIContent labelName = new GUIContent( $"{property.displayName}" );

            Rect minValueRect = new Rect(
                position.xMin + xOffsetFromLabel + DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET,
                position.y,
                valueFieldWidth,
                GetPropertyHeight( property, label ) );
            _minValue = EditorGUI.FloatField( minValueRect, _minValue );

            Rect maxValueRect = new Rect(
                position.xMax - valueFieldWidth,
                position.y,
                valueFieldWidth,
                GetPropertyHeight( property, label )
                );
            _maxValue = EditorGUI.FloatField( maxValueRect, _maxValue );

            Rect sliderRect = new Rect(
                position.xMin + sliderXOffsetFromLabel - GetXOffset_BasedOnIndentation( position ),
                position.y,
                sliderSizeWithNoIndent + ( GetXOffset_BasedOnIndentation( position ) * 2 ),
                GetPropertyHeight( property, label )
                );

            EditorGUI.LabelField( position, labelName );

            EditorGUI.BeginProperty( position, label, property );

            CheckSliderValuesAndClampIt( minMaxSliderAttribute.Min, minMaxSliderAttribute.Max );
            
            EditorGUI.MinMaxSlider( sliderRect, ref _minValue, ref _maxValue, minMaxSliderAttribute.Min, minMaxSliderAttribute.Max );

            EditorGUI.EndProperty();
        }

        private void CheckSliderValuesAndClampIt( float minLimit, float maxLimit )
        {
            EditorGUI.BeginChangeCheck();
            if ( _minValue < minLimit )
            {
                _minValue = minLimit;
            }

            if ( _maxValue < minLimit && !( _maxValue > maxLimit )
                || _maxValue > maxLimit )
            {
                _maxValue = minLimit;
            }
            EditorGUI.EndChangeCheck();
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}