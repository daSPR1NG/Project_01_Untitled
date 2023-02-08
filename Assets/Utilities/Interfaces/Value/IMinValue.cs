namespace dnSR_Coding
{
    public interface IMinValue<T>
    {
        public bool HasMinValue { get; }
        public T MinValue { get; }
    }
}