using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class UI_LoadingBar : UI_FilledBar
    {
        private void Start() => Init();

        private void Init()
        {
            SetImageFillAmount( 0, 1 );
        }

        public override void SetImageFillAmount( float currentValue, float maxValue )
        {
            _fillImage.fillAmount = currentValue / maxValue;
            Debug.Log( _fillImage.fillAmount + " / " + ExtMathfs.FloorToInt( _fillImage.fillAmount * 100 ).ToString() );
            SetFillBarValueText( currentValue * 100 + "%" );
        }

        public override void SetFillBarValueText( string input )
        {
            if ( _fillAmountValueText.text.Equals( input ) ) { return; }

            _fillAmountValueText.SetText( input );
        }
    }
}