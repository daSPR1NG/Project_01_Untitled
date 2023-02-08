using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class UI_StatExperienceBar : MonoBehaviour, IDebuggable, IObserver, IFilledBarWithText
    {
        [field: SerializeField] public Subject Subject { get; private set; }
        [field: SerializeField] public StatType ObservedStatType { get; set; }

        [Header( "UI COMPONENTS" )]

        [SerializeField] private TMP_Text _levelValueText;
        [field: SerializeField] public Image FilledImage { get; set; }
        [field: SerializeField] public TMP_Text FilledBarValueText { get; set; }

        PlayerCharacterProperties _playerCharacterProperties;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable()
        {
            TryGetSubject();
            Helper.SubscribeToSubject( observer: this, Subject );
        }
        void OnDisable() => Helper.UnsubscribeToSubject( observer: this, Subject );

        #endregion

        public void TryGetSubject()
        {
            if ( _playerCharacterProperties.IsNull() ) 
            {
                _playerCharacterProperties = FindObjectOfType<PlayerCharacterProperties>();
            }

            if ( _playerCharacterProperties.IsNull() )
            {
                Debug.LogError( "Subject could not be found for this observer.", transform );
                return; 
            }

            Stat lookedForStat = _playerCharacterProperties.GetStatSheet().GetStatByType( ObservedStatType );

            if ( lookedForStat.IsNull() ) 
            {
                Debug.LogError( "The stat you're looking for doesn't exists.", transform );
                return; 
            }

            Subject = lookedForStat;
            //Debug.Log( Subject.ToLogValue() );

#if UNITY_EDITOR
            if ( !Application.isPlaying )
            {
                Subject.AddObserver( this );
            }
#endif
        }

        private void SetLevelValueText( string input )
        {
            if ( !_levelValueText.Equals( input ) ) { _levelValueText.SetText( input ); }
        }

        public void OnNotify( object value )
        {
            Helper.Log( this, "On being notified" );
            Stat stat = value as Stat;

            float currentExperience = stat.CurrentExperience;
            float minExperienceValue = 0;
            float maxExperienceValue = stat.RequiredExperienceToNextLevel;

            Helper.SetFilledBar(
                FilledImage,
                currentExperience, minExperienceValue, maxExperienceValue );

            string fillValueInput = currentExperience.ToString() + " / " + maxExperienceValue.ToString();
            SetFillBarValueText( fillValueInput );

            string levelValueInput = "Level - " + stat.Level.ToString();
            SetLevelValueText( levelValueInput );
        }

        public void SetFillBarValueText( string input )
        {
            if ( !FilledBarValueText.Equals( input ) ) { FilledBarValueText.SetText( input ); }
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            TryGetSubject();
        }    
        
#endif

        #endregion
    }
}