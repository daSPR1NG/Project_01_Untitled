using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace dnSR_Coding
{
    ///<summary> SceneManager description <summary>
    [Component("SceneDataManager", "")]
    [DisallowMultipleComponent]
    public class SceneDataManager : Singleton<SceneDataManager>, IDebuggable
    {
        [Header( "Scenes" )]

        [SerializeField] private List<SceneData> _sceneData = new();

        [System.Serializable]
        public struct SceneData
        {
            public string Name;
            [AllowNesting, Scene] public int Index;
        }

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion
        
        // Set all datas that need it at the start of the game
        protected override void Init( bool dontDestroyOnLoad )
        {
            base.Init( true );
        }

        public void LoadSpecificScene( int index )
        {
            if ( SceneManager.GetActiveScene().buildIndex == index ) 
            {
                Helper.Log( this, 
                    "This scene is already loaded and active. There is no point trying to load it.", 
                    Helper.LogType.Error );

                return; 
            }

            SceneManager.LoadScene( index );
        }
    }
}