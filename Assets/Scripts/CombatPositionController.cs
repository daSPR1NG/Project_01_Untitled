using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class CombatPositionController : MonoBehaviour, IObserver, IDebuggable
    {
        [Header( "Dependencies" )]

        [SerializeField] private int _combatPositionLimit;
        [SerializeField] private GameObject _combatPositionPrefab;

        [Header( "Spacing settings" )]

        [SerializeField] private bool _isThePositioningAsymmetric = true;
        [SerializeField] private Vector3 _spaceBetweenParentPositions = Vector3.zero;
        [SerializeField] private Vector3 _spaceBetweenPlayerPositions = Vector3.zero;
        [SerializeField, ShowIf( "_isThePositioningAsymmetric" )] 
        private Vector3 _spaceBetweenEnemyPositions = Vector3.zero;

        private Transform GetPlayerPositionsParent => transform.GetFirstChild();
        private Transform GetEnemyPositionsParent => transform.GetChild(1);

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            CombatManager.OnEnteringCombat += OnNotification;
        }

        void OnDisable() 
        {
            CombatManager.OnEnteringCombat -= OnNotification;
        }

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
            HidePositionParents();

            HideEachCombatPosition( false );
            HideEachCombatPosition( true );
        }

        private void DisplayPositionParents()
        {
            GetPlayerPositionsParent.gameObject.TryToDisplay();
            GetEnemyPositionsParent.gameObject.TryToDisplay();
        }

        private void HidePositionParents()
        {
            GetPlayerPositionsParent.gameObject.TryToHide();
            GetEnemyPositionsParent.gameObject.TryToHide();
        }

        private void HideEachCombatPosition( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;

            foreach ( Transform trs in parent )
            {
                trs.gameObject.TryToHide();
            }
        }

        private void SetupPositionsOnEnteringCombat( CombatManager.CombatInfos combatInfos )
        {
            DisplayPositionParents();

            for ( int i = 0; i < combatInfos.PlayerUnitCount; i++ )
            {
                GetPlayerPositionsParent.GetChild( i ).gameObject.SetActive( true );
            }

            for ( int i = 0; i < combatInfos.EnemyUnitCount; i++ )
            {
                GetEnemyPositionsParent.GetChild( i ).gameObject.SetActive( true );
            }

            SpaceOutPositionsFromCenter( true );
            SpaceOutPositionsFromCenter( false );

            Debug.Log( combatInfos.PlayerUnitCount + " | " + combatInfos.EnemyUnitCount );
            Debug.Log( "Setup Positions On Entering Combat" );
        }

        public void OnNotification( object value )
        {
            SetupPositionsOnEnteringCombat( ( CombatManager.CombatInfos ) value );
        }

        private void SpaceOutPositionsFromCenter( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;
            Vector3 spacing = !_isThePositioningAsymmetric ?
                _spaceBetweenPlayerPositions :
                applyForPlayer ?
                _spaceBetweenPlayerPositions : _spaceBetweenEnemyPositions;

            //Debug.Log( "Child count : " + parent.GetExactChildCount(), parent );

            if ( parent.GetExactChildCount() == 0 ) { return; }

            List<Transform> children = new();

            for ( int i = 0; i < parent.GetExactChildCount(); i++ )
            {
                children.AppendItem( parent.GetChild( i ) );
            }

            switch ( parent.GetExactChildCount() )
            {
                case 1:
                    children [ 0 ].position = parent.localPosition;
                    break;

                case 2:
                    children [ 0 ].position = new Vector3(
                        parent.localPosition.x + spacing.x,
                        parent.localPosition.y,
                        parent.localPosition.z + spacing.z );

                    children [ 1 ].position = new Vector3(
                        parent.localPosition.x - spacing.x,
                        parent.localPosition.y,
                        parent.localPosition.z - spacing.z );
                    break;

                case 3:
                    children [ 0 ].position = new Vector3(
                        parent.localPosition.x + spacing.x,
                        parent.localPosition.y,
                        parent.localPosition.z + spacing.z );

                    children [ 1 ].position = parent.localPosition;

                    children [ 2 ].position = new Vector3(
                        parent.localPosition.x - spacing.x,
                        parent.localPosition.y,
                        parent.localPosition.z - spacing.z );
                    break;
            }
        }

        #region OnValidate

#if UNITY_EDITOR
        private void CreateCombatPosition( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;
            Color positionRendererColor = applyForPlayer ? Color.blue : Color.red;

            if ( parent.childCount >= _combatPositionLimit ) { return; }

            GameObject combatPos = Instantiate( 
                _combatPositionPrefab, 
                Vector3.zero, 
                _combatPositionPrefab.transform.rotation );

            combatPos.transform.GetChild( 1 ).GetComponent<SpriteRenderer>().color = positionRendererColor;
            combatPos.transform.SetParent( parent );
        }

        private void SpaceOutParentPositionFromCenter( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;
            parent.localPosition = applyForPlayer ?
                _spaceBetweenParentPositions : -_spaceBetweenParentPositions;
        }

        [Button]
        private void AddCombatPositionToPlayer()
        {
            CreateCombatPosition( true );
            SpaceOutPositionsFromCenter( applyForPlayer: true );
        }
        [Button]
        private void AddCombatPositionToEnemy()
        {
            CreateCombatPosition( false );
            SpaceOutPositionsFromCenter( applyForPlayer: false );
        }

        [Button]
        private void RotatePlayerPositionParent()
        {
            GetPlayerPositionsParent.localEulerAngles += new Vector3 ( 0, 45 % 360, 0 );
        }
        [Button]
        private void RotateEnemyPositionParent()
        {
            GetEnemyPositionsParent.localEulerAngles += new Vector3( 0, 45 % 360, 0 );
        }

        private void OnValidate()
        {
            GetLinkedComponents();

            SpaceOutParentPositionFromCenter( applyForPlayer: true );
            SpaceOutParentPositionFromCenter( applyForPlayer: false );

            SpaceOutPositionsFromCenter( applyForPlayer: true );
            SpaceOutPositionsFromCenter( applyForPlayer: false ); 
        }
        
#endif

        #endregion
    }
}