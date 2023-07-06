using dnSR_Coding.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/Gameplay/Plant/PlantSettings")]
    public class PlantSettings : ScriptableObject
    {
        [field: SerializeField, Foldout( "Runtime settings" )] public Enums.PlantType PlantType { get; private set; }
        [field: SerializeField, Foldout( "Runtime settings" )] public float LifeCycleDuration { get; private set; } = 1;
        [field: SerializeField, Foldout( "Runtime settings" )] public GameObject InGameAppearance { get; private set; }

        [field: SerializeField, Foldout( "UI settings" )] public Sprite Icon { get; private set; }

        public void SetLifeCycleDuration( float value )
        {
            if ( LifeCycleDuration == value ) { return; }
            LifeCycleDuration = value;
        }

        public Transform[] GetAppearances() 
        {
            if ( InGameAppearance.IsNull<GameObject>() ) {
                Debug.LogError( $"Unable to find InGameAppearance transform." );
                return null;
            }
            return InGameAppearance.transform.GetComponentsInChildren<Transform>();
        }
    }
}