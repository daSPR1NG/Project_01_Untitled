using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities;
using static dnSR_Coding.Utilities.EditorHelper;

namespace dnSR_Coding
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

            if ( _minValue < minMaxSliderAttribute.Min ) {
                _minValue = minMaxSliderAttribute.Min;
            }

            if ( _maxValue < minMaxSliderAttribute.Min && !( _maxValue > minMaxSliderAttribute.Max ) 
                || _maxValue > minMaxSliderAttribute.Max ) 
            {
                _maxValue = minMaxSliderAttribute.Min;
            }

            Debug.Log( $"Min: {minMaxSliderAttribute.Min}" );
            Debug.Log( $"Max: {minMaxSliderAttribute.Max}" );

            GUIContent labelName = new GUIContent( $"{property.displayName}" );
            EditorGUI.LabelField( position, labelName );

            EditorGUI.BeginProperty( position, label, property );

            Rect minValueRect = new Rect( 
                position.x + GetGUIContentSize( labelName ).x + 12, 
                position.y, 
                50, 
                GetPropertyHeight( property, label ) );
            _minValue = EditorGUI.FloatField( minValueRect, _minValue );

            float sliderBaseWidth = IsIndented() ? GetInspectorWidth_BasedOnIndentation( position ) : GetInspectorWidth_BasedOnPositionX( position );
            Rect sliderRect = new Rect( 
                position.x + minValueRect.x - 6,
                position.y,
                sliderBaseWidth / 2 + 12,
                GetPropertyHeight( property, label )
                );
            EditorGUI.MinMaxSlider( sliderRect, ref _minValue, ref _maxValue, minMaxSliderAttribute.Min, minMaxSliderAttribute.Max );

            Rect maxValueRect = new Rect(
                position.xMax - 50,
                position.y,
                50,
                GetPropertyHeight( property, label )
                );
            _maxValue = EditorGUI.FloatField( maxValueRect, _maxValue );

            if ( _maxValue < _minValue ) {
                _maxValue = _minValue;
            }

            EditorGUI.EndProperty();
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}