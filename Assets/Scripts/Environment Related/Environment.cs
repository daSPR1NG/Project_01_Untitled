using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using ExternalPropertyAttributes;
using Cinemachine;

namespace dnSR_Coding
{
    public enum EnvironmentType { Unassigned, Hunter, Miner_Lumberjack, }

    ///<summary> Environment description <summary>
    public class Environment : MonoBehaviour, IDebuggable
    {
        [Header( "DEPENDENCIES" )]

        [SerializeField] private EnvironmentType _environmentType = EnvironmentType.Unassigned;

        [ShowNonSerializedField] private Transform _cameraPivot;
        [ShowNonSerializedField] private CinemachineVirtualCamera _cvc;

        public static Action<bool> OnSettingDisplayedState;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        void Awake() => Init();
        void Init() { GetLinkedComponents(); }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            SetEnvironmentCameraInfos();
        }

        private void SetEnvironmentCameraInfos()
        {
            if ( transform.GetLastChild().IsNull() ) { return; }

            if ( _cameraPivot.IsNull() )
            { _cameraPivot = transform.GetLastChild(); }

            if ( !_cameraPivot.GetFirstChild().IsNull() && _cvc.IsNull() ) 
            { _cvc = _cameraPivot.GetFirstChild().GetComponent<CinemachineVirtualCamera>(); }

            if ( !_cvc.IsNull() && _cvc.transform.name != transform.name + " Camera" )
            { _cvc.transform.name = transform.name + " Camera"; }
        }

        #region Environment Visibility Handle

        public void ToggleEnvironmentVisibility( bool shouldBeDisplayed )
        {
            if ( shouldBeDisplayed )
            {
                DisplayEnvironment();
                return;
            }

            HideEnvironment();
        }
        private void DisplayEnvironment()
        {
            gameObject.TryToDisplay();

            OnSettingDisplayedState?.Invoke( gameObject.IsActive() );
        }
        private void HideEnvironment()
        {
            gameObject.TryToHide();

            OnSettingDisplayedState?.Invoke( gameObject.IsActive() );
        }

        public bool IsDisplayed() => gameObject.IsActive();

        #endregion

        public EnvironmentType GetEnvironmentType() => _environmentType;
        public Transform GetCameraPivot() => _cameraPivot;
        public CinemachineVirtualCamera GetVirtualCamera() => _cvc;

        #region OnValidate

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

        #endregion
    }
}