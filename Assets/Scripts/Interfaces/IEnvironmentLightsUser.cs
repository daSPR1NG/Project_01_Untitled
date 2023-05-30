using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> IEnvironmentLightsUser description <summary>
    public interface IEnvironmentLightsUser
    {
        public EnvironmentLightsContainer EnvironmentLightsContainer { get; set; }
        public Light MainLight { get; set; }
        public Light AdditionalLight { get; set; }

        /// <summary>
        /// Wrapper method to set all dependencies
        /// </summary>
        public abstract void SetLightsReference();
    }
}