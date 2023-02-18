using NaughtyAttributes;
using UnityEngine;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private GameDataReference _gameDataReference;
        public GameDataReference GetGameDataReference() => _gameDataReference;

        protected override void Init( bool dontDestroyOnLoad = false )
        {
            base.Init();
#if UNITY_EDITOR
            Refresh();
#endif
        }

#if UNITY_EDITOR
        [Button]
        private void Refresh()
        {
            _gameDataReference.StatDataReference.SetTypesNameInEditor();
        }
#endif
    }
}