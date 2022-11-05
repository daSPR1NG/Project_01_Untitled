using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum GameState
    {
        Playing, Paused
    }

    ///<summary> GameManager description <summary>
    [Component( "GAME MANAGER", "Handles global things about the game." )]
    public class GameManager : Singleton<GameManager>, IDebuggable
    {
        [SerializeField] private GameState _gameState = GameState.Playing;

        public static Action OnGamePaused { get; set; }
        public static Action OnGameResumed { get; set; }

        #region Debug

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable()
        {
            
        }

        void OnDisable()
        {
            
        }

        #endregion


        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }

        private void Update()
        {
            TryToPauseTheGame();
        }

        public void TryToPauseTheGame()
        {
            if ( PlayerInputsHelper.Instance.GetTogglePauseMenuAction().WasPerformedThisFrame() )
            {
                if ( !UIManager.Instance.IsNull() && UIManager.Instance.AnyWindowIsDisplayed() ) return;

                Helper.Log( this, "Trying to pause the game" );

                ResumeOrPauseTheGame();
            }
        }

        #region GameState Handle

        #region GameState state getters

        public bool IsGamePlaying() { return GetCurrentGameState() == GameState.Playing || Time.timeScale != 0; }
        public bool IsGamePaused() { return GetCurrentGameState() == GameState.Paused || Time.timeScale == 0; }

        #endregion

        private void ResumeOrPauseTheGame()
        {
            if ( IsGamePaused() )
            {
                OnGameResumed?.Invoke();
                return;
            }

            OnGamePaused?.Invoke();
        }

        public void ChangeGameState( GameState gameState )
        {
            if ( _gameState == gameState )
            {
                Helper.Log( this, "GameState changed to: " + gameState.ToString().ToLogValue() );

                return;
            }

            _gameState = gameState;
            Helper.Log( this, "GameState changed to: " + gameState.ToString().ToLogValue() );
        }

        public GameState GetCurrentGameState() { return _gameState; }

        #endregion


        public static void QuitApplication()
        {
#if UNITY_EDITOR
            if ( Application.isEditor )
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            Application.Quit();
        }

        #region On GUI

        private void OnGUI()
        {
            if ( !Application.isEditor ) { return; }

            GUIContent content = new ( GetCurrentGameState().ToString() + " | tS: " + Time.timeScale );

            GUI.Label( new Rect( 5, 5, 105, 25 ), content );
        }

        #endregion
    }
}