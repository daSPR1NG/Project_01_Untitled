using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class LoadingScreenController : MonoBehaviour, IObserver
    {
        private Transform Content => transform.GetFirstChild();

        #region Enable, Disable

        void OnEnable() => SceneController.OnGameSceneChanged += OnNotification;
        void OnDisable() => SceneController.OnGameSceneChanged -= OnNotification;

        #endregion

        private void Awake() => Init();
        private void Init()
        {
            HideContent();
        }

        [Button]
        public void DisplayContent() => Content.gameObject.TryToDisplay();
        [Button]
        public void HideContent() => Content.gameObject.TryToHide();

        public void OnNotification( object value )
        {
            bool needToBeHidden = ( bool ) value;

            if ( needToBeHidden )
            {
                HideContent();
                return;
            }

            DisplayContent();
        }
    }
}