using dnSR_Coding.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace dnSR_Coding
{
    [RequireComponent( typeof( CharacterController ), typeof( Rigidbody ) )]
    [DisallowMultipleComponent]
    public abstract class CharacterLocomotion : StateManager
    {
        [Header( "LOCOMOTION SETTINGS" )]

        [SerializeField] protected bool _isMovementBasedOnCameraOrientation = false;
        [SerializeField] protected float _walkSpeed; // Can be replaced by a stat

        [SerializeField] protected bool _canSprint = true;
        [SerializeField, ShowIf( "_canSprint" )] protected float _sprintSpeed; // Can be replaced by a stat

        [Header( "Rotation speed SETTINGS" )]

        [SerializeField, Range( 0, 720 )] protected float _rotationSpeed = 360f;
        private bool _isRotationIsFacingMovementInput = false;

        protected CharacterController _controller;

        protected float _movementSpeed;
        protected Vector3 _currentLocation = Vector3.zero;
        protected Vector3 _movementDirection = Vector3.zero;

        protected virtual void Awake() => Init();
        protected virtual void Init()
        {
            GetLinkedComponents();

            SetMovementSpeedValue( _walkSpeed );
        }
        protected virtual void GetLinkedComponents()
        {
            if ( _controller.IsNull() ) { _controller = GetComponent<CharacterController>(); }
        }

        protected override void Update() => base.Update();

        protected void TryToMoveController( CharacterController controller, Vector2 movement )
        {
            if ( !CanMove() ) { return; }

            _currentLocation = Vector3.zero;

#if !ENABLE_INPUT_SYSTEM
            _movementDirection = new Vector3( Input.GetAxis( "Horizontal" ), 0, Input.GetAxis( "Vertical" ) );
#else
            _movementDirection = new Vector3( movement.x, 0, movement.y );
#endif

            if ( _movementDirection == Vector3.zero )
            {
                SwitchToAnotherState( GetSpecificState( StateType.Idle ) );
                return;
            }

            Vector3 newPosition = _currentLocation
                + _movementSpeed
                * Helper.GetRealFixedDeltaTime()
                * GetInputMovementDirection();

            TryToRotateControllerBeforeMowing( controller, newPosition );

            if ( !_isRotationIsFacingMovementInput ) { return; }

            controller.Move( newPosition );
            SwitchToAnotherState( GetSpecificState( StateType.Moving ) );
        }

        private void TryToRotateControllerBeforeMowing( CharacterController controller, Vector3 positionToMoveTo )
        {
            Transform controllerTrs = controller.transform;
            Quaternion toRotation = Quaternion.LookRotation( positionToMoveTo, Vector3.up );

            if ( controllerTrs.rotation == toRotation )
            {
                _isRotationIsFacingMovementInput = true;
                return;
            }

            _isRotationIsFacingMovementInput = false;

            controllerTrs.rotation = Quaternion.RotateTowards( 
                controllerTrs.rotation, 
                toRotation, 
                _rotationSpeed * Helper.GetRealFixedDeltaTime() );
        }

        protected void SetMovementSpeedValue( float value )
        {
            if ( _movementSpeed == value ) { return; }

            _movementSpeed = value;
        }

        private Vector3 GetInputMovementDirection()
        {
            return _isMovementBasedOnCameraOrientation
                ? GetInputMovementRelativeToCamera( _movementDirection )
                : _movementDirection.normalized;
        }
        private Vector3 GetInputMovementRelativeToCamera( Vector3 input )
        {
            Camera mainCamera = Helper.GetMainCamera();

            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward = forward.normalized;
            right = right.normalized;

            Vector3 forwardRelative = input.z * forward;
            Vector3 rightRelative = input.x * right;

            return forwardRelative + rightRelative;
        }

        protected bool CanMove()
        {
            return !GameManager.Instance.IsGamePaused();
        }
        protected bool IsSprinting() => _movementSpeed == _sprintSpeed;

#region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

#endregion
    }
}