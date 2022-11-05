using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using System;
using ExternalPropertyAttributes;
using Random = UnityEngine.Random;

namespace dnSR_Coding
{
    public enum EnvironmentType { Unassigned, Hunter, Miner_Lumberjack, }

    ///<summary> Environment description <summary>
    public class Environment : MonoBehaviour, IDebuggable
    {
        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] private EnvironmentType _environmentType = EnvironmentType.Unassigned;

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
        void Init() { }

        #region Environment Visibility Handle

        public void ToggleEnvironmentVisibility( bool displayIt )
        {
            if ( displayIt )
            {
                LoadEnvironment();
                return;
            }

            UnloadEnvironment();
        }
        private void LoadEnvironment()
        {
            gameObject.TryToDisplay();

            OnSettingDisplayedState?.Invoke( gameObject.IsActive() );
        }
        private void UnloadEnvironment()
        {
            gameObject.TryToHide();

            OnSettingDisplayedState?.Invoke( gameObject.IsActive() );
        }

        public bool IsDisplayed() => gameObject.IsActive();

        #endregion

        public EnvironmentType GetEnvironmentType() => _environmentType;

        #region OnValidate

#if UNITY_EDITOR
        private EnvironmentManager _environmentManager;

        private void OnValidate()
        {
            if ( _environmentManager.IsNull() ) { _environmentManager = transform.parent.GetComponent<EnvironmentManager>(); }

            ToggleEnvironmentVisibility( gameObject.IsActive() );
            _environmentManager.UpdateEnvironmentsDisplayedState( gameObject.IsActive() );
        }
#endif

        #endregion
    }
}