using UnityEngine;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    ///<summary> Plant description <summary>
    [DisallowMultipleComponent]
    public class Plant : InteractiveObject, ISaveable
    {
        [SerializeField, ReadOnly, FoldoutGroup( "Plant Group" )] private string _id;
        public string ID { get => _id; set => _id = value; }

        [SerializeField, FoldoutGroup( "Plant Group" )] private PlantSettings _plantSettings;
        private Enums.Plant_GrowingState _growingState = Enums.Plant_GrowingState.Seed;
        private float _currentLifeCycleValue;

        private Transform RendererChildTrs => transform.GetFirstChild();
        private GameObject _plantRenderer;

        [ System.Serializable]
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

            AddRenderer_BasedOnSettings();

            PlantData plantData = ( PlantData ) DataSaveManager.Instance.LoadData<PlantData>( ID );

            if ( plantData.IsNull<PlantData>() )
            {
                _growingState = Enums.Plant_GrowingState.Seed;
                _currentLifeCycleValue = 0;
            }
            
            Load( DataSaveManager.Instance.LoadData<PlantData>( ID ) );
            DisplayPlantAppearance( ( int ) _growingState );
        }

        private void AddRenderer_BasedOnSettings()
        {
            if ( _plantSettings.InGameAppearance.IsNull<GameObject>() ) { return; }

            _plantRenderer = Instantiate( _plantSettings.InGameAppearance );
            _plantRenderer.transform.SetParent( RendererChildTrs );

            HideAllAppearances();
        }

        #endregion

        #region Life cycle - Runtime
        private void Update() => ProcessLifeCycle();

        private void ProcessLifeCycle()
        {
            if ( _growingState == Enums.Plant_GrowingState.Plant ) { return; }

            _currentLifeCycleValue += Helper.GetDeltaTime();
            //this.Debugger( $"Life Cycle value : {_currentLifeCycleValue}");

            if ( _currentLifeCycleValue >= ( _plantSettings.LifeCycleDuration / 2 )
                && _growingState == Enums.Plant_GrowingState.Seed ) 
            {
                _growingState = Enums.Plant_GrowingState.Sprout;
                OnReachingSproutState();
            }

            if ( _currentLifeCycleValue >= _plantSettings.LifeCycleDuration
                && _growingState == Enums.Plant_GrowingState.Sprout )
            {
                _currentLifeCycleValue = _plantSettings.LifeCycleDuration;
                _growingState = Enums.Plant_GrowingState.Plant;

                OnReachingPlantState();
            }
        }

        private void OnReachingSproutState()
        {
            this.Debugger( "Sprout state reached" );

            // Changer l'apparence > Animation - Tween
            DisplayPlantAppearance( ( int ) Enums.Plant_GrowingState.Sprout );

            // SFX
            // VFX
        }

        private void OnReachingPlantState()
        {
            this.Debugger( "Plant state reached" );
            IsInteractive = true;

            // Changer l'apparence > Animation - Tween
            DisplayPlantAppearance( ( int ) Enums.Plant_GrowingState.Plant );

            // SFX
            // VFX
        }

        private void DisplayPlantAppearance( int index )
        {
            this.Debugger( $"Appearance {index} is now displayed." );

            if ( _plantSettings.InGameAppearance.IsNull<GameObject>() ) { return; }

            HideAllAppearances();
            _plantSettings.GetAppearances() [ index ].gameObject.Display();
        }

        private void HideAllAppearances()
        {
            this.Debugger( "Hiding all plant appearances." );

            if ( _plantSettings.InGameAppearance.IsNull<GameObject>() ) { return; }

            foreach ( Transform trs in _plantSettings.GetAppearances() ) {
                trs.gameObject.Hide();
            }
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