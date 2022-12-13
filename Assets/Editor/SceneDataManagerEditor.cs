using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> SceneDataManagerEditor description <summary>
    [CustomEditor(typeof(SceneDataManager))]
    public class SceneDataManagerEditor : Editor
    {
        private int sceneId = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space( 10f );

            SceneDataManager sceneDataManager = ( SceneDataManager ) target;

            EditorGUILayout.LabelField( "Button to load a scene corresponding to the given id".ToUpper() );

            using ( new EditorGUILayout.HorizontalScope() )
            {
                GUIStyle buttonStyle = new( GUI.skin.button )
                {
                    fontStyle = FontStyle.Bold
                };

                GUIContent buttonContent = new( "Load Specific Scene by ID".ToUpper() );

                if ( GUILayout.Button( buttonContent, buttonStyle,
                    GUILayout.Width( EditorGUIUtility.labelWidth + ( EditorGUIUtility.labelWidth * 1.25f ) ) ) )
                {
                    if ( !Application.isPlaying ) { return; }
                    sceneDataManager.LoadSpecificScene( sceneId );
                }                

                sceneId = EditorGUILayout.IntField( sceneId );
            }
        }
    }
}