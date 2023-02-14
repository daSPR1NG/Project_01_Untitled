using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    public enum GameSceneType { Menu = 0, World = 1, Combat = 2 }

    [DisallowMultipleComponent]
    public class SceneController : Singleton<SceneController>, ISubject, IDebuggable
    {
        [Header( "Scene management details" )]

        [SerializeField, Scene] private List<string> _gameScenes = new();
        private GameSceneType _gameSceneType;
        private AsyncOperation _loadingLevelOperation = null;

        public static Action<object> OnGameSceneChanged;
        public static Action<object> OnGameSceneLoading;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        #endregion

        protected override void Init( bool dontDestroyOnLoad )
        {
            base.Init( true );
        }

        #region Scene Management

        public void LoadSceneByType( GameSceneType gameSceneType )
        {
            string lookedForSceneName = _gameScenes [ ( int ) gameSceneType ];
            Scene lookedForScene = SceneManager.GetSceneByName( lookedForSceneName );

            if ( SceneManager.GetActiveScene() == lookedForScene )
            {
                Debug.Log( "Scene is already active and loaded." );
                return;
            }
            
            StartCoroutine( LoadSceneAsync( lookedForSceneName ) );            

            _gameSceneType = gameSceneType;
            _loadingLevelOperation.allowSceneActivation = true;
        }

        private IEnumerator LoadSceneAsync( string sceneName )
        {
            _loadingLevelOperation = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );
            _loadingLevelOperation.allowSceneActivation = false;

            OnModification( OnGameSceneChanged, false );

            float artificialWaiting = 0;

            while ( !_loadingLevelOperation.isDone /*artificialWaiting < 30*/ )
            {
                artificialWaiting++;
                float loadingProgress = Mathf.Clamp01( _loadingLevelOperation.progress / .9f );
                OnModification( OnGameSceneLoading, loadingProgress );
                Debug.Log( loadingProgress );
                yield return null;
            }
        }

        public GameSceneType GetActiveSceneType() => _gameSceneType;

        #endregion

        public void OnModification( Action<object> actionToExecute, object dataToPush )
        {
            ISubjectExtensions.TriggerAction( actionToExecute, dataToPush );
            //Debug.Log( "On modification", transform );
        }       

        private void OnSceneLoaded( Scene scene, LoadSceneMode loadSceneMode )
        {
            OnModification( OnGameSceneChanged, true );
        }
    }
}