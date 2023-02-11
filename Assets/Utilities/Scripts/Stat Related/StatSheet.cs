using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/Character/Stats/New Stat Sheet")]
    public class StatSheet : ScriptableObject
    {
        [SerializeField] private List<Stat> _stats = new();

        public void InitialiazeStatSheet()
        {
            //foreach ( Stat stat in _stats ) 
            //{
            //    stat.NotifyObservers( stat );
            //}
        }

        public Stat GetStatByType( StatTypeEnum statType )
        {
            if ( _stats.IsEmpty() ) 
            {
                Debug.LogError( "There is no stats in this stat sheet." );
                return null;
            }

            for ( int i = 0; i < _stats.Count; i++ )
            {
                if ( _stats [ i ].GetStatType() != statType ) { continue; }

                return _stats [ i ];
            }

            return _stats[0];
        }

        #region OnValidate

#if UNITY_EDITOR

        private void SetStatsNameInEditor()
		{
			foreach ( var stat in _stats )
			{
				stat.SetNameInEditor();
			}
		}

        private void SetStatsValueInEditor()
        {
            foreach ( var stat in _stats )
            {
                stat.SetValueInEditor();
            }
        }

        [Button]
        private void ResetStatPropertiesInEditor()
        {
            foreach ( var stat in _stats )
            {
                stat.ResetStatPropertiesInEditor();
            }
        }
        
        [Button]
        private void AddExperienceToStrengthStatInEditor()
        {
            GetStatByType( StatTypeEnum.Strength ).AddExperience( 5 );
        }

        [Button]
        private void AddExperienceToEnduranceStatInEditor()
        {
            GetStatByType( StatTypeEnum.Endurance ).AddExperience( 5 );
        }

        [Button]
        private void AddExperienceToDexterityStatInEditor()
        {
            GetStatByType( StatTypeEnum.Dexterity ).AddExperience( 5 );
        }

        private void OnValidate()
		{
			SetStatsNameInEditor();
            SetStatsValueInEditor();
        }

#endif

		#endregion
	}
}