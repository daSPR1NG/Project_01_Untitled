using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    ///<summary> CustomCursorManager description <summary>
    [DisallowMultipleComponent]
    public class CustomCursorManager : MonoBehaviour, IDebuggable
    {
        [SerializeField] private CursorLockMode _lockMode;
        [SerializeField] private List<CustomCursorSetting> _customCursorSettings = new();
        private CustomCursorSetting _currentSetting = null;

        private Enums.Cursor_SelectionType _currentRelatedAction;
        private bool _isLeftClickPressed = false;

        private float _currentFrameTimer;
        private int _currentFrame;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            EventManager.OnCursorHover += SetCursorFirstAppearance_ByAction;
        }

        void OnDisable() 
        {
            EventManager.OnCursorHover -= SetCursorFirstAppearance_ByAction;
        }

        #endregion

        #region Setup

        void Awake() => Init();

        // Set all datas that need it at the start of the game
        void Init()
        {
            Helper.SetCursorLockMode( _lockMode );
            Helper.SetCursorVisibility( true );

            SetCursorSettings( Enums.Cursor_SelectionType.Default );
            SetCursorFirstAppearance_ByAction( Enums.Cursor_SelectionType.Default );
        }

        #endregion

        void Update()
        {
            SetCursorAppearance_OnLeftClickState();
            ExecuteCursorSpriteSequence();
        }

        private void SetCursor( Texture2D texture, Vector2 offset, CursorMode mode )
        {
            if ( texture.IsNull<Texture2D>() )
            {
                texture.LogNullException();
                return;
            }
            Cursor.SetCursor( texture, offset, mode );
        }
        private void SetCursorSettings( Enums.Cursor_SelectionType relatedAction )
        {
            _currentSetting = GetCustomCursorSetting_ByType( relatedAction );
        }

        /// <summary>
        /// Used to set the cursor appearance once + set the current related action if different.
        /// </summary>
        /// <param name="relatedAction"></param>
        private void SetCursorFirstAppearance_ByAction( Enums.Cursor_SelectionType relatedAction )
        {
            if ( _currentSetting.SequenceSprites.IsEmpty() )
            {
                _currentSetting.SequenceSprites.LogIsEmpty();
                return;
            }

            // Reaffecting the cursor related action if different...
            if ( _currentRelatedAction != relatedAction ) { _currentRelatedAction = relatedAction; }

            // Fetch the setting you're looking for...
            SetCursorSettings( relatedAction );

            // We check if the left click is pressed to avoid to set a wrong texture...
            Texture2D overridenTexture = _isLeftClickPressed ? _currentSetting.PressedSprite.texture : _currentSetting.SequenceSprites [ 0 ].texture;

            // Set cursor appearence.
            SetCursor( overridenTexture, _currentSetting.HotspotOffset, CursorMode.Auto );
        }

        /// <summary>
        /// Execute the cursor sprite sequence if there is one.
        /// </summary>
        /// <param name="relatedAction"></param>
        private void ExecuteCursorSpriteSequence()
        {
            if ( _currentSetting.SequenceSprites.IsEmpty() )
            {
                _currentSetting.SequenceSprites.LogIsEmpty();
                return;
            }
            if ( _currentSetting.IsNull<CustomCursorSetting>() || _currentSetting.SequenceSprites.Count <= 1 )
            {
                this.Debugger( "No setting set or the current setting contains only one frame sprite" );
                return; 
            }

            if ( _isLeftClickPressed ) { return; }

            // Timer decremente
            _currentFrameTimer -= Helper.GetDeltaTime();

            // A zero qlq chose se passe et on reset le timer
            if ( _currentFrameTimer <= 0 )
            {
                _currentFrameTimer += _currentSetting.FrameRate;
                _currentFrame = ( _currentFrame + 1 ) % _currentSetting.SequenceSprites.Count;
                SetCursor( _currentSetting.SequenceSprites [ _currentFrame ].texture, _currentSetting.HotspotOffset, CursorMode.Auto );
            }
        }

        private void SetCursorAppearance_OnLeftClickState()
        {
            if ( Helper.IsLeftClickPressed() ) 
            {
                _isLeftClickPressed = true;
                SetCursor( _currentSetting.PressedSprite.texture, _currentSetting.HotspotOffset, CursorMode.Auto );
            }
            else if ( Helper.IsLeftClickUnpressed() ) 
            {
                _isLeftClickPressed = false;
                SetCursor( _currentSetting.SequenceSprites [ 0 ].texture, _currentSetting.HotspotOffset, CursorMode.Auto );
            }
        }

        private CustomCursorSetting GetCustomCursorSetting_ByType( Enums.Cursor_SelectionType relatedAction )
        {
            if ( _customCursorSettings.IsEmpty() ) 
            {
                _customCursorSettings.LogIsEmpty();
                return null; 
            }

            for ( int i = 0; i < _customCursorSettings.Count; i++ )
            {
                if ( _customCursorSettings [ i ].RelatedAction != relatedAction ) { continue; }

                return _customCursorSettings [ i ];
            }

            return null;
        }
    }
}