using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
//using NaughtyAttributes;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> ItemReader description <summary>
    [Component("ItemReader", "")]
    [DisallowMultipleComponent]
    public class ItemReader : MonoBehaviour, IItemReadable
    {
        [SerializeField] private Item _item;
        private Item.InfosData _infosData;

        public Item Item => _item;
        public Item.InfosData InfoData => _infosData;

        [Button]
        private void ReadItemInfosButton()
        {
            ReadItemInfos( Item );
        }

        public void ReadItemInfos( Item item )
        {
            _infosData = new( item );

            Debug.Log(
                _infosData.Name + '\n' +
                _infosData.ID + '\n' +
                _infosData.Description + '\n' +

                _infosData.CanBeEquipped + '\n' +
                _infosData.IsEquipped + '\n' +
                _infosData.LinkedBodyPart + '\n' +

                _infosData.IsStackable + '\n' +
                _infosData.StackSize + '\n' +
                _infosData.MaxStackSize + '\n' +

                _infosData.HasStats );
        }
    }
}