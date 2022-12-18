using UnityEngine;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    public enum LinkedBodyPart
    {
        Unassigned, Head, Body, Feet, LeftHand, RightHand
    }

    public enum Rarity
    {
        Unassigned, Common, Magic, Rare, Unique
    }

    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Items/New Item" )]
    public class Item : ScriptableObject
    {
        [Header( "Details" )]
        [SerializeField] private string _name = "[TYPE HERE]";
        [SerializeField] private int _id;
        [SerializeField, ResizableTextArea] private string _description = "TYPE HERE";

        //----------------------------------------------------------------------------------------------------

        [Header( "Item settings" )]
        [SerializeField] private Rarity _rarity = Rarity.Unassigned;
        [SerializeField] private bool _canBeEquipped = false;
        [SerializeField, ShowIf( "_canBeEquipped" )] 
        private bool _isEquipped = false;
        [SerializeField, ShowIf( "_canBeEquipped" )] 
        private LinkedBodyPart _linkedBodyPart = LinkedBodyPart.Unassigned;

        //----------------------------------------------------------------------------------------------------

        [Header( "Stack settings" )]
        [SerializeField] private bool _isStackable = false;
        [SerializeField, ShowIf( "_isStackable" )] 
        private int _stackSize = 1;
        [SerializeField, Range( 1, 40 ), ShowIf( "_isStackable" )] 
        private int _maxStackSize = 1;

        //----------------------------------------------------------------------------------------------------

        [Header( "Stat Settings" )]
        [SerializeField] private bool _hasStats = false;
        //[SerializeField, ShowIf( "_hasStats" )] private List<Stats> _stats = new();

        //----------------------------------------------------------------------------------------------------

        [Header( "UI elements" )]
        [SerializeField, ShowAssetPreview( 64, 64 )] private Sprite _icon = null;

        public class InfosData
        {
            private readonly Item _item;

            public string Name { get; }
            public int ID { get; }
            public string Description { get; }

            public bool CanBeEquipped { get; }
            public bool IsEquipped { get; }
            public Rarity Rarity { get; }
            public LinkedBodyPart LinkedBodyPart { get; }

            public bool IsStackable { get; }
            public int StackSize { get; }
            public int MaxStackSize { get; }

            public bool HasStats { get; }
            //public List<Stats> Stats { get; }

            public Sprite Icon { get; }
            
            public InfosData( Item item )
            {
                _item = item;

                Name = _item._name;
                ID = _item._id;
                Description = _item._description;

                CanBeEquipped = _item._canBeEquipped;
                IsEquipped = _item._isEquipped;
                Rarity = _item._rarity;
                LinkedBodyPart = _item._linkedBodyPart;

                IsStackable = _item._isStackable;
                StackSize = _item._stackSize;
                MaxStackSize = _item._maxStackSize;

                HasStats = _item._hasStats;
                //Stats = _item._stats;

                Icon = _item._icon;
            }
        }

        private void Reset()
        {
            _name = "[TYPE HERE]";
            _id = 0;
            _description = "[TYPE HERE]";

            _canBeEquipped = false;
            _linkedBodyPart = LinkedBodyPart.Unassigned;

            _isStackable = false;
            _stackSize = 1;
            _maxStackSize = 1;

            _hasStats = false;
            //_stats.Clear();

            _icon = null;
        }

        #region Editor

#if UNITY_EDITOR
        [Button]
        private void GenerateIDButton()
        {
            _id = GetInstanceID();
        }
#endif

        #endregion
    }
}