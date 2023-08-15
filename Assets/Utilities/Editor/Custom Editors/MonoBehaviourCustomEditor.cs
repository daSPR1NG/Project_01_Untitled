using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using static dnSR_Coding.Utilities.Helpers.EditorHelper;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> MonoBehaviourCustomEditor description <summary>
    [CustomEditor( typeof( MonoBehaviour ), true )]
    public class MonoBehaviourCustomEditor : UnityEditor.Editor
    {
        private string _assetPath;
        private string _scriptName;

        private const float INITIAL_X_OFFSET = 18;
        private const float INITIAL_Y_OFFSET = 6;
        private const float BUTTON_HEIGHT = 24;

        private void OnEnable()
        {
            _assetPath = AssetDatabase.GetAssetPath( MonoScript.FromMonoBehaviour( target as MonoBehaviour ) );
            _scriptName = MonoScript.FromMonoBehaviour( target as MonoBehaviour ).name;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawBanner( out Vector2 bannerSize );
            HorizontalLineAttribute horizontalLineAttribute = CreateHorizontalLine( bannerSize );

            float offsetFromBanner = bannerSize.y
                + horizontalLineAttribute.Height
                + horizontalLineAttribute.YOffset
                + 10;

            DrawButtonSection(
            offsetFromBanner, out float sectionTotalHeight );

            EditorGUILayout.EndVertical();

            GUILayout.Space( offsetFromBanner + sectionTotalHeight );

            // Base Inspector, minus the default script field
            DrawPropertiesExcluding( serializedObject, new string [] { "m_Script", } );
        }

        #region Banner

        private Rect DrawBanner( out Vector2 contentSize )
        {
            GUIStyle style = new GUIStyle( GUI.skin.label )
            {
                fontStyle = FontStyle.Bold,
                fontSize = 19,
                alignment = TextAnchor.MiddleCenter,
                border = GetDefaultRectOffset(),
            };

            GUIContent content = new GUIContent( _scriptName.ToUpper() );
            contentSize.x = GetCurrentViewWidth( INITIAL_X_OFFSET );
            contentSize.y = style.GetGUIContentSize( content ).y;

            Rect rect = new Rect(
                    INITIAL_X_OFFSET,
                    INITIAL_Y_OFFSET,
                    contentSize.x,
                    contentSize.y );

            EditorGUI.LabelField( rect, content, style );

            return rect;
        }
        private HorizontalLineAttribute CreateHorizontalLine( Vector2 bannerSize )
        {
            HorizontalLineAttribute horizontalLine = new HorizontalLineAttribute( 1, 4, EditorColor.Grey );
            HorizontalLineAttributeDrawer.DrawLine(
                 new Rect(
                        INITIAL_X_OFFSET,
                        bannerSize.y + ( horizontalLine.YOffset * .5f ),
                        0,
                        0 ),
                GetCurrentViewWidth( INITIAL_X_OFFSET ),
                horizontalLine.Height,
                horizontalLine.YOffset,
                horizontalLine.Color );
            return horizontalLine;
        }

        #endregion

        #region Button Section

        private void DrawButtonSection(
            float yPositionOffset,
            out float sectionTotalHeight )
        {
            Rect openInVisualStudioButton;
            Rect pingButton;
            Rect openInFolderButton;

            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button )
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            using ( new EditorGUILayout.HorizontalScope() )
            {
                openInVisualStudioButton = DrawButton(
               new Rect(
                   INITIAL_X_OFFSET ,
                   yPositionOffset,
                   GetCurrentViewWidth( INITIAL_X_OFFSET ) * .2f,
                   BUTTON_HEIGHT ),
               new GUIContent( "OPEN", "Open in visual studio" ),
               buttonStyle,
               EditorColor.Blue,
               () => OpenInVisualStudio() );

                pingButton = DrawButton(
                new Rect(
                    INITIAL_X_OFFSET + GetCurrentViewWidth( INITIAL_X_OFFSET ) * .2f,
                    yPositionOffset,
                    GetCurrentViewWidth( INITIAL_X_OFFSET ) * .4f,
                    BUTTON_HEIGHT ),
                new GUIContent( "Ping asset".ToUpper(), "Ping asset in project" ),
                buttonStyle,
                EditorColor.Blue,
                () => PingAsset() );

                openInFolderButton = DrawButton(
                    new Rect(
                        INITIAL_X_OFFSET + GetCurrentViewWidth( INITIAL_X_OFFSET ) * .6f,
                        yPositionOffset,
                        GetCurrentViewWidth( INITIAL_X_OFFSET ) * .4f,
                        BUTTON_HEIGHT ),
                    new GUIContent(
                        "Show in folder".ToUpper(),
                        "Show asset in folder" ),
                    buttonStyle,
                    EditorColor.Blue,
                () => ShowInExplorer() );
            }

            sectionTotalHeight = BUTTON_HEIGHT;
        }

        private Rect DrawButton(
            Rect buttonRect,
            GUIContent content,
            GUIStyle style,
            EditorColor color,
            System.Action onButtonClick )
        {
            CreateEditorButton(
                buttonRect,
                content,
                style,
                GetColor( color ),
                onButtonClick );

            return buttonRect;
        }

        #endregion

        private void ShowInExplorer()
        {
            try
            {
                EditorUtility.RevealInFinder( _assetPath );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't reveal target file {e.StackTrace} {e.Message}" );
                throw;
            }
        }

        private void OpenInVisualStudio()
        {
            try
            {
                AssetDatabase.OpenAsset( AssetDatabase.LoadMainAssetAtPath( _assetPath ) );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't open target file in VS: {e.StackTrace} {e.Message}" );
                throw;
            }
        }

        private void PingAsset()
        {
            try
            {
                EditorGUIUtility.PingObject( AssetDatabase.LoadMainAssetAtPath( _assetPath ) );
            }
            catch ( System.Exception e )
            {

                Debug.Log( $"Can't open target file {e.StackTrace} {e.Message}" );
                throw;
            }
        }
    }
}