using UnityEngine;
using UnityEngine.InputSystem;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding.Utilities.Runtime
{
    ///<summary> PlayerInputsHelper description <summary>
    public static class PlayerInputsHelper
    {
        private static PlayerInputs _inputs = null;
        public static PlayerInputs GetInputs()
        {
            if ( _inputs.IsNull() ) { _inputs = new PlayerInputs(); }
            return _inputs;
        }

        public static InputAction GetTogglePauseMenuAction()
        {
            InputAction action = GetInputs().IsNull<PlayerInputs>()
                ? null
                : GetInputs().UI.TogglePauseMenu;

            return action;
        }

        public static InputAction GetToggleCharacterSheetMenuAction()
        {
            InputAction action = GetInputs().IsNull<PlayerInputs>()
                ? null
                : GetInputs().UI.ToggleCharacterSheetMenu;

            return action;
        }
    }
}