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
                "Name : " + _infosData.Name + '\n' +
                "ID : " + _infosData.ID + '\n' +
                "Description : " + _infosData.Description + '\n' +

                "Rarity : " +_infosData.Rarity + '\n' +
                "Can be equipped ? : " +_infosData.CanBeEquipped + '\n' +
                "Is equipped ? : " +_infosData.IsEquipped + '\n' +
                "Linked body part : " + _infosData.LinkedBodyPart + '\n' +                

                "Is stackable ? : " +_infosData.IsStackable + '\n' +
                "Stack Size : " +_infosData.StackSize + '\n' +
                "Max Stack Size : " + _infosData.MaxStackSize + '\n' +

                "Has stats ? : " + _infosData.HasStats );

            // Add the stats reading, name + value
        }
    }
}