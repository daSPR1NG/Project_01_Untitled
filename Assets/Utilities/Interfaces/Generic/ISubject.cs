using System.Collections.Generic;
using dnSR_Coding.Utilities;
using UnityEngine;

namespace dnSR_Coding
{
    public interface ISubject
    {
        public List<IObserver> Observers { get; }

        public void AddObserver( IObserver observer )
        {
            if ( observer.IsNull() )
            {
                Debug.LogError( "Observer you are trying to add is null." );
            }

            Observers.AppendItem( observer, debugMessage: false );
        }
        public void RemoveObserver( IObserver observer )
        {
            if ( observer.IsNull() )
            {
                Debug.LogError( "Observer you are trying to remove is null." );
            }

            Observers.RemoveItem( observer, debugMessage: false );
        }

        public abstract void NotifyObservers( object value );
    }
}