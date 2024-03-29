using UnityEngine;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;

namespace dnSR_Coding
{
    ///<summary> EnvironmentLightsReferencer description <summary>
    [DisallowMultipleComponent]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class EnvironmentLightsReferencer : MonoBehaviour, IDebuggable
    {
        [SerializeField, ReadOnly]
        [FoldoutGroup( "Lights" )]
        private GameObject _lightsContainer;

        [SerializeField, ReadOnly]
        [FoldoutGroup( "Lights" )]
        private LightController _mainLightController;

        [SerializeField, ReadOnly]
        [FoldoutGroup( "Lights" )]
        private LightController _additionalLightController;

        [SerializeField, ReadOnly]
        [FoldoutGroup( "Lights" )]
        private LightController _thunderLightController;

        public LightController MainLightController { get => _mainLightController; private set => _mainLightController = value; }
        public LightController AdditionalLightController { get => _additionalLightController; private set => _additionalLightController = value; }
        public LightController ThunderLightController { get => _thunderLightController; private set => _thunderLightController = value; }

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        #region Setup
#if UNITY_EDITOR

        void Awake() => Init();
        void Init() => SetLightsReference( ref _lightsContainer );

        /// <summary>
        ///     This method tries to reference the main light and the additional light used for simulating a sun influence.
        /// </summary>
        /// <param name="lightsContainer"></param>
        [Button]
        private void SetLightsReference( ref GameObject lightsContainer )
        {
            EnvironmentLightContainer lightContainer;
            bool isContainerCreated = !transform.GetFirstChild().IsNull<Transform>();
            if ( isContainerCreated )
            {
                lightsContainer = transform.GetFirstChild().gameObject;
                Debug.Log( $"{lightsContainer} when container might be created" );

                lightContainer = lightsContainer.GetComponent<EnvironmentLightContainer>();
                lightContainer.Init( this );

                return;
            }

            GameObject container = new GameObject( "Light Container", typeof( EnvironmentLightContainer ) );
            container.transform.SetParent( transform );

            lightsContainer = container;
            Debug.Log( $"{lightsContainer} when container was not created" );

            lightContainer = lightsContainer.GetComponent<EnvironmentLightContainer>();
            lightContainer.Init( this );
        }
#endif
        #endregion

        public void SetMainLightController( LightController lightController )
        {
            _mainLightController = lightController;
        }
        public void SetAdditionalLightController( LightController lightController )
        {
            _additionalLightController = lightController;
        }
        public void SetThunderLightController( LightController lightController )
        {
            _thunderLightController = lightController;
        }
    }
}