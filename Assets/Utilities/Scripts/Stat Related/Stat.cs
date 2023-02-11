using UnityEngine;
using System;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    public enum StatTypeEnum { Unassigned, Strength, Endurance, Dexterity, }

    [Serializable]
    public class Stat : ISubject, IMinValue<int>, IModifiableStatValue, ILevelable
    {
        [HideInInspector] public string Name;
        [SerializeField] private StatTypeEnum _type;

        private readonly List<IObserver> _observers = new();
        public List<IObserver> Observers => _observers;

        public bool HasMinValue { get => true; }
        public int MinValue { get => 1; }

        [field: SerializeField, AllowNesting, ReadOnly] public int Value { get; private set; }

        #region Experience variables

        [field: SerializeField, AllowNesting, ReadOnly] public int Level { get; private set; }

        [field: SerializeField] public int InitialRequiredExperience { get; set; }

        [field: SerializeField, AllowNesting, ReadOnly] public int CurrentExperience { get; private set; }
        [field: SerializeField, AllowNesting, ReadOnly] public int RequiredExperienceToNextLevel { get; private set; }

        [field: SerializeField] public float RequiredExperienceScalingFactor { get; set; }
        [field: SerializeField] public int ExperienceMultiplier { get; set; }

        #endregion

        [field: SerializeField] public List<StatModifier> StatModifiers { get; set; }

        public static Action<object> OnStatModification;

        public void SetValue( int value )
        {
            Value = value;
        }
        public int GetTotalValue()
        {
            Value = Level;

            foreach ( var modifiers in StatModifiers )
            {
                modifiers.Apply( this );
            }

            return Helper.GetClampedValueWithMinValueFrom( Value, MinValue, HasMinValue );
        }

        public void AddExperience( int amount )
        {
            Debug.Log( "Add " + amount + " experience to : " + _type );

            int properAmount = Helper.GetMultipliedValueFrom( amount, ExperienceMultiplier );
            CurrentExperience += properAmount;

            while ( CurrentExperience >= RequiredExperienceToNextLevel )
            {
                Debug.Log( CurrentExperience );
                CurrentExperience -= RequiredExperienceToNextLevel;
                AddLevel();
                Debug.Log( Level );
            }

            Debug.Log( CurrentExperience );
            OnModification( OnStatModification, this );
        }
        public void IncreaseRequiredExperienceToNextLevel()
        {
            float raisedValue = Mathf.Pow( ( InitialRequiredExperience * Level ), RequiredExperienceScalingFactor );
            RequiredExperienceToNextLevel =  ExtMathfs.FloorToInt( raisedValue );
        }

        public void AddLevel()
        {
            Level++;
            IncreaseRequiredExperienceToNextLevel();
            OnModification( OnStatModification, this );
        }
        public void SetLevel( int value )
        {
            Level = value;
        }

        public void OnModification( Action<object> actionToExecute, object dataToPush )
        {
            ISubjectExtensions.TriggerEvent( actionToExecute, dataToPush );
        }

        public StatTypeEnum GetStatType() => _type;

        #region Constructors

        public Stat() : base() { }
        public Stat( StatTypeEnum type, int level ) : base()
        {
            Name = type.ToString();
            _type = type;
            Level = level;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        public void SetNameInEditor()
        {
            string name = _type.ToString() + " - " + GetTotalValue().ToString();
            if ( !Name.Equals( name ) ) { Name = name; }
        }
        
        public void ResetStatPropertiesInEditor()
        {
            StatModifiers.Clear();

            Level = MinValue;
            Value = Level;

            InitialRequiredExperience = 25;
            RequiredExperienceScalingFactor = 1;
            ExperienceMultiplier = 1;
            
            CurrentExperience = 0;
            RequiredExperienceToNextLevel = InitialRequiredExperience;

            SetNameInEditor();
            OnModification( OnStatModification, this );
        }

        public void SetValueInEditor()
        {
            if ( Value != GetTotalValue() ) { Value = GetTotalValue(); }
        }       

#endif
        #endregion
    }
}