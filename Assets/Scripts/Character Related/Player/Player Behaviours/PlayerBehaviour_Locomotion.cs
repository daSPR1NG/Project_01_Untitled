using UnityEngine;
using dnSR_Coding.Utilities;
using UnityEngine.InputSystem;
using System;

namespace dnSR_Coding
{
    ///<summary> PlayerBehaviour_Locomotion description <summary>
    [Component( "PlayerBehaviour_Locomotion", "Handles the settings and parameters linked to the player character locomotion." )]
    [DisallowMultipleComponent]
    [RequireComponent( typeof( CharacterController ) )]
    public class PlayerBehaviour_Locomotion : CharacterLocomotion
    {
        [SerializeField] private bool _isSprintActionAToggle = true;

        private InputAction _move;
        private InputAction _sprint;

        public static Action<bool> OnSprinting;

        #region Enable, Disable

        void OnEnable()
        {
            if ( !PlayerInputsHelper.Instance.IsNull() ) { PlayerInputsHelper.Instance.Enable(); }

            _sprint.performed += context => ToggleSprint( _sprintSpeed, false );
            _sprint.canceled += context => ToggleSprint( _walkSpeed, true );
        }

        void OnDisable()
        {
            if ( !PlayerInputsHelper.Instance.IsNull() ) { PlayerInputsHelper.Instance.Disable(); }
        }

        #endregion

        protected override void Awake() => base.Awake();
        protected override void Init()
        {
            base.Init();

            RegisterInputs();
        }
        protected override void GetLinkedComponents()
        {
            base.GetLinkedComponents();
        }

        protected override void Update()
        {
            base.Update();

            TryToMoveController( _controller, _move.ReadValue<Vector2>() );
        }

        void RegisterInputs()
        {
            PlayerInputs inputs = PlayerInputsHelper.Instance.GetInputs();

            _move = inputs.Player.Move;
            _sprint = inputs.Player.Sprint;
        }

        #region Sprint handle

        private bool CanToggleSprint( float speed ) => !GameManager.Instance.IsGamePaused() && _movementSpeed != speed;
        //Does not work 
        private void ToggleSprint( float speed, bool hasBeenCanceled )
        {
            if ( !CanToggleSprint( speed ) ) { return; }

            hasBeenCanceled = false;

            Helper.Log( this, "Has been canceled : " + hasBeenCanceled );

            if ( _isSprintActionAToggle && hasBeenCanceled ) { return; }

            SetMovementSpeedValue( speed );
            OnSprinting?.Invoke( IsSprinting() );

            Helper.Log( this, "Toggling sprint" );
        }

        #endregion

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