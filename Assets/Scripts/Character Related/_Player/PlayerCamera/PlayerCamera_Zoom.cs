using UnityEngine;
using dnSR_Coding.Utilities;
using Cinemachine;
using System;
using System.Collections;

namespace dnSR_Coding
{
    ///<summary> PlayerCamera_RenderingSettings description <summary>
    [Component( "Player Camera Zoom", "Handle the zoom of this camera." )]
    public class PlayerCamera_Zoom : PlayerCamera_DefaultSettings, IValidatable
    {
        [Space( 10 )]

        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] private bool _isZoomingEnabled = true;

        [Space( 10 )]

        [Title( "ORTHOGRAPHIC SETTINGS", 12, "white" )]

        [Range( 0, 15 ), SerializeField] private float _size = 12f;
        [Range( 0.1f, 20 ), SerializeField] private float _minSizeValue = 0.1f;
        [Range( 2, 15 ), SerializeField] private float _maxSizeValue = 15f;

        [Space( 10 )]

        [Title( "FOV SETTINGS", 12, "white" )]

        [Range( 0, 90 ), SerializeField] private float _verticalFOV = 60f;
        [Range( 0, 20 ), SerializeField] private float _minFOVValue = 0f;
        [Range( 20, 90 ), SerializeField] private float _maxFOVValue = 60f;

        [Space( 10 )]

        [Title( "ZOOM SETTINGS", 12, "white" )]

        [Range( 0, 10 ), SerializeField] private float _scrollStep = 1f;
        [Range( 1, 30 ), SerializeField] private float _scrollForceMultiplier = 15f;
        [SerializeField] private float _scrollForceMultiOrthoOverrider = 1.35f;

        [Space( 10 )]

        [Title( "COMPLETE ZOOM SETTINGS", 12, "white" )]

        [SerializeField] private bool _isCompleteZoomOvertime = true;
        [SerializeField] private float _completeZoomUpdateSpeed = 2f;
        private Coroutine _completeZoomCoroutine = null;

        private float _currentScrollForceMultiplier = 0f;
        private float _zoomPercentage;

        private CinemachineVirtualCamera _virtualCamera;

        public static Action<float, float> OnCameraZoom;

        public bool IsValid { get; private set; }

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        void Awake() => Init();
        void Init()
        {
            SetScrollForceMultiplierValue();

            _size = 12f;
            ModifyOrthographicSize( _size );

            ModifyFovValue( 60 );
        }

        private void Update()
        {
            if ( KeyCode.KeypadPlus.IsPressed() )
            {
                CompleteZoom( true, _isCompleteZoomOvertime );
            }

            if ( KeyCode.KeypadMinus.IsPressed() )
            {
                CompleteZoom( false, _isCompleteZoomOvertime );
            }

            Zoom(); 
        }

        private void ModifyOrthographicSize( float value )
        {
            if ( !IsCameraOrthographic() ) return;

            if ( _virtualCamera.m_Lens.OrthographicSize == value ) return;

            _virtualCamera.m_Lens.OrthographicSize = value;
            _virtualCamera.m_Lens.OrthographicSize = _virtualCamera.m_Lens.OrthographicSize.Clamped( 0.1f, _maxSizeValue );
        }

        private void ModifyFovValue( float value )
        {
            if ( IsCameraOrthographic() ) return;

            if ( _virtualCamera.m_Lens.FieldOfView == value ) return;

            value = value.Clamped( _minFOVValue, _maxFOVValue );
            _virtualCamera.m_Lens.FieldOfView = value;

            CalculateZoomPercentage();
            OnCameraZoom?.Invoke( value, _maxFOVValue );
        }

        private void Zoom()
        {
            bool canZoom = !GameManager.Instance.IsGamePaused();

            if ( !canZoom ) { return; }

            float mouseScrollWheelYValue = Input.mouseScrollDelta.y;

            if ( !_isZoomingEnabled || mouseScrollWheelYValue == 0 ) return;
            Helper.Log( this, "Zooming with the camera." );

            if ( !IsCameraOrthographic() ) { SetScrollForceMultiplierValue(); }                       

            float appliedScrollForce = IsCameraOrthographic() ? 
                ( _currentScrollForceMultiplier * _scrollStep ) : ( _currentScrollForceMultiplier * _scrollStep );

            // Zoom in
            if ( mouseScrollWheelYValue > 0 )
            {
                switch ( IsCameraOrthographic() )
                {
                    case true:
                        _size -= appliedScrollForce;
                        _size = _size.Clamped( _minSizeValue, _maxSizeValue );

                        ModifyOrthographicSize( _size );
                        break;
                    case false:
                        _verticalFOV -= appliedScrollForce;
                        _verticalFOV = _verticalFOV.Clamped( _minFOVValue, _maxFOVValue );

                        ModifyFovValue( _verticalFOV );
                        break;
                }               

                return;
            }

            // Zoom Out
            switch ( IsCameraOrthographic() )
            {
                case true:
                    _size += appliedScrollForce;
                    _size = _size.Clamped( _minSizeValue, _maxSizeValue );

                    ModifyOrthographicSize( _size );
                    break;
                case false:
                    _verticalFOV += appliedScrollForce;
                    _verticalFOV = _verticalFOV.Clamped( _minFOVValue, _maxFOVValue );

                    ModifyFovValue( _verticalFOV );
                    break;
            }
        }

        private void CalculateZoomPercentage()
        {
            // Zoom percentage calculation - 100% meaning you're at the max zoom out unit.

            if ( IsCameraOrthographic() )
            {
                _zoomPercentage = _size / _maxSizeValue;
                Helper.Log( this, "Current zoom percentage : " + ( _zoomPercentage * 100 ).ToLogValue() );

                return;
            }

            _zoomPercentage = _verticalFOV / _maxFOVValue;
            Helper.Log( this, "Current zoom percentage : " + ( _zoomPercentage * 100 ).ToLogValue() );
        }

        public void CompleteZoom( bool zoomIn, bool isOvertime = false )
        {
            float fovToMatch = zoomIn ? _minFOVValue : _maxFOVValue;

            if ( _verticalFOV == fovToMatch ) { return; }

            if ( !_completeZoomCoroutine.IsNull() ) { StopCoroutine( _completeZoomCoroutine ); }

            if ( isOvertime ) 
            {
                _completeZoomCoroutine = StartCoroutine( 
                    FocusedZoomCoroutine( zoomIn, fovToMatch, _completeZoomUpdateSpeed ) ); 
            }            
            else 
            {
                _verticalFOV = fovToMatch;
                ModifyFovValue( _verticalFOV );
            }
        }

        private IEnumerator FocusedZoomCoroutine( bool zoomIn, float fovToMatch, float fovUpdateSpeed )
        {
            float delta = zoomIn ? -fovUpdateSpeed : fovUpdateSpeed;

            do
            {
                _verticalFOV += delta;
                _verticalFOV = _verticalFOV.Clamped( _minFOVValue, _maxFOVValue );
                ModifyFovValue( _verticalFOV );

                yield return null;

            } while ( _verticalFOV != fovToMatch );
        }

        private void SetScrollForceMultiplierValue()
        {
            float value = IsCameraOrthographic() ? _scrollForceMultiOrthoOverrider : _scrollForceMultiplier;

            if ( _currentScrollForceMultiplier != value )  { _currentScrollForceMultiplier = value; }
        }

        #region OnValidate

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if ( _virtualCamera.IsNull() ) _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            IsValid = !_virtualCamera.IsNull() && _virtualCamera.enabled;

            base.OnValidate();

            SetScrollForceMultiplierValue();

            ModifyOrthographicSize( _size );
            ModifyFovValue( _verticalFOV );
        }
#endif

        #endregion
    }
}