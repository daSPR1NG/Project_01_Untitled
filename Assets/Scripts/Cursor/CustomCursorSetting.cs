using UnityEngine;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Cursor/Settings/Create New Setting" )]
    public class CustomCursorSetting : ScriptableObject
    {
        [Header( "Main settings" )]
        public Enums.Cursor_RelatedAction RelatedAction = Enums.Cursor_RelatedAction.Default;
        public float FrameRate;
        public Vector2 HotspotOffset = Vector2.zero;

        [Header( "Sprites" )]
        public List<Sprite> SequenceSprites = new();
    }
}