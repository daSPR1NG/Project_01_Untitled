using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [Component("MainCameraSettings", "")]
    [DisallowMultipleComponent]
    public class MainCameraSettings : MonoBehaviour, IDebuggable
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

        private readonly List<CinemachineVirtualCamera> _virtualCameras = new();
        private Camera _attachedCameraComponent;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
            TryToFindAnyVirtualCameraInScene();
            ApplySettingsToAnyActiveVirtualCamera();
        }
        private void GetLinkedComponents()
        {
            if ( _attachedCameraComponent.IsNull() ) { _attachedCameraComponent = GetComponent<Camera>(); }
        }

        void TryToFindAnyVirtualCameraInScene()
        {
            var cvc = FindObjectOfType<CinemachineVirtualCamera>();

            _virtualCameras.AppendItem( cvc );
        }
        void ApplySettingsToAnyActiveVirtualCamera()
        {
            if ( _virtualCameras.IsEmpty() ) { return; }

            for ( int i = 0; i < _virtualCameras.Count; i++ )
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

            TryToFindAnyVirtualCameraInScene();
            ApplySettingsToAnyActiveVirtualCamera();
        }
#endif

        #endregion
    }
}