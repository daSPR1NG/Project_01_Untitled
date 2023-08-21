using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    ///<summary> EnvironmentLightContainer description <summary>
    [DisallowMultipleComponent]
    public class EnvironmentLightContainer : MonoBehaviour, IDebuggable
    {
        private EnvironmentLightsReferencer _environmentLightsReferencer;

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        #region SETUP

        void Awake() => Init();

        // Set all datas that need it at the start of the game
        public void Init()
        {
            GetLinkedComponents();

            if ( _environmentLightsReferencer.MainLightController.IsNull<LightController>() )
            {
                SetMainLightReference();
            }

            if ( _environmentLightsReferencer.AdditionalLightController.IsNull<LightController>() )
            {
                SetAdditionalLightReference();
            }

            if ( _environmentLightsReferencer.ThunderLightController.IsNull<LightController>() )
            {
                SetThunderLightReference();
            }
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _environmentLightsReferencer.IsNull<EnvironmentLightsReferencer>() )
            {
                _environmentLightsReferencer = transform.parent.GetComponent<EnvironmentLightsReferencer>();
            }
        }

        #endregion

        #region MAIN LIGHT REFERENCE

        private void SetMainLightReference()
        {
            LightController mainLightController = GetLightControllerByType( Enums.Light_Type.Main );

            if ( !mainLightController.IsNull<LightController>() )
            {
                _environmentLightsReferencer.SetMainLightController( mainLightController );
                _environmentLightsReferencer.MainLightController.SetControllerLight( mainLightController.GetControllerLight() );
                return;
            }

            Debug.Log( "No main light was found so a new one has been created." );
            CreateMainLightReference();
        }
        private void CreateMainLightReference()
        {
            //Object creation and component assignment
            GameObject mainLightGO = new( "Main Light", typeof( LightController ) );
            mainLightGO.transform.SetParent( transform );

            LightController mainLightController = mainLightGO.GetComponent<LightController>();
            _environmentLightsReferencer.SetMainLightController( mainLightController );

            mainLightController.Init( mainLightGO.GetComponent<Light>(), Enums.Light_Type.Main );
            SetMainLightParametersAsDefault( mainLightController );
        }
        private void SetMainLightParametersAsDefault( LightController controller )
        {
            controller.SetLightParameters(
                LightType.Directional,
                LightmapBakeType.Realtime,
                Color.white,
                1,
                LightShadows.Soft,
                .1f,
                .85f );
        }

        #endregion

        #region ADDITIONAL LIGHT REFERENCE

        private void SetAdditionalLightReference()
        {
            LightController additionalLightController = GetLightControllerByType( Enums.Light_Type.Additional );

            if ( !additionalLightController.IsNull<LightController>() )
            {
                _environmentLightsReferencer.SetAdditionalLightController( additionalLightController );
                _environmentLightsReferencer.AdditionalLightController.SetControllerLight( additionalLightController.GetControllerLight() );
                return;
            }

            Debug.Log( "No additional light was found so a new one has been created." );
            CreateAdditionalLightReference();
        }
        private void CreateAdditionalLightReference()
        {
            //Object creation and component assignment
            GameObject additionalLightGO = new( "Additional Light", typeof( LightController ) );
            additionalLightGO.transform.SetParent( transform );

            LightController additionalLightController = additionalLightGO.GetComponent<LightController>();
            _environmentLightsReferencer.SetAdditionalLightController( additionalLightController );

            additionalLightController.Init( additionalLightGO.GetComponent<Light>(), Enums.Light_Type.Additional );
            SetAdditionalLightParametersAsDefault( additionalLightController );
            additionalLightController.DisableLight();
        }
        private void SetAdditionalLightParametersAsDefault( LightController controller )
        {
            Color additionalLightColor = Color.white;
            if ( ColorUtility.TryParseHtmlString( "#FFCB00", out Color color ) ) { additionalLightColor = color; }

            controller.SetLightParameters( LightType.Directional, LightmapBakeType.Mixed, additionalLightColor, 1 );
        }

        #endregion

        private void SetThunderLightReference()
        {
            LightController thunderLightController = GetLightControllerByType( Enums.Light_Type.Thunder );

            if ( !thunderLightController.IsNull<LightController>() )
            {
                _environmentLightsReferencer.SetThunderLightController( thunderLightController );
                _environmentLightsReferencer.ThunderLightController.SetControllerLight( thunderLightController.GetControllerLight() );
                return;
            }

            Debug.Log( "No thunder light was found so a new one has been created." );
            CreateThunderLightReference();
        }
        private void CreateThunderLightReference()
        {
            //Object creation and component assignment
            GameObject thunderLightGO = new( "Thunder Light", typeof( LightController ) );
            thunderLightGO.transform.SetParent( transform );
            thunderLightGO.transform.eulerAngles = new Vector3( 90, 0, 0 );

            LightController thunderLightController = thunderLightGO.GetComponent<LightController>();
            _environmentLightsReferencer.SetThunderLightController( thunderLightController );

            thunderLightController.Init( thunderLightGO.GetComponent<Light>(), Enums.Light_Type.Thunder );
            SetThunderLightParametersAsDefault( thunderLightController );
        }
        private void SetThunderLightParametersAsDefault( LightController controller )
        {
            int defaultLM = 1 << 0;
            int ignoreRaycastLM = 1 << 2;
            int groundLM = 1 << 6;

            int cullingMask = defaultLM | ignoreRaycastLM | groundLM;

            controller.SetLightParameters( LightType.Directional, LightmapBakeType.Mixed, LightRenderMode.ForceVertex, Color.white, 0, cullingMask );
        }

        public LightController GetLightControllerByType( Enums.Light_Type type )
        {
            foreach ( Transform trs in transform )
            {
                LightController controller = trs.GetComponent<LightController>();

                if ( controller.IsNull<LightController>() || controller.GetLightType() != type ) { continue; }
                return controller;
            }

            Debug.LogError( $"No controller of type {type} found." );
            return null;
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            Init();
        }
#endif

        #endregion
    }
}