using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using Cinemachine;

namespace dnSR_Coding
{
    ///<summary> PlayerCamera_Locomotion description <summary>
    [Component("Locomotion settings", "Handles the locomotion behaviours of this camera." )]
    public class PlayerCamera_Locomotion : PlayerCamera_DefaultSettings, IValidatable
    {
        [Space( 5 )]

        [Header( "LOCOMOTION SETTINGS" )]

        [SerializeField] private float _cameraLocomotionSpeed = 15f;

        private CinemachineVirtualCamera _virtualCamera;

        public bool IsValid { get; private set; }

        void Awake() => Init();
        void Init()
        {
            //Set all datas that need it at the start of the game
        }

        #region OnValidate

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if ( _virtualCamera.IsNull() ) _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            IsValid = !_virtualCamera.IsNull() && _virtualCamera.enabled;
        }
#endif

        #endregion
    }
}