using UnityEngine;
using UnityEngine.InputSystem;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;

namespace dnSR_Coding.Utilities.Runtime
{
    ///<summary> PlayerInputsHelper description <summary>
    public class PlayerInputsHelper : Singleton<PlayerInputsHelper>, IDebuggable
    {
        [Header( "STATE SETTINGS" )]

        [SerializeField] private bool _isEnabledAtStart = true;

        private PlayerInputs _inputs;

        #region Debug

        [Space( 10 ),/* HorizontalLine( .5f, EColor.Gray )*/]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() => EnableInputs();

        void OnDisable() => DisableInputs();

        private void EnableInputs()
        {
            if ( _inputs.IsNull<PlayerInputs>() ) { return; }

            if ( !_isEnabledAtStart )
            {
                DisableInputs();
                return;
            }

            _inputs.Enable();
        }
        private void DisableInputs()
        {
            if ( _inputs.IsNull<PlayerInputs>() ) { return; }

            _inputs.Disable();
        }

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();

            _inputs = new();

            this.Debugger( _inputs.ToString() );
        }

        public PlayerInputs GetInputs() => _inputs;

        public InputAction GetTogglePauseMenuAction()
        {
            InputAction action = _inputs.IsNull<PlayerInputs>()
                ? null
                : _inputs.UI.TogglePauseMenu;

            return action;
        }

        public InputAction GetToggleCharacterSheetMenuAction()
        {
            InputAction action = _inputs.IsNull<PlayerInputs>()
                ? null
                : _inputs.UI.ToggleCharacterSheetMenu;

            return action;
        }
    }
}