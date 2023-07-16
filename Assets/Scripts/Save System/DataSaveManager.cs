using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using System;
using dnSR_Coding.Utilities;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dnSR_Coding
{
    ///<summary> DataSaveManager description <summary>
    [DisallowMultipleComponent]
    public class DataSaveManager : Singleton<DataSaveManager>, IDebuggable
    {
        private const string SAVE_DIRECTORY = "/Saves" + "/Save";
        public const string SAVE_FILE_NAME = "/Save.json";

        #region DEBUG

        //[Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region SETUP

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();
        }

        #endregion

        private void CreateSaveDirectory()
        {
            string path = Application.persistentDataPath + SAVE_DIRECTORY;

            if ( Directory.Exists( path ) ) { return; }

            Directory.CreateDirectory( path );

            Debug.Log( "A save directory has been created at : " + path );
        }

        [Button( "Save datas " )]
        public void SaveDatas()
        {
            CreateSaveDirectory();

            string path = Application.persistentDataPath + SAVE_DIRECTORY + SAVE_FILE_NAME;

            // Fetch all ISaveable to manipulate them
            IEnumerable<ISaveable> datas = FindObjectsOfType( typeof( MonoBehaviour ) ).OfType<ISaveable>();
            this.Debugger( "Data amount to save : " + datas.Count() );
            this.Debugger( datas.Count() >= 1 ? "Data : " + datas.ElementAt( 0 ) : "No data to save." );

            List<object> datasContent = new();

            foreach ( ISaveable data in datas ) {
                datasContent.AppendItem( data.GetData() );
            }

            try
            {
                string jsonContent = JsonConvert.SerializeObject( datasContent, Formatting.Indented, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                } );

                File.Delete( path );
                this.Debugger( "Deleting previous save..." );

                this.Debugger( "Creating new save json..." );
                using FileStream stream = File.Create( path );
                stream.Close();

                File.WriteAllText( path, jsonContent, Encoding.UTF8 );

                //Debug.Log(
                //    $"{jsonContent} has been saved "
                //    + '\n'
                //    + $"at path {path}", this );

#if UNITY_EDITOR
                AssetDatabase.Refresh();
                OpenSaveFile();
#endif
            }
            catch ( Exception e )
            {
                Debug.LogError( $"Unable to save data due to : {e.Message} {e.StackTrace}" );
                throw;
            }
        }

        public object LoadData<T>( string ID )
        {
            string path = Application.persistentDataPath + SAVE_DIRECTORY + SAVE_FILE_NAME;

            if ( !File.Exists( path ) )
            {
                Debug.LogError( $"Cannot load file at path {path}." );
                throw new FileNotFoundException( $"{path} does not exists!" );
            }

            try
            {
                string jsonContent = File.ReadAllText( path );
                object data = null;
                this.Debugger( jsonContent );

                JArray array = JArray.Parse( File.ReadAllText( path ) );
                this.Debugger( array.Count() );
                foreach ( JToken item in array )
                {
                    for ( int i = 0; i < item.Count(); i++ )
                    {
                        if ( item.ElementAt( i ).First().ToString().Equals( ID ) )
                        {
                            this.Debugger( item.ElementAt( i ).First() );
                            data = JsonConvert.DeserializeObject( item.ToString(), typeof( T ) );
                        }
                    }
                }

                return data;
            }
            catch ( Exception e )
            {
                Debug.LogError( $"Unable to load data from {path} | {e.Message} {e.StackTrace}" );
                throw;
            }
        }

#if UNITY_EDITOR
        [Button( "Open Save file " )]
        public void OpenSaveFile()
        {
            UnityEditor.EditorUtility.OpenWithDefaultApp( Application.persistentDataPath + SAVE_DIRECTORY + SAVE_FILE_NAME );
        }
#endif
    }
}