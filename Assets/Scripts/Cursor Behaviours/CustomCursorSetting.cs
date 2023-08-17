using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Cursor/Settings/Create New Setting" )]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class CustomCursorSetting : ScriptableObject
    {
        [Header( "Main settings" )]
        public Enums.Cursor_SelectionType RelatedAction = Enums.Cursor_SelectionType.Default;

        [Range( 0.01f, 1f )] public float FrameRate;

        public Vector2 HotspotOffset = Vector2.zero;

        [Header( "Click Pressed settings" )]
        [SerializeField] private bool _hasAPressedSprite = true;

        [ShowIf( "_hasAPressedSprite" ), PreviewField] 
        public Sprite PressedSprite;

        [Header( "Sequence settings" )]
        [ListDrawerSettings( DraggableItems = false, ShowIndexLabels = true, ShowItemCount = true )]
        [PreviewField] public List<Sprite> SequenceSprites = new();

        public bool HasAPressedSprite => _hasAPressedSprite;
    }
}