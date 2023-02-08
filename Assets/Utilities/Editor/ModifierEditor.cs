using UnityEngine;
using UnityEditor;

namespace dnSR_Coding
{
    ///<summary> ModifierEditor description <summary>
    [CustomEditor(typeof(Modifier))]
    public class ModifierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}