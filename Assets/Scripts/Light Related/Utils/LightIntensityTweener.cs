using DG.Tweening;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public class LightIntensityTweener
    {
        public void TweenLightIntensity(
            Tween tween,
            LightController lightController,
            float lightIntensity,
            float shiftDuration
            )
        {
            if ( lightController.IsNull() ) { return; }

            bool isLightSetOrBeingSet = 
                lightController.DoesLightIntensityEquals( lightIntensity ) || tween.IsActive();
            if ( isLightSetOrBeingSet ) { return; }

            TweenMainLightIntensity(
                tween,
                lightController,
                lightIntensity,
                shiftDuration,
                () => lightController.SetLightIntensity( lightIntensity ) );
        }

        private void TweenMainLightIntensity(
            Tween tween,
            LightController mainLightController,
            float valueToReach,
            float duration,
            System.Action onCompleteAction
            )
        {
            tween = DOTween.To(
                () => mainLightController.GetControllerLight().intensity,
                _ => mainLightController.GetControllerLight().intensity = _,
                valueToReach,
                duration );

            tween.OnComplete( () =>
            {
                onCompleteAction?.Invoke();
                tween.Kill();
            } );
        }
    }
}