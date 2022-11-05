namespace dnSR_Coding
{
    ///<summary> If this interface is on an object, it means that this object is debuggable and can use Logs in console. <summary>
    public interface IDebuggable
    {
        public bool IsDebuggable { get; }
    }
}