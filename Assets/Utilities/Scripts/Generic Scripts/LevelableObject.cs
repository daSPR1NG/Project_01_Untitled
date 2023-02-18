using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;
using static UnityEngine.Rendering.DebugUI;

namespace dnSR_Coding
{
    public abstract class LevelableObject
    {
        #region Experience variables

        // Level
        private int _level;

        // Exp details and settings
        private int _initialRequiredExp;
        private int _currentExp;
        private int _requiredExpToNextLevel;

        // Scaling factors
        private float _requiredExpScalingFactor;
        private float _expMultiplier;

        #endregion
        
        #region Level handle

        public virtual void AddLevel( int amount )
        {
            _level += amount;
            IncreaseRequiredExpToNextLevel();
        }

        public int GetLevel() => _level;
        public virtual void SetLevel( int value ) => _level = value;

        #endregion

        #region Experience handle

        public virtual void AddExp( int amount )
        {
            int properAmount = Helper.GetMultipliedValueFrom( amount, _expMultiplier );
            _currentExp += properAmount;

            while ( _currentExp >= _requiredExpToNextLevel )
            {
                Debug.Log( _currentExp );
                _currentExp -= _requiredExpToNextLevel;
                AddLevel( 1 );
                Debug.Log( _level );
            }

            Debug.Log( _currentExp );
        }

        public int GetCurrentExp() => _currentExp;
        public void SetCurrentExp( int value ) => _currentExp = value;

        public int GetInitialRequiredExp() => _initialRequiredExp;
        public void SetInitialRequiredExp( int value ) => _initialRequiredExp = value;

        protected int GetRequireExpToNextLevel() => _requiredExpToNextLevel;
        public void SetRequireExpToNextLevel( int value ) => _requiredExpToNextLevel = value;

        private void IncreaseRequiredExpToNextLevel()
        {
            float raisedValue = Mathf.Pow( ( _initialRequiredExp * _level ), _requiredExpScalingFactor );
            _requiredExpToNextLevel = ExtMathfs.FloorToInt( raisedValue );
        }

        #endregion

        #region Scaling factors handle

        //public float GetRequiredExpScalingFactor() => _requiredExpScalingFactor;
        public void SetRequiredExpScalingFactor( float value ) => _requiredExpScalingFactor = value;

        //public float GetExperienceMultiplier() => _expMultiplier;
        public void SetExperienceMultiplier( float value ) => _expMultiplier = value;

        #endregion

        #region On Editor

#if UNITY_EDITOR
        protected void ResetExp()
        {
            SetInitialRequiredExp( 25 );
            SetRequiredExpScalingFactor( 1 );
            SetExperienceMultiplier( 1 );

            SetCurrentExp( 0 );
            SetRequireExpToNextLevel( GetInitialRequiredExp() );
        }
#endif

        #endregion
    }
}