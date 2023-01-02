using UnityEngine;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum UnitTeam { Ally, Enemy, }

    ///<summary> CombatManager description <summary>
    [Component("CombatManager", "")]
    [DisallowMultipleComponent]
    public class CombatManager : MonoBehaviour, IDebuggable
    {
        //[Header( "Title" )]

        // Variables

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

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