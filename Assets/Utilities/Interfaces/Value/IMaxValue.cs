namespace dnSR_Coding
{
    public interface IMaxValue<T>
    {
        public bool HasMaxValue { get; }
        public T MaxValue { get; }

        public abstract void SetMaxValue( T value );
    }
}