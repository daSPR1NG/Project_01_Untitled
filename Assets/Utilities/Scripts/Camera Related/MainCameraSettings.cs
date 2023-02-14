using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using static UnityEngine.Rendering.DebugUI;

namespace dnSR_Coding
{
    [Component("MainCameraSettings", "")]
    [DisallowMultipleComponent]
    public class MainCameraSettings : Singleton<MainCameraSettings>, IObserver, IDebuggable
    {
        private enum CameraProjection { Perspective, Orthographic }

        [Header( "GENERAL SETTINGS" )]
        [SerializeField] private CameraProjection _projection;

        [Header( "Perspective VIEW SETTINGS" )]
        [SerializeField, ShowIf( "IsPerspective" )] private float _verticalFov = 60;

        [Header( "Perspective VIEW SETTINGS" )]
        [SerializeField, ShowIf( "IsOrthographic" )] private float _orthographicSize = 12.5f;

        [Header( "Common VIEW SETTINGS" )]
        [SerializeField] private float _nearClipPlane = 0.1f;
        [SerializeField] private float _farClipPlane = 100f;
        [SerializeField, Range( -180f, 180f)] private float _dutch = 0f;

        [Header( "Position settings" )]
        [SerializeField] private Vector3 _position = Vector3.zero;

        /*private readonly*/ public List<CinemachineVirtualCamera> _virtualCameras = new();
        private Camera _attachedCameraComponent;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion


        #region Enable, Disable

        void OnEnable() { SceneController.OnGameSceneChanged += OnNotification; }

        void OnDisable() { SceneController.OnGameSceneChanged -= OnNotification; }

        #endregion

        protected override void Init( bool dontDestroyOnLoad )
        {
            base.Init( true );
            GetLinkedComponents();
            ManageAnyVirtualCameraInTheScene();
        }
        private void GetLinkedComponents()
        {
            if ( _attachedCameraComponent.IsNull() ) { _attachedCameraComponent = GetComponent<Camera>(); }
        }

        private void ManageAnyVirtualCameraInTheScene()
        {
            TryToFindAnyVirtualCameraInScene();
            ApplySettingsToAnyActiveVirtualCamera( !Application.isPlaying );
        }
        private void TryToFindAnyVirtualCameraInScene()
        {
            _virtualCameras.Clear();

            var cvc = FindObjectOfType<CinemachineVirtualCamera>();

            _virtualCameras.AppendItem( cvc );
        }
        private void ApplySettingsToAnyActiveVirtualCamera( bool setFOV )
        {
            if ( _virtualCameras.IsEmpty() ) { return; }

            for ( int i = 0; i < _virtualCameras.Count; i++ )
            {
                if ( setFOV )
                {
                    switch ( _projection )
                    {
                        case CameraProjection.Perspective:

                            _virtualCameras [ i ].m_Lens.FieldOfView = _verticalFov;
                            break;
                        case CameraProjection.Orthographic:

                            _virtualCameras [ i ].m_Lens.OrthographicSize = _orthographicSize;
                            break;
                    }
                }               

                _virtualCameras [ i ].m_Lens.NearClipPlane = _nearClipPlane;
                _virtualCameras [ i ].m_Lens.FarClipPlane = _farClipPlane;
                _virtualCameras [ i ].m_Lens.Dutch = _dutch;

                Debug.Log( "Virtual cameras lens settings set." );
            }
        }

        public bool IsOrthographic => _projection == CameraProjection.Orthographic;
        public bool IsPerspective => _projection == CameraProjection.Perspective;

        #region OnValidate

#if UNITY_EDITOR
        
        void ApplyProjectionInEditor()
        {
            if ( !_attachedCameraComponent.IsNull() )
            {
                switch ( _projection )
                {
                    case CameraProjection.Perspective:
                        _attachedCameraComponent.orthographic = false;
                        break;
                    case CameraProjection.Orthographic:
                        _attachedCameraComponent.orthographic = true;
                        break;
                }
            }
        }
        private void ApplyPositionInEditor()
        {
            if ( _virtualCameras.IsEmpty() ) { return; }

            for ( int i = 0; i < _virtualCameras.Count; i++ )
            {
                if ( _virtualCameras [ i ].IsNull() ) { continue; }

                bool hasAParent = !_virtualCameras [ i ].transform.parent.IsNull();

                if ( hasAParent )
                {
                    _virtualCameras [ i ].transform.parent.position = _position;
                    continue;
                }

                _virtualCameras [ i ].transform.position = _position;
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();

            ApplyProjectionInEditor();
            ApplyPositionInEditor();

            ManageAnyVirtualCameraInTheScene();
        }

        public void OnNotification( object value )
        {
            ManageAnyVirtualCameraInTheScene();
        }
#endif

        #endregion
    }
}