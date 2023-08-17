using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Gameplay/Plant/PlantSettings" )]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class PlantSettings : ScriptableObject
    {
        [field: SerializeField, FoldoutGroup( "Runtime settings" )] 
        public Enums.Plant_Type PlantType { get; private set; }

        [field: SerializeField, FoldoutGroup( "Runtime settings" )] 
        public float LifeCycleDuration { get; private set; } = 1;

        [field: SerializeField, FoldoutGroup( "Runtime settings" )] 
        public GameObject InGameAppearance { get; private set; }

        [field: SerializeField, FoldoutGroup( "UI settings" )] 
        public Sprite Icon { get; private set; }

        public void SetLifeCycleDuration( float value )
        {
            if ( LifeCycleDuration == value ) { return; }
            LifeCycleDuration = value;
        }

        public Transform [] GetAppearances()
        {
            if ( InGameAppearance.IsNull<GameObject>() )
            {
                Debug.LogError( $"Unable to find InGameAppearance transform." );
                return null;
            }
            return InGameAppearance.transform.GetComponentsInChildren<Transform>();
        }
    }
}