namespace dnSR_Coding
{
    public interface IClampedValue<T>
    {
        public bool HasMinValue { get; }
        public bool HasMaxValue { get; }

        public T MinValue { get; }
        public T MaxValue { get; }

        public abstract void SetNewMaxValue( T newMaxValue );
    }
}