using UnityEngine;
using System.Collections.Generic;
using dnSR_Coding.Utilities.Interfaces;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    public abstract class WeatherSystemModule<T> : ScriptableObject, IDebuggable
    {
        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        [SerializeField] protected List<T> Settings = new();
    }
}