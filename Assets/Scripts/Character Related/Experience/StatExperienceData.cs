using UnityEngine;

namespace dnSR_Coding
{
    [System.Serializable]
    public class StatExperienceData : ExperienceData
    {
        [SerializeField] private StatType _associatedStat = StatType.Unassigned;
        public StatType GetAssociatedStat() => _associatedStat;

        #region Editor

#if UNITY_EDITOR
        public void SetName()
        {
            string newName = _associatedStat.ToString();
            if ( !Name.Equals( newName ) ) { Name = newName; }
        }
#endif

        #endregion
    }
}