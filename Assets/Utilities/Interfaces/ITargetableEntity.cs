using UnityEngine;

namespace dnSR_Coding
{
    public interface ITargetableEntity
    {
        public bool IsTargetable { get; }
        public Transform SelectionFeedback { get; }

        public abstract void Select();
        public abstract void Deselect();
    }
}