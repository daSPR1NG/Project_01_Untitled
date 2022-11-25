using UnityEngine;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum BuildingType
    {
        Unassigned, Farm, Forge, Mine, Sawmill,
    }

    ///<summary> Building description <summary>
    [DisallowMultipleComponent]
    public class Building : MonoBehaviour
    {
        [Title( "Building Settings", 12, "white" )]

        [SerializeField] private bool _canGenerateResources = true;
        [SerializeField] private BuildingType _type = BuildingType.Unassigned;

        [Space( 10f )]

        [Title( "Linked Resources Settings", 12, "white" )]

        [SerializeField, /*ShowIf( "_canGenerateResources" )*/] 
        private bool _hasMaxLimit = true;
        [SerializeField, /*ShowIf( "_canGenerateResources" )*/] 
        private int _totalAmountOfResources = 0;
        [SerializeField, ShowIf( EConditionOperator.And, "_canGenerateResources", "_hasMaxLimit" )]
        private int _maxLimit = 500;

        [Title( "Generation Settings", 12, "white" )]

        [SerializeField, /*ShowIf( "_canGenerateResources" )*/] 
        private int _amountOfResourcesCreated = 1;
        [SerializeField,/* ShowIf( "_canGenerateResources" )*/] 
        private float _frequencyOfGettingResources = 1f;

        [Space( 5f ), SerializeField, Expandable, /*ShowIf( "_canGenerateResources" )*/] 
        private BasicResource _linkedResource;

        public BasicResource LinkedResource { get => _linkedResource; }

        public int MaxLimit 
        { get => _maxLimit; private set => _maxLimit = value; }
        public int TotalAmountOfResources
        {
            get => _totalAmountOfResources;
            private set => _totalAmountOfResources = value;
        }

        public int AmountOfResourcesCreated { get => _amountOfResourcesCreated; 
            private set => _amountOfResourcesCreated =  value ; }
        public float FrequencyOfGettingResources { get => _frequencyOfGettingResources; 
            private set => _frequencyOfGettingResources =  value ; }

        private float _currentGeneratingTimer = 0;

        private void Update() => GenerateResourcesOvertime();
        public void GenerateResourcesOvertime()
        {
            if ( GameManager.Instance.IsGamePaused() || !_canGenerateResources ) { return; }

            _currentGeneratingTimer += Time.deltaTime;

            if ( _currentGeneratingTimer >= FrequencyOfGettingResources )
            {
                _currentGeneratingTimer = 0;
                AddResources( AmountOfResourcesCreated );
                Debug.Log( LinkedResource.Type.ToString()
                    + ": "
                    + TotalAmountOfResources );
            }
        }

        #region Add/Remove

        public void AddResources( int value )
        {
            if ( MaxLimitIsReached( value ) )
            {
                TotalAmountOfResources = MaxLimit;
                return;
            }

            TotalAmountOfResources += value;
            // UI event
        }
        public void RemoveResources( int value )
        {
            if ( MinLimitIsReached( value ) )
            {
                TotalAmountOfResources = 0;
                return;
            }

            TotalAmountOfResources -= value;
            // UI event
        }

        #endregion

        #region Upgrades

        public void UpgradeMaxLimit( int value )
        {
            if ( MaxLimit == value ) { return; }
            MaxLimit = value;
        }
        public void UpgradeAmountOfResourcesCreated( int value )
        {
            if ( AmountOfResourcesCreated == value ) { return; }
            AmountOfResourcesCreated = value;
        }
        public void UpgradeFrequency( float value )
        {
            if ( FrequencyOfGettingResources == value ) { return; }
            FrequencyOfGettingResources = value;
        }

        #endregion

        public bool MinLimitIsReached( int value )
        { 
            return TotalAmountOfResources - value <= 0;
        }
        public bool MaxLimitIsReached( int value )
        {
            return  _hasMaxLimit && TotalAmountOfResources + value >= MaxLimit;
        }

        [Button]
        public void Reset()
        {
            _type = BuildingType.Unassigned;

            TotalAmountOfResources = 0;
            AmountOfResourcesCreated = 1;

            _linkedResource = null;
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
        }
#endif

        #endregion
    }
}