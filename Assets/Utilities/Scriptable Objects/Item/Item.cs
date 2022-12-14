using UnityEngine;
using ExternalPropertyAttributes;
using dnSR_Coding.Utilities;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public enum LinkedBodyPart
    {
        Unassigned, Head, Chest, Legs, Hands, Weapon_Left, Weapon_Right,
    }

    public enum Rarity
    {
        Unassigned, Common, Magic, Rare, Unique
    }

    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Items/New Item" )]
    public class Item : ScriptableObject
    {
        [Header( "Details" )]
        [SerializeField, ReadOnly] 
        private string _name = "[TYPE HERE]";
        [SerializeField, ReadOnly] 
        private int _id;
        [SerializeField, ReadOnly] 
        private string _description = "TYPE HERE";

        //----------------------------------------------------------------------------------------------------

        [Header( "Item settings" )]
        [SerializeField, ReadOnly] 
        private Rarity _rarity = Rarity.Unassigned;
        [SerializeField, ReadOnly] 
        private bool _canBeEquipped = false;
        [SerializeField, ReadOnly]
        private bool _isEquipped = false;
        [SerializeField, ReadOnly] 
        private LinkedBodyPart _linkedBodyPart = LinkedBodyPart.Unassigned;

        //----------------------------------------------------------------------------------------------------

        [Header( "Stack settings" )]
        [SerializeField, ReadOnly] 
        private bool _isStackable = false;
        [SerializeField, ReadOnly] 
        private int _stackSize = 1;
        [SerializeField, ReadOnly] 
        private int _maxStackSize = 1;

        //----------------------------------------------------------------------------------------------------

        [Header( "Stat Settings" )]
        [SerializeField, ReadOnly] private bool _hasStats = false;
        [SerializeField, AllowNesting, ReadOnly] 
        private List<Stat> _stats = new();

        //----------------------------------------------------------------------------------------------------

        [Header( "Visuals" )]
        [SerializeField, ReadOnly] private Sprite _icon = null;
        [SerializeField, ReadOnly] private GameObject _prefab = null;

        private ItemDatas _datas;
        public ItemDatas Datas
        {
            get 
            {
                if ( _datas.IsNull() )
                {
                    _datas = new ItemDatas( this );
                }
                return _datas;
            }
            set { _datas = value; }
        }

        public class ItemDatas
        {
            private readonly Item _item;

            public string Name { get; set; }
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
            public List<Stat> Stats { get; }

            public Sprite Icon { get; }
            public GameObject Prefab { get; }
            
            public ItemDatas( Item item )
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
                Stats = _item._stats;

                Icon = _item._icon;
                Prefab = _item._prefab;
            }

            public void ReadDatas()
            {
                Debug.Log(
                "Name : " + Name + '\n' +
                "ID : " + ID + '\n' +
                "Description : " + Description + '\n' +

                "Rarity : " + Rarity + '\n' +
                "Can be equipped ? : " + CanBeEquipped + '\n' +
                "Is equipped ? : " + IsEquipped + '\n' +
                "Linked body part : " + LinkedBodyPart + '\n' +

                "Is stackable ? : " + IsStackable + '\n' +
                "Stack Size : " + StackSize + '\n' +
                "Max Stack Size : " + MaxStackSize + '\n' +

                "Has stats ? : " + HasStats + '\n' +
                ReadStats() );
            }

            private string ReadStats()
            {
                string readStats = "Stats :" + '\n';

                for ( int i = 0; i < Stats.Count; i++ )
                {
                    readStats += Stats [ i ].Name.ToString() + " - " + Stats [ i ].GetPoints().ToString() + '\n';
                }

                return readStats;
            }

            public Stat GetStatByType( StatType statType )
            {
                if ( Stats.IsEmpty() )
                {
                    Debug.LogError( "No stats assigned, it might be a bug, try to refresh the item asset", _item );
                    return null;
                }

                for ( int i = 0; i < Stats.Count; i++ )
                {
                    if ( Stats [ i ].GetStatType() != statType ) { continue; }

                    return Stats [ i ];
                }

                Debug.LogError( "No stats assigned, it might be a bug, try to refresh the item asset", _item );
                return null;
            }
        }

        public void Reset()
        {
            _name = "[TYPE HERE]";
            _id = GetInstanceID();
            _description = "[TYPE HERE]";

            _canBeEquipped = false;
            _rarity = Rarity.Unassigned;
            _linkedBodyPart = LinkedBodyPart.Unassigned;

            _isStackable = false;
            _stackSize = 1;
            _maxStackSize = 1;

            _hasStats = false;

            _stats.Clear();
            CreateStatEntriesInEditor();

            _icon = null;
            _prefab = null;
        }

        #region Constructors

        public Item() : base() { }

        public Item( string name ) : base()
        {
            _name = name;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        public void CreateStatEntriesInEditor()
        {
            int amountOfStat = Helper.GetEnumLength( typeof( StatType ) ) - 1;
            int statTypeIndex = 1;

            if ( _stats.Count >= amountOfStat ) { return; }

            if ( _stats.Count < amountOfStat )
            {
                for ( int i = 0; i < amountOfStat; i++ )
                {
                    _stats.AppendItem( new Stat( ( StatType ) Helper.GetEnumToArray( typeof( StatType ) ).GetValue ( statTypeIndex ), 0 ) );
                    statTypeIndex++;
                }                
            }
        }

        private void OnValidate()
        {
            if ( Application.isPlaying ) { return; }

            CreateStatEntriesInEditor();
        }

#endif

        #endregion
    }
}