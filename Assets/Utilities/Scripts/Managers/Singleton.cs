using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> This is used for singleton classes, it avoids to write down the singleton logic each time it is needed. <summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance  { get; private set; }

        private void Awake() => Init();

        protected virtual void Init( bool dontDestroyOnLoad = false )
        {
            if ( !Instance.IsNull<Singleton<T>>() && Instance != this as T )
            { 
                Instance.gameObject.DestroyInRuntimeOrEditor();
            }

            Instance = this as T;
            if ( dontDestroyOnLoad ) { DontDestroyOnLoad( Instance ); }

            //Debug.Debug( "The instance of " + name +  " has been set on initialization.");
        }
    }
}