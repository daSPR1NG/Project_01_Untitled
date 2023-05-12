using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> UICloseButton description <summary>
    [DisallowMultipleComponent]
    public class UICloseButton : DefaultUIButton
    {
        [Header( "Specific details" )]
        [SerializeField] private DefaultUIWindow _windowToClose;

        public override void OnClick()
        {
            Debug.Log( "ON CLICK" );
            _windowToClose.ContextualToggleDisplay();
        }

        #region OnValidate

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
        }
        
#endif
        #endregion
    }
}