using UnityEngine;

namespace dnSR_Coding
{
    [System.Serializable]
    public class Stat
    {
        [HideInInspector] public string Name;

        [Header( "Details" )]
        [SerializeField] private StatType _type;

        [Header( "Points" )]
        [SerializeField] private int _points = 1;

        public void AddPoint( int amount ) => _points += amount;
        public void RemovePoint( int amount ) => _points -= amount;
        public void ResetPoints() => _points = 0;

        public StatType GetStatType() => _type;
        public int GetPoints() => _points;

        public Stat() : base() {}
        public Stat( StatType type, int points ) : base() 
        {
            Name = type.ToString();
            _type = type;
            this._points = points;
        }

        #region Editor

#if UNITY_EDITOR
        public void SetName()
        {
            string name = _type.ToString() + " - " + _points.ToString();
            if ( !Name.Equals( name ) ) { Name = name; }
        }
#endif
        #endregion
    }
}