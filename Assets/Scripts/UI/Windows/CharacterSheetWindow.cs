using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> CharacterSheetWindow description <summary>
    [Component("CharacterSheetWindow", "")]
    [DisallowMultipleComponent]
    public class CharacterSheetWindow : DefaultUIWindow
    {
        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion
        
        // Set all datas that need it at the start of the game
        protected override void Init()
        {
            base.Init();
        }

        protected override void Update()
        {
            base.Update();

            if ( /*_relatedInputAction.action.WasPressedThisFrame()*/ PlayerInputsHelper.Instance.GetToggleCharacterSheetMenuAction().WasPressedThisFrame())
            {
                Debug.Log("Trying to open character sheet window.");
                ContextualToggleDisplay();
            }
        }

        protected override void DisplayWindow()
        {
            base.DisplayWindow();
        }
        protected override void HideWindow()
        {
            base.HideWindow();
        }
    }
}