namespace dnSR_Coding
{
    ///<summary> IItemReader description <summary>
    public interface IItemReader
    {
        public Item Item { get; }
        public Item.ItemDatas InfoData { get; }

        public abstract void ReadItemDatas( Item item );
    }
}