using UnityEngine;
using UnityEngine.EventSystems;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using UnityEngine.UI;

namespace dnSR_Coding
{
    // TODO :
    // - Add obstructor and selection instance by default in editor.

    [RequireComponent( typeof( Image ) )]
    [RequireComponent( typeof( UIButtonTextSettings ) )]

    ///<summary> DefaultUIButton description <summary>
    [DisallowMultipleComponent]
    public abstract class DefaultUIButton : 
        MonoBehaviour, 
        IDebuggable,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Let this at top.
        #region Debug

        [Space( 10 ), /*HorizontalLine( .5f, EColor.Gray )*/]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        [SerializeField] private bool _isInteractive = true;
        [SerializeField] private bool _goesBackToDefaultOnClick = true;

        [Header( "Selection settings" )]
        [SerializeField] private Transform _selectionTrs;
        [SerializeField] private Color _selectionColor = Color.white;
        private bool _isSelected = false;

        [Header( "Obstructor settings" )]
        [SerializeField] private bool _togglesOnInteraction = true;
        [SerializeField] private Transform _obstructor;
        [SerializeField] private Color _obstructorDisplayedColor;        

        [Header( "Clickable area settings" )]
        //[InfoBox( "The clickable area is defined by the component of _type Image attached to this transform" )]
        [SerializeField, Range( -20, 0 )] private float _paddingOffset = 0;

        private Image _requiredImageComponent;

        protected virtual void Awake() => Init();
        protected virtual void Init()
        {
            GetLinkedComponents();

            SetClickableArea();

            ApplySelectedLook();
            _isSelected = false;

            HideSelection();
            DisplayObstructor();
        }
        private void GetLinkedComponents()
        {
            if ( _requiredImageComponent.IsNull<Image>()
                && TryGetComponent( out Image imageComponent )) {
                _requiredImageComponent = imageComponent; 
            }
        }

        public abstract void OnClick();

        #region Pointer Events

        public virtual void OnPointerEnter( PointerEventData eventData )
        {
            if ( !_isInteractive ) { return; }

            DisplaySelection();
            HideObstructor();
        }

        public virtual void OnPointerExit( PointerEventData eventData )
        {
            if ( !_isInteractive ) { return; }

            HideSelection();
            DisplayObstructor();
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            if ( !_isInteractive ) { return; }

            _isSelected = !_isSelected;

            if ( _goesBackToDefaultOnClick ) { HideSelection(); }

            OnClick();
        }

        #endregion

        #region Selection Display Options

        private void ApplySelectedLook()
        {
            if ( !_isInteractive 
                || _selectionTrs.IsNull<Transform>() )
            {
                Debug.LogError( "Expected selection transform has not been found." );
                return;
            }

            if ( _selectionTrs.TryGetComponent( out Image imageComponent ) ) {
                imageComponent.SetColor( _selectionColor );
            }
        }

        private void DisplaySelection()
        {
            if ( !_isInteractive || _isSelected ) return;

            this.Debugger( "Display selection." );            

            if ( !_selectionTrs.IsNull<Transform>() ) {
                _selectionTrs.gameObject.Display(); 
            }
        }

        private void HideSelection()
        {
            this.Debugger( "Hide selection." );

            if ( !_selectionTrs.IsNull<Transform>() )
            {
                _selectionTrs.gameObject.Hide();
                _isSelected = false;
            }
        }

        #endregion

        #region Obstructor Display Options

        private void DisplayObstructor()
        {
            if ( _togglesOnInteraction 
                && !_obstructor.gameObject.IsActive() ) {
                _obstructor.gameObject.Display();
            }

            SetObstructorColor( _obstructorDisplayedColor );
        }

        private void HideObstructor()
        {
            if ( _togglesOnInteraction 
                && _obstructor.gameObject.IsActive() ) {
                _obstructor.gameObject.Hide();
            }

            Color hiddenColor = new ( 0, 0, 0, 0 );
            SetObstructorColor( hiddenColor );
        }

        private void SetObstructorColor( Color color )
        {
            Image obstructorImage = _obstructor.GetComponent<Image>();
            if ( obstructorImage.color != color ) { obstructorImage.color = color; }
        }

        #endregion

        #region Clickable area handle

        private void SetClickableArea()
        {
            if ( _requiredImageComponent.IsNull<Image>() )
            {
                Debug.LogError( "No image component attached found, this mono requires one !", transform );
                return;
            }

            _requiredImageComponent.color = _requiredImageComponent.color.WithAlpha( 0f );
        }

        private void ApplyPaddingOffsetToClickableArea()
        {
            if ( _requiredImageComponent.IsNull<Image>() ) 
            {
                Debug.LogError( "No image component attached found, this mono requires one !", transform );
                return; 
            }

            _requiredImageComponent.SetRaycastPaddingFloatInput( _paddingOffset );
        }        

        #endregion

        #region OnValidate

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            GetLinkedComponents();

            SetClickableArea();

            ApplySelectedLook();
            ApplyPaddingOffsetToClickableArea();            
        }        
#endif
        #endregion
    }
}
