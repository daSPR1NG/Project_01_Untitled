using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;

namespace dnSR_Coding.Project
{
    ///<summary> Plant description <summary>
    [DisallowMultipleComponent]
    public class Plant : MonoBehaviour, ISaveable, IDebuggable
    {
        [ReadOnly]
        [SerializeField] private string _id;
        public string ID { get => _id; set => _id = value; }

        [Header( "SETTINGS" )]
        [SerializeField] private float _lifeCycleDuration = 5f;
        private Enums.Plant_GrowingState _growingState = Enums.Plant_GrowingState.Seed;
        private float _currentLifeCycleValue;

        #region DEBUG

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        [Serializable]
        public struct PlantData
        {
            public string ID;
            public Enums.Plant_GrowingState GrowingState;
            public float CurrentLifeCycleValue;

            public PlantData( string iD, Enums.Plant_GrowingState growingState, float currentLifeCycleValue )
            {
                ID = iD;
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
            _growingState = Enums.Plant_GrowingState.Seed;
            _currentLifeCycleValue = 0;
            
            Load( DataSaveManager.Instance.LoadData<PlantData>( ID ) );
        }

        #endregion

        private void Update() => ProcessLifeCycle();

        private void ProcessLifeCycle()
        {
            if ( _growingState == Enums.Plant_GrowingState.Plant ) { return; }

            _currentLifeCycleValue += Helper.GetDeltaTime();
            this.Debugger( $"Life Cycle value : {_currentLifeCycleValue}");

            if ( _currentLifeCycleValue >= ( _lifeCycleDuration / 2 ) ) {
                _growingState = Enums.Plant_GrowingState.Sprout;
            }

            if ( _currentLifeCycleValue >= _lifeCycleDuration ) 
            {
                _currentLifeCycleValue = _lifeCycleDuration;
                _growingState = Enums.Plant_GrowingState.Plant;
            }
        }

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
    }
}