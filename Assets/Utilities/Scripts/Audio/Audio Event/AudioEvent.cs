using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> AudioEvent description <summary>
    public abstract class AudioEvent : ScriptableObject
    {
        public abstract void Play( AudioSource source );
    }
}