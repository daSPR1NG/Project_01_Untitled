namespace dnSR_Coding
{
    public interface IObserver
    {
        public ISubject Subject { get; }

        public abstract void TryGetSubject();
        public abstract void OnNotification( object value );
    }
}