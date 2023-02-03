using dnSR_Coding.Utilities;
using UnityEngine;

namespace dnSR_Coding
{
    public interface IObserver
    {
        public Subject Subject { get; }

        public abstract void SubscribeSelfToSubject();
        public abstract void UnsubscribeSelfToSubject();

        public abstract void OnNotify( object value );
    }
}