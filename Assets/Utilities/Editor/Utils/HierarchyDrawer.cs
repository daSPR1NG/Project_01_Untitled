//using UnityEngine;
//using UnityEditor;
//using UnityEngine.SceneManagement;
//using UnityEditor.SceneManagement;
//using System.Collections.Generic;
//using System;
//using dnSR_Coding.Utilities.Helpers;
//using System.Linq;

//namespace dnSR_Coding.Utilities.Editor
//{
//    [InitializeOnLoad]
//    public static class HierarchyDrawer
//    {
//        private static bool _isInitialized = false;
//        private static InstanceInfo _currentInstanceInfo;
//        private static readonly Dictionary<int, InstanceInfo> _sceneInstances = new();

//        private const int ICON_SIZE = 16;
//        private const float SEPARATOR_HEIGHT = .5f;
//        private const float LABEL_BACKGROUND_HEIGHT_REDUCTION = 2f;
//        private const float ELEMENTS_WIDTH_OFFSET = 8f;

//        static HierarchyDrawer() => Init();

//        [Serializable]
//        struct InstanceInfo
//        {
//            public string GoName;
//            public bool IsGoActive;
//            public EditorColor HierarchyColor;
//        }

//        private static void Init()
//        {
//            if ( _isInitialized )
//            {
//                EditorApplication.hierarchyWindowItemOnGUI -= DrawCore;
//                EditorApplication.hierarchyChanged -= GetHierarchyElements;

//                return;
//            }

//            EditorApplication.hierarchyWindowItemOnGUI += DrawCore;
//            EditorApplication.hierarchyChanged += GetHierarchyElements;
//        }

//        private static void DrawCore( int instanceID, Rect rect )
//        {
//            _isInitialized = true;

//            if ( !_sceneInstances.ContainsKey( instanceID ) ) { return; }

//            Rect backgroundRect = new Rect(
//                rect.xMin,
//                rect.yMin + 1,
//                rect.width - rect.xMin + ELEMENTS_WIDTH_OFFSET,
//                rect.height - LABEL_BACKGROUND_HEIGHT_REDUCTION );

//            GameObject go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;

//            if ( go.IsNull() ) { return; }

//            //DrawSeparator( rect );
//            DrawElementsBackground( backgroundRect );
//            DrawVisibilityToggle( go, rect, 2 );

//            _currentInstanceInfo = _sceneInstances [ instanceID ];
//            //DrawLabel( backgroundRect, _currentInstanceInfo );
//        }

//        private static void GetHierarchyElements()
//        {
//            _sceneInstances.Clear();

//            // Trouver le moyen de récupérer les "objets" de la scène présents la hiérarchie
//            GameObject [] sceneObjects;
//            Scene activeScene;

//            for ( int i = 0; i < SceneManager.sceneCount; i++ )
//            {
//                activeScene = SceneManager.GetSceneAt( i );

//                if ( activeScene.isLoaded )
//                {
//                    sceneObjects = activeScene.GetRootGameObjects();
//                    //Debug.Log( $"{sceneObjects.Length} " );

//                    for ( int j = 1; j < sceneObjects.Length; j++ )
//                    {
//                        GameObject go = sceneObjects [ j ];

//                        if ( _sceneInstances.ContainsKey( go.GetInstanceID() )
//                            || go.IsNull()
//                            || !go.transform.parent.IsNull() )
//                        {
//                            continue;
//                        }

//                        InstanceInfo newInfo = new()
//                        {
//                            GoName = go.name,
//                            IsGoActive = go.activeInHierarchy,
//                            HierarchyColor = j % 2 == 0 ? EditorColor.Black : EditorColor.Grey,
//                        };

//                        _sceneInstances.Add( go.GetInstanceID(), newInfo );
//                    }
//                }
//            }
//        }

//        private static void DrawVisibilityToggle( GameObject go, Rect rect, int xPosOffset = 1 )
//        {
//            Rect visibilityToggle = new Rect(
//                rect.xMax - ICON_SIZE * xPosOffset,
//                rect.yMin,
//                ICON_SIZE,
//                ICON_SIZE );

//            bool wasGOActive = go.activeSelf;
//            bool isGOActive = GUI.Toggle( visibilityToggle, wasGOActive, "" );

//            if ( wasGOActive != isGOActive )
//            {
//                go.SetActive( isGOActive );
//                if ( !EditorApplication.isPlaying )
//                {
//                    EditorSceneManager.MarkSceneDirty( go.scene );
//                    EditorUtility.SetDirty( go );
//                }
//            }
//        }

//        private static void DrawElementsBackground( Rect rect )
//        {
//            GUIStyle style = new GUIStyle();
//            style.normal.background = EditorHelper.CreateBackgroundWithGradient(
//                ( int ) rect.width,
//                ( int ) rect.height,
//                EditorHelper.GetColor( EditorColor.Blue ).WithAlpha( .5f ),
//                EditorHelper.GetColor( EditorColor.Clear ).WithAlpha( 0 ) );

//            if ( Event.current.type == EventType.Repaint )
//            {
//                GUI.depth -= 100;
//                style.Draw(
//                    new Rect(
//                        rect.x,
//                        rect.y,
//                        rect.width,
//                        rect.height ),
//                    false, false, false, false );
//            }
//        }

//        private static void DrawLabel( Rect rect, InstanceInfo instanceInfo )
//        {
//            // Label
//            GUIStyle style = new GUIStyle
//            {
//                alignment = TextAnchor.MiddleLeft,
//                fontStyle = FontStyle.Bold,
//                fontSize = 11,
//                clipping = TextClipping.Clip,
//                normal = new GUIStyleState()
//                {
//                    textColor = EditorHelper.GetColor( EditorColor.White ),
//                }
//            };

//            EditorGUI.LabelField( rect, instanceInfo.GoName.ToUpper(), style );
//        }

//        private static void DrawSeparator( Rect rect )
//        {
//            Color color = EditorHelper.GetColor( EditorColor.Clear );

//            // Separator
//            EditorGUI.DrawRect(
//                    new Rect(
//                        rect.xMin,
//                        rect.yMax - ( LABEL_BACKGROUND_HEIGHT_REDUCTION / 2 ),
//                        rect.width - rect.xMin + ELEMENTS_WIDTH_OFFSET,
//                        SEPARATOR_HEIGHT ),
//                    color );
//        }
//    }
//}