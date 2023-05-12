using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> UIMenuButton description <summary>
    [DisallowMultipleComponent]
    public class UIMenuButton_QuitGame : DefaultUIButton
    {
        public override void OnClick()
        {
            this.Debugger( "Quit the game" );
            Helper.QuitApplication();
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