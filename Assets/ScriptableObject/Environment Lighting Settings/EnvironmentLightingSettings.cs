using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "EnvironmentLightingSettings", menuName = "Scriptable Objects/Environment/LightingSettings")]
    public class EnvironmentLightingSettings : ScriptableObject
    {
        [Header( "DEPENDENCIES" )]
        [SerializeField] private WeatherType _relatedWeatherType = WeatherType.None;
        [SerializeField] private VolumeProfile _relatedVolumeProfile;

        [Space( 10f )]

        [Header( "LIGHT INTENSITY SETTINGS" )]
        [SerializeField] private float _greaterLightIntensity;
        [SerializeField] private float _lowerLightIntensity;

        [Space( 10f )]

        [Header( "GRADIENTS SETTINGS" )]
        [SerializeField] private Gradient _ambientColor;
        [SerializeField] private Gradient _directionalColor;
        [SerializeField] private Gradient _fogColor;

        public WeatherType RelatedWeatherType => _relatedWeatherType;
        public VolumeProfile RelatedVolumeProfile => _relatedVolumeProfile;

        public float GreaterLightIntensity => _greaterLightIntensity;
        public float LowerLightIntensity => _lowerLightIntensity;

        public Gradient AmbientColor => _ambientColor;
        public Gradient DirectionalColor => _directionalColor;
        public Gradient FogColor => _fogColor;
    }
}