using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Utilities;
using System;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [Component("CombatManager", "")]
    [DisallowMultipleComponent]
    public class CombatManager : Singleton<CombatManager>, IDebuggable/*, ISubject*/
    {
        public const int COMBAT_POSITION_LIMIT = 3;

        //public List<CombatTurnAction> _combatTurnActions = new();
        [Header( "Combat settings" )]

        [SerializeField] private bool _isUsingATimer = false;

        [Header("Combat infos")]

        public int TurnCounter = 0;
        [SerializeField, ReadOnly] private Enums.TurnInfluence _turnInfluence;
        [SerializeField, ReadOnly] private CombatInfos _currentCombat;

        [Header( "Combat location dependencies" )]

        [SerializeField, Validation( "Need to reference this transform to fill the dependencies" ) ] 
        private Transform _locationsParent;
        private readonly List<Transform> _locations = new();

        private CombatantHolder _playerCombatantReference = null;

        private Transform CombatContent => transform.GetFirstChild();
        private bool HidesContentOnStart => !IsDebuggable;
        
        public static Action<object> OnEnteringCombat;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        [Serializable]
        public class CombatInfos
        {
            public List<CombatantData> Combatants = new();

            private int _playerCombatantCount = 0;
            private int _enemyCombatantCount = 0;

            public void AddCombatant( CombatantData combatant )
            {
                Combatants.AppendItem( combatant );

                if ( combatant.IsTeamAlly() )
                {
                    _playerCombatantCount++;
                    return;
                }

                _enemyCombatantCount++;
            }
            public void RemoveCombatant( CombatantData combatant )
            {
                Combatants.RemoveItem( combatant );

                if ( combatant.IsTeamAlly() )
                {
                    _playerCombatantCount--;
                    return;
                }

                _enemyCombatantCount--;
            }

            public void ClearCombat()
            {
                Combatants.Clear();
                _playerCombatantCount = 0;
                _enemyCombatantCount = 0;
            }
        }

        // Set all datas that need it at the start of the game
        protected override void Init( bool dontDestroyOnLoad )
        {
            base.Init();
            GetLinkedComponents();
        }
        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            GetCombatLocations();
            if ( Application.isPlaying && HidesContentOnStart ) { HideCombatElements(); }

            GetPlayerCombatantHolder();
        }

        private void GetPlayerCombatantHolder()
        {
            if ( !_playerCombatantReference.IsNull() ) { return; }
            
            _playerCombatantReference = Helper.GetPlayerTransformReference().GetComponent<CombatantHolder>();
        }

        private void Update()
        {
            if ( KeyCode.Alpha1.IsPressed() )
            {
                EnterCombat();
            }

            if ( KeyCode.Alpha2.IsPressed() )
            {
                ExitCombat();
            }
        }

        #region Combat Elements Handle

        private void GetCombatLocations()
        {
            if ( _locationsParent.IsNull() ) { return; }

            foreach ( Transform trs in _locationsParent )
            {
                _locations.AppendItem( trs, false );
            }

            HideEachCombatLocationsOnExitingCombat();

            //Debug.Log( "_currentCombat locations : " + _locations.Count );
        }

        private void DisplayRandomCombatLocationOnEnteringCombat()
        {
            int randomLocation = UnityEngine.Random.Range( 0, _locations.Count - 1 );
            _locations [ randomLocation ].gameObject.TryToDisplay();
        }
        private void HideEachCombatLocationsOnExitingCombat()
        {
            if ( _locations.IsEmpty() || !Application.isPlaying ) { return; }

            for ( int i = 0; i < _locations.Count; i++ )
            {
                _locations [ i ].gameObject.TryToHide();
            }
        }

        private void DisplayCombatElements()
        {
            CombatContent.gameObject.TryToDisplay();
        }
        private void HideCombatElements()
        {
            CombatContent.gameObject.TryToHide();
        }

        #endregion

        private void EnterCombat()
        {
            DisplayRandomCombatLocationOnEnteringCombat();
            DisplayCombatElements();

            SceneController.Instance.LoadSceneByType( GameSceneType.Combat );

            //CombatInfos combatInfos = new();

            //CombatantData ally = new();
            //ally.SetTeam( Enums.Team.Ally );
            //combatInfos.AddCombatant( ally );

            //CombatantData enemy = new();
            //enemy.SetTeam( Enums.Team.Enemy );
            //combatInfos.AddCombatant( enemy );

            //CombatantData enemy2 = new();
            //enemy2.SetTeam( Enums.Team.Enemy );
            //combatInfos.AddCombatant( enemy2 );

            //_currentCombat = combatInfos;

            //SetTurnOrderOnCombatStart( combatInfos );

            TurnCounter = 1;

            //OnModification( OnEnteringCombat, _currentCombat );
        }

        private void ExitCombat()
        {
            HideEachCombatLocationsOnExitingCombat();

            SceneController.Instance.LoadSceneByType( GameSceneType.World );

            // Clear combat infos
            _currentCombat.ClearCombat();
            _currentCombat = null;

            // ResetExp Turn influence
            _turnInfluence = Enums.TurnInfluence.Unassigned;
            TurnCounter = 0;

            HideCombatElements();
        }

        private void SetTurnOrderOnCombatStart( CombatInfos combatInfos )
        {
            //combatInfos.Combatants.Sort( ( i, j ) =>
            //j.GetStats().GetStatByType( Enums.StatType.Initiative_INI ).Value.
            //CompareTo( i.GetStats().GetStatByType( Enums.StatType.Initiative_INI ).Value ) );

            _turnInfluence =
                combatInfos.Combatants [ 0 ].IsTeamAlly() ? Enums.TurnInfluence.Player : Enums.TurnInfluence.IA;
            ApplyInfluence( _turnInfluence );

            // Play VFX and SFX on combat started

            //Debug.Log( combatInfos.Combatants [ 0 ].Get );
        }

        private void ApplyInfluence( Enums.TurnInfluence currentInfluence )
        {
            Debug.Log( "Apply Influence" );

            if ( _turnInfluence != currentInfluence ) { _turnInfluence = currentInfluence; }

            switch ( currentInfluence )
            {
                case Enums.TurnInfluence.Player:
                    // Display UI for the player
                    Debug.Log( "Display Player UI" );
                    break;

                case Enums.TurnInfluence.IA:
                    // Do something ?
                    Debug.Log( "Hide Player UI" );
                    break;
            }
        }

        private void StartNewTurn()
        {
            TurnCounter++;
            Debug.Log( "New turn : " + TurnCounter );
            // Update turn text counter
            // Update selector UI on who is currently playing
            // Play VFX and SFX on turn started ?_?
            ApplyTimerForThisTurn();
        }

        private void ApplyTimerForThisTurn()
        {
            if ( _isUsingATimer || GameManager.Instance.IsGamePaused() ) { return; }

            // ResetExp timer before applying
            // timer --;

            // When timer is under a certain threshold do something
            // if timer ends =>
            //EndTurn( _turnInfluence );
        }

        private void EndTurn( Enums.TurnInfluence currentInfluence )
        {
            Debug.Log( "End Turn" );

            currentInfluence =
                currentInfluence == Enums.TurnInfluence.Player ? Enums.TurnInfluence.IA : Enums.TurnInfluence.Player;

            ApplyInfluence( currentInfluence );
            Debug.Log( "Current Influence on turn ended : " + currentInfluence );

            StartNewTurn();

            // Play VFX and SFX on turn ended ??
        }

        public void OnModification( EventArgs e )
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

        // => Changement de scène avec transition animée comme dans Pokémon, le temps du chargement de cette scène
        // Contenu de cette scène :
        // - Décor (choisi au hasard selon le lieu ?, pré-défini et activable si sélectionné et les positions sont choisies dans celui-ci)
        // - Personnages (joueur et adversaire)

        // => Notification de ce changement de scène pour les autres behaviours

        // 2. Assigne un emplacement aléatoire pour le personnage du joueur et de l'adversaire, ainsi qu'un décor de combat
        // 3. Dispose l'ordre de jeu
        // 4. Observe le temps de jeu d'un tour
        // 5. Donne les droits à celui qui joue
        // 6. Passe les tours si : a. le temps restant est écoulé, b. si l'action choisie par un combattant est considérée comme terminée
        // 7. Gère les actions d'un tour et les sauvegarde sous forme de commande
        // 8. Change la scène à la fin du combat et clear la liste d'actions effectuées


        // - UI de combat :
        // - Ordre de jeu (Similaire à DOFUS ou The Ruined King)
        // - _statSheet personnages :
        // - Pour tout le monde : Nom, Niveau, PV (?)
        // - Pour le joueur : les commandes de jeu (actions sous forme de boutons)
        // - Temps restant (Affichage distinct et répété autour du cadre de l'ordre de jeu et d'une barre de temps (DOFUS))
    }
}