using System;

namespace dnSR_Coding
{
    public interface ISubject
    {
        public abstract void OnModification( Action<object> actionToExecute, object dataToPush );
    }

    public interface ISubjectExtensions
    {
        public static void TriggerAction( Action<object> actionToExecute, object dataToPush )
        {
            actionToExecute?.Invoke( dataToPush );
        }
    }
}