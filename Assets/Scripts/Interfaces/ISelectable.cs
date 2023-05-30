using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> ISelectable description <summary>
    public interface ISelectable
    {
        public Enums.Cursor_RelatedAction CursorRelatedAction { get; set; }

        public abstract void OnMouseEnter();
        public abstract void OnMouseOver();
        public abstract void OnMouseExit();
    }
}