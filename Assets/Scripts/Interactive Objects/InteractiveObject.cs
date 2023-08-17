using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    // Notes :
    // -> Deux grands types d'interactions :
    //          - En one shot :
    //              -> Appui input -> interaction finie avec r�sultat.
    //          - En un temps t d�fini :
    //              -> Appui input d�clenche le d�but de l'interaction, apr�s un d�lai d, l'interaction est finie avec r�sultat.
    //              -> Lorsque l'interaction est interrompue le delai est sauvegard� pour ne pas avoir � refaire l'interaction du d�but
    //              (sauf si volont� marqu�e de le faire ainsi).

    ///<summary> InteractiveObject is an abstract class used by objects representing item that the player can interact with. <summary>
    [DisallowMultipleComponent]
    public abstract class InteractiveObject : MonoBehaviour, IInteractable, ISelectable, IOutlineable, IDebuggable
    {
        #region Interaction variables

        internal string _groupName = "Interaction";

        [field: SerializeField, ReadOnly, FoldoutGroup( "$_groupName" )]
        public bool IsInteractive { get; set; } = false;
        public object Interactor { get; set; } = null;

        #endregion

        #region Selection variables

        [field: SerializeField, FoldoutGroup( "$_groupName" )]
        public Enums.Cursor_SelectionType CursorSelectionType { get; set; } = Enums.Cursor_SelectionType.Default;

        [field: SerializeField, FoldoutGroup( "$_groupName" ), InfoBox(
            "By default, the selection is only possible when the object is interactive. This overrides the default behaviour.")]
        public bool IsItAlwaysSelectable { get; set; } = false;

        [field: SerializeField, Range( 0, 3 ), FoldoutGroup( "$_groupName" )] 
        public float OutlineWidth { get; set; } = .5f;
        public bool IsOutlined { get => !_outlineComponent.IsNull<Outline>() && _outlineComponent.enabled; }

        private Outline _outlineComponent;
        public Outline OutlineComponent
        {
            get
            {
                if ( _outlineComponent.IsNull<Outline>() && gameObject.TryGetComponent( out Outline component ) ) {
                    _outlineComponent = component;
                }
                else if ( _outlineComponent.IsNull<Outline>() && gameObject.GetComponent<Outline>().IsNull<Outline>() ) {
                    _outlineComponent = gameObject.AddComponent<Outline>();
                    _outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
                }

                return _outlineComponent;
            }
        }

        #endregion

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )]
        public bool IsDebuggable { get; set; } = true;

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

        #region Outline handling

        public void DisplayOutline( float width = 1 )
        {
            if ( !IsInteractive && !IsItAlwaysSelectable || IsOutlined ) { return; }

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

        #region Mouse events handling

        public void OnMouseEnter()
        {
            this.Debugger( "On Mouse Enter" );
            DisplayOutline( OutlineWidth );

            // Ajouter le changement de curseur => sp�cifique
            EventManager.OnCursorHover( CursorSelectionType );
        }

        public void OnMouseOver()
        {
            this.Debugger( "On Mouse Over" );
            if ( !IsInteractive && !IsItAlwaysSelectable ) { HideOutline(); }
        }


        public void OnMouseExit()
        {
            this.Debugger( "On Mouse Exit" );
            HideOutline();

            // Ajouter le changement de curseur => default
            EventManager.OnCursorHover( Enums.Cursor_SelectionType.Default );
        }

        

        #endregion

    }
}