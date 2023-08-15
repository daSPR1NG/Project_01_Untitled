using UnityEngine;
using UnityEditor;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    ///<summary> ClampValueAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( ClampValueAttribute ), true )]
    public class ClampValueAttributeDrawer : PropertyDrawer
    {
        private int _intValue = 0;
        private float _floatValue = 0;
        private float _minValue = 0;
        private float _maxValue = 1;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            // Displays the property name
            EditorGUI.LabelField( position, new GUIContent( property.name ) );

            GUIContent content = new GUIContent( "MIN" );

            // Get a rect representing the zone where we can draw our elements
            float offsetFromInitialPosition = position.xMin + EditorGUIUtility.labelWidth
                - GetXOffset_BasedOnIndentation( position )
                + DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;

            float totalWidth = position.width - EditorGUIUtility.labelWidth
                + GetXOffset_BasedOnIndentation( position )
                - DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET;

            Rect totalRect = GetTotalRectForElements( position, offsetFromInitialPosition, totalWidth );

            float indentWidthCorrection = GetXOffset_BasedOnIndentation( totalRect );

            DrawMinValueField( ref position, ref totalRect, indentWidthCorrection );

            Rect maxValueRect = new Rect(
                totalRect.xMin + ExtMathfs.Ceil( totalRect.width * .33f ) + DEFAULT_MIN_HORIZONTAL_LAYOUT_OFFSET * 1.5f,
                position.y,
                ExtMathfs.Ceil( totalRect.width * .33f ) + indentWidthCorrection / 2,
                position.height
                );

            Rect valueRect = new Rect(
                totalRect.xMax - ExtMathfs.Ceil( totalRect.width * .33f ) - indentWidthCorrection / 2,
                position.y,
                ExtMathfs.Ceil( totalRect.width * .33f ) + indentWidthCorrection / 2,
                position.height
                );

            EditorGUI.LabelField( totalRect, content, new GUIStyle( GUI.skin.box ) );
            EditorGUI.LabelField( maxValueRect, content, new GUIStyle( GUI.skin.button ) );
            EditorGUI.LabelField( valueRect, content, new GUIStyle( GUI.skin.button ) );
        }

        private Rect DrawMinValueField( ref Rect position, ref Rect totalRect, float indentWidthCorrection )
        {
            Rect rect = new Rect(
                totalRect.xMin,
                position.y,
                ExtMathfs.Ceil( totalRect.width * .33f ) + indentWidthCorrection,
                position.height );

            GUIContent content = new GUIContent( "Min" );
            float gap = 6;

            EditorGUI.LabelField( rect, content );
            _minValue = EditorGUI.FloatField(
                new Rect(
                    rect.xMin + GetGUIContentSize( GUIStyle.none, content ).x + gap,
                    rect.y,
                    rect.width - GetGUIContentSize( GUIStyle.none, content ).x - gap,
                    rect.height ),
                _minValue );

            return rect;
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }

        private void DrawValueAsIntField( Rect position, SerializedProperty property )
        {
            if ( !IsPropertyTypeOfInt( property ) )
            {
                Debug.Log( $"{property.name} is a float" );
                return;
            }

            _intValue = EditorGUI.IntField(
                    position, new GUIContent( $"{property.name} " + $"<{_maxValue}, {_minValue}>" ), _intValue );

            if ( _intValue < _minValue )
            {
                Debug.Log( "INFERIOR" );
                _intValue = EditorGUI.IntField( position, ( int ) _minValue );
            }
            else if ( _intValue > _maxValue )
            {
                Debug.Log( "SUPERIOR" );
                _intValue = EditorGUI.IntField( position, ( int ) _maxValue );
            }
        }

        private void DrawValueAsFloatField( Rect position, SerializedProperty property )
        {
            if ( IsPropertyTypeOfInt( property ) )
            {

                Debug.Log( $"{property.name} is an int" );
                return;
            }

            _floatValue = EditorGUI.FloatField(
                    position, new GUIContent( $"{property.name} " + $"<{_maxValue}, {_minValue}>" ), _floatValue );

            if ( _floatValue < _minValue )
            {
                Debug.Log( "INFERIOR" );
                _floatValue = EditorGUI.FloatField( position, _minValue );
            }
            else if ( _floatValue > _maxValue )
            {
                Debug.Log( "SUPERIOR" );
                _floatValue = EditorGUI.FloatField( position, _maxValue );
            }
        }

        private bool IsPropertyTypeOfInt( SerializedProperty property )
        {
            return property.propertyType == SerializedPropertyType.Integer;
        }
    }
}