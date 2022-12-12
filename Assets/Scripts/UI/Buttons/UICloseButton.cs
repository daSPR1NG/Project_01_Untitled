using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> UICloseButton description <summary>
    [Component("UICloseButton", "")]
    [DisallowMultipleComponent]
    public class UICloseButton : DefaultUIButton, IValidatable
    {
        [Header( "Specific details" )]
        [Validation( "Need to reference the Window to close component." )]
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

        public bool IsValid => !_windowToClose.IsNull();
#endif
        #endregion
    }
}