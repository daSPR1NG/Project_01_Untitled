using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;

namespace dnSR_Coding
{
    ///<summary> BuildingManager description <summary>
    [Component("ExperienceSourcesManager", "")]
    [DisallowMultipleComponent]
    public class ExperienceSourcesManager : Singleton<ExperienceSourcesManager>
    {
        [SerializeField] private List<ExperienceSource> _sources = new();

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }

        #region OnValidate

#if UNITY_EDITOR

        void FindBuildings()
        {
            _sources.Clear();

            for ( int i = 0; i < transform.childCount; i++ )
            {
                if ( transform.GetChild( i ).TryGetComponent( out ExperienceSource building ) )
                {
                    _sources.AppendItem( building );
                }
            }
        }

        private void OnValidate()
        {
            FindBuildings();
        }
#endif

        #endregion
    }
}