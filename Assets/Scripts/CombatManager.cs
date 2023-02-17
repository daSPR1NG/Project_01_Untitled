using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Utilities;
using System;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [Component("CombatManager", "")]
    [DisallowMultipleComponent]
    public class CombatManager : Singleton<CombatManager>, IDebuggable/*,*/ /*ISubject*/
    {
        public const int COMBAT_POSITION_LIMIT = 3;

        //public List<CombatTurnAction> _combatTurnActions = new();
        [Header("CurrentCombat infos")]

        public int TurnCounter = 0;
        public Enums.TurnInfluence TurnInfluence;
        //public CombatInfos CurrentCombat;

        [Header( "Combat location dependencies" )]

        [SerializeField, Validation( "Need to reference this transform to fill the dependencies" ) ] 
        private Transform _combatLocationsParent;
        private readonly List<Transform> _combatLocations = new();

        private Transform CombatContent => transform.GetFirstChild();

#if UNITY_EDITOR
        private bool HidesContentOnStart => !IsDebuggable;
#endif

        public static Action<object> OnSceneChanged;
        public static Action<object> OnEnteringCombat;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

//        [Serializable]
//        public class CombatInfos
//        {
//            public List<Combatant> Combatants = new();

//            public int PlayerCombatantCount = 0;
//            public int EnemyCombatantCount = 0;

//            public void AddCombatant( Combatant combatant )
//            {
//                Combatants.AppendItem( combatant );

//                if ( combatant.GetTeam() == Enums.Team.Ally )
//                {
//                    PlayerCombatantCount++;
//                    return;
//                }

//                EnemyCombatantCount++;
//            }

//            public Combatant GetCombatantByTeam( Enums.Team team )
//            {
//                foreach ( Combatant combatant in Combatants )
//                {
//                    if ( combatant.GetTeam() == team )
//                    {
//                        return combatant;
//                    }
//                }

//                return null;
//            }

//            public void ClearCombat()
//            {
//                Combatants.Clear();
//                PlayerCombatantCount = 0;
//                EnemyCombatantCount = 0;
//            }
//        }        

//        #region Enable, Disable

//        void OnEnable() { }

//        void OnDisable() { }

//        #endregion
        
//        // Set all datas that need it at the start of the game
//        protected override void Init( bool dontDestroyOnLoad )
//        {
//            base.Init();
//            GetLinkedComponents();
//        }
//        // Put all the get component here, it'll be easier to follow what we need and what we collect.
//        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
//        void GetLinkedComponents()
//        {
//            GetCombatLocations();
//            if ( Application.isPlaying && HidesContentOnStart ) { HideCombatElements(); }
//        }

//        private void Update()
//        {
//            if ( KeyCode.Alpha1.IsPressed() )
//            {
//                EnterCombat();
//            }

//            if ( KeyCode.Alpha2.IsPressed() )
//            {
//                ExitCombat();
//            }        
//        }

//        #region Combat Elements Handle

//        private void GetCombatLocations()
//        {
//            if ( _combatLocationsParent.IsNull() ) { return; }

//            foreach ( Transform trs in _combatLocationsParent )
//            {
//                _combatLocations.AppendItem( trs, false );
//            }

//            HideEachCombatLocationsOnExitingCombat();

//            //Debug.Log( "CurrentCombat locations : " + _combatLocations.Count );
//        }

//        private void DisplayRandomCombatLocationOnEnteringCombat()
//        {
//            int randomLocation = UnityEngine.Random.Range( 0, _combatLocations.Count - 1 );
//            _combatLocations [ randomLocation ].gameObject.TryToDisplay();
//        }

//        private void HideEachCombatLocationsOnExitingCombat()
//        {
//            if ( _combatLocations.IsEmpty() || !Application.isPlaying ) { return; }

//            for ( int i = 0; i < _combatLocations.Count; i++ )
//            {
//                _combatLocations [ i ].gameObject.TryToHide();
//            }
//        }

//        private void DisplayCombatElements()
//        {
//            CombatContent.gameObject.TryToDisplay();
//        }

//        private void HideCombatElements()
//        {
//            CombatContent.gameObject.TryToHide();
//        }

//        #endregion

//        // 1. Gère le passage en mode combat :

//        private void EnterCombat()
//        {
//            DisplayRandomCombatLocationOnEnteringCombat();
//            DisplayCombatElements();

//            SceneController.Instance.LoadSceneByType( GameSceneType.Combat );
//            OnModification( OnSceneChanged, GameSceneType.Combat ); // => notifie l'UI qui s'affiche

//            CombatInfos combatInfos = new();           

//            Combatant ally = new();
//            ally.SetTeam( Enums.Team.Ally );
//            combatInfos.AddCombatant( ally );

//            Combatant enemy = new();
//            enemy.SetTeam( Enums.Team.Enemy );
//            combatInfos.AddCombatant( enemy );

//            Combatant enemy2 = new();
//            enemy2.SetTeam( Enums.Team.Enemy );
//            combatInfos.AddCombatant( enemy2 );

//            CurrentCombat = combatInfos;
            
//            SetTurnOrderOnCombatStart( combatInfos );

//            TurnCounter = 1;

//            OnModification( OnEnteringCombat, CurrentCombat );
//        }

//        private void ExitCombat()
//        {
//            HideEachCombatLocationsOnExitingCombat();

//            SceneController.Instance.LoadSceneByType( GameSceneType.World );
//            OnModification( OnSceneChanged, GameSceneType.World );

//            // Clear combat infos
//            CurrentCombat.ClearCombat();
//            CurrentCombat = null;

//            // Reset Turn influence
//            TurnInfluence = Enums.TurnInfluence.Unassigned;
//            TurnCounter = 0;
//            InfluencerTarget = null;

//            HideCombatElements();
//        }

//        private void SetTurnOrderOnCombatStart( CombatInfos combatInfos )
//        {
//            combatInfos.Combatants.Sort( ( i, j ) => 
//            j.GetStats().GetStatByType( Enums.StatType.Initiative_INI ).Value.
//            CompareTo( i.GetStats().GetStatByType( Enums.StatType.Initiative_INI ).Value ) );

//            TurnInfluence = 
//                combatInfos.Combatants [ 0 ].GetTeam() == Enums.Team.Ally ? Enums.TurnInfluence.Player : Enums.TurnInfluence.IA;
//            ApplyInfluence( TurnInfluence );

//            // Play VFX and SFX on combat started

//            Debug.Log( combatInfos.Combatants [ 0 ].Get );
//        }

//        private void ApplyInfluence( Enums.TurnInfluence currentInfluence )
//        {
//            Debug.Log( "Apply Influence" );

//            if ( TurnInfluence != currentInfluence ) { TurnInfluence = currentInfluence; }

//            switch ( currentInfluence )
//            {
//                case Enums.TurnInfluence.Player:
//                    // Display UI for the player
//                    Debug.Log( "Display Player UI" );
//                    break;

//                case Enums.TurnInfluence.IA:
//                    // Do something ?
//                    Debug.Log( "Hide Player UI" );
//                    break;
//            }        
//        }

//        private void StartNewTurn()
//        {
//            TurnCounter++;
//            Debug.Log( "New turn : " + TurnCounter );
//            // Update turn text counter
//            // Update selector UI on who is currently playing
//            // Play VFX and SFX on turn started ?_?
//            ApplyTimerForThisTurn();
//            InfluencerTarget = null;
//        }

//        private void ApplyTimerForThisTurn()
//        {
//            // Reset timer before applying
//            // timer --;

//            // When timer is under a certain threshold do something
//            // if timer ends =>
//            //EndTurn( TurnInfluence );
//        }

//        private void EndTurn( Enums.TurnInfluence currentInfluence )
//        {
//            Debug.Log( "End Turn" );

//            currentInfluence =
//                currentInfluence == Enums.TurnInfluence.Player ? Enums.TurnInfluence.IA : Enums.TurnInfluence.Player;

//            ApplyInfluence( currentInfluence );
//            Debug.Log( "Current Influence on turn ended : " + currentInfluence );

//            StartNewTurn();

//            // Play VFX and SFX on turn ended ??
//        }

//        [Button]
//        private void ApplyDamageToAlly()
//        {
//            InfluencerTarget = CurrentCombat.GetCombatantByTeam( Enums.Team.Ally );

//            if ( InfluencerTarget.IsNull() ) 
//            {
//                Debug.Log( "Target isn't selected" );
//                return; 
//            }

//            // Apply Damage to Ally
//            if ( TurnInfluence == Enums.TurnInfluence.IA )
//            {
//                CurrentCombat.GetCombatantByTeam( Enums.Team.Ally ).GetStats().GetStatByType( Enums.StatType.Health_HP ).Value -= 
//                    CurrentCombat.GetCombatantByTeam( Enums.Team.Enemy ).GetStats().GetStatByType( Enums.StatType.Damage_DMG ).Value;
//                Debug.Log( "Ally lost : " + CurrentCombat.GetCombatantByTeam( Enums.Team.Enemy ).Damage );

//                if ( CurrentCombat.GetCombatantByTeam( Enums.Team.Ally ).Health <= 0 )
//                {
//                    Debug.Log( "Defeat" );
//                    ExitCombat();
//                }

//                EndTurn( Enums.TurnInfluence.IA );
//            }
//        }

//        [Button]
//        private void ApplyDamageToEnemy()
//        {
//            InfluencerTarget = CurrentCombat.GetCombatantByTeam( Enums.Team.Enemy );

//            if ( InfluencerTarget.IsNull() )
//            {
//                Debug.Log( "Target isn't selected" );
//                return;
//            }

//            // Apply Damage to Enemy
//            if ( TurnInfluence == Enums.TurnInfluence.Player )
//            {
//                InfluencerTarget.Health -= CurrentCombat.GetCombatantByTeam( Enums.Team.Ally ).Damage;
//                Debug.Log( "Enemy lost : " + CurrentCombat.GetCombatantByTeam( Enums.Team.Ally ).Damage );

//                if ( InfluencerTarget.Health <= 0 )
//                {
//                    Debug.Log( "Victory" );
//                    ExitCombat();
//                }

//                EndTurn( Enums.TurnInfluence.Player );
//            }
//        }

//        [Button]
//        private void SwapEnemyTarget()
//        {
//            if ( TurnInfluence != Enums.TurnInfluence.Player ) { return; }

//            List<Combatant> targets = new();

//            foreach ( Combatant opponent in CurrentCombat.Combatants )
//            {
//                if ( opponent._team == Enums.Team.Enemy )
//                {
//                    targets.AppendItem( opponent );
//                }
//            }

//            Debug.Log( "Swapped target" );

//            if ( InfluencerTarget.IsNull() ) 
//            {
//                InfluencerTarget = targets [ 0 ];
//                Debug.Log( "New target : " + InfluencerTarget.Name );
//                return; 
//            }

//            InfluencerTarget = InfluencerTarget == targets [ 0 ] ? targets [ 1 ] : targets [ 0 ];
//            Debug.Log( "New target : " + InfluencerTarget.Name );
//        }

//        public void OnModification( Action<object> actionToExecute, object dataToPush )
//        {
//           ISubjectExtensions.TriggerAction( actionToExecute, dataToPush );
//        }
        
//        #region OnValidate

//#if UNITY_EDITOR

//        private void OnValidate()
//        {
//            GetLinkedComponents();
//        }
//#endif

//        #endregion

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