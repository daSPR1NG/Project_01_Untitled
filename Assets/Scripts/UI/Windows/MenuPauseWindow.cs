using dnSR_Coding.Utilities;
using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> This class manages all the logic connected to the menu pause used in game. <summary>
    public class MenuPauseWindow : DefaultUIWindow, IObserver
    {
        public ISubject Subject { get; private set; }

        #region Enable, Disable

        private void OnEnable() 
        {
            TryGetSubject();
            Helper.SubscribeToSubject( observer: this, Subject );
        }

        private void OnDisable() => Helper.UnsubscribeToSubject( observer: this, Subject );

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

        public void TryGetSubject()
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if ( gameManager.IsNull() ) 
            {
                Debug.LogError( "There is no reference to a GameManager, can't cach Subject.", transform );
                return; 
            }

            Subject = gameManager;
        }

        public void OnNotification( object value )
        {
            GameState notifiedGameState = ( GameState ) value;

            if ( notifiedGameState == GameState.Paused ) 
            {
                DisplayWindow();
                return; 
            }

            HideWindow();
        }
    }
}