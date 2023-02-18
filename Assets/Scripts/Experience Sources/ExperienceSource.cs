using UnityEngine;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using System;

namespace dnSR_Coding
{
    ///<summary> ExperienceSource description <summary>
    [DisallowMultipleComponent]
    public class ExperienceSource : MonoBehaviour, IDebuggable
    {
        [SerializeField] private bool _canGenerate = true;

        [Header( "Generation Settings" )]
        [SerializeField,/* ShowIf( "_canGenerate" )*/]
        private float _creationFrequency = 1f;

        [Space( 5f ), SerializeField /*ShowIf( "_canGenerate" )*/] 
        private List<GeneratedExperienceData> _generatedExperiences = new();
        public float CreationFrequency 
        { 
            get => _creationFrequency;
            private set => _creationFrequency =  value ; 
        }

        [ShowNonSerializedField] private float _typeLimit = 0;
        [ShowNonSerializedField] private float _currentGenerationTimer = 0;

        public static Action<Enums.StatType, int> OnGeneratingExperience;

        [System.Serializable]
        public class GeneratedExperienceData
        {
            private const int HIGH_EXP_VALUE = 10;
            private const int MEDIUM_EXP_VALUE = 7;
            private const int LOW_EXP_VALUE = 4;
            private const int VERYLOW_EXP_VALUE = 2;

            public enum ValueType { High, Medium, Low, VeryLow }

            [SerializeField] private string Name;
            public Enums.StatType LinkedStatType;
            [SerializeField] private ValueType Type;
            public int GenerationCapacity { get; private set; }

            public void UpgradeCapacity( int newValue )
            {
                if ( GenerationCapacity != newValue ) { GenerationCapacity = newValue; }               
            }

            public void SetToDefault()
            {
                Name = LinkedStatType.ToString();
                switch ( Type )
                {
                    case ValueType.High:

                        GenerationCapacity = HIGH_EXP_VALUE;
                        break;

                    case ValueType.Medium:

                        GenerationCapacity = MEDIUM_EXP_VALUE;
                        break;

                    case ValueType.Low:

                        GenerationCapacity = LOW_EXP_VALUE;
                        break;

                    case ValueType.VeryLow:

                        GenerationCapacity = VERYLOW_EXP_VALUE;
                        break;
                }
            }

            public GeneratedExperienceData( Enums.StatType linkedStatType )
            {
                LinkedStatType = linkedStatType;
                Type = ValueType.VeryLow;

                SetToDefault();
            }
        }

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        private void Awake() => Init();
        private void Init()
        {
            _typeLimit = 3;
            SetGeneratedExperienceDatasToDefault();
        }

        private void Update() => ProcessGeneration();

        /// <summary>
        /// Updates the generation timer.
        /// </summary>
        public void ProcessGeneration()
        {
            if ( GameManager.Instance.IsGamePaused() || !_canGenerate ) { return; }

            _currentGenerationTimer += Helper.GetRealDeltaTime( true );

            if ( _currentGenerationTimer >= CreationFrequency )
            {
                _currentGenerationTimer = 0;
                GenerateExperience();
            }
        }

        /// <summary>
        /// Generates experience when the timer reaches a certain value.
        /// </summary>
        private void GenerateExperience()
        {
            if ( !_generatedExperiences.IsEmpty() )
            {
                for ( int i = 0; i < _generatedExperiences.Count; i++ )
                {
                    if ( _generatedExperiences [ i ].IsNull() )
                    {
                        Debug.LogError( "One or more of linked resources are null, this means something is wrong." );
                        continue;
                    }

                    PushExperience( 
                        _generatedExperiences [ i ].LinkedStatType, 
                        _generatedExperiences [ i ].GenerationCapacity );

                    //Debug.Log( _generatedExperiences [ i ].ToString() );
                }
            }
        }

        /// <summary>
        /// Pushes the generated experience in order to update listeners.
        /// </summary>
        /// <param name="type"> The type of experience being pushed </param>
        /// <param name="value"> The amount pushed </param>
        public void PushExperience( Enums.StatType type, int value )
        {
            Helper.Log( this, "Pushing " + value + " experience _points for " + type.ToString() );

            OnGeneratingExperience?.Invoke( type, value );
            // Energy consomation
            // UI event
        }

        #region Upgrades

        public void UpgradeCreationFrequency( float value )
        {
            if ( CreationFrequency == value ) { return; }
            CreationFrequency = value;
        }

        public void UpgradeGeneratedCapacity( Enums.StatType type, int value )
        {
            if ( _generatedExperiences.IsEmpty() ) 
            {
                Debug.LogError( "The list about generated experiences is empty, this means something is wrong." );
                return; 
            }

            for ( int i = _generatedExperiences.Count - 1; i >= 0; i-- )
            {
                if ( _generatedExperiences [ i ].LinkedStatType != type ) { continue; }

                _generatedExperiences [ i ].UpgradeCapacity( value );
            }
        }

        [Button]
        private void TestAddExperienceDataOfType()
        {
            AddExperienceDataOfType( Enums.StatType.Dexterity_DEX );
        }

        public void AddExperienceDataOfType( Enums.StatType statType )
        {
            if ( _generatedExperiences.Count >= _typeLimit )
            {
                Debug.LogError( "Cant add another type." );
                return;
            }

            _generatedExperiences.AppendItem( new GeneratedExperienceData( statType ) );
        }

        #endregion

        private void SetGeneratedExperienceDatasToDefault()
        {
            if ( _generatedExperiences.IsEmpty() )
            {
                Debug.LogError( "The list about generated experiences is empty, this means something is wrong." );
                return;
            }

            for ( int i = _generatedExperiences.Count - 1; i >= 0; i-- )
            {
                _generatedExperiences [ i ].SetToDefault();
            }
        }

        [Button]
        public void Reset()
        {
            _canGenerate = false;
            _currentGenerationTimer = 0;

            CreationFrequency = 1;

            _generatedExperiences.Clear();
        }

        #region OnValidate

#if UNITY_EDITOR        

        private void OnValidate()
        {
            if ( Application.isPlaying ) { return; }

            _typeLimit = 3;
            SetGeneratedExperienceDatasToDefault();
        }
#endif

        #endregion
    }
}