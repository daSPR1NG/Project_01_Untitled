namespace dnSR_Coding
{
    ///<summary> ILoadableData description <summary>
    public interface ILoadableData<T>
    {
        public void Load( T data );
    }
}