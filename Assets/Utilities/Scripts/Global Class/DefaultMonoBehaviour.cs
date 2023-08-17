using UnityEngine;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    ///<summary> CustomMonoBehaviour description <summary>
    [DisallowMultipleComponent]
    public class CustomMonoBehaviour : MonoBehaviour, IDebuggable
    {
        #region DEBUG

        [SerializeField, BoxGroup]
        private readonly bool _isDebuggable = true;
        [BoxGroup]
        [field: SerializeField, FoldoutGroup("TEST")] public bool IsDebuggable { get; set; } = true;

        #endregion
    }
}