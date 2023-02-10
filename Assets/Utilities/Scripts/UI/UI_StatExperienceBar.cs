using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class UI_StatExperienceBar : UI_FilledBar, IDebuggable, IObserver
    {
        public ISubject Subject { get; private set; }

        [Header( "Experience Bar details" )]

        [SerializeField] private StatType _observedStatType;
        [SerializeField] private TMP_Text _levelValueText;

        private PlayerCharacterProperties _playerCharacterProperties;

        private Stat _observedStat = null;
        private float _currentExperience;
        private float _maxExperienceValue;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable()
        {
            TryGetSubject();
            Helper.SubscribeToSubject( observer: this, Subject, onInitialization: () => UpdateFillBarElements( Subject ) );
        }
        void OnDisable() => Helper.UnsubscribeToSubject( observer: this, Subject );

        #endregion

        void Start() => gameObject.TryToHide();

        public void TryGetSubject()
        {
            Debug.Log( "Try to get subject." );

            Subject = null;

            if ( _playerCharacterProperties.IsNull() ) 
            {
                _playerCharacterProperties = FindObjectOfType<PlayerCharacterProperties>();
            }

            if ( _playerCharacterProperties.IsNull() )
            {
                Debug.LogError( "ISubject could not be found for this observer.", transform );
                return; 
            }

            Stat lookedForStat = _playerCharacterProperties.GetStatSheet().GetStatByType( _observedStatType );

            if ( lookedForStat.IsNull() ) 
            {
                Debug.LogError( "The _observedStat you're looking for doesn't exists.", transform );
                return; 
            }

            Subject = lookedForStat;

#if UNITY_EDITOR
            if ( !Application.isPlaying )
            {
                Subject.AddObserver( this );
            }
#endif
        }

        public void OnNotification( object value )
        {
            Helper.Log( this, "On being notified" );
            UpdateFillBarElements( value );
        }

        private void UpdateFillBarElements( object value )
        {
            _observedStat = ( Stat ) value;

            _currentExperience = _observedStat.CurrentExperience;
            _maxExperienceValue = _observedStat.RequiredExperienceToNextLevel;

            SetImageFillAmount( _currentExperience, _maxExperienceValue );

            SetFillBarValueText( _currentExperience.ToString() + " / " + _maxExperienceValue.ToString() );
            SetLevelValueText( "Level - " + _observedStat.Level.ToString() );
        }

        public override void SetImageFillAmount( float currentValue, float maxValue )
        {
            float fillAmount = currentValue / maxValue;
            _fillImage.fillAmount = fillAmount;
        }

        private void SetLevelValueText( string input )
        {
            _levelValueText.text = input;
        }

        public void SetFillBarValueText( string input )
        {
            _fillAmountValueText.text = input;
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            if ( !Application.isPlaying )
                TryGetSubject();
        }        

#endif

        #endregion
    }
}