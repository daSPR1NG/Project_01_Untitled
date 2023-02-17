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
            Refresh();
        }

        [Button]
        private void Refresh()
        {
            _gameDataReference.StatDataReference.SetTypesNameInEditor();
        }
    }
}