using UnityEngine;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using System.Linq;
using NaughtyAttributes;

namespace dnSR_Coding
{
    ///<summary> ChildPositioner description <summary>
    [DisallowMultipleComponent]
    public class ChildPositioner : MonoBehaviour, IDebuggable
    {
        //[Header( "Title" )]

        // Variables
        [SerializeField, ReorderableList] private List<ChildPositioning> _childPositions = new();

        [System.Serializable]
        public class ChildPositioning
        {
            [HideInInspector] public string Name;
            public int Position;
            public Transform ChildrenTrs;

            public ChildPositioning( int position, Transform childrenTrs )
            {
                Name = childrenTrs.name;
                Position = position;
                ChildrenTrs = childrenTrs;
            }
        }


        #region DEBUG

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region SETUP

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _childPositions.Count() >= transform.GetExactChildCount( true ) ) { return; }

            int i = 0;
            foreach ( Transform trs in transform ) {
                ChildPositioning childPositioning = new ChildPositioning( i, trs );

                _childPositions.AppendItem( childPositioning );
                this.Debugger( trs.name );
                i++;
            }

            this.Debugger( _childPositions.Count() );
        }

        #endregion

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate() {
            GetLinkedComponents();
        }
#endif

        #endregion
    }
}