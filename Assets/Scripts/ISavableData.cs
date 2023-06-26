namespace dnSR_Coding
{
    ///<summary> ISavableData description <summary>
    public interface ISavableData<T>
    {
        public int ID { get; set; }
        public T Get();
    }
}