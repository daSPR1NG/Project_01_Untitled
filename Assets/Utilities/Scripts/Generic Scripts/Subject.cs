using UnityEngine;
using System.Collections.Generic;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    public abstract class Subject
    {
        private readonly List<IObserver> _observers = new();

        public void AddObserver( IObserver observer ) => _observers.AppendItem( observer );
        public void RemoveObserver( IObserver observer ) => _observers.RemoveItem( observer );

        protected void NotifyObservers( object value )
        {
            foreach ( var observer in _observers )
            {
                observer?.OnNotify( value );
            }
        }
    }
}