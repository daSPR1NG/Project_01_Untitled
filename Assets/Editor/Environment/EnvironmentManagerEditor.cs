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

            EditorGUILayout.Space( 10f );

            base.OnInspectorGUI();

            RefreshScriptButtonEditor( eM );
            ClearDatasButtonEditor( eM );

            FocusOnOneEnvironmentButtonEditor( eM );
        }

        private void RefreshScriptButtonEditor( EnvironmentManager eM )
        {
            EditorGUILayout.Space( 10f );

            #region Refresh button

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            GUIContent content = new( "Refresh script".ToUpper() );

            if ( GUILayout.Button( content, buttonStyle,
                GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 10f ) ) )
            {
                eM.HandleEnvironmentDatasListModifications();
                eM.HandleEnvironmentCameraDatasListModifications();
                Debug.Log( "Refreshing." );
            }

            #endregion
        }

        private void AddCameraDataButtonEditor( EnvironmentManager eM )
        {
            EditorGUILayout.Space( 10f );

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
                    GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) ) ) )
                    {
                        eM.AddEnvironmentData( _possibleEnvironmentTrs );

                        _possibleEnvironmentTrs = null;
                        Debug.Log( "Add Camera data." );
                    }
                }

                #endregion
            }

            EditorGUILayout.Space( 10f );
        }

        private void ClearDatasButtonEditor( EnvironmentManager eM )
        {
            EditorGUILayout.Space( 5f );

            #region Clear datas button

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };

            GUIContent content = new( "Clear datas".ToUpper() );

            if ( GUILayout.Button( content, buttonStyle,
                GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 5f ) ) )
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

        private void FocusOnOneEnvironmentButtonEditor( EnvironmentManager eM )
        {
            EditorGUILayout.Space( 10f );

            if ( eM.EnvironmentCameraDatas().IsEmpty() || eM.EnvironmentCameraDatas().Count == 1 ) { return; }

            GUIStyle buttonStyle = new( GUI.skin.button )
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            for ( int i = 0; i < eM.EnvironmentCameraDatas().Count; i++ )
            {
                if ( eM.EnvironmentCameraDatas() [ i ].EnvironmentParent.IsNull() ) { continue; }

                string environmentName = !eM.EnvironmentCameraDatas() [ i ].EnvironmentParent.IsNull() 
                    ? eM.EnvironmentCameraDatas() [ i ].EnvironmentParent.transform.name : string.Empty;

                GUIContent content = eM.EnvironmentCameraDatas() [ i ].IsFocused 
                    ? new( environmentName + " is currently focused." )
                    : new( "Focus On : ".ToUpper() + eM.EnvironmentCameraDatas() [ i ].Name );

                using ( new EditorGUI.DisabledGroupScope( eM.EnvironmentCameraDatas() [ i ].IsFocused ) )
                {
                    if ( GUILayout.Button( content, buttonStyle,
                    GUILayout.Height( buttonStyle.CalcHeight( content, EditorGUIUtility.labelWidth ) + 10f ) ) )
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