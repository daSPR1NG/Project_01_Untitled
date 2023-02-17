namespace dnSR_Coding
{
    public interface IDeathable
    {
        public bool CanDie { get; }
        public abstract void OnDeath();
    }
}