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
        private Enums.Cursor_RelatedAction _currentRelatedAction;
        private float _currentFrameTimer;
        private int _currentFrame;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        // Ajouter event
        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        void Awake() => Init();

        // Set all datas that need it at the start of the game
        void Init()
        {
            SetCursorLockMode( _lockMode );
            SetCursorVisibility( true );
            SetCursorAppearance_ByAction( Enums.Cursor_RelatedAction.Default );
        }

        void Update()
        {
            if ( KeyCode.I.IsPressed() )
            {
                _currentRelatedAction = Enums.Cursor_RelatedAction.Interaction;
                SetCursorAppearance_ByAction( Enums.Cursor_RelatedAction.Interaction );
            }

            if ( KeyCode.D.IsPressed() )
            {
                _currentRelatedAction = Enums.Cursor_RelatedAction.Default;
                SetCursorAppearance_ByAction( Enums.Cursor_RelatedAction.Default );
            }
            ExecuteCursorSpriteSequence();
        }

        /// <summary>
        /// Used to set the cursor appearance once + set the current related action if different.
        /// </summary>
        /// <param name="relatedAction"></param>
        private void SetCursorAppearance_ByAction( Enums.Cursor_RelatedAction relatedAction )
        {
            // Reaffecting the cursor related action if different...
            if ( _currentRelatedAction != relatedAction ) { _currentRelatedAction = relatedAction; }

            // Fetch the setting you're looking for...
            _currentSetting = GetCustomCursorSetting_ByType( relatedAction );

            // Set cursor appearence.
            Cursor.SetCursor( _currentSetting.SequenceSprites [ 0 ].texture, _currentSetting.HotspotOffset, CursorMode.Auto );
        }

        /// <summary>
        /// Execute the cursor sprite sequence if there is one.
        /// </summary>
        /// <param name="relatedAction"></param>
        private void ExecuteCursorSpriteSequence()
        {
            if ( _currentSetting.IsNull() || _currentSetting.SequenceSprites.Count <= 1 ) 
            {
                this.Debugger( "No setting set or the current setting contains only one frame sprite" );
                return; 
            }

            // Timer decremente
            _currentFrameTimer -= Helper.GetDeltaTime();

            // A zero qlq chose se passe et on reset le timer
            if ( _currentFrameTimer <= 0 )
            {
                _currentFrameTimer += _currentSetting.FrameRate;
                _currentFrame = ( _currentFrame + 1 ) % _currentSetting.SequenceSprites.Count;
                Cursor.SetCursor( _currentSetting.SequenceSprites [ _currentFrame ].texture, _currentSetting.HotspotOffset, CursorMode.Auto );
            }
        }

        public void SetCursorLockMode( CursorLockMode lockMode )
        {
            Cursor.lockState = lockMode;
        }
        public void SetCursorVisibility( bool isVisible )
        {
            Cursor.visible = isVisible;
        }

        private CustomCursorSetting GetCustomCursorSetting_ByType( Enums.Cursor_RelatedAction relatedAction )
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