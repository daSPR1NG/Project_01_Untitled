using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace dnSR_Coding
{
    [System.Serializable]
    public class Stat
    {
        [Header( "Details" )]

        [HideInInspector] public string Name;
        public StatType Type;

        [Header( "Value" )]

        public int Points = 1;        

        public void AddPoint( int amount ) => Points += amount;
        public void RemovePoint( int amount ) => Points -= amount;
        public void ResetPoints() => Points = 0;

        public void SetName()
        {
            string typeName = Type.ToString();
            if ( !Name.Equals( typeName ) ) { Name = typeName; }
        }
    }
}