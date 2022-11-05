using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> AudioManager description <summary>
    [Component( "AUDIO MANAGER", "Manages how sound is used in-game." )]
    public class AudioManager : Singleton<AudioManager>
    {
        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }
    }
}