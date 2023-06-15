using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> IEnvironmentLightsUser description <summary>
    public interface IEnvironmentLightsUser
    {
        public EnvironmentLightsReferencer EnvironmentLightsReferencer { get; set; }
        public LightController MainLightController { get; set; }
        public LightController AdditionalLightController { get; set; }

        /// <summary>
        /// Wrapper method to set all dependencies
        /// </summary>
        public abstract void SetLightsReference();
    }
}