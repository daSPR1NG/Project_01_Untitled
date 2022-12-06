//using UnityEngine;
//using System.Collections;
//using dnSR_Coding.Utilities;
//using NaughtyAttributes;
//using Cinemachine;

//namespace dnSR_Coding
//{
//    public enum LockingState
//    {
//        Locked, FreeLook,
//    }

//    ///<summary> PlayerCamera_Targeting description <summary>
//    [Component( "Targeting settings", "Handles the targeting behaviours of this camera." )]
//    public class PlayerCamera_Targeting : PlayerCamera_DefaultSettings, IValidatable
//    {
//        [Space( 10 )]

//        [Title( "TARGETING SETTINGS", 12, "white" )]

//        [SerializeField] private Transform _targetTrs;
//        [Range( 5, 50 ),SerializeField] private float _distanceFromTarget = 5f;
//        [SerializeField] private float _thresholdBeforeUpdatingPos = 2f;
//        [SerializeField] private float _distanceMatchingRate = 5f;

//        private float _distanceFromTargetToCamera;
//        private Vector3 _offsetFromTarget = Vector3.zero;

//        [Space( 10 )]

//        [Title( "LOCKING STATE SETTINGS", 12, "white" )]

//        [SerializeField] private bool _canSwitchLockingState = true;
//        [SerializeField] private KeyCode _switchLockingKey = KeyCode.Tab;

//        private CinemachineVirtualCamera _virtualCamera;
//        private LockingState _lockingState = LockingState.Locked;

//        public bool IsValid { get; private set; }

//        #region Enable, Disable

//        void OnEnable() 
//        {

//        }

//        void OnDisable() 
//        {

//        }

//        #endregion

//        void Awake() => Init();
//        void Init()
//        {
//            SetCameraTarget( _targetTrs, true );
//        }

//        private void Update()
//        {
//            if ( GameManager.Instance.IsGamePaused() ) return;

//            ToggleLockingState();
//            FollowTarget( _targetTrs );
//        }

//        #region Lock state(s)

//        private void ToggleLockingState()
//        {
//            if ( !_canSwitchLockingState || !_switchLockingKey.IsPressed() ) return;

//            if ( IsCameraLocked() )
//            {
//                UnlockCameraView();
//                return;
//            }

//            LockCameraView();            
//        }

//        private void LockCameraView()
//        {
//            _lockingState = LockingState.Locked;
//            Helper.Log( this, "Camera is locked on target." + "< Locked >".ToLogValue() );
//        }

//        private void UnlockCameraView()
//        {
//            _lockingState = LockingState.FreeLook;
//            Helper.Log( this, "Camera is orbitating." + "< Free Look >".ToLogValue() );
//        }

//        private bool IsCameraLocked() => _lockingState == LockingState.Locked;

//        #endregion

//        public void SetCameraTarget( Transform trs, bool lockOnTarget = false )
//        {
//            if ( trs.IsNull() || !trs.gameObject.IsActive() ) return;

//            _targetTrs = trs;

//            if ( lockOnTarget ) { LockCameraView(); }
//        }

//        private void SetCameraDistanceFromTarget( float distance )
//        {
//            _offsetFromTarget =  new ( 0f, ( distance * 2f ), ( -distance * 1f ) );

//            transform.position = _offsetFromTarget;
//        }

//        private void FollowTarget( Transform trs )
//        {
//            if ( trs.IsNull() || !IsCameraLocked() ) { return; }

//            _distanceFromTargetToCamera = Vector3.Distance( 
//                trs.position, 
//                new Vector3 (
//                    transform.position.x,
//                    transform.position.y - _offsetFromTarget.y,
//                    transform.position.z - _offsetFromTarget.z ) );

//            //Debug.Log( "Distance from target" + _distanceFromTargetToCamera .ToLogComponent());

//            if ( _distanceFromTargetToCamera <= 0.01f ) 
//            {
//                _distanceFromTargetToCamera = 0f;
//                return; 
//            }

//            float updateRate = _distanceFromTargetToCamera >= _thresholdBeforeUpdatingPos ?
//                Time.deltaTime * _distanceMatchingRate : Time.deltaTime * 1.5f;

//            transform.localPosition = Vector3.Lerp(
//                    transform.localPosition,
//                    new Vector3( trs.position.x,
//                    trs.position.y + _offsetFromTarget.y,
//                    trs.position.z + _offsetFromTarget.z ),
//                    updateRate );
//        }      

//        #region OnValidate

//#if UNITY_EDITOR
//        protected override void OnValidate()
//        {
//            base.OnValidate();

//            if ( _virtualCamera.IsNull() ) _virtualCamera = GetComponent<CinemachineVirtualCamera>();

//            IsValid = !_virtualCamera.IsNull() && _virtualCamera.enabled;

//            SetCameraDistanceFromTarget( _distanceFromTarget );
//        }
//#endif

//        #endregion
//    }
//}