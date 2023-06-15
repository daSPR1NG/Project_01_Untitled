using UnityEngine;
using dnSR_Coding.Utilities;
using dnSR_Coding.Interfaces.Gameplay;
using NaughtyAttributes;

namespace dnSR_Coding
{
    // Notes :
    // -> Deux grands types d'interactions :
    //          - En one shot :
    //              -> Appui input -> interaction finie avec résultat.
    //          - En un temps t défini :
    //              -> Appui input déclenche le début de l'interaction, après un délai d, l'interaction est finie avec résultat.
    //              -> Lorsque l'interaction est interrompue le delai est sauvegardé pour ne pas avoir à refaire l'interaction du début
    //              (sauf si volonté marquée de le faire ainsi).

    ///<summary> InteractiveObject is an abstract class used by objects representing item that the player can interact with. <summary>
    [DisallowMultipleComponent]
    public abstract class InteractiveObject : MonoBehaviour, IInteractable, ISelectable, IOutlineable, IDebuggable
    {
        [field: Header( "Interaction Settings" )]

        [field: SerializeField] public bool IsInteractive { get; set; }
        [field: SerializeField] public Enums.Cursor_RelatedAction CursorRelatedAction { get; set; }
        public object Interactor { get; set; }

        public bool IsOutlined { get => !_outlineComponent.IsNull() && _outlineComponent.enabled; }
        [field: SerializeField, Range( 0, 10 )] public float OutlineWidth { get; set; }

        private Outline _outlineComponent;
        public Outline OutlineComponent
        {
            get
            {
                if ( _outlineComponent.IsNull() && gameObject.TryGetComponent( out Outline component ) ) {
                    _outlineComponent = component;
                }
                else if ( _outlineComponent.IsNull() && gameObject.GetComponent<Outline>().IsNull() ) {
                    _outlineComponent = gameObject.AddComponent<Outline>();
                }

                return _outlineComponent;
            }
        }

        #region DEBUG

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        public virtual void BeginInteraction( object interactor )
        {
            if ( !IsInteractive ) { 
                // Feedback is not interactive ?
                return; 
            }

            Interactor = interactor;
        }
        public virtual void EndInteraction()
        {
            Interactor = null;
        }

        #region Outline group

        public void DisplayOutline( float width = 1 )
        {
            if ( !IsInteractive || IsOutlined ) { return; }

            OutlineComponent.OutlineWidth = width;
            OutlineComponent.enabled = true;

            this.Debugger( "Outline !" );
        }
        public void HideOutline()
        {
            if ( !IsOutlined ) { return; }

            OutlineComponent.enabled = false;
            this.Debugger( "Remove Outline !" );
        }

        #endregion

        #region Mouse events group

        public void OnMouseEnter()
        {
            this.Debugger( "On Mouse Enter" );
            DisplayOutline( OutlineWidth );

            // Ajouter le changement de curseur => spécifique
            EventManager.OnCursorHover( CursorRelatedAction );
        }

        public void OnMouseOver()
        {
            this.Debugger( "On Mouse Over" );
            if ( !IsInteractive ) { HideOutline(); }
        }


        public void OnMouseExit()
        {
            this.Debugger( "On Mouse Exit" );
            HideOutline();

            // Ajouter le changement de curseur => default
            EventManager.OnCursorHover( Enums.Cursor_RelatedAction.Default );
        }

        #endregion

    }
}