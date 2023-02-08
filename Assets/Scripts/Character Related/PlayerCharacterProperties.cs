using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> PlayerPropertiesManager description <summary>
    [DisallowMultipleComponent]
    public class PlayerCharacterProperties : Singleton<PlayerCharacterProperties>
    {
        [SerializeField] private StatSheet _characterStatSheet;
        
        protected override void Init( bool dontDestroyOnload )
        {
            base.Init( true );
        }

        public StatSheet GetStatSheet() => _characterStatSheet;
    }
}