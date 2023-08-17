using UnityEngine;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEditor.Toolbars;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary> 
    /// Permits to select a scene by a custom menu
    ///<summary>
    [Overlay( typeof( SceneView ), "Scene Selection" )]
    [Icon( K_ICON )]
    public class SceneSelectionOverlayUtility : ToolbarOverlay
    {
        public const string K_ICON = "Assets/Utilities/Editor/Icons/UnityIcon.png";

        SceneSelectionOverlayUtility() : base( SceneDropdownToggle.K_ID ) { }

        [EditorToolbarElement( K_ID, typeof( SceneView ) )]
        class SceneDropdownToggle : EditorToolbarButton, IAccessContainerWindow
        {
            public const string K_ID = "SceneSelectionOverlay/SceneDropdownToggle";
            public EditorWindow containerWindow { get; set; }

            SceneDropdownToggle()
            {
                text = " Scene Selection ";
                tooltip = "Select a scene to load";
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>( K_ICON );

                //dropdownClicked += ShowSceneMenu;
                clicked += ShowSceneMenu;
            }

            private void ShowSceneMenu()
            {
                GenericMenu menu = new();

                Scene currentScene = EditorSceneManager.GetActiveScene();

                //Debug.Log( Application.dataPath );
                //Debug.Log( Application.dataPath + "/Scenes" );

                string filter = "t: scene";
                string sceneFolderPath = "Assets/Scenes";

                string [] sceneGuids = AssetDatabase.FindAssets(
                    filter,
                    Directory.Exists( sceneFolderPath ) ? new [] { sceneFolderPath } : null );                

                for ( int i = 0; i < sceneGuids.Length; i++ )
                {
                    string path = AssetDatabase.GUIDToAssetPath( sceneGuids [ i ] );
                    string name = Path.GetFileNameWithoutExtension( path );

                    if ( string.Compare( currentScene.name, name ) == 0 ) {
                        menu.AddDisabledItem( new GUIContent( name ) );
                    }
                    else
                    {
                        menu.AddItem( 
                            new GUIContent( name + "/Single"),
                            false, 
                            () => OpenScene( currentScene, path, OpenSceneMode.Single ) );

                        menu.AddItem( 
                            new GUIContent( 
                                name + "/Additive" ), 
                            false, 
                            () => OpenScene( currentScene, path, OpenSceneMode.Additive ) );
                    }
                }

                menu.ShowAsContext();
            }

            private void OpenScene( Scene currentScene, string path, OpenSceneMode openSceneMode )
            {
                if ( currentScene.isDirty )
                {
                    if ( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() ) {
                        EditorSceneManager.OpenScene( path, openSceneMode );
                    }

                    return;
                }

                EditorSceneManager.OpenScene( path, openSceneMode );
            }
        }
    }
}