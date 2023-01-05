using UnityEngine;
using ExternalPropertyAttributes;
using TMPro;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    // TODO :
    // - Add text size by type following HTML5 principles.

    ///<summary> UIButtonTextSettings description <summary>
    [Component("UIButtonTextSettings", "")]
    [DisallowMultipleComponent]
    public class UIButtonTextSettings : MonoBehaviour, IDebuggable
    {
        [Header( "Button text settings" )]
        [SerializeField] private bool _hasText = false;
        [SerializeField, ShowIf( "_hasText" ), Multiline] 
        private string _buttonTextInput = "_type Here";
        [SerializeField, ShowIf( "_hasText" ), Range( 0.1f, 36 )]
        private float _buttonTextMaxSize = 24;
        [SerializeField, ShowIf( "_hasText" ), Range( -20, 20 )]
        private float _buttonTextMargin = 0;
        private Transform _content;
        private TextMeshProUGUI _buttonTextComponent;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();

            TryToSetButtonTextValue();
            TryToSetButtonTextMaxSize();
            TryToSetButtonTextPadding();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            _content = transform.GetFirstChild();

            TextMeshProUGUI possibleText = _content.GetComponentInChildren<TextMeshProUGUI>();
            if ( !possibleText.IsNull() ) { _buttonTextComponent = possibleText; }
        }

        #region Button text handle

        private void TryToSetButtonTextValue()
        {
            if ( !_hasText ) { return; }

            if ( _buttonTextComponent.IsNull() )
            {
                Debug.LogError( "No text mesh pro component found, can't set the text value.", transform );
                return;
            }

            if ( !_buttonTextComponent.text.Equals( _buttonTextInput ) )
            {
                _buttonTextComponent.text = _buttonTextInput;
            }
        }

        private void TryToSetButtonTextPadding()
        {
            if ( !_hasText ) { return; }

            if ( _buttonTextComponent.IsNull() )
            {
                Debug.LogError( "No text mesh pro component found, can't set the text value.", transform );
                return;
            }

            Vector4 margin = new( _buttonTextMargin, _buttonTextMargin, _buttonTextMargin, _buttonTextMargin );

            if ( _buttonTextComponent.margin != margin )
            {
                _buttonTextComponent.margin = margin;
            }
        }

        private void TryToSetButtonTextMaxSize()
        {
            if ( !_hasText ) { return; }

            if ( _buttonTextComponent.IsNull() )
            {
                Debug.LogError( "No text mesh pro component found, can't set the text value.", transform );
                return;
            }

            if ( _buttonTextComponent.fontSizeMax != _buttonTextMaxSize )
            {
                _buttonTextComponent.fontSizeMax = _buttonTextMaxSize;
            }
        }

        #endregion

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            GetLinkedComponents();

            TryToSetButtonTextValue();
            TryToSetButtonTextMaxSize();
            TryToSetButtonTextPadding();
        }
#endif

        #endregion
    }
}