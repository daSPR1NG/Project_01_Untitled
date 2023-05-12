namespace dnSR_Coding
{
    public interface IMinimizedValue<T>
    {
        public bool HasMinValue { get; }
        public T MinValue { get; }

        public abstract void SetMinValue( T value );
    }
}