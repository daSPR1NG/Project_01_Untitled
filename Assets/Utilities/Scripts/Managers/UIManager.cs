using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> UIManager description <summary>
    [Component( "UI MANAGER", "" )]
    public class UIManager : Singleton<UIManager>
    {
        private int _displayedWindowAmount = 0;
        public int DisplayedWindowAmount
        {
            get { return _displayedWindowAmount; }
            set 
            { 
                _displayedWindowAmount = value;
                _displayedWindowAmount = _displayedWindowAmount.Clamped( 0, _displayedWindowAmount );
            }
        }

        private bool _aWindowWasDisplayed = false;

        #region Enable, Disable

        void OnEnable() 
        {
            DefaultUIWindow.OnWindowDisplayed += AddDisplayedWindow;
            DefaultUIWindow.OnWindowHidden += RemoveDisplayedWindow;
        }

        void OnDisable() 
        {
            DefaultUIWindow.OnWindowDisplayed -= AddDisplayedWindow;
            DefaultUIWindow.OnWindowHidden -= RemoveDisplayedWindow;
        }

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }

        #region Displayed Windows Handle

        private void AddDisplayedWindow( bool canBeSelfHidden ) 
        {
            if ( canBeSelfHidden)
            {
                DisplayedWindowAmount++;
                _aWindowWasDisplayed = true;
            }
        }
        private void RemoveDisplayedWindow( bool canBeSelfHidden ) 
        {
            if ( canBeSelfHidden ) { DisplayedWindowAmount--; }
            if ( DisplayedWindowAmount == 0 ) { _aWindowWasDisplayed = false; }
        }

        public bool AWindowIsDisplayed() => _aWindowWasDisplayed;

        #endregion
    }
}