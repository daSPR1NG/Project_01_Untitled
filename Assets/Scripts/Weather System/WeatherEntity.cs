using UnityEngine;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding
{
    ///<summary> WeatherEntity description <summary>
    [DisallowMultipleComponent]
    public class WeatherEntity : MonoBehaviour, IDebuggable
    {
        // This entity register itself in when it gets created
        // And unregister itself when destroy

        [CenteredHeader("Main settings")]
        [SerializeField, Required] private Shader _snowShader;
        [SerializeField] private bool _canBeAffectedBySnow = true;

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )] 
        public bool IsDebuggable { get; set; } = true;

        #endregion

        #region ENABLE, DISABLE

        void OnEnable() => RegisterSelf();

        void OnDisable() => UnregisterSelf();

        #endregion

        private void RegisterSelf()
        {
            // Send event to listener(s)
            EventManager.WeatherEntity_OnRegistration.Call( this );

            this.Debugger("I, as a weather entity registered myself to listener(s)"  );
        }
        private void UnregisterSelf()
        {
            // Send event to listener(s)
            EventManager.WeatherEntity_OnUnregistration.Call( this );

            this.Debugger( "I, as a weather entity unregistered myself to listener(s)" );
        }

        #region Utils

        public bool CanBeAffectedBySnow() => HasMaterialWithASnowShader() && _canBeAffectedBySnow;
        private bool HasMaterialWithASnowShader()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            return !meshRenderer.IsNull() && meshRenderer.sharedMaterial.shader == _snowShader;
        }

        #endregion
    }
}