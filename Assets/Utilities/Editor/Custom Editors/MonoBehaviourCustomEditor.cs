//using UnityEngine;
//using UnityEditor;
//using dnSR_Coding.Utilities.Helpers;
//using dnSR_Coding.Utilities.Attributes;
//using static dnSR_Coding.Utilities.Helpers.EditorHelper;
//using Sirenix.OdinInspector.Editor;

//namespace dnSR_Coding.Utilities.Editor
//{
//    ///<summary> MonoBehaviourCustomEditor description <summary>
//    [CustomEditor( typeof( CustomMonoBehaviour ), true )]
//    public class MonoBehaviourCustomEditor : OdinEditor
//    {
//        private string _assetPath;
//        private string _scriptName;

//        private const float INITIAL_X_OFFSET = 18;
//        private const float INITIAL_Y_OFFSET = 6;
//        private const float BUTTON_HEIGHT = 24;

//        protected override void OnEnable()
//        {
//            base.OnEnable();
//            _assetPath = AssetDatabase.GetAssetPath( MonoScript.FromMonoBehaviour( ( MonoBehaviour ) target ) );
//            _scriptName = MonoScript.FromMonoBehaviour( ( MonoBehaviour ) target ).name;
//        }

//        public override void OnInspectorGUI()
//        {
//            EditorGUILayout.BeginVertical();

//            DrawBanner( out Vector2 bannerSize );
//            HorizontalLineAttribute horizontalLineAttribute = CreateHorizontalLine( bannerSize );

//            float offsetFromBanner = bannerSize.y
//                + horizontalLineAttribute.Height
//                + horizontalLineAttribute.YOffset
//                + 10;

//            DrawButtonSection(
//            offsetFromBanner, out float sectionTotalHeight );

//            EditorGUILayout.EndVertical();

//            GUILayout.Space( offsetFromBanner + sectionTotalHeight );

//            DrawPropertiesExcluding( serializedObject, "m_Script" );
//            serializedObject.ApplyModifiedProperties();

//            Debug.Log( "COUCOU" );
//        }

//        #region Banner

//        private Rect DrawBanner( out Vector2 contentSize )
//        {
//            GUIStyle style = new GUIStyle( GUI.skin.label )
//            {
//                fontStyle = FontStyle.Bold,
//                fontSize = 19,
//                alignment = TextAnchor.MiddleCenter,
//                border = GetDefaultRectOffset(),
//            };

//            GUIContent content = new GUIContent( _scriptName.ToUpper() );
//            contentSize.x = GetCurrentViewWidth( INITIAL_X_OFFSET );
//            contentSize.y = style.GetGUIContentSize( content ).y;

//            Rect rect = new Rect(
//                    INITIAL_X_OFFSET,
//                    INITIAL_Y_OFFSET,
//                    contentSize.x,
//                    contentSize.y );

//            EditorGUI.LabelField( rect, content, style );

//            return rect;
//        }
//        private HorizontalLineAttribute CreateHorizontalLine( Vector2 bannerSize )
//        {
//            HorizontalLineAttribute horizontalLine = new HorizontalLineAttribute( 1, 4, EditorColor.Grey );
//            HorizontalLineAttributeDrawer.DrawLine(
//                 new Rect(
//                        INITIAL_X_OFFSET,
//                        bannerSize.y + ( horizontalLine.YOffset * .5f ),
//                        0,
//                        0 ),
//                GetCurrentViewWidth( INITIAL_X_OFFSET ),
//                horizontalLine.Height,
//                horizontalLine.YOffset,
//                horizontalLine.Color );
//            return horizontalLine;
//        }

//        #endregion

//        #region Button Section

//        private void DrawButtonSection(
//            float yPositionOffset,
//            out float sectionTotalHeight )
//        {
//            Rect openInVisualStudioButton;
//            Rect pingButton;
//            Rect openInFolderButton;

//            GUIStyle buttonStyle = new GUIStyle( GUI.skin.button )
//            {
//                fontSize = 11,
//                fontStyle = FontStyle.Bold,
//                alignment = TextAnchor.MiddleCenter,
//            };

//            using ( new EditorGUILayout.HorizontalScope() )
//            {
//                openInVisualStudioButton = DrawButton(
//               new Rect(
//                   INITIAL_X_OFFSET,
//                   yPositionOffset,
//                   GetCurrentViewWidth( INITIAL_X_OFFSET ) * .2f,
//                   BUTTON_HEIGHT ),
//               new GUIContent( "OPEN", "Open in visual studio" ),
//               buttonStyle,
//               EditorColor.Blue,
//               () => OpenInVisualStudio( _assetPath ) );

//                pingButton = DrawButton(
//                new Rect(
//                    INITIAL_X_OFFSET + GetCurrentViewWidth( INITIAL_X_OFFSET ) * .2f,
//                    yPositionOffset,
//                    GetCurrentViewWidth( INITIAL_X_OFFSET ) * .4f,
//                    BUTTON_HEIGHT ),
//                new GUIContent( "Ping asset".ToUpper(), "Ping asset in project" ),
//                buttonStyle,
//                EditorColor.Blue,
//                () => PingAsset( _assetPath ) );

//                openInFolderButton = DrawButton(
//                    new Rect(
//                        INITIAL_X_OFFSET + GetCurrentViewWidth( INITIAL_X_OFFSET ) * .6f,
//                        yPositionOffset,
//                        GetCurrentViewWidth( INITIAL_X_OFFSET ) * .4f,
//                        BUTTON_HEIGHT ),
//                    new GUIContent(
//                        "Show in folder".ToUpper(),
//                        "Show asset in folder" ),
//                    buttonStyle,
//                    EditorColor.Blue,
//                () => ShowInExplorer( _assetPath ) );
//            }

//            sectionTotalHeight = BUTTON_HEIGHT;
//        }

//        private Rect DrawButton(
//            Rect buttonRect,
//            GUIContent content,
//            GUIStyle style,
//            EditorColor color,
//            System.Action onButtonClick )
//        {
//            CreateEditorButton(
//                buttonRect,
//                content,
//                style,
//                GetColor( color ),
//                onButtonClick );

//            return buttonRect;
//        }

//        #endregion
//    }
//}