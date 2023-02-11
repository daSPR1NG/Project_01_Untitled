using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

namespace dnSR_Coding
{
    public enum GameState { Playing, Paused }    

    ///<summary> GameManager description <summary>
    [Component( "GAME MANAGER", "Handles global things about the game." )]
    public class GameManager : Singleton<GameManager>, ISubject, IDebuggable
    {
        [Header( "Game State details" )]

        [SerializeField] private GameState _gameState = GameState.Playing;
        [SerializeField] private int _timeScale = 1;        

        public static Action<object> OnGameStateChanged;        

        #region Debug

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
            Application.targetFrameRate = 24;
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
        
        public void OnModification( Action<object> actionToExecute, object dataToPush )
        {
            ISubjectExtensions.TriggerEvent( actionToExecute, dataToPush );
            Debug.Log( "On modification", transform );
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
                OnModification( OnGameStateChanged, GameState.Playing );
                //OnGameResumed?.Invoke();
                return;
            }

            OnModification( OnGameStateChanged, GameState.Paused );
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
                Helper.Log( this, "GameState changed to: " + gameState.ToString().ToLogValue() );
                return;
            }

            _gameState = gameState;
            Helper.Log( this, "GameState changed to: " + gameState.ToString().ToLogValue() );
        }


        public GameState GetCurrentGameState() { return _gameState; }
        public bool IsGamePaused() { return GetCurrentGameState() == GameState.Paused || Time.timeScale == 0; }

        #endregion        

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