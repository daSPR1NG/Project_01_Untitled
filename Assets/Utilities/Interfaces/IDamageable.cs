namespace dnSR_Coding
{
    public interface IDamageable<T>
    {
        public bool IsDamageable { get; }
        public abstract void OnDamageTaken( object target, T damageTaken, object assailant );
    }
}