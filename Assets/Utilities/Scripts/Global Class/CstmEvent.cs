using dnSR_Coding.Utilities.Helpers;
using System;
using UnityEngine;

namespace dnSR_Coding
{
    public class CstmEvent
    {
        private Action _action = delegate { };

        public void Call()
        {
            _action?.Invoke();
        }

        public void Subscribe( Action listener )
        {
            _action += listener;
        }

        public void Unsubscribe( Action listener )
        {
            _action -= listener;
        }
    }

    public class CstmEvent<T>
    {
        private Action<T> _action = delegate {};

        public void Call( T args )
        {
            _action?.Invoke( args );
        }

        public void Subscribe( Action<T> listener )
        {
            _action += listener;
        }

        public void Unsubscribe( Action<T> listener )
        {
            _action -= listener;
        }
    }
}