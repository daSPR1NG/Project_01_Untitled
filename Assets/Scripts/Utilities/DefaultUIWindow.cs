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

        [Header( "VISIBILITY SETTINGS" )]
        [SerializeField] private bool _isShownOnStart = true;
        [SerializeField] private bool _selfHidesOnPressingEscape = true;
        [SerializeField] private bool _ignoresTimeScale = false;
        private bool _isDisplayed = false;
        private Coroutine _toggleDisplayCoroutine = null;

        [Header( "CONTEXTUAL DISPLAY SPEED SETTINGS" )]
        [SerializeField] private bool _isDisplayDynamic = true;
        [Range( 0.5f, 100), SerializeField] private float _displayingSpeed = 10f;
        [Range( 0.5f, 100 ), SerializeField] private float _hidingSpeed = 10f;

        private Transform _window;
        private CanvasGroup _canvasGroup;

        public static Action<bool> OnWindowDisplayed;
        public static Action<bool> OnWindowHidden;

        public bool IsValid => !_window.IsNull() && _relatedKeyCode != KeyCode.None;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = false;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        protected virtual void Awake() => Init();
        protected virtual void Init()
        {
            HideWindow();
        }

        protected virtual void Update()
        {
            if ( _selfHidesOnPressingEscape 
                && KeyCode.Escape.IsPressed() )
            {
                HideWindow(); 
            }
        }

        #region Displaying Options

        /// <summary>
        /// Toggles the visible of this window, handling if it's dynamic or direct.
        /// </summary>
        public void ContextualToggleDisplay()
        {
            if ( !_ignoresTimeScale
                && GameManager.Instance.IsGamePaused() )
            { 
                return; 
            }

            if ( _isDisplayDynamic )
            {
                DynamicToggleDisplay();
                return;
            }

            DirectToggleDisplay();
        }

        #region Different types of toggle - Direct / Dynamic

        /// <summary>
        /// Directly toggles the visible of this window, it is not dynamic.
        /// </summary>
        public virtual void DirectToggleDisplay()
        {
            if ( _isDisplayed )
            {
                HideWindow();
                return;
            }

            DisplayWindow();
        }

        /// <summary>
        /// Dynamicly toggles the visible of this window, it is not direct.
        /// </summary>
        protected virtual void DynamicToggleDisplay()
        {
            if ( !_isDisplayDynamic ) { return; }

            Helper.Log( this, "DynamicToggleDisplay is processing." );

            if ( _toggleDisplayCoroutine.IsNull() )
            {
                _toggleDisplayCoroutine = StartCoroutine( DynamicToggleDisplayCoroutine( IsDisplayed() ) );
                return;
            }

            StopCoroutine( _toggleDisplayCoroutine );
            _toggleDisplayCoroutine = StartCoroutine( DynamicToggleDisplayCoroutine( IsDisplayed() ) );
        }

        #endregion

        /// <summary>
        /// Coroutine used to dynamicly toggle the display of a window. It ignores Timescale by default.
        /// </summary>
        /// <param name="visible"> Returns weither if this window is actually displayed or not. </param>
        /// <returns></returns>
        protected virtual IEnumerator DynamicToggleDisplayCoroutine( bool visible )
        {
            bool alphaValueHasBeenReached = visible ?
                _canvasGroup.alpha <= 0 : _canvasGroup.alpha >= 1;

            do
            {
                switch ( visible )
                {
                    case true:

                        _canvasGroup.alpha -= ( Time.unscaledDeltaTime * _hidingSpeed );

                        if ( _canvasGroup.alpha <= 0 )
                        {
                            _canvasGroup.alpha = 0;
                            HideWindow();
                        }

                        break;
                    case false:

                        _window.gameObject.TryToDisplay();
                        _canvasGroup.alpha += ( Time.unscaledDeltaTime * _displayingSpeed );

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

        /// <summary>
        /// Displays this window by handling the canvas settings and by throwing an event.
        /// </summary>
        protected virtual void DisplayWindow()
        {
            _window.gameObject.TryToDisplay();
            SetCanvasGroupSettings();

            _isDisplayed = true;

            OnWindowDisplayed?.Invoke( _selfHidesOnPressingEscape );
        }

        /// <summary>
        /// Hides this window by handling the canvas settings and by throwing an event.
        /// </summary>
        protected virtual void HideWindow()
        {
            _window.gameObject.TryToHide();
            SetCanvasGroupSettings( true );

            _isDisplayed = false;

            OnWindowHidden?.Invoke( _selfHidesOnPressingEscape );
        }

        #endregion

        /// <summary>
        /// Set the canvas group attached settings weither it is hiding or displaying.
        /// </summary>
        /// <param name="visible"> Returns weither if this window is actually displayed or not. </param>
        private void SetCanvasGroupSettings( bool visible = false )
        {
            _canvasGroup.alpha = visible ? 0f : 1f;
            _canvasGroup.blocksRaycasts = !visible;
            _canvasGroup.interactable = !visible;
        }

        public KeyCode RelatedKeyCode() => _relatedKeyCode;
        public bool IsDisplayed() => _isDisplayed;

        #region OnValidate

#if UNITY_EDITOR

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
            if ( transform.HasNoChild() )
            {
                Debug.LogError( "The window element cannot be found, this means that something is wrong.", transform );
                return; 
            }

            if ( _window.IsNull()
                && transform.GetChild( 0 ).TryGetComponent( out CanvasGroup cG ) )
            {
                _window = transform.GetChild( 0 );
                _canvasGroup = cG;
                return;
            }
        }

        private void OnValidate()
        {
            CreateOrFindWindowInEditor();

            if ( _isShownOnStart ) { DisplayWindow(); }
            else { HideWindow(); }

        }
#endif

        #endregion
    }
}