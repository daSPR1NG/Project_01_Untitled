using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/Plant" )]

    ///<summary> Plant description <summary>
    [DisallowMultipleComponent]
    public class Plant : MonoBehaviour, IDebuggable
    {
        [Header( "SETTINGS" )]
        [SerializeField] private float _lifeCycleDuration = 5f;
        private Enums.Plant_GrowingState _growingState = Enums.Plant_GrowingState.Seed;
        private float _currentLifeCycleValue;

        // Variables

        #region DEBUG

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region ENABLE, DISABLE

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        #region SETUP

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();
            _growingState = Enums.Plant_GrowingState.Seed;
            _currentLifeCycleValue = 0;
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {

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

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

        #endregion
    }
}