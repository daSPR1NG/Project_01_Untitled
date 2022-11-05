using UnityEngine;
using dnSR_Coding.Utilities;
using UnityEngine.InputSystem;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> PlayerInputsHelper description <summary>
    [Component("PlayerInputsHelper", "Helps to get all inputs from one place.")]
    public class PlayerInputsHelper : Singleton<PlayerInputsHelper>, IDebuggable
    {
        [Title( "STATE SETTINGS", 12, "white" )]

        [SerializeField] private bool _isEnabledAtStart = true;

        private PlayerInputs _inputs;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() => EnableInputs();

        void OnDisable() => DisableInputs();

        private void EnableInputs()
        {
            if ( !_isEnabledAtStart ) 
            {                
                DisableInputs();
                return;
            }
            
            _inputs.Enable();
        }
        private void DisableInputs()
        {
            _inputs.Disable();
        }

        #endregion

        void Awake() => Init();
        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );

            _inputs = new ();

            Helper.Log( this, _inputs.ToString() );
        }

        public PlayerInputs GetInputs() => _inputs;

        public InputAction GetTogglePauseMenuAction()
        {
            return _inputs.UI.TogglePauseMenu;
        }
    }
}