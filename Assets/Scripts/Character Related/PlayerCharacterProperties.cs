using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> PlayerPropertiesManager description <summary>
    [DisallowMultipleComponent]
    public class PlayerCharacterProperties : Singleton<PlayerCharacterProperties>
    {
        protected override void Init( bool dontDestroyOnload )
        {
            base.Init();            
        }
    }
}