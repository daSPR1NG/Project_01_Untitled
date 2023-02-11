using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using System;

namespace dnSR_Coding
{
    [Component("CombatManager", "")]
    [DisallowMultipleComponent]
    public class CombatManager : Singleton<CombatManager>, IDebuggable, ISubject
    {
        public static Action<object> OnSceneChanged;

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
        protected override void Init( bool dontDestroyOnLoad )
        {
            base.Init( true );
            GetLinkedComponents();
        }
        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {

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

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

        #endregion

        // 1. Gère le passage en mode combat :

        private void EnterCombat()
        {
            SceneController.Instance.LoadSceneByType( GameSceneType.Combat );
            OnModification( OnSceneChanged, GameSceneType.Combat );
        }

        private void ExitCombat()
        {
            SceneController.Instance.LoadSceneByType( GameSceneType.World );
            OnModification( OnSceneChanged, GameSceneType.World );
        }

        public void OnModification( Action<object> actionToExecute, object dataToPush )
        {
           ISubjectExtensions.TriggerEvent( actionToExecute, dataToPush );
        }

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
        // - Stats personnages :
        // - Pour tout le monde : Nom, Niveau, PV (?)
        // - Pour le joueur : les commandes de jeu (actions sous forme de boutons)
        // - Temps restant (Affichage distinct et répété autour du cadre de l'ordre de jeu et d'une barre de temps (DOFUS))
    }
}