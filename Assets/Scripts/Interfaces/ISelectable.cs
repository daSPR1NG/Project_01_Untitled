using UnityEngine;
using dnSR_Coding.Utilities.Helpers;

namespace dnSR_Coding
{
    ///<summary> ISelectable description <summary>
    public interface ISelectable
    {
        public Enums.Cursor_SelectionType CursorSelectionType { get; set; }

        public abstract void OnMouseEnter();
        public abstract void OnMouseOver();
        public abstract void OnMouseExit();
    }
}