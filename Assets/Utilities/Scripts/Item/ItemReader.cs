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
    public class ItemReader : MonoBehaviour, IItemReader
    {
        [SerializeField] private Item _item;
        private Item.ItemInfos _infosData;

        public Item Item => _item;
        public Item.ItemInfos InfoData => _infosData;

        [Button]
        private void ReadItemInfosButton()
        {
            ReadItemDatas( Item );
        }

        public void ReadItemDatas( Item item )
        {
            _infosData = new( item );

            _infosData.ReadInfos();

            // Add the stats reading, name + value
        }
    }
}