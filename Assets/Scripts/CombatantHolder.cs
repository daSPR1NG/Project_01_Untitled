using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/CombatantHolder" )]

    ///<summary> CombatantHolder description <summary>
    [DisallowMultipleComponent]
    public class CombatantHolder : MonoBehaviour, IDebuggable
    {
        [SerializeField] private Combatant _combatant;

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