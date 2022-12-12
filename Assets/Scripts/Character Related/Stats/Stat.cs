using UnityEngine;

namespace dnSR_Coding
{
    [System.Serializable]
    public class Stat
    {
        [HideInInspector] public string Name;

        [Header( "Details" )]
        public StatType Type;

        [Header( "Points information" )]
        public int Points = 1;        

        public void AddPoint( int amount ) => Points += amount;
        public void RemovePoint( int amount ) => Points -= amount;
        public void ResetPoints() => Points = 0;

        #region Editor

#if UNITY_EDITOR
        public void SetName()
        {
            string typeName = Type.ToString();
            if ( !Name.Equals( typeName ) ) { Name = typeName; }
        }
#endif
        #endregion
    }
}