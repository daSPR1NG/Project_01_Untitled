using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using dnSR_Coding.Interfaces.Gameplay;

namespace dnSR_Coding.Project
{
    ///<summary> Plant description <summary>
    [DisallowMultipleComponent]
    public class Plant : InteractiveObject, ISaveable, IDebuggable
    {
        [ReadOnly]
        [SerializeField] private string _id;
        public string ID { get => _id; set => _id = value; }

        [Header( "SETTINGS" )]
        [SerializeField] private float _lifeCycleDuration = 5f;
        private Enums.Plant_GrowingState _growingState = Enums.Plant_GrowingState.Seed;
        private float _currentLifeCycleValue;

        [System.Serializable]
        public struct PlantData : IDataIdentifiers
        {
            public string ID { get; set; }
            public string TypeName { get; set; }

            public Enums.Plant_GrowingState GrowingState { get; private set; }
            public float CurrentLifeCycleValue { get; private set; }

            public PlantData( string iD, Enums.Plant_GrowingState growingState, float currentLifeCycleValue )
            {
                ID = iD;
                TypeName = Helper.GetTypeName( typeof( PlantData ) );

                GrowingState = growingState;
                CurrentLifeCycleValue = currentLifeCycleValue;
            }
        }

        #region SETUP

        void Start() => Init();
        
        // Set all datas that need it at the start of the game
        [ContextMenu( "Init Plant datas" )]
        public void Init()
        {
            IsInteractive = false;
            Interactor = null;

            _growingState = Enums.Plant_GrowingState.Seed;
            _currentLifeCycleValue = 0;
            
            Load( DataSaveManager.Instance.LoadData<PlantData>( ID ) );
        }

        #endregion

        #region Life cycle
        private void Update() => ProcessLifeCycle();

        private void ProcessLifeCycle()
        {
            if ( _growingState == Enums.Plant_GrowingState.Plant ) { return; }

            _currentLifeCycleValue += Helper.GetDeltaTime();
            //this.Debugger( $"Life Cycle value : {_currentLifeCycleValue}");

            if ( _currentLifeCycleValue >= ( _lifeCycleDuration / 2 )
                && _growingState == Enums.Plant_GrowingState.Seed ) 
            {
                _growingState = Enums.Plant_GrowingState.Sprout;
                OnReachingSproutState();
            }

            if ( _currentLifeCycleValue >= _lifeCycleDuration 
                && _growingState == Enums.Plant_GrowingState.Sprout )
            {
                _currentLifeCycleValue = _lifeCycleDuration;
                _growingState = Enums.Plant_GrowingState.Plant;

                OnReachingPlantState();
            }
        }

        private void OnReachingSproutState()
        {
            this.Debugger( "Sprout state reached" );

            // Changer l'apparence > Animation - Tween
            // SFX
            // VFX
        }

        private void OnReachingPlantState()
        {
            this.Debugger( "Plant state reached" );
            IsInteractive = true;

            // Changer l'apparence > Animation - Tween
            // SFX
            // VFX
        }
        #endregion

        #region Interaction

        public override void BeginInteraction( object interactor )
        {
            base.BeginInteraction( interactor );
        }

        public override void EndInteraction()
        {
            base.EndInteraction();
        }

        #endregion

        #region Save / Load

        public object GetData()
        {
            _id = ISaveable.ISaveableExtensions.GetID( _id );
            return new PlantData( _id, _growingState, _currentLifeCycleValue );
        }

        public void Load( object data )
        {
            PlantData plantData = ( PlantData ) data;

            _growingState = plantData.GrowingState;
            _currentLifeCycleValue = plantData.CurrentLifeCycleValue;
        }

        #endregion
    }
}