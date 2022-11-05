namespace dnSR_Coding
{
    ///<summary> 
    /// If this interface is on an object, it means that this object receives a validation check corresponding to evaluated parameters. 
    ///<summary>
    public interface IValidatable
    {
        bool IsValid { get; }
    }
}