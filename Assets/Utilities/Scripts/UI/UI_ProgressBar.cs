using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class UI_ProgressBar : UI_FilledBar, IObserver
    {

        #region Enable, Disable

        void OnEnable() => SceneController.OnGameSceneLoading += OnNotification;

        void OnDisable() => SceneController.OnGameSceneLoading -= OnNotification;

        #endregion
        
        private void Start() => Init();

        private void Init()
        {
            SetImageFillAmount( 0, 100 );
        }

        public override void SetImageFillAmount( float currentValue, float maxValue )
        {
            _fillImage.fillAmount = ( currentValue / maxValue ) * 100;
            Debug.Log( _fillImage.fillAmount + " / " + ExtMathfs.FloorToInt( _fillImage.fillAmount * 100 ).ToString() );
            SetFillBarValueText( ExtMathfs.FloorToInt( _fillImage.fillAmount * 100 ).ToString() + "%" );
        }

        public override void SetFillBarValueText( string input )
        {
            _fillAmountValueText.text = input;
        }

        public void OnNotification( object value )
        {
            SetImageFillAmount( ( float ) value, 100 );
        }
    }
}