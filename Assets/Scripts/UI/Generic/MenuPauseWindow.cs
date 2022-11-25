using dnSR_Coding.Utilities;
using System.Collections;
using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> This class manages all the logic connected to the menu pause used in game. <summary>
    public class MenuPauseWindow : DefaultUIWindow
    {
        #region Enable, Disable

        protected override void OnEnable() 
        {
            base.OnEnable();

            GameManager.OnGameResumed += ContextualToggleDisplay;
            GameManager.OnGamePaused += ContextualToggleDisplay;
        }

        protected override void OnDisable() 
        {
            base.OnDisable();

            GameManager.OnGameResumed -= ContextualToggleDisplay;
            GameManager.OnGamePaused -= ContextualToggleDisplay;
        }

        #endregion

        protected override void DisplayWindow()
        {
            base.DisplayWindow();

            if ( Application.isPlaying && !GameManager.Instance.IsNull() ) 
            {
                GameManager.Instance.ChangeGameState( GameState.Paused ); 
            }

            Helper.SetTimeScale( 0 );
        }

        protected override void HideWindow()
        {
            Helper.SetTimeScale( 1 );

            base.HideWindow();

            if ( Application.isPlaying && !GameManager.Instance.IsNull() ) 
            {
                GameManager.Instance.ChangeGameState( GameState.Playing ); 
            }
        }
    }
}