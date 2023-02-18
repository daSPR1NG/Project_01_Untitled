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
        [Header( "Experience Bar details" )]

        [SerializeField] private Enums.StatType _observedStatType;
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

        void OnEnable() => Stat.OnStatModification += UpdateFillBarElements;
        void OnDisable() => Stat.OnStatModification -= UpdateFillBarElements;

        #endregion

        public void OnNotification( object value )
        {
            Helper.Log( this, "On being notified" );
            UpdateFillBarElements( value );
        }

        private void UpdateFillBarElements( object value )
        {
            _observedStat = ( Stat ) value;

            if ( _observedStat.IsNull() 
                || _observedStat.GetStatType() != _observedStatType ) 
            {
                return; 
            }

            //_currentExperience = _observedStat.CurrentExperience;
            //_maxExperienceValue = _observedStat.RequiredExperienceToNextLevel;

            //SetImageFillAmount( _currentExperience, _maxExperienceValue );

            //SetFillBarValueText( _currentExperience.ToString() + " / " + _maxExperienceValue.ToString() );
            //SetLevelValueText( "Level - " + _observedStat.Level.ToString() );
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

        public override void SetFillBarValueText( string input )
        {
            _fillAmountValueText.text = input;
        }
    }
}