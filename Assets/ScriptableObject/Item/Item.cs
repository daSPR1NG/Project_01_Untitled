using UnityEngine;
using ExternalPropertyAttributes;
using dnSR_Coding.Utilities;
using static UnityEditor.Progress;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        //[SerializeField, ShowIf( "_hasStats" )] private List<Stats> _stats = new();

        //----------------------------------------------------------------------------------------------------

        [Header( "UI elements" )]
        [SerializeField, ReadOnly, ShowAssetPreview( 64, 64 )] private Sprite _icon = null;

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
                //Stats = _item._stats;

                Icon = _item._icon;
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

                "Has stats ? : " + HasStats );
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

        //[Button( "Rename File" )]
        //private void RenameFileAccordingToDatasNameButton()
        //{
        //    string newName = Datas.Name;

        //    string assetPath = AssetDatabase.GetAssetPath( GetInstanceID() );
        //    AssetDatabase.RenameAsset( assetPath, newName );
        //    AssetDatabase.SaveAssets();
        //}
        //[Button( "Generate ID" )]
        //private void GenerateIDButton()
        //{
        //    _id = GetInstanceID();
        //}

        //[AssetSelection]
        //private void OnAssetSelection()
        //{
        //    Debug.Log( "Selection : " + name );
        //}

#endif

        #endregion
    }
}