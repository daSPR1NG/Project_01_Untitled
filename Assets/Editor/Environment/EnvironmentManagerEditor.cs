using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> EnvironmentManager description <summary>
    [CustomEditor(typeof(EnvironmentManager))]
    public class EnvironmentManagerEditor : Editor
    {
        private Transform _possibleEnvironmentTrs;

        public override void OnInspectorGUI()
        {
            EnvironmentManager eM = ( EnvironmentManager ) target;

            AddCameraDataButtonEditor( eM );

            EditorGUILayout.LabelField( "Environment Datas Count : " + eM.EnvironmentDatas().Count.ToString() );
            EditorGUILayout.LabelField( "Environment Camera Datas Count : " + eM.EnvironmentCameraDatas().Count.ToString() );

            EditorGUILayout.Space( 2f );
            using ( new EditorGUILayout.VerticalScope() )
            {
                FocusCameraButtonEditor( eM );
            }
            EditorGUILayout.Space( 10f );

            base.OnInspectorGUI();

            EditorGUILayout.Space( 10f );
            using ( new EditorGUILayout.HorizontalScope() )
            {
                GUILayout.FlexibleSpace();
                RefreshScriptButtonEditor( eM );
                ClearDatasButtonEditor( eM );
                GUILayout.FlexibleSpace();
            }            
        }

        private void AddCameraDataButtonEditor( EnvironmentManager eM )
        {
            using ( new EditorGUILayout.HorizontalScope() )
            {
                _possibleEnvironmentTrs = 
                    ( Transform ) EditorGUILayout.ObjectField( _possibleEnvironmentTrs, typeof( Transform ), true );

                #region Add camera data button

                GUIStyle buttonStyle = new( GUI.skin.button )
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold
                };

                GUIContent content = _possibleEnvironmentTrs.IsNull() ?
                    new( "Possible environment is not set".ToUpper() ) : new( "Add environment data".ToUpper() );

                using ( new EditorGUI.DisabledGroupScope( _possibleEnvironmentTrs.IsNull() ) )
                {
                    if ( GUILayout.Button( content, buttonStyle,
                    GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 15f ) ) )
                    {
                        eM.AddEnvironmentData( _possibleEnvironmentTrs );

                        _possibleEnvironmentTrs = null;
                        Debug.Log( "Add Camera data." );
                    }
                }

                #endregion
            }
        }
        private void FocusCameraButtonEditor( EnvironmentManager eM )
        {
            foreach ( var ecd in eM.EnvironmentCameraDatas() )
            {
                GUIStyle buttonStyle = new( GUI.skin.button )
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                };

                if ( ecd.EnvironmentComponent.IsNull() ) { continue; }

                string environmentName = ecd.EnvironmentComponent.name + " Camera";

                GUIContent content = ecd.IsFocused
                    ? new( environmentName + " is currently focused." )
                    : new( "Focus On : ".ToUpper() + environmentName );

                using ( new EditorGUI.DisabledGroupScope( ecd.IsFocused ) )
                {
                    if ( GUILayout.Button( content, buttonStyle,
                    GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 5f ) ) )
                    {
                        eM.ResetFocusForEachCamera();
                        ecd.Focus();
                    }
                }
            }
        }
        
        private void RefreshScriptButtonEditor( EnvironmentManager eM )
        {
            #region Refresh button

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };

            GUIContent content = new( "Refresh script".ToUpper() );

            if ( GUILayout.Button( content, buttonStyle,
                GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 5f) ) )
            {
                eM.HandleEnvironmentDatasListModifications();
                eM.HandleEnvironmentCameraDatasListModifications();
                Debug.Log( "Refreshing." );
            }

            #endregion
        }
        private void ClearDatasButtonEditor( EnvironmentManager eM )
        {
            #region Clear datas button

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };

            GUIContent content = new( "Clear datas".ToUpper() );

            if ( GUILayout.Button( content, buttonStyle,
                GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) +5f ) ) )
            {
                eM.ResetFocusForEachCamera();

                eM.EnvironmentDatas().Clear();
                eM.EnvironmentCameraDatas().Clear();

                _possibleEnvironmentTrs = null;
                Debug.Log( "Clear datas." );
            }

            #endregion

            EditorGUILayout.Space( 5f );
        }
    }
}