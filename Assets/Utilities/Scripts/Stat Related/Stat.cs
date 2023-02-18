using UnityEngine;
using System;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [Serializable]
    public class Stat : LevelableObject, ISubject, IMinValue<int>, IModifiableStat
    {
        [HideInInspector] public string Name;
        [SerializeField] private Enums.StatType _type;

        private readonly List<IObserver> _observers = new();
        public List<IObserver> Observers => _observers;

        public bool HasMinValue { get => true; }
        public int MinValue { get => 1; }

        [field: SerializeField, AllowNesting, ReadOnly] public int Value { get; private set; }

        [field: SerializeField] public List<StatModifier> StatModifiers { get; set; }

        public static Action<object> OnStatModification;

        public void SetValue( int value )
        {
            Value = value;
        }
        public int GetTotalValue()
        {
            Value = GetLevel();

            foreach ( var modifiers in StatModifiers )
            {
                modifiers.Apply( this );
            }

            return Helper.GetClampedValueWithMinValueFrom( Value, MinValue, HasMinValue );
        }

        public override void AddExp( int amount )
        {
            base.AddExp( amount );
            OnModification( OnStatModification, new Tuple<int, int>( GetCurrentExp(), GetRequireExpToNextLevel() ) );
        }

        public override void AddLevel( int amount )
        {
            base.AddLevel( amount );
            OnModification( OnStatModification, this );
        }

        public void OnModification( Action<object> actionToExecute, object dataToPush )
        {
            ISubjectExtensions.TriggerAction( actionToExecute, dataToPush );
        }

        public Enums.StatType GetStatType() => _type;

        #region Constructors

        public Stat() : base() { }
        public Stat( Enums.StatType type, int level ) : base()
        {
            Name = type.ToString();
            _type = type;
            SetLevel( level );
        }

        #endregion

        public void ResetStatProperties( bool resetExp )
        {
            StatModifiers.Clear();

            if ( resetExp ) { ResetExp(); }

            SetLevel( MinValue );
            Value = GetLevel();

#if UNITY_EDITOR
            SetNameInEditor();
#endif
            OnModification( OnStatModification, this );
        }

        #region Editor

#if UNITY_EDITOR

        public void SetNameInEditor()
        {
            string name = _type.ToString() + " - " + GetTotalValue().ToString();
            if ( !Name.Equals( name ) ) { Name = name; }
        }        

        public void SetValueInEditor()
        {
            if ( Value != GetTotalValue() ) { Value = GetTotalValue(); }
        }       

#endif
        #endregion
    }
}