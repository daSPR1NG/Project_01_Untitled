using UnityEngine;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class PersistentElementsManager : Singleton<PersistentElementsManager>
    {
        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init( true );
        }
    }
}