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
        private List<EnvironmentData> _environmentDatas = new ();
        private List<EnvironmentCameraData> _environmentCameraDatas = new();

        public static Action<EnvironmentType> OnFocusingOnEnvironment;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
        }

        void OnDisable() 
        {
        }

        #endregion

        private void Awake() => Init();
        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
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

        public List<EnvironmentData> EnvironmentDatas() => _environmentDatas;
        public List<EnvironmentCameraData> EnvironmentCameraDatas() => _environmentCameraDatas;

        #region OnValidate

#if UNITY_EDITOR

        #region Environment Datas Handle

        private void SetEachEnvironmentDatasProperties()
        {
            if ( _environmentDatas.IsEmpty() )
            {
                _environmentCameraDatas.Clear();
                return;
            }

            for ( int i = 0; i < _environmentDatas.Count; i++ )
            {
                EnvironmentData eD = _environmentDatas [ i ];

                if ( eD.EnvironmentTrs.IsNull() ) { continue; }

                SetEnvironmentData( eD, eD.EnvironmentTrs );
            }

            FocusOnDefaultEnvironmentOnInit();
        }

        public void AddEnvironmentData( Transform trs )
        {
            if ( !_environmentDatas.IsEmpty() && DataHasAlreadyBeenSetForThisEnvironment( trs ) )
            {
                return;
            }

            EnvironmentData environmentData = new( trs )
            {
                Name = trs.name
            };
            SetEnvironmentData( environmentData, trs );

            _environmentDatas.AddItem( environmentData, IsDebuggable );

            CinemachineVirtualCamera cvc = environmentData.CameraTrs.GetChild( 0 ).GetComponent<CinemachineVirtualCamera>();

            _environmentCameraDatas.AddItem(
                            new EnvironmentCameraData(
                                environmentData.EnvironmentComponent,
                                cvc,
                                cvc.Priority )
                            {
                                Name = environmentData.CameraTrs.GetChild( 0 ).name
                            },
                            IsDebuggable );

            ResetFocusForEachCamera();
            FocusOnDefaultEnvironmentOnInit();
        }

        private void SetEnvironmentData( EnvironmentData environmentData, Transform possibleEnvironmentTrs )
        {
            environmentData.SetEnvironmentComponent( possibleEnvironmentTrs.GetComponent<Environment>() );
            environmentData.EnvironmentComponent.ToggleEnvironmentVisibility( true );

            environmentData.SetCameraTransform( possibleEnvironmentTrs.GetChild( possibleEnvironmentTrs.childCount - 1 ) );
            environmentData.SetVirtualCameraTransformName( possibleEnvironmentTrs.name + " Camera" );
        }

        public void HandleEnvironmentDatasListModifications()
        {
            if ( _environmentDatas.IsEmpty() ) { return; }

            for ( int i = _environmentDatas.Count - 1; i >= 0; i-- )
            {
                EnvironmentData eD = _environmentDatas [ i ];

                if ( eD.EnvironmentTrs.IsNull() ) { _environmentDatas.RemoveItem( eD, IsDebuggable ); }
            }
        }

        public void HandleEnvironmentCameraDatasListModifications()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            for ( int i = _environmentCameraDatas.Count - 1; i >= 0; i-- )
            {
                EnvironmentCameraData ecd = _environmentCameraDatas [ i ];

                if ( ecd.EnvironmentParent.IsNull() ) { _environmentCameraDatas.RemoveItem( ecd, IsDebuggable ); }
            }
        }

        private bool DataHasAlreadyBeenSetForThisEnvironment( Transform trs )
        {
            if ( _environmentDatas.IsEmpty() ) { return false; }

            for ( int i = 0; i < _environmentDatas.Count; i++ )
            {
                if ( _environmentDatas [ i ].EnvironmentTrs == trs )
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        private void OnValidate()
        {
            if ( Application.isPlaying ) { return; }

            SetEachEnvironmentDatasProperties();

            HandleEnvironmentDatasListModifications();
            HandleEnvironmentCameraDatasListModifications();
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