using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum ResourceType
    {
        Unassigned, Wood, Stone, Minerals, Food
    }

    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Resources/New BasicResource" )]
    public class BasicResource : ScriptableObject
    {
        [Title( "general settings", 12, "white" )]

        [SerializeField] private string _name = "TYPE HERE";
        [SerializeField] private ResourceType _type = ResourceType.Unassigned;
        [SerializeField, ResizableTextArea] private string _description = "TYPE HERE";
        [SerializeField] private int _defaultValueInGold = 15;

        [Space( 10f )]

        [Title( "ui settings", 12, "white" )]

        [SerializeField, ShowAssetPreview( 64, 64 )] private Sprite _icon = null;

        public ResourceType Type { get => _type; }
        public string Description { get => _description; }
        public int ValueInGold { get; private set; }

        //

        public Sprite Icon { get => _icon; }

        public void SetValue( int newValue )
        {
            if ( ValueInGold == newValue ) { return; }

            ValueInGold = newValue;
        }

        private void Reset()
        {
            SetValue( _defaultValueInGold );
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            SetValue( _defaultValueInGold );
        }
#endif

        #endregion
    }
}