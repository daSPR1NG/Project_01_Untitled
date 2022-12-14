using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> PlayerPropertiesManager description <summary>
    [DisallowMultipleComponent]
    public class PlayerCharacterProperties : Singleton<PlayerCharacterProperties>
    {
        [SerializeField] private StatSheet _characterStats;
        [SerializeField] private ExperienceManager _characterExperience;
        
        protected override void Init( bool dontDestroyOnload )
        {
            base.Init( true );
        }
    }
}