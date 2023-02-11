using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [DisallowMultipleComponent]
    public class LoadingScreenController : MonoBehaviour, IObserver, IDebuggable
    {
        private Transform Content => transform.GetFirstChild();

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() => SceneController.OnGameSceneChanged += OnNotification;
        void OnDisable() => SceneController.OnGameSceneChanged -= OnNotification;

        #endregion

        private void Awake() => Init();
        private void Init()
        {
            HideContent();
        }

        public void DisplayContent() => Content.gameObject.TryToDisplay();
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