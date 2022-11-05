using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> EnvironmentManager description <summary>
    [CustomEditor(typeof(EnvironmentManager))]
    public class EnvironmentManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EnvironmentManager eM = (EnvironmentManager)target;

            base.OnInspectorGUI();

            EditorGUILayout.Space( 10f );

            if ( eM.EnvironmentCameraDatas().IsEmpty() ) { return; }

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            for ( int i = 0; i < eM.EnvironmentCameraDatas().Count; i++ )
            {
                GUIContent content = new( eM.EnvironmentCameraDatas() [ i ].Name );

                using ( new EditorGUI.DisabledGroupScope( eM.EnvironmentCameraDatas() [ i ].IsFocused ) )
                {
                    if ( GUILayout.Button(
                    "Focus On : ".ToUpper() + content,
                    buttonStyle,
                    GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 5f ) ) )
                    {
                        eM.ResetFocusForEachCamera();
                        eM.EnvironmentCameraDatas() [ i ].Focus();
                    }
                }                
            }

            EditorGUILayout.Space( 10f );
        }
    }
}