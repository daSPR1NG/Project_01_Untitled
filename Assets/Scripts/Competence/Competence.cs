using ExternalPropertyAttributes;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/_currentCombat/Unit/Competence")]
    public class Competence : ScriptableObject
    {
        [Header( "Details" )]
        [SerializeField] private string _name;
        [SerializeField, ResizableTextArea] private string _description;

        [Header( "Visuals Settings" )]
        [SerializeField, ShowAssetPreview] private Sprite _icon;
        [SerializeField] private CompetenceAnimationData _competenceAnimationData;

        [Header( "Audio Settings" )]
        [SerializeField] private CompetenceAudioData _competenceAudioData;

        [Header( "EffectOnHit Settings" )]
        [SerializeField] private EffectOnHit _effectOnHit;

        public class CompetenceAnimationData
        {
            public AnimationClip ExecutionAnimation;
            public AnimationClip OnHitAnimation;
            public float ExecutionAnimationSpeed = 1f;
            public float OnHitAnimationSpeed = 1f;
        }

        public class CompetenceAudioData
        {
            public SimpleAudioEvent AudioEvent;
            public float Delay = 0.0f;
        }

        public class EffectOnHit
        {
            public enum Type { Damage, Heal, Status }
            public enum Target { Self, Enemy, Ally }
        }
    }
}