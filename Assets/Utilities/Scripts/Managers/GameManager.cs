using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace dnSR_Coding.Utilities
{
    public enum GameState { Playing, Paused }    

    ///<summary> GameManager description <summary>
    public class GameManager : Singleton<GameManager>, IDebuggable
    {
        [Header( "Game State details" )]

        [SerializeField] private GameState _gameState = GameState.Playing;
        [SerializeField] private int _timeScale = 1;

        [Header( "Debug section" )]

        [SerializeField, ShowIf( "IsDebuggable" ), ReadOnly] 
        private int _targetFrame = 60;

        public static Action<object> OnGameStateChanged;

        #region Debug

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            _targetFrame = Application.targetFrameRate;
        }

        private void Update()
        {
            //CheckIfPauseInputHasBeenPressed();

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

                this.Debugger( "Trying to pause the game" );

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
                //OnGameResumed?.Invoke();
                return;
            }

            //OnGamePaused?.Invoke();
        }

        /// <summary>
        /// Sets the game state to the given value.
        /// </summary>
        /// <param name="gameState"> New game state value to apply. </param>
        public void SetGameState( GameState gameState )
        {
            if ( _gameState == gameState )
            {
                this.Debugger( "GameState changed to: " + gameState.ToString().ToLogValue() );
                return;
            }

            _gameState = gameState;
            this.Debugger( "GameState changed to: " + gameState.ToString().ToLogValue() );
        }

        public bool IsGamePaused() { return _gameState == GameState.Paused || Time.timeScale == 0; }

        #endregion

        #region On GUI

        private void OnGUI()
        {
            if ( !Application.isEditor ) { return; }

            GUIContent content = new ( _gameState.ToString() + " | Time Scale : " + Time.timeScale );

            GUI.Label( new Rect( 5, 5, 150, 25 ), content );
        }

        #endregion
    }
}