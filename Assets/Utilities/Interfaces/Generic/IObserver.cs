namespace dnSR_Coding
{
    public interface IObserver
    {
        public abstract void OnNotification( object value );
    }
}