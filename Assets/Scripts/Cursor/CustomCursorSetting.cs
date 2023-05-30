using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Cursor/Settings/Create New Setting" )]
    public class CustomCursorSetting : ScriptableObject
    {
        [Header( "Main settings" )]
        public Enums.Cursor_RelatedAction RelatedAction = Enums.Cursor_RelatedAction.Default;
        [Range( 0.01f, 1f )] public float FrameRate;
        public Vector2 HotspotOffset = Vector2.zero;

        [Header( "Click Pressed settings" )]
        [SerializeField] private bool _hasAPressedSprite = true;
        [ShowIf( "_hasAPressedSprite" )] public Sprite PressedSprite;

        [Header( "Sequence settings" )]
        public List<Sprite> SequenceSprites = new();

        public bool HasAPressedSprite => _hasAPressedSprite;
    }
}