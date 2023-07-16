using UnityEngine;
using System;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace dnSR_Coding.Utilities
{
    public enum GameState { Playing, Paused }    

    ///<summary> GameManager description <summary>
    public class GameManager : Singleton<GameManager>, IDebuggable
    {
        [CenteredHeader( "Game State details" )]
        [SerializeField] private GameState _gameState = GameState.Playing;
        [CenteredHeader( "Game State details" )]
        [SerializeField] private int _timeScale = 1;

        //[Header( "Debug section" )]

        [SerializeField, ShowIf( "IsDebuggable" )]
        private int _refreshRate = 60;

        public static Action<object> OnGameStateChanged;

        public List<TestIndentation> _testIndentations = new(3);

        [System.Serializable]
        public class TestIndentation
        {
            [CenteredHeader( "Test - 1", true )]
            public string Name = "Test - 1";
            public TestIndentation2 TestIndentation2;
        }

        [System.Serializable]
        public class TestIndentation2
        {
            [CenteredHeader( "Test - 2", 64 )]
            public string Name = "Test - 2";
            public TestIndentation3 TestIndentation3;
        }

        [System.Serializable]
        public class TestIndentation3
        {
            [CenteredHeader( "Test - 3", true )]
            public string Name = "Test - 3";
        }

        #region Debug

        [HorizontalLine( 1, 4, EditorColor.Grey )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();
            Application.targetFrameRate = _refreshRate;
        }

        private void Update()
        {
            //CheckIfPauseInputHasBeenPressed();
        }

        /// <summary>
        /// Checks if the player presses the pause input,
        /// if so this tries to resume or pause the game according to current game state.
        /// </summary>
        public void CheckIfPauseInputHasBeenPressed()
        {
            InputAction input = PlayerInputsHelper.Instance.GetTogglePauseMenuAction();

            if ( !input.IsNull<InputAction>() && input.WasPerformedThisFrame() )
            {
                if ( !UIManager.Instance.IsNull<UIManager>() 
                    && UIManager.Instance.AWindowIsDisplayed() ) {
                    return;
                }

                this.Debugger( "Trying to pause the game" );

                ResumeOrPauseTheGame();
            }
        }

#if UNITY_EDITOR
        //[Button]
#endif
        public void SetTimeScale()
        {
            Helper.SetTimeScale( _timeScale );
            Debug.Log( $"Time scale set to {Time.timeScale.ToString().ToLogValue()}." );
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

        private int frameCount;
        private float elapsedTime;
        private double frameRate;

        private void OnGUI()
        {
            if ( !Application.isEditor ) { return; }

            GUIContent timeScale = new ( _gameState.ToString() + " | Time Scale : " + Time.timeScale );

            GUI.Label( new Rect( 15, 5, 150, 25 ), timeScale );

            // FPS calculation
            frameCount++;
            elapsedTime += Helper.GetDeltaTime();
            if ( elapsedTime > 0.33f )
            {
                frameRate = System.Math.Round( frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero );
                frameCount = 0;
                elapsedTime = 0;
            }

            GUIContent fps = new( ( frameRate ).ToString( "0" ).ToUpper().Bolded() );
            GUI.Label( new Rect( 15, 25, 150, 25 ), fps );
        }

        #endregion
    }
}