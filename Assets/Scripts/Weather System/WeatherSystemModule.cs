using UnityEngine;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public abstract class WeatherSystemModule<T> : ScriptableObject
    {
        [SerializeField] protected List<T> Settings = new();
    }
}