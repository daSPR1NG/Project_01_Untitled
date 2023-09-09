using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{

    public class LightRotator
    {
        public void RotateLight_BasedOnCurrentTimeOfDay( LightController lightController, float timeOfDay )
        {
            if ( lightController.IsNull<LightController>() ) { return; }

            lightController.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timeOfDay * 360f ) - 90f, 170f, 0 ) );
        }
    }
}