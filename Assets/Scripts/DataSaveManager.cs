using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Project;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEditor;
using System;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> DataSaveManager description <summary>
    [DisallowMultipleComponent]
    public class DataSaveManager : Singleton<DataSaveManager>, IDebuggable
    {
        private const string SAVE_DIRECTORY = "/Saves" + "/Save";
        public const string SAVE_FILE_NAME = "/SavedDatas.json";
        public const string PLANT_DATAS_FILE_NAME = "/PlantDatas.json";

        #region DEBUG

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region SETUP

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();
        }

        #endregion

        #region Json Wrapper

        [Serializable]
        private class DataWrapper<T>
        {
            public T [] Datas;
        }

        public T [] FromJson<T>( string json )
        {
            DataWrapper<T> wrapper = JsonUtility.FromJson<DataWrapper<T>>( json );
            return wrapper.Datas;
        }

        public string ToJson<T>( T [] array, bool prettyPrint )
        {
            DataWrapper<T> wrapper = new() {
                Datas = array
            };
            return JsonUtility.ToJson( wrapper, prettyPrint );
        }

        #endregion

        private void TryToCreateSaveDirectory()
        {
            string path = Application.persistentDataPath + SAVE_DIRECTORY;

            if ( Directory.Exists( path ) ) { return; }

            Directory.CreateDirectory( path );

            Debug.Log( "A save directory has been created at : " + path );
        }

        public void SaveData( string relativePath, object data )
        {
            TryToCreateSaveDirectory();

            string path = Application.persistentDataPath + SAVE_DIRECTORY + relativePath;
            Debug.Log( path + " does exist : " + Directory.Exists( path ) );

            try
            {
                File.Delete( path );
                Debug.Log( "Deleting previous save...", this );

                using FileStream fileStream = File.Create( path );
                fileStream.Close();
                Debug.Log( "Creating new save json...", this );

                File.WriteAllText( path, data.ToString(), Encoding.UTF8 );
                Debug.Log( $"{data} is saved.", this );

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
            catch ( Exception e )
            {
                Debug.LogError( $"Unable to save data due to : {e.Message} {e.StackTrace}" );
                throw;
            }
        }

        [ContextMenu( "Save All Plants" )]
        public void SaveAllPlants()
        {
            IEnumerable<Plant> plants = FindObjectsOfType<Plant>();
            Debug.Log( "Plants found : " + plants.Count() );

            List<Plant.PlantData> datas = new();
            foreach ( Plant p in plants ) {
                datas.AppendItem( p.GetData() );
                Debug.Log( "datas found : " + datas.Count() );
            }

            SaveData( PLANT_DATAS_FILE_NAME, ToJson( datas.ToArray(), true ) );

            datas.Clear();
            GC.Collect();
        }

        public ISavableData<T> LoadedData<T>( string relativePath, int ID )
        {
            // Retrieve savableData file from specific path...
            string path = Application.persistentDataPath + SAVE_DIRECTORY + relativePath;
            // Read savableData file...
            IEnumerable<T> datas = FromJson<T>( File.ReadAllText( path ) );
            Debug.Log( datas.Count() );
            Debug.Log( datas is ISavableData<T> );
            ISavableData<T> dataToLoad = null;

            Debug.Log( "Looked for ID : " + ID );

            foreach ( ISavableData<T> data in datas.Select( data => ( ISavableData<T> ) data ) )
            {
                Debug.Log( data.ID + " / " + ID );
                if ( data.ID != ID ) { continue; }
                dataToLoad = data;
            }

            return dataToLoad;
        }
    }
}