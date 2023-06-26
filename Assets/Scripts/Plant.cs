using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;

namespace dnSR_Coding.Project
{
    ///<summary> Plant description <summary>
    [DisallowMultipleComponent]
    public class Plant : MonoBehaviour, ILoadableData<Plant.PlantData>, IDebuggable
    {
        [Header( "SETTINGS" )]
        [SerializeField] private float _lifeCycleDuration = 5f;
        private Enums.Plant_GrowingState _growingState = Enums.Plant_GrowingState.Seed;
        private float _currentLifeCycleValue;
        private PlantData _plantData;

        private int ID => GetInstanceID();

        #region DEBUG

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        [Serializable]
        public struct PlantData : ISavableData<PlantData>
        {
            public Enums.Plant_GrowingState GrowingState;
            public float CurrentLifeCycleValue;

            [field: SerializeField] public int ID { get; set; }
            public readonly PlantData Get() => this;

            public void Set( int iD, Enums.Plant_GrowingState growingState, float currentLifeCycleValue )
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

            ISavableData<PlantData> savedData = DataSaveManager.Instance.LoadedData<PlantData>( DataSaveManager.PLANT_DATAS_FILE_NAME, ID );
            Load( savedData.Get() );
        }

        #endregion

        //private void Update() => ProcessLifeCycle();

        private void ProcessLifeCycle()
        {
            if ( _growingState == Enums.Plant_GrowingState.Plant ) { return; }

            _currentLifeCycleValue += Helper.GetDeltaTime();
            //this.Debugger( $"Life Cycle value : {_currentLifeCycleValue}");

            if ( _currentLifeCycleValue >= ( _lifeCycleDuration / 2 ) ) {
                _growingState = Enums.Plant_GrowingState.Sprout;
            }

            if ( _currentLifeCycleValue >= _lifeCycleDuration ) 
            {
                _currentLifeCycleValue = _lifeCycleDuration;
                _growingState = Enums.Plant_GrowingState.Plant;
            }
        }

        public PlantData GetData()
        {
            _plantData.Set( ID, _growingState, _currentLifeCycleValue );
            return _plantData;
        }

        [ContextMenu( "Save" )]
        public void Save()
        {
            DataSaveManager.Instance.SaveData( DataSaveManager.PLANT_DATAS_FILE_NAME, JsonUtility.ToJson( GetData(), true ) );
        }

        public void Load( PlantData data )
        {
            _growingState = data.GrowingState;
            _currentLifeCycleValue = data.CurrentLifeCycleValue;
        }
    }
}