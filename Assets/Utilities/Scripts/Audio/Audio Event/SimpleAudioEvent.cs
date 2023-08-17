using UnityEngine;
using System.Collections.Generic;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using Sirenix.OdinInspector;

namespace dnSR_Coding.Utilities.Runtime
{
    ///<summary> SimpleAudioEvent description <summary>
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Audio Events/Simple" )]
    public class SimpleAudioEvent : AudioEvent
    {
        [Header( "SETTINGS" )]
        [MinMaxSlider( -3, 3 )] public Vector2 Pitch = new( 0, 1 );
        [MinMaxSlider( 0, 1 )] public Vector2 Volume = new( 1, 1 );

        [Space( 4f ), HorizontalLine( .5f, EditorColor.Grey )]

        public List<ClipData> Clips = new();

        private AudioClip _chosenClip = null;
        public AudioClip ChosenClip
        {
            get
            {
                if ( _chosenClip == null && !Clips.IsEmpty() ) return Clips [ 0 ].Clip;
                else return _chosenClip;
            }
        }

        private float _lastPitchValue = 0;
        private float _lastVolumeValue = 0;
        public float GetLastPitchValue => _lastPitchValue;
        public float GetLastVolumeValue => _lastVolumeValue;

        [System.Serializable]
        public class ClipData
        {
            [HideInInspector] public string Name = "";
            public AudioClip Clip = null;

            public ClipData( string name, AudioClip clip )
            {
                Name = name;
                Clip = clip;
            }

            public ClipData() : base()
            {
                Name = "New Clip - Unreferenced";
                Clip = null;
            }
        }

        public override void Play( AudioSource source )
        {
            _chosenClip = Clips [ Random.Range( 0, Clips.Count ) ].Clip;

            if ( Clips.Count == 0 || _chosenClip.IsNull<SimpleAudioEvent>() ) { return; }

            source.clip = _chosenClip;

            source.pitch = Random.Range( Pitch.x, Pitch.y );
            _lastPitchValue = source.pitch;

            source.volume = Random.Range( Volume.x, Volume.y );
            _lastVolumeValue = source.volume;

            string clipName = _chosenClip.GetName().ToUpper();

            Debug.Log( "SETTINGS CHOSEN FOR " + clipName.ToLogValue() + '\n' + "Pitch: " + source.pitch + " / " + "Volume: " + source.volume );

            source.Play();
        }

        #region OnValidate

#if UNITY_EDITOR
        private void OnValidate()
        {
            if ( Clips.Count == 0 ) { return; }

            for ( int i = Clips.Count - 1; i >= 0; i-- )
            {
                if ( Clips [ i ].Clip.IsNull<AudioClip>() ) { continue; }

                Clips [ i ].Name = i + " - " + Clips [ i ].Clip.GetName().ToUpper();
            }
        }
#endif

        #endregion
    }
}