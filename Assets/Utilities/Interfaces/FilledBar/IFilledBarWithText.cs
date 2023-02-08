using TMPro;
using UnityEngine.UI;

namespace dnSR_Coding
{
    public interface IFilledBarWithText
    {
        public Image FilledImage { get; }

        public TMP_Text FilledBarValueText { get; }
        public abstract void SetFillBarValueText( string input );
    }
}