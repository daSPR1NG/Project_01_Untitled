using UnityEngine;
using dnSR_Coding.Utilities;
using UnityEngine.InputSystem;
using System;
using NaughtyAttributes;

namespace dnSR_Coding
{
    ///<summary> PlayerBehaviour_Locomotion description <summary>
    [Component( "PlayerBehaviour_Locomotion", "Handles the settings and parameters linked to the player character locomotion." )]
    [DisallowMultipleComponent]
    [RequireComponent( typeof( CharacterController ) )]
    public class PlayerBehaviour_Locomotion : CharacterLocomotion
    {
        [Header( "Player Locomotion settings" )]

        [SerializeField, ShowIf( "_canSprint" )] private bool _maintainInputToSprint = true;

        private InputAction _move;
        private InputAction _sprint;

        public static Action<bool> OnSprinting;

        #region Enable, Disable

        void OnEnable()
        {
            if ( !PlayerInputsHelper.Instance.IsNull() ) { PlayerInputsHelper.Instance.Enable(); }

            _sprint.performed += context => TryToSprint();

            _sprint.canceled += context =>
            {
                if ( _maintainInputToSprint ) { CancelSprint(); }
            };
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

        protected override void Update() => base.Update();

        private void FixedUpdate()
        {
            TryToMoveController( _controller, _move.ReadValue<Vector2>() );
        }

        void RegisterInputs()
        {
            PlayerInputs inputs = PlayerInputsHelper.Instance.GetInputs();

            _move = inputs.Player.Move;
            _sprint = inputs.Player.Sprint;
        }

        #region Sprint handle

        private bool CanSprint() => !GameManager.Instance.IsGamePaused() && _canSprint;

        private void TryToSprint()
        {
            if ( !CanSprint() ) { return; }

            if ( !_maintainInputToSprint && IsSprinting() ) 
            {
                CancelSprint();
                return;
            }

            SetMovementSpeedValue( _sprintSpeed );
            
            OnSprinting?.Invoke( IsSprinting() );

            Helper.Log( this, "Is sprinting now." );
        }

        private void CancelSprint()
        {
            SetMovementSpeedValue( _walkSpeed );

            OnSprinting?.Invoke( false );

            Helper.Log( this, "Is walking now." );
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