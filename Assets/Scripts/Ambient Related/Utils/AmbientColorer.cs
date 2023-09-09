using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public class AmbientColorer
    {
        /// <summary>
        /// Sets ambient elements parameters and values (ambient color, main light color, fog color).
        /// </summary>
        /// <param name="lightController"></param>
        /// <param name="lightingPreset"></param>
        /// <param name="initialFogColor"></param>
        /// <param name="currentTimeOfDay"></param>
        public void SetAmbientElementsColor(
            LightController lightController, 
            AmbientLightingPreset lightingPreset, 
            Color initialFogColor, 
            float currentTimeOfDay )
        {
            // Ambient light color
            RenderSettings.ambientLight = lightingPreset.AmbientColor.Evaluate( currentTimeOfDay );

            // Main light color
            if ( !lightController.IsNull<LightController>() )
            {
                lightController.SetLightColor( lightingPreset.DirectionalColor.Evaluate( currentTimeOfDay ) );
            }

            // Fog color
            if ( RenderSettings.fog )
            {
                RenderSettings.fogColor = initialFogColor.MultiplyRGB( lightingPreset.FogColor.Evaluate( currentTimeOfDay ) );
            }
        }
    }
}