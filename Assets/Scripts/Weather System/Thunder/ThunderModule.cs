using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using dnSR_Coding.Attributes;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Thunder Module Settings" )]
    public class ThunderModule : WeatherSystemModule<ThunderModule.ThunderSettings>
    {
        [Serializable]
        public struct ThunderSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _associatedThunderType;
                }
            }

            [SerializeField, NamedArrayElement( typeof( Enums.ThunderType ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.ThunderType _associatedThunderType;

            [field: Header( "Audio" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, AllowNesting, ShowIf( "_hasAudio" )]
            public SimpleAudioEvent AudioEvent { get; private set; }
        }
        public ThunderSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        public void ApplySettings( Enums.ThunderType thunderType )
        {
            if ( Settings.IsEmpty() )
            {
                Settings.LogIsEmpty();
                return;
            }

            ThunderSettings settings = GetSettingsByID( ( int ) thunderType );

            this.Log( $"Thunder setting has been applied with ID : {settings.ID}." );
        }
        public void Stop()
        {

            this.Log( "Thunder setting has been stopped." );
        }
    }
}