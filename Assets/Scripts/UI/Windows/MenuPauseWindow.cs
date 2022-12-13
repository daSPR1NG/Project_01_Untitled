using dnSR_Coding.Utilities;
using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> This class manages all the logic connected to the menu pause used in game. <summary>
    public class MenuPauseWindow : DefaultUIWindow
    {
        #region Enable, Disable

        private void OnEnable() 
        {
            GameManager.OnGameResumed += ContextualToggleDisplay;
            GameManager.OnGamePaused += ContextualToggleDisplay;
        }

        private void OnDisable() 
        {
            GameManager.OnGameResumed -= ContextualToggleDisplay;
            GameManager.OnGamePaused -= ContextualToggleDisplay;
        }

        #endregion

        protected override void DisplayWindow()
        {
            Helper.SetTimeScale( 0 );

            base.DisplayWindow();

            if ( Application.isPlaying 
                && !GameManager.Instance.IsNull() ) 
            {
                GameManager.Instance.SetGameState( GameState.Paused ); 
            }            
        }
        protected override void HideWindow()
        {
            Helper.SetTimeScale( 1 );

            base.HideWindow();

            if ( Application.isPlaying 
                && !GameManager.Instance.IsNull() ) 
            {
                GameManager.Instance.SetGameState( GameState.Playing ); 
            }
        }
    }
}