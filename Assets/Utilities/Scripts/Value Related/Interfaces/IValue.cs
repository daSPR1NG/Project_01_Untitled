namespace dnSR_Coding
{
    ///<summary> IValue description <summary>
    public interface IValue<T>
    {
        public T Value { get; set; }
        public T GetValue { get => Value; }        
    }
}