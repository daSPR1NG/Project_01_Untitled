namespace dnSR_Coding
{
    public interface IClampedValue<T>
    {
        public bool HasMinValue { get; set; }
        public bool HasMaxValue { get; set; }

        public T MinValue { get; }
        public T MaxValue { get; }

        public T GetMinValue { get => MinValue; }
        public T GetMaxValue { get => MaxValue; }

        public abstract void SetNewMaxValue( T newMaxValue );
    }
}