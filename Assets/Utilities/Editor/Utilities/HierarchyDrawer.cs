using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    [InitializeOnLoad]
    public static class HierarchyDrawer
    {
        private static bool _isInitialized = false;
        private static InstanceInfo _currentItem;
        private static Dictionary<int, InstanceInfo> _sceneInstances = new ();

        static HierarchyDrawer() {
            Init();
            Debug.Log( _isInitialized );
        }

        [Serializable]
        struct InstanceInfo
        {
            public string GoName;
            public bool IsGoActive;
            public Color HierarchyColor;
        }

        private static void Init()
        {
            if ( _isInitialized )
            {
                EditorApplication.hierarchyWindowItemOnGUI -= DrawCore;
                EditorApplication.hierarchyChanged -= GetHierarchyElements;
            }

            EditorApplication.hierarchyWindowItemOnGUI += DrawCore;
            EditorApplication.hierarchyChanged += GetHierarchyElements;
            // Dessiner une ligne séparatrice entre ces "objets"
            // Ajouter un toggle pour désactiver/activer l'objet dans la scène/game view
            // Possibilité d'ajouter une icône représentant le.s composant.s principal.aux
        }

        private static void DrawCore( int instanceID, Rect selectionRect )
        {
            _isInitialized = true;

            if ( !_sceneInstances.ContainsKey( instanceID ) ) { return; }

            _currentItem = _sceneInstances [ instanceID ];
            GameObject go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;

            if ( go.IsNull<GameObject>() ) { return; }            

            Rect backgroundRect = new Rect( 
                selectionRect.xMin, selectionRect.yMin,
                        width: selectionRect.width - selectionRect.xMin + 8, 
                        height: selectionRect.height );

            // Background
            EditorGUI.DrawRect( backgroundRect, _currentItem.HierarchyColor );

            // Label
            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 11,
                normal = new GUIStyleState()
                {
                    textColor = Color.white,
                }
            };

            EditorGUI.LabelField( backgroundRect, _currentItem.GoName.ToUpper(), style );

            // Separators
            EditorGUI.DrawRect(
                    new Rect(
                        selectionRect.xMin /*+ 16*/, selectionRect.yMax - 1,
                        width: selectionRect.width - selectionRect.xMin + 8, height: .5f ),
                    new Color( 1, 1, 1, 1f ) );

            DrawVisibilityToggle( go, selectionRect, 2 );

            Debug.Log( _isInitialized );
        }

        private static void GetHierarchyElements()
        {
            if ( Application.isPlaying ) { return; }

            _sceneInstances.Clear();

            // Trouver le moyen de récupérer les "objets" de la scène présents la hiérarchie
            GameObject [] sceneObjects;
            Scene activeScene;

            for ( int i = 0; i < SceneManager.sceneCount; i++ )
            {
                activeScene = SceneManager.GetSceneAt( i );

                if ( activeScene.isLoaded )
                {
                    sceneObjects = activeScene.GetRootGameObjects();
                    Debug.Log( $"{sceneObjects.Length} " );

                    for ( int j = 1; j < sceneObjects.Length; j++ )
                    {
                        GameObject go = sceneObjects [ j ];

                        if ( _sceneInstances.ContainsKey( go.GetInstanceID() )
                            || go.IsNull<GameObject>() 
                            || !go.transform.parent.IsNull<GameObject>()) {
                            continue;
                        }

                        Color grey = new Color( .5f, .5f, .5f, 1f );
                        Color black = new Color( 0, 0, 0, 1f );

                        InstanceInfo newInfo = new()
                        {
                            GoName = go.name,
                            IsGoActive = go.activeInHierarchy,
                            HierarchyColor = j % 2 == 0 ? grey : black,
                        };

                        _sceneInstances.Add( go.GetInstanceID(), newInfo );
                        
                        Debug.Log( sceneObjects [ j ].name );
                        Debug.Log( sceneObjects [ j ].GetInstanceID() );
                    }
                }

                Debug.Log( $"{_sceneInstances.Count}");
            }
        }

        private static void DrawVisibilityToggle( GameObject go, Rect selectionRect, int xPosOffset = 1 )
        {
            Rect visibilityToggle = new Rect( 
                selectionRect.xMax - 16 * xPosOffset, selectionRect.yMin, 
                width: 16, height: 16 );

            bool wasGOActive = go.activeSelf;
            bool isGOActive = GUI.Toggle( visibilityToggle, wasGOActive, "" );

            if ( wasGOActive != isGOActive )
            {
                go.SetActive( isGOActive );
                if ( !EditorApplication.isPlaying )
                {
                    EditorSceneManager.MarkSceneDirty( go.scene );
                    EditorUtility.SetDirty( go );
                }
            }
        }
    }
}