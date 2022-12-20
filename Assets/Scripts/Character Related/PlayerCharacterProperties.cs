using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    // Required components or public findable enum here.
    [RequireComponent( typeof( CharacterStats ) )]
    [RequireComponent( typeof( StatsExperienceManager ) )]

    // Uncomment this block right below if you need to use add component menu for this component.
    [AddComponentMenu( menuName:"Custom Scripts/PlayerCharacterProperties" )]

    ///<summary> PlayerPropertiesManager description <summary>
    [Component("PlayerCharacterProperties", "")]
    [DisallowMultipleComponent]
    public class PlayerCharacterProperties : Singleton<PlayerCharacterProperties>
    {
        public CharacterStats CharacterStats { get; private set; }
        public ExperienceManager CharacterExperience { get; private set; }
        
        // Set all datas that need it at the start of the game
        protected override void Init( bool dontDestroyOnload )
        {
            base.Init( true );
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( CharacterStats.IsNull() ) { CharacterStats = GetComponent<CharacterStats>(); }
            if ( CharacterExperience.IsNull() ) { CharacterExperience = GetComponent<StatsExperienceManager>(); }
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

        #endregion
    }
}