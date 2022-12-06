namespace dnSR_Coding
{
    ///<summary> IItemReadable description <summary>
    public interface IItemReadable
    {
        public Item Item { get; }
        public Item.InfosData InfoData { get; }

        public abstract void ReadItemInfos( Item item );
    }
}