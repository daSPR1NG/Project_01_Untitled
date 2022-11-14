using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace dnSR_Coding
{
    [RequireComponent( typeof( CustomPostProcessVolume ) )]

    ///<summary> CameraVolumeManager description <summary>
    [Component("CameraVolumeManager", "")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class CameraVolumeManager : MonoBehaviour, IDebuggable
    {
        [Title( "SETTINGS", 12, "white" )]

        [SerializeField] private float _gammaFadingSpeed = .5f;
        [SerializeField, ExposedScriptableObject ] private VolumeProfileSettings _volumeProfileSettings;

        private CustomPostProcessVolume _volume;
        private VolumeProfile _profile;
        private WeatherSystemManager _weatherSystemManager;
        private CameraVolumeManager _cameraVolumeManager;

        WeatherSequence _sequence;
        LiftGammaGain _profileLGG = null;
        LiftGammaGain _sequenceProfileLGG = null;
        private bool _profileGammaNeedsToBeUpdated = false;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable()
        {
            EnvironmentLightingManager.OnLightingSettingsChanged += NeedToBeUpdated;
        }

        void OnDisable()
        {
            EnvironmentLightingManager.OnLightingSettingsChanged -= NeedToBeUpdated;
        }

        #endregion

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            if ( !_volumeProfileSettings.IsNull() ) { /*SetVolumeProfileSettingsGammaValue();*/ }
            GetLinkedComponents();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _volume.IsNull() ) { _volume = GetComponent<CustomPostProcessVolume>(); }

            if ( _profile.IsNull() ) { _profile = _volume.sharedProfile; }

            // Set this script profile.
            if ( _profile.TryGet( out LiftGammaGain foundProfileLGG ) ) { _profileLGG = foundProfileLGG; }
        }

        void Update()
        {
            SetVolumeProfileGammaColor();
        }

        public void NeedToBeUpdated()
        {
            _profileGammaNeedsToBeUpdated = true;
        }

        private void SetVolumeProfileGammaColor()
        {
            if ( !_profileGammaNeedsToBeUpdated ) { return; }

            // Set sequence.
            if ( _sequence != _weatherSystemManager.ActiveWeather )  { _sequence = _weatherSystemManager.ActiveWeather; }

            // Set active sequence profile.
            if ( !_sequence.IsNull() 
                && _sequence.GetLightingSettings().RelatedVolumeProfile.TryGet( out LiftGammaGain foundSequenceProfileLGG ) ) 
            { 
                if ( _sequenceProfileLGG != foundSequenceProfileLGG ) { _sequenceProfileLGG = foundSequenceProfileLGG; }
            }

            // Set gamma value
            Vector4 gammaValue = _sequence.IsNull() ? new Vector4( 1f, 1f, 1f, 0f) : _sequenceProfileLGG.gamma.value;

            // Guard block - Gamma value reached
            if ( _profileLGG.gamma.value == gammaValue ) 
            {
                _profileGammaNeedsToBeUpdated = false;
                //Debug.Log( "Volume profile gamma has been set" );
                return; 
            }

            // Gamma is lerping
            _profileLGG.gamma.value = Vector4.Lerp( _profileLGG.gamma.value, gammaValue, Time.deltaTime * _gammaFadingSpeed );
            //Debug.Log( "Set volume profile gamma" );
        }        

        public VolumeProfileSettings GetVolumeProfileSettings() => _volumeProfileSettings;
        public CustomPostProcessVolume GetActiveVolume() => _volume;

        #region OnValidate

#if UNITY_EDITOR
        
        private void OnValidate()
        {
            GetLinkedComponents();

            if ( _cameraVolumeManager.IsNull() ) { _cameraVolumeManager = GetComponent<CameraVolumeManager>(); }

            if ( _weatherSystemManager.IsNull() ) 
            { 
                _weatherSystemManager = ( WeatherSystemManager ) FindObjectOfType( typeof( WeatherSystemManager ) ); 
            }
        }
#endif

        #endregion
    }
}