using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;
using dnSR_Coding.Utilities.Editor;

namespace dnSR_Coding
{
    ///<summary> HiddenFieldAttributeDrawer description <summary>
    [CustomPropertyDrawer( typeof( HiddenFieldAttribute ), true )]
    public class HiddenFieldAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            MonoBehaviour target = ( MonoBehaviour ) property.serializedObject.targetObject;

            GUIContent content = new GUIContent()
            {
                text = $"Edit script".ToUpper(),
                tooltip = $"Open {target.name} in visual studio to edit it."
            };
            Vector2 contentSize = GetGUIContentSize( GUIStyle.none, content );

            float yOffset = 5;

            Rect testRect = new Rect(
                position.x,
                position.y + yOffset,
                contentSize.x * 1.5f,
                contentSize.y * 1.5f );

            if ( Button( testRect, content, out Rect decorator ) )
            {
                string path = AssetDatabase.GetAssetPath( MonoScript.FromMonoBehaviour( target ) );
                OpenInVisualStudio( path );
            }

            GUILayout.Space( testRect.height + yOffset * 2 + decorator.height );
        }

        private bool Button( Rect position, GUIContent label, out Rect decorator )
        {
            decorator = new Rect();

            GUIStyle buttonStyle = GUIStyles.CreateBackgroundFromStyle(
                GUI.skin.verticalScrollbarUpButton.ToString(),
                EditorColor.Black );

            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 12;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUIUtility.AddCursorRect( position, MouseCursor.Link );

            if ( Event.current.type == EventType.Repaint )
            {
                GUIStyle decoratorStyle = GUIStyles.CreateBackgroundFromStyle(
                    GUI.skin.verticalScrollbarUpButton.ToString(),
                    EditorColor.Orange );

                decorator = new Rect( position.x, position.yMax, position.width, 2 );
                decoratorStyle.Draw(
                    decorator, GUIContent.none,
                    false, false, false, false );
            }

            return GUI.Button( position, label, buttonStyle );
        }

        ///<inheritdoc/>
        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return 0;
        }
    }
}