public interface IInitializedValue<T>
{
    public T InitialValue { get; set; }
    public T GetInitialValue { get => InitialValue; }

    public abstract void InitializeValue( T initialValue );
}
