using UnityEngine;
using UnityEngine.EventSystems;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> DefaultUIButton description <summary>
    [DisallowMultipleComponent]
    [Component( "UI BUTTON", "Handle all behaviours used by an UI button." )]
    public abstract class DefaultUIButton : MonoBehaviour, IDebuggable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header( "DEPENDENCIES" )]
        [SerializeField] private bool _goesBackToDefaultOnClick = true;
        [Validation( "Need to reference the Selection child object transform." )]
        [SerializeField] private Transform _selectionTrs;
        [SerializeField] private Color _selectionColor = Color.white;

        [Space( 5f )]

        [Header( "OBSTRUCTOR SETTINGS" )]
        [SerializeField] private bool _togglesOnInteraction = true;
        [Validation( "Need to reference the Obstructor object transform." )]        
        [SerializeField] private Transform _obstructor;
        [SerializeField] private Color _obstructorDisplayedColor;
        [SerializeField] private Color _obstructorHiddenColor;

        [Space( 5f )]

        [Header( "TEXT SETTINGS" )]
        [SerializeField] private Color _textDefaultColor;
        [SerializeField] private Color _textHighlightColor = Color.white;
        private readonly List<TMP_Text> _texts = new();

        private bool _isSelected = false;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        protected virtual void Awake() => Init();
        protected virtual void Init()
        {
            SetSelectionLook();
            _isSelected = false;

            HideSelection();
            DisplayObstructor();

            GetAnyButtonTexts();
        }

        public abstract void OnClick();

        #region Pointer Events

        public void OnPointerEnter( PointerEventData eventData )
        {
            DisplaySelection();
            HighlightAnyTexts();
            HideObstructor();
        }

        public void OnPointerExit( PointerEventData eventData )
        {
            HideSelection();
            DarkenAnyTexts();
            DisplayObstructor();
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            _isSelected = !_isSelected;

            if ( _goesBackToDefaultOnClick )
            {
                HideSelection();
                DisplayObstructor();
            }

            OnClick();
        }

        #endregion

        #region Selection Display Options

        private void SetSelectionLook()
        {
            if ( _selectionTrs.IsNull() ) 
            {
                Debug.LogError( "Expected selection transform has not been found." );
                return; 
            }

            Image selectionImage = _selectionTrs.GetComponent<Image>();

            if ( selectionImage.color != _selectionColor ) { selectionImage.color = _selectionColor; }
        }

        private void DisplaySelection()
        {
            if ( _isSelected ) return;

            Helper.Log( this, "Display selection." );

            if ( _selectionTrs && !_selectionTrs.gameObject.IsActive() )
            {
                _selectionTrs.gameObject.TryToDisplay();
            }
        }

        private void HideSelection()
        {
            Helper.Log( this, "Hide selection." );

            if ( _selectionTrs && _selectionTrs.gameObject.IsActive() )
            {
                _selectionTrs.gameObject.TryToHide();
                _isSelected = false;
            }
        }

        #endregion

        #region Texts Handle

        private void GetAnyButtonTexts()
        {
            if ( transform.childCount == 0 ) return;

            for ( int i = 0; i < transform.childCount; i++ )
            {
                if ( transform.GetChild( i ).GetComponent<TMP_Text>() )
                {
                    _texts.Add( transform.GetChild( i ).GetComponent<TMP_Text>() );
                }

                if ( transform.GetChild( i ).GetComponentInChildren<TMP_Text>() )
                {
                    _texts.Add( transform.GetChild( i ).GetComponentInChildren<TMP_Text>() );
                }
            }

            DarkenAnyTexts();
        }

        private void DarkenAnyTexts()
        {
            if ( _texts.Count == 0 ) return;

            for ( int i = 0; i < _texts.Count; i++ )
            {
                _texts [ i ].color = _textDefaultColor;
            }
        }

        private void HighlightAnyTexts()
        {
            if ( _texts.Count == 0 ) return;

            for ( int i = 0; i < _texts.Count; i++ )
            {
                _texts [ i ].color = _textHighlightColor;
            }
        }

        #endregion

        #region Obstructor Display Options

        private void DisplayObstructor()
        {
            if ( _togglesOnInteraction && !_obstructor.gameObject.IsActive() ) 
            {
                _obstructor.gameObject.TryToDisplay();
                return;
            }

            ModifyObstructorColor( _obstructorDisplayedColor );
        }

        private void HideObstructor()
        {
            if ( _togglesOnInteraction && _obstructor.gameObject.IsActive() ) 
            {
                _obstructor.gameObject.TryToHide();
                return;
            }

            ModifyObstructorColor( _obstructorHiddenColor );
        }

        private void ModifyObstructorColor( Color color )
        {
            Image obstructorImage = _obstructor.GetComponent<Image>();
            if ( obstructorImage.color != color ) { obstructorImage.color = color; }
        }

        #endregion


        #region OnValidate

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetSelectionLook();
        }
#endif

        #endregion

    }
}
