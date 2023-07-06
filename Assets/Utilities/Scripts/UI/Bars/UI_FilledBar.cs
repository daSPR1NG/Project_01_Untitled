using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public abstract class UI_FilledBar : MonoBehaviour
    {
        [Header( "Fill bar settings" )]
        [SerializeField] protected Image _fillImage;
        [SerializeField] protected bool _hasText = false;
        [SerializeField, /*NaughtyAttributes.ShowIf( "_hasText" )*/] protected TMP_Text _fillAmountValueText;

        public abstract void SetImageFillAmount( float currentValue, float maxValue );
        public abstract void SetFillBarValueText( string input );
    }
}