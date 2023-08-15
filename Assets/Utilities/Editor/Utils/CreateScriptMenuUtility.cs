using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace dnSR_Coding.Utilities.Editor
{
    ///<summary>
    /// Create Script Menu helps us create scripts
    ///</summary>
     public static class CreateScriptMenuUtility
     {
        private const string FOLDER_DIRECTORY = "/Utilities/ScriptTemplates";

        #region Behaviour Related

        #region MonoBehaviour

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Create MonoBehaviour", priority = 0 )]
        [MenuItem( "Create Script Menu/Create MonoBehaviour", priority = 0 )]
        #endregion
        static void CreateMonoBehaviourMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Mono Behaviour", GetCurrentPath(), "NewMonoBehaviour.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Mono/MonoBehaviourTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Abstract Class

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Mono/Create Abstract Class" )]
        [MenuItem( "Create Script Menu/Mono/Create Abstract Class" )]
        #endregion
        static void CreateAbstractClassMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Abstract Class", GetCurrentPath(), "NewAbstractClass.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Mono/AbstractClassTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Scriptable Object

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Mono/Create Scriptable Object" )]
        [MenuItem( "Create Script Menu/Mono/Create Scriptable Object" )]
        #endregion
        static void CreateScriptableObjectMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create ScriptableObject", GetCurrentPath(), "NewScriptableObject.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Mono/ScriptableObjectTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Interface

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Mono/Create Interface" )]
        [MenuItem( "Create Script Menu/Mono/Create Interface" )]
        #endregion
        static void CreateInterfaceMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Interface", GetCurrentPath(), "NewInterface.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Mono/InterfaceTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #endregion

        #region Editor Related

        #region Default Editor Class

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Editor/Create Editor" )]
        [MenuItem( "Create Script Menu/Editor/Create Editor" )]
        #endregion
        static void CreateEditorMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Editor", GetCurrentPath(), "NewEditor.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Editor/EditorTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Editor Attribute

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Editor/Create Attribute" )]
        [MenuItem( "Create Script Menu/Editor/Create Attribute" )]
        #endregion
        static void CreateAttributeMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Attribute", GetCurrentPath(), "NewAttribute.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Editor/AttributeTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Property Drawer

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Editor/Create Property Drawer" )]
        [MenuItem( "Create Script Menu/Editor/Create Property Drawer" )]
        #endregion
        static void CreatePropertyDrawerMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Property Drawer", GetCurrentPath(), "NewPropertyDrawer.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Editor/PropertyDrawerTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #region Decorator Drawer

        #region Menu Items
        [MenuItem( "Assets/Create Script Menu/Editor/Create Decorator Drawer" )]
        [MenuItem( "Create Script Menu/Editor/Create Decorator Drawer" )]
        #endregion
        static void CreateDecoratorDrawerMenuItem()
        {
            string pathToNewFile = UnityEditor.EditorUtility.SaveFilePanel( "Create Decorator Drawer", GetCurrentPath(), "NewDecoratorDrawer.cs", "cs" );
            string pathToTemplate = Application.dataPath + FOLDER_DIRECTORY + "/Editor/DecoratorDrawerTemplate.txt";

            MakeScriptFromTemplate( pathToNewFile, pathToTemplate );
        }

        #endregion

        #endregion

        static void MakeScriptFromTemplate( string pathToNewFile, string pathToTemplate )
        {
            if ( !string.IsNullOrWhiteSpace( pathToNewFile ) )
            {
                FileInfo fileInfo = new( pathToNewFile );
                string nameOfScript = Path.GetFileNameWithoutExtension( fileInfo.Name );

                string text = File.ReadAllText( pathToTemplate );

                text = text.Replace( "#SCRIPTNAME#", nameOfScript );
                text = text.Replace( "#SCRIPTNAMEWITHOUTEDITOR#", nameOfScript.Replace( "Editor", "") );
                text = text.Replace( "#SCRIPTNAMEWITHOUTDRAWER#", nameOfScript.Replace( "Drawer", "") );

                File.WriteAllText( pathToNewFile, text );
                AssetDatabase.Refresh();
            }
        }

        static string GetCurrentPath()
        {
            string path = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs [ 0 ] );
            if ( path.Contains(".") )
            {
                int index = path.LastIndexOf( "/" );
                path = path.Substring( 0, index );
            }

            return path;
        }
     }
}