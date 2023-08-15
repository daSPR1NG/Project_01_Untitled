using UnityEngine;

namespace dnSR_Coding.Utilities.Runtime
{
    ///<summary> AudioEvent description <summary>
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play( AudioSource source );
    }
}