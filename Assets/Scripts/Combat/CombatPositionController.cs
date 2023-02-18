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

        [SerializeField] private GameObject _combatPositionPrefab;

        [Header( "Parent spacing settings" )]

        [SerializeField] private bool _areTheParentsAligned = true;
        [SerializeField] private Vector3 _spaceBetweenParentPositions = Vector3.zero;
        [SerializeField] private Vector3 _positionOffset = Vector3.zero;

        [Header( "Positions spacing settings" )]

        [SerializeField] private bool _isThePositioningAsymmetric = true;
        [SerializeField] private Vector3 _spaceBetweenPlayerPositions = Vector3.zero;
        [SerializeField, ShowIf( "_isThePositioningAsymmetric" )]
        private Vector3 _spaceBetweenEnemyPositions = Vector3.zero;

        private Transform GetPlayerPositionsParent => transform.GetFirstChild();
        private Transform GetEnemyPositionsParent => transform.GetChild( 1 );

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

        private void SetupPositionsOnEnteringCombat( /*CombatManager.CombatInfos combatInfos*/ )
        {
            HideEachCombatPosition( true );
            HideEachCombatPosition( false );

            //for ( int i = 0; i < combatInfos._playerCombatantCount; i++ )
            //{
            //    GetPlayerPositionsParent.GetChild( i ).gameObject.SetActive( true );
            //}

            //for ( int i = 0; i < combatInfos._enemyCombatantCount; i++ )
            //{
            //    GetEnemyPositionsParent.GetChild( i ).gameObject.SetActive( true );
            //}

            //SpaceOutPositionsFromCenter( true );
            //SpaceOutPositionsFromCenter( false );

            //Debug.Log( combatInfos._playerCombatantCount + " | " + combatInfos._enemyCombatantCount );
            //Debug.Log( "Setup Positions On Entering Combat" );
        }

        private void HideEachCombatPosition( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;

            foreach ( Transform trs in parent )
            {
                trs.gameObject.TryToHide();
            }
        }

        public void OnNotification( object value )
        {
            //SetupPositionsOnEnteringCombat( ( CombatManager.CombatInfos ) value );
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

            Vector3 parentLocalPos = parent.localPosition + _positionOffset;

            Vector3 sidePosition1 = new(
                        parentLocalPos.x + spacing.x,
                        parentLocalPos.y,
                        parentLocalPos.z + spacing.z );

            Vector3 sidePosition2 = new(
                        parentLocalPos.x - spacing.x,
                        parentLocalPos.y,
                        parentLocalPos.z - spacing.z );

            switch ( parent.GetExactChildCount() )
            {
                case 1:
                    if ( children [ 0 ].position != parentLocalPos ) 
                    {
                        children [ 0 ].position = parentLocalPos;
                    }
                    break;

                case 2:
                    if ( children [ 0 ].position != sidePosition1 )
                    {
                        children [ 0 ].position = sidePosition1;
                    }

                    if ( children [ 1 ].position != sidePosition2 )
                    {
                        children [ 1 ].position = sidePosition2;
                    }
                    break;

                case 3:
                    if ( children [ 0 ].position != sidePosition1 )
                    {
                        children [ 0 ].position = sidePosition1;
                    }

                    if ( children [ 1 ].position != parentLocalPos )
                    {
                        children [ 1 ].position = parentLocalPos;
                    }

                    if ( children [ 2 ].position != sidePosition2 )
                    {
                        children [ 2 ].position = sidePosition2;
                    }
                    break;
            }
        }


        #region OnValidate

#if UNITY_EDITOR

        [Button]
        private void RefreshPositioning()
        {
            SpaceOutPositionsFromCenter( true );
            SpaceOutPositionsFromCenter( false );
        }

        private void CreateCombatPosition( bool applyForPlayer )
        {
            Transform parent = applyForPlayer ? GetPlayerPositionsParent : GetEnemyPositionsParent;
            Color positionRendererColor = applyForPlayer ? Color.blue : Color.red;

            if ( parent.childCount >= CombatManager.COMBAT_POSITION_LIMIT ) { return; }

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

            float xSpacingValue = applyForPlayer ? _spaceBetweenParentPositions.z : -_spaceBetweenParentPositions.z;
            float zSpacingValue =
                _areTheParentsAligned ? _spaceBetweenParentPositions.x :
                applyForPlayer ? _spaceBetweenParentPositions.x : -_spaceBetweenParentPositions.x;

            Vector3 spacing = new( xSpacingValue, 0, zSpacingValue );
            Vector3 newPosition = _positionOffset + spacing;

            if ( parent.localPosition == newPosition ) { return; }

            parent.localPosition = _positionOffset + spacing;
        }

        private void ApplyPositionOffset()
        {
            if ( transform.localPosition == _positionOffset ) { return; }

            transform.localPosition = _positionOffset;
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
            GetPlayerPositionsParent.localEulerAngles += new Vector3( 0, 45 % 360, 0 );
        }
        [Button]
        private void RotateEnemyPositionParent()
        {
            GetEnemyPositionsParent.localEulerAngles += new Vector3( 0, 45 % 360, 0 );
        }

        private void OnValidate()
        {
            SpaceOutParentPositionFromCenter( applyForPlayer: true );
            SpaceOutParentPositionFromCenter( applyForPlayer: false );

            SpaceOutPositionsFromCenter( applyForPlayer: true );
            SpaceOutPositionsFromCenter( applyForPlayer: false );
            ApplyPositionOffset();
        }

#endif

        #endregion
    }
}