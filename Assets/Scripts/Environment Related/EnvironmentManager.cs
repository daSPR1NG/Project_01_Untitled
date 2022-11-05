using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using System;
using ExternalPropertyAttributes;
using Cinemachine;

namespace dnSR_Coding
{
    ///<summary> EnvironmentManager description <summary>
    [Component("EnvironmentManager", "")]
    public class EnvironmentManager : Singleton<EnvironmentManager>, IDebuggable
    {
        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] private List<EnvironmentData> _environmentDatas = new ();
        [SerializeField] private List<EnvironmentCameraData> _environmentCameraDatas = new();

        public static Action<EnvironmentType> OnFocusingOnEnvironment;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            Environment.OnSettingDisplayedState += UpdateEnvironmentsDisplayedState;
        }

        void OnDisable() 
        {
            Environment.OnSettingDisplayedState -= UpdateEnvironmentsDisplayedState;
        }

        #endregion

        private void Awake() => Init();
        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );

            //FindEnvironmentCameras();
        }

        public void UpdateEnvironmentsDisplayedState( bool displayed )
        {
            if ( _environmentDatas.IsEmpty() ) { return; }

            for ( int i = 0; i < _environmentDatas.Count; i++ )
            {
                if ( _environmentDatas [ i ].Displayed == displayed ) { continue; }

                _environmentDatas [ i ].Displayed = displayed;
            }
        }

        #region Environment Focus Handle

        private void FocusOnDefaultEnvironmentOnInit()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            _environmentCameraDatas [ 0 ].Focus();

            OnFocusingOnEnvironment?.Invoke( _environmentCameraDatas [ 0 ].EnvironmentParent.GetEnvironmentType() );
        }

        public void FocusOnSpecificEnvironment( int index )
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            _environmentCameraDatas [ index ].Focus(); 

            OnFocusingOnEnvironment?.Invoke( _environmentCameraDatas [ index ].EnvironmentParent.GetEnvironmentType() );
        }

        public void ResetFocusForEachCamera()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            for ( int i = 0; i < _environmentCameraDatas.Count; i++ )
            {
                _environmentCameraDatas [ i ].ResetFocus();
            }
        }

        public EnvironmentCameraData GetFocusedEnvironmentCamera()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return null; }

            for ( int i = 0; i < _environmentCameraDatas.Count; i++ )
            {
                if ( !_environmentCameraDatas [ i ].VirtualCamera.isActiveAndEnabled )
                {
                    continue;
                }

                return _environmentCameraDatas [ i ];
            }

            return null;
        }

        #endregion

        private void FindEnvironmentCameras()
        {
            if ( _environmentDatas.IsEmpty()
                || _environmentCameraDatas.Count >= _environmentDatas.Count )
            {
                return;
            }

            for ( int i = 0; i < _environmentDatas.Count; i++ )
            {
                Transform localEnvironmentTrs = _environmentDatas [ i ].EnvironmentTrs;

                if ( localEnvironmentTrs.IsNull() ) { continue; }

                Transform localCameraTrs = _environmentDatas [ i ].CameraTrs;
                Environment environment = localEnvironmentTrs.GetComponent<Environment>();
                CinemachineVirtualCamera cvc = localCameraTrs.GetChild( 0 ).GetComponent<CinemachineVirtualCamera>();

                _environmentCameraDatas.AddItem(
                            new EnvironmentCameraData(
                                environment,
                                cvc,
                                cvc.Priority )
                            {
                                Name = localCameraTrs.GetChild( 0 ).gameObject.name
                            },
                            IsDebuggable );
            }

            FocusOnDefaultEnvironmentOnInit();
        }

        public List<EnvironmentCameraData> EnvironmentCameraDatas() => _environmentCameraDatas;


        #region OnValidate

#if UNITY_EDITOR
        private void SetEnvironmentsInEditor()
        {
            if ( _environmentDatas.IsEmpty() )
            {
                if ( !_environmentCameraDatas.IsEmpty() )
                {
                    _environmentCameraDatas.Clear();
                }

                return;
            }

            for ( int i = 0; i < _environmentDatas.Count; i++ )
            {
                EnvironmentData eD = _environmentDatas [ i ];

                if ( eD.EnvironmentTrs.IsNull() ) { continue; }

                eD.SetName( _environmentDatas [ i ].EnvironmentTrs.name );

                eD.SetCameraDatas( 
                    eD.EnvironmentTrs.name + " Camera", 
                    eD.EnvironmentTrs.GetChild( eD.EnvironmentTrs.childCount - 1 ) );

                eD.SetEnvironmentComponentReference( eD.EnvironmentTrs.GetComponent<Environment>() );
                eD.EnvironmentComponent.ToggleEnvironmentVisibility( eD.Displayed );
            }

            FindEnvironmentCameras();
        }

        private void OnValidate()
        {
            if ( Application.isPlaying ) { return; }

            SetEnvironmentsInEditor();
        }
#endif

        #endregion

        private void OnGUI()
        {
            if ( !Application.isEditor || GetFocusedEnvironmentCamera().IsNull() ) { return; }                       

            GUIContent content = new( 
                GetFocusedEnvironmentCamera().EnvironmentParent.GetEnvironmentType()
                + " Environment is active." );

            GUI.Label( new Rect( 5, Screen.height - 25, 350, 25 ), content );
        }
    }
}