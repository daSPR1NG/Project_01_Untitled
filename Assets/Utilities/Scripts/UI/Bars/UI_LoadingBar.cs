using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

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
            _fillAmountValueText.text = input;
        }
    }
}