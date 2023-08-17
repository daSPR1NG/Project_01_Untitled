using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding
{
    ///<summary> ClampValueAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( ClampValueAttribute ), true )]
    public class ClampValueAttributeDrawer : PropertyDrawer, IHelpBoxUser
    {
        private float _value = 0;
        private float _minValue = 0;
        private float _maxValue = 0;

        public Rect? HelpBoxRect { get; set; } = null;
        public float HelpBoxHeight { get; set; } = 0;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            if ( !property.IsPropertyTypeOf( SerializedPropertyType.Float )
                && !property.IsPropertyTypeOf( SerializedPropertyType.Integer ) )
            {
                CreateHelpBox(
                     new Rect
                     ( position.x, position.y, position.width, position.height),
                     $"{property.name} can only be a float or a int !",
                     MessageType.Error,
                     out Rect helpBox );

                IHelpBoxUser.IHelpBoxUserExtension.InitHelpBoxHeight( this, helpBox.height );
                HelpBoxRect = helpBox;

                return;
            }

            // Displays the property name
            EditorGUI.LabelField( position, new GUIContent( property.name ) );

            // Get a clampField representing the zone where we can draw our elements
            Rect containerRect = GetFieldContainer( position );
            float resetButtonSize = containerRect.width * .075f;

            EditorGUI.indentLevel = 0;

            // 100% == containerRect.width

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>( RESET_ICON );
            EditorGUIUtility.SetIconSize( new Vector2( 12, 12 ) );

            CreateEditorButton( new Rect(
                    containerRect.xMin,
                    containerRect.y,
                    resetButtonSize,
                    containerRect.height
                    ),
                    new GUIContent( icon, "Reset all value to 0." ),
                    GUI.skin.button,
                    GetColor( EditorColor.Black ),
                    () =>
                    {
                        ResetValues();
                    } );

            DrawClampField(
                new Rect(
                    containerRect.xMin + resetButtonSize,
                    containerRect.y,
                    containerRect.width * 0.325f,
                    containerRect.height
                    ),
                new GUIContent( "Value" ),
                out Rect valueField );
            _value = property.IsPropertyTypeOf( SerializedPropertyType.Integer )
                ? EditorGUI.IntField( valueField, ( int ) _value )
                : EditorGUI.FloatField( valueField, _value );
            _value = Mathf.Clamp( _value, _minValue, _maxValue );

            DrawClampField(
                new Rect(
                    containerRect.xMin + containerRect.width * .4f,
                    containerRect.y,
                    containerRect.width * .3f,
                    containerRect.height
                    ),
                new GUIContent( "Min" ),
                out Rect minValueField );
            _minValue = property.IsPropertyTypeOf( SerializedPropertyType.Integer )
                ? EditorGUI.IntField( minValueField, ( int ) _minValue )
                : EditorGUI.FloatField( minValueField, _minValue );
            _minValue = Mathf.Clamp( _minValue, _minValue, _maxValue );

            DrawClampField(
                new Rect(
                    containerRect.xMin + containerRect.width * .7f,
                    containerRect.y,
                    containerRect.width * .3f,
                    containerRect.height
                    ),
                new GUIContent( "Max" ),
                out Rect maxValueField );
            _maxValue = property.IsPropertyTypeOf( SerializedPropertyType.Integer )
                ? EditorGUI.IntField( maxValueField, ( int ) _maxValue )
                : EditorGUI.FloatField( maxValueField, _maxValue );
            _maxValue = Mathf.Clamp( _maxValue, _minValue, _maxValue );
        }

        private Rect DrawClampField( Rect rect, GUIContent content, out Rect valueField )
        {
            float labelPadding = 8;
            float contenSize = GetGUIContentSize( GUIStyle.none, content ).x + labelPadding * 2;

            EditorGUI.indentLevel = 0;

            Rect labelRect = new Rect(
                    rect.xMin,
                    rect.y,
                    contenSize,
                    rect.height
                    );

            GUIStyle labelStyle = new GUIStyle( GUI.skin.label )
            {
                alignment = TextAnchor.MiddleCenter,
            };
            labelStyle.normal.textColor = GetColor( EditorColor.White );

            CreateEditorBackground( labelRect, GUI.skin.box, GetColor( EditorColor.Black ) );
            EditorGUI.LabelField( labelRect, content, labelStyle );

            valueField = new Rect(
                    rect.xMin + contenSize,
                    rect.y,
                    rect.width - contenSize,
                    rect.height );

            return rect;
        }

        private void ResetValues()
        {
            _value = 0;
            _minValue = 0;
            _maxValue = 0;
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUI.GetPropertyHeight( property, label );
        }
    }
}