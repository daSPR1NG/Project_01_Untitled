using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using ExternalPropertyAttributes;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.Events;

namespace dnSR_Coding
{
    public enum GameState { Playing, Paused }

    ///<summary> GameManager description <summary>
    [Component( "GAME MANAGER", "Handles global things about the game." )]
    public class GameManager : Singleton<GameManager>, IDebuggable
    {
        [SerializeField] private GameState _gameState = GameState.Playing;
        [SerializeField] private int _timeScale = 1;

        public static Action OnGamePaused;
        public static Action OnGameResumed;

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
            CheckIfPauseInputHasBeenPressed();

#if UNITY_EDITOR

            if ( KeyCode.Alpha0.IsPressed() )
            {
                Helper.SetTimeScale( _timeScale );
            }
#endif
        }

        /// <summary>
        /// Checks if the player presses the pause input,
        /// if so this tries to resume or pause the game according to current game state.
        /// </summary>
        public void CheckIfPauseInputHasBeenPressed()
        {
            InputAction input = PlayerInputsHelper.Instance.GetTogglePauseMenuAction();

            if ( !input.IsNull() && input.WasPerformedThisFrame() )
            {
                if ( !UIManager.Instance.IsNull() 
                    && UIManager.Instance.AWindowIsDisplayed() ) 
                {
                    return;
                }

                Helper.Log( this, "Trying to pause the game" );

                ResumeOrPauseTheGame();
            }
        }

        #region GameState Handle

        /// <summary>
        /// Its in charge of trying to resume or pause the game according to the current game state.
        /// It throws an event for both situations.
        /// </summary>
        private void ResumeOrPauseTheGame()
        {
            if ( IsGamePaused() )
            {
                OnGameResumed?.Invoke();
                return;
            }

            OnGamePaused?.Invoke();
        }

        /// <summary>
        /// Sets the game state to the given value.
        /// </summary>
        /// <param name="gameState"> New game state value to apply. </param>
        public void SetGameState( GameState gameState )
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
        public bool IsGamePaused() { return GetCurrentGameState() == GameState.Paused || Time.timeScale == 0; }

        #endregion

        /// <summary>
        /// Quit the game : close the application in a build context and stops playmod in Editor.
        /// </summary>
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

            GUIContent content = new ( GetCurrentGameState().ToString() + " | Time Scale : " + Time.timeScale );

            GUI.Label( new Rect( 5, 5, 150, 25 ), content );
        }

        #endregion
    }
}