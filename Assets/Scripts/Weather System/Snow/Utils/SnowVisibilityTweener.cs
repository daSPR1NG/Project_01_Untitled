using UnityEngine;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;
using DG.Tweening;
using Sirenix.Utilities;
using System.Collections;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    public class SnowVisibilityTweener
    {
        private readonly float _snowCycleTotalDuration;
        private float _currentSnowCycleValue;
        private readonly Material [] _snowMaterials;

        public SnowVisibilityTweener( float snowCycleTotalDuration, Material [] snowMaterials )
        {
            _snowCycleTotalDuration = snowCycleTotalDuration;
            _currentSnowCycleValue = 0;
            _snowMaterials = snowMaterials;
        }

        public void TweenSnowVisibility( MonoBehaviour monoBehaviour )
        {
            monoBehaviour.StartCoroutine( ExecuteTweenerCoroutine() );
        }

        #region Utils

        private IEnumerator ExecuteTweenerCoroutine()
        {
            do
            {
                // Update simulation
                _currentSnowCycleValue += Helper.GetDeltaTime() / _snowCycleTotalDuration;

                float visibilityPercentage = ( _currentSnowCycleValue / _snowCycleTotalDuration ) * 2;
                _snowMaterials [ 0 ].SetFloat( "_SnowOpacity", visibilityPercentage );

                // Exit condition
                if ( _currentSnowCycleValue >= _snowCycleTotalDuration )
                {
                    _currentSnowCycleValue = _snowCycleTotalDuration;
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            } while ( _currentSnowCycleValue < _snowCycleTotalDuration );
        }

        #endregion
    }
}