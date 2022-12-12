using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> UIMenuButton description <summary>
    [Component("UIMenuButton_QuitGame", "")]
    [DisallowMultipleComponent]
    public class UIMenuButton_QuitGame : DefaultUIButton
    {
        public override void OnClick()
        {
            Helper.Log( this, "Quit the game" );
            GameManager.QuitApplication();
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