#if UNITY_EDITOR
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

        private const string MAIN_LIGHT_NAME = "Main Light";
        private const string ADDITIONAL_LIGHT_NAME = "Additional Light";
        private const string THUNDER_LIGHT_NAME = "Thunder Light";

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        private struct LightReferenceInfo
        {
            public string Name;
            public Enums.Light_Type LightType;
            public EnvironmentLightsReferencer LightsReferencer;
            public LightController LightController;

            public LightReferenceInfo(
                string name,
                Enums.Light_Type lightType,
                EnvironmentLightsReferencer lightsReferencer,
                LightController lightController )
            {
                Name = name;
                LightType = lightType;
                LightsReferencer = lightsReferencer;
                LightController = lightController;
            }
        }

        #region SETUP

        // Set all datas that need it at the start of the game
        public void Init( EnvironmentLightsReferencer environmentLightsReferencer )
        {
            _environmentLightsReferencer = environmentLightsReferencer;
            this.Debugger( $"Initializing light container, referencer is : {_environmentLightsReferencer}." );

            LightReferenceInfo mainLightReferenceInfo = new LightReferenceInfo(
                    MAIN_LIGHT_NAME,
                    Enums.Light_Type.Main,
                    _environmentLightsReferencer,
                    GetLightControllerByType( Enums.Light_Type.Main ) );
            SetLightReference( mainLightReferenceInfo, false );

            LightReferenceInfo additionalLightReferenceInfo = new LightReferenceInfo(
                    ADDITIONAL_LIGHT_NAME,
                    Enums.Light_Type.Additional,
                    _environmentLightsReferencer,
                    GetLightControllerByType( Enums.Light_Type.Additional ) );
            SetLightReference( additionalLightReferenceInfo, true );

            LightReferenceInfo thunderLightReferenceInfo = new LightReferenceInfo(
                    THUNDER_LIGHT_NAME,
                    Enums.Light_Type.Thunder,
                    _environmentLightsReferencer,
                    GetLightControllerByType( Enums.Light_Type.Thunder ) );
            SetLightReference( thunderLightReferenceInfo, false );
        }

        #endregion

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

        private void SetAdditionalLightParametersAsDefault( LightController controller )
        {
            Color additionalLightColor = Color.white;
            if ( ColorUtility.TryParseHtmlString( "#FFCB00", out Color color ) ) { additionalLightColor = color; }

            controller.SetLightParameters( LightType.Directional, LightmapBakeType.Mixed, additionalLightColor, 1 );
        }

        private void SetThunderLightParametersAsDefault( LightController controller )
        {
            int defaultLM = 1 << 0;
            int ignoreRaycastLM = 1 << 2;
            int groundLM = 1 << 6;

            int cullingMask = defaultLM | ignoreRaycastLM | groundLM;

            controller.SetLightParameters( LightType.Directional, LightmapBakeType.Mixed, LightRenderMode.ForceVertex, Color.white, 0, cullingMask );
        }

        #region UTILS

        private void SetLightReference(
            LightReferenceInfo lightReferenceInfo,
            bool isLightDisabled )
        {
            if ( !lightReferenceInfo.LightController.IsNull() )
            {
                Debug.Log( $"Light of type {lightReferenceInfo.LightType} was found so we set it in case parameters are not." );
                SetControllersAndTheirParameters( lightReferenceInfo );
                return;
            }

            Debug.Log( $"No light of type {lightReferenceInfo.LightType} was found so a new one has been created." );

            // LIGHT REFERENCE
            CreateLightReference( lightReferenceInfo, isLightDisabled );
        }

        private void CreateLightReference(
            LightReferenceInfo lightReferenceInfo,
            bool isLightDisabled )
        {
            Debug.Log( $"In the process of creating a light of type {lightReferenceInfo.LightType}" );

            //Object creation and component assignment
            GameObject createdLightGO = new( lightReferenceInfo.Name, typeof( LightController ) );
            createdLightGO.transform.SetParent( transform );

            LightController lightController = createdLightGO.GetComponent<LightController>();
            lightReferenceInfo.LightController = lightController;

            lightController.Init( createdLightGO.GetComponent<Light>(), lightReferenceInfo.LightType );

            switch ( lightReferenceInfo.LightType )
            {
                case Enums.Light_Type.Main:
                    lightReferenceInfo.LightsReferencer.SetMainLightController( lightController );
                    break;
                case Enums.Light_Type.Additional:
                    lightReferenceInfo.LightsReferencer.SetAdditionalLightController( lightController );
                    break;
                case Enums.Light_Type.Thunder:
                    lightReferenceInfo.LightsReferencer.SetThunderLightController( lightController );
                    break;
            }

            SetControllersLightParameters( lightReferenceInfo );

            if ( isLightDisabled ) { lightController.DisableLight(); }
        }

        private void SetControllersAndTheirParameters( LightReferenceInfo lightReferenceInfo )
        {
            EnvironmentLightsReferencer lightsReferencer = lightReferenceInfo.LightsReferencer;
            LightController lightController = lightReferenceInfo.LightController;

            switch ( lightReferenceInfo.LightType )
            {
                case Enums.Light_Type.Main:
                    lightsReferencer.SetMainLightController( lightController );
                    lightsReferencer.MainLightController.SetControllerLight( lightController.GetControllerLight() );
                    break;

                case Enums.Light_Type.Additional:
                    lightsReferencer.SetAdditionalLightController( lightController );
                    lightsReferencer.AdditionalLightController.SetControllerLight( lightController.GetControllerLight() );
                    break;

                case Enums.Light_Type.Thunder:
                    lightsReferencer.SetThunderLightController( lightController );
                    lightsReferencer.ThunderLightController.SetControllerLight( lightController.GetControllerLight() );
                    break;
            }

            SetControllersLightParameters( lightReferenceInfo );
        }

        private void SetControllersLightParameters( LightReferenceInfo lightReferenceInfo )
        {
            switch ( lightReferenceInfo.LightType )
            {
                case Enums.Light_Type.Main:
                    SetMainLightParametersAsDefault( lightReferenceInfo.LightController );
                    break;

                case Enums.Light_Type.Additional:
                    SetAdditionalLightParametersAsDefault( lightReferenceInfo.LightController );
                    break;

                case Enums.Light_Type.Thunder:
                    SetThunderLightParametersAsDefault( lightReferenceInfo.LightController );
                    break;
            }
        }

        public LightController GetLightControllerByType( Enums.Light_Type type )
        {
            foreach ( Transform trs in transform )
            {
                LightController controller = trs.GetComponent<LightController>();

                if ( controller.IsNull<LightController>() || controller.GetLightType() != type ) { continue; }
                return controller;
            }

            this.Debugger(
                $"No controller of type {type} found.",
                DebugType.Error );
            return null;
        }

        #endregion
    }
}
#endif