using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using System;
using ExternalPropertyAttributes;
using Cinemachine;
using System.Linq;

namespace dnSR_Coding
{
    ///<summary> EnvironmentManager description <summary>
    [Component("EnvironmentManager", "")]
    public class EnvironmentManager : Singleton<EnvironmentManager>, IDebuggable
    {
        [/*HideInInspector,*/ SerializeField] private List<EnvironmentData> _environmentDatas = new ();
        [/*HideInInspector,*/ SerializeField] private List<EnvironmentCameraData> _environmentCameraDatas = new();

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

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }

        #region Environment Focus Handle

        private void FocusOnDefaultEnvironmentOnInit()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            _environmentCameraDatas [ 0 ].Focus();

            OnFocusingOnEnvironment?.Invoke( _environmentCameraDatas [ 0 ].EnvironmentComponent.GetEnvironmentType() );
        }

        public void FocusOnSpecificEnvironment( int index )
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            _environmentCameraDatas [ index ].Focus(); 

            OnFocusingOnEnvironment?.Invoke( _environmentCameraDatas [ index ].EnvironmentComponent.GetEnvironmentType() );
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

        public void AddEnvironmentData( Transform trs )
        {
            EnvironmentData environmentData = new( trs.GetComponent<Environment>() )
            {
                Name = trs.name
            };
            SetAddedEnvironmentData( environmentData, trs );

            _environmentDatas.AppendItem( environmentData, IsDebuggable );


            CinemachineVirtualCamera cvc = environmentData.EnvironmentComponent.GetVirtualCamera();

            _environmentCameraDatas.AppendItem(
                            new EnvironmentCameraData(
                                environmentData.EnvironmentComponent,
                                cvc,
                                cvc.Priority ),
                            false );

            ResetFocusForEachCamera();
            FocusOnDefaultEnvironmentOnInit();
        }
        private void SetAddedEnvironmentData( EnvironmentData environmentData, Transform possibleEnvironmentTrs )
        {
            environmentData.SetEnvironmentComponent( possibleEnvironmentTrs.GetComponent<Environment>() );
            environmentData.EnvironmentComponent.ToggleEnvironmentVisibility( true );
        }

        public void HandleEnvironmentDatasListModifications()
        {
            if ( _environmentDatas.IsEmpty() ) { return; }

            for ( int i = _environmentDatas.Count - 1; i >= 0; i-- )
            {
                EnvironmentData eD = _environmentDatas [ i ];

                if ( eD.EnvironmentComponent.IsNull() ) { _environmentDatas.RemoveItem( eD, false ); }
            }
        }
        public void HandleEnvironmentCameraDatasListModifications()
        {
            if ( _environmentCameraDatas.IsEmpty() ) { return; }

            for ( int i = _environmentCameraDatas.Count - 1; i >= 0; i-- )
            {
                EnvironmentCameraData ecd = _environmentCameraDatas [ i ];
                if ( ecd.EnvironmentComponent.IsNull() ) { _environmentCameraDatas.RemoveItem( ecd, false ); }
            }
        }

        #endregion

        private void OnValidate()
        {
            if ( Application.isPlaying ) { return; }

            HandleEnvironmentDatasListModifications();
            HandleEnvironmentCameraDatasListModifications();
        }
#endif

        #endregion

        private void OnGUI()
        {
            if ( !Application.isEditor || GetFocusedEnvironmentCamera().IsNull() ) { return; }                       

            GUIContent content = new( 
                GetFocusedEnvironmentCamera().EnvironmentComponent.GetEnvironmentType()
                + " Environment is active." );

            GUI.Label( new Rect( 5, Screen.height - 25, 350, 25 ), content );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            foreach ( var item in _environmentCameraDatas )
            {
                Gizmos.DrawSphere( item.VirtualCamera.transform.position, 0.15f );
            }            
        }
    }
}