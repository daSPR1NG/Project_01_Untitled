using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/BuildingManager" )]

    ///<summary> BuildingManager description <summary>
    [Component("BuildingManager", "")]
    [DisallowMultipleComponent]
    public class BuildingManager : Singleton<BuildingManager>, IDebuggable
    {
        //[Title( "TITLE", 12, "white" )]

        [SerializeField] private List<Building> _buildings = new();

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion
                
        // Set all datas that need it at the start of the game
        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );

            GetLinkedComponents();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {

        }

        #region OnValidate

#if UNITY_EDITOR

        void FindBuildings()
        {
            _buildings.Clear();

            for ( int i = transform.childCount - 1; i >= 0; i-- )
            {
                transform.GetChild( i ).TryGetComponent( out Building building);
                _buildings.AppendItem( building );
            }
        }

        private void OnValidate()
        {
            FindBuildings();
        }
#endif

        #endregion
    }
}