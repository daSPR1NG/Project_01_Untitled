using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Cursor/Settings/Create New Setting" )]
    public class CustomCursorSetting : ScriptableObject
    {
        [Header( "Main settings" )]
        public Enums.Cursor_SelectionType RelatedAction = Enums.Cursor_SelectionType.Default;
        [Range( 0.01f, 1f )] public float FrameRate;
        public Vector2 HotspotOffset = Vector2.zero;

        [Header( "Click Pressed settings" )]
        [SerializeField] private bool _hasAPressedSprite = true;
        [ShowIf( "_hasAPressedSprite" ), ShowAssetPreview] public Sprite PressedSprite;

        [Header( "Sequence settings" )]
        [ShowAssetPreview( 15, 15 )] public List<Sprite> SequenceSprites = new();

        public bool HasAPressedSprite => _hasAPressedSprite;
    }
}