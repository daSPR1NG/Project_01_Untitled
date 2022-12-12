using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using System.Collections;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    // TODO :
    // - Refactoring, adding sums and detailling everything's purpose

    ///<summary> DefaultUIWindow description <summary>
    [DisallowMultipleComponent]
    [Component( "UI MENU", "Handles behaviours for a menu." )]
    public abstract class DefaultUIWindow : MonoBehaviour, IDebuggable, IValidatable
    {
        [Header( "INPUT" )]

        [SerializeField] private KeyCode _relatedKeyCode = KeyCode.None;

        [Space( 5 )]

        [Header( "VISIBILITY SETTINGS" )]

        [SerializeField] private bool _showOnStart = true;
        [SerializeField] private bool _hidesOnPressingEscape = true;
        [SerializeField] private bool _canBeDisplayedWhenGameIsPaused = false;

        private bool _isDisplayed = false;

        private Coroutine _toggleDisplayCoroutine = null;

        [Space( 5 )]

        [Header( "CONTEXTUAL DISPLAY SPEED SETTINGS" )]

        [SerializeField] private bool _isDynamic = true;
        [Range( 0.5f, 100), SerializeField] private float _displayUpdateSpeed = 10f;
        [Range( 0.5f, 100 ), SerializeField] private float _hideUpdateSpeed = 10f;

        private Transform _window;
        private CanvasGroup _canvasGroup;

        public static Action<bool> OnDisplay;
        public static Action<bool> OnHide;

        public bool IsValid => !_window.IsNull() && _relatedKeyCode != KeyCode.None;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        protected virtual void OnEnable()
        {
            HideWindow();
        }
        protected virtual void OnDisable()
        {
            HideWindow();
        }

        #endregion

        #region Initialization

        protected virtual void Awake() => Init();
        protected virtual void Init()
        {
            SetWindowTransformInEditor();

            HideWindow();
        }

        #endregion

        protected virtual void Update()
        {
            if ( _hidesOnPressingEscape && KeyCode.Escape.IsPressed() )
            {
                HideWindow(); 
            }

            //ContextualToggleDisplay();
            //DynamicToggleDisplay();
        }

        #region Displayer Options

        public void ContextualToggleDisplay()
        {
            if ( !_canBeDisplayedWhenGameIsPaused
                && GameManager.Instance.IsGamePaused() )
            { 
                return; 
            }

            if ( _isDynamic )
            {
                DynamicToggleDisplay();
                return;
            }

            DirectToggleDisplay();
        }

        public virtual void DirectToggleDisplay()
        {
            if ( _isDisplayed )
            {
                HideWindow();
                return;
            }

            DisplayWindow();
        }

        protected virtual void DynamicToggleDisplay()
        {
            if ( !_isDynamic ) { return; }

            Helper.Log( this, "DynamicToggleDisplay is processing." );

            if ( _toggleDisplayCoroutine.IsNull() ) 
            {
                _toggleDisplayCoroutine = StartCoroutine( DynamicToggleDisplayCoroutine( IsDisplayed() ) );
                return;
            }

            StopCoroutine( _toggleDisplayCoroutine );
            _toggleDisplayCoroutine = StartCoroutine( DynamicToggleDisplayCoroutine( IsDisplayed() ) );
        }

        protected virtual IEnumerator DynamicToggleDisplayCoroutine( bool display )
        {
            bool alphaValueHasBeenReached = display ?
                _canvasGroup.alpha <= 0 : _canvasGroup.alpha >= 1;

            do
            {
                switch ( display )
                {
                    case true:

                        _canvasGroup.alpha -= ( Time.unscaledDeltaTime * _hideUpdateSpeed );

                        if ( _canvasGroup.alpha <= 0 )
                        {
                            _canvasGroup.alpha = 0;
                            HideWindow();
                        }

                        break;
                    case false:

                        _window.gameObject.TryToDisplay();
                        _canvasGroup.alpha += ( Time.unscaledDeltaTime * _displayUpdateSpeed );

                        if ( _canvasGroup.alpha >= 1 )
                        {
                            _canvasGroup.alpha = 1;
                            DisplayWindow();
                        }

                        break;
                }

                yield return null;

            } while ( !alphaValueHasBeenReached );
        }

        protected virtual void DisplayWindow()
        {
            _window.gameObject.TryToDisplay();
            SetCanvasGroupVariables();

            _isDisplayed = true;

            OnDisplay?.Invoke( _hidesOnPressingEscape );
        }

        protected virtual void HideWindow()
        {
            _window.gameObject.TryToHide();
            SetCanvasGroupVariables( true );

            _isDisplayed = false;

            OnHide?.Invoke( _hidesOnPressingEscape );
        }

        #endregion

        #region Canvas Group Handle

        private void SetCanvasGroupVariables( bool visible = false )
        {
            _canvasGroup.alpha = visible ? 0f : 1f;
            _canvasGroup.blocksRaycasts = !visible;
            _canvasGroup.interactable = !visible;
        }

        #endregion

        //protected virtual void SendNotificationToUpdate()
        //{
        //    if ( !_needsToBeUpdated ) { _needsToBeUpdated = true; }
        //}

        public KeyCode RelatedKeyCode() => _relatedKeyCode;
        public bool IsDisplayed() => _isDisplayed;

        #region OnValidate

#if UNITY_EDITOR
        private void OnValidate()
        {
            CreateOrFindWindowInEditor();

            if ( _showOnStart ) { DisplayWindow(); }
            else { HideWindow(); }

        }

        private void CreateOrFindWindowInEditor()
        {
            if ( _window.IsNull() && transform.HasNoChild() )
            {
                GameObject windowGo = new();

                windowGo.transform.parent = transform;
                windowGo.name = "Window";

                windowGo.AddComponent<RectTransform>();
                windowGo.AddComponent<CanvasGroup>();

                SetWindowTransformInEditor();

                return;
            }

            if ( _window.IsNull()
                || !_window.IsNull() && _window != transform.GetChild( 0 ) )
            {
                //Debug.LogError( "Window object was not correct." 
                //    + '\n'
                //    + "<b>Trying to get Child( 0 ) as Window object.</b>");
                _window = null;

                SetWindowTransformInEditor();
            }
        }

        private void SetWindowTransformInEditor()
        {
            if ( transform.HasNoChild() ) return;

            if ( _window.IsNull()
                && transform.GetChild( 0 ).TryGetComponent( out CanvasGroup cG ) )
            {
                _window = transform.GetChild( 0 );
                _canvasGroup = cG;
                return;
            }
        }
#endif

        #endregion
    }
}