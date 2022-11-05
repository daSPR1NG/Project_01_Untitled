using System;

namespace dnSR_Coding
{
    ///<summary> CharacterState is a component that declares the composition of a state. <summary>
    public abstract class CharacterState
    {
        public abstract void Enter( StateManager manager );
        public abstract void Process( StateManager manager );
        public abstract void Exit( StateManager manager );

        public virtual void DebugState( StateManager manager, object message )
        {
            if ( !manager.IsDebuggable ) { return; }

            Utilities.Helper.Log( manager, message );
        }

        // Block form to copy paste to another created state 

        //public override void Enter( StateManager manager )
        //{
        //// Pass info, init variable, animations, etc... all we need on entering state.
        //DebugState( manager, "Entering - State" );
        //}

        //public override void Process( StateManager manager )
        //{
        //// What the state do overtime, at runtime.
        //DebugState( manager, "Processing - State" );
        //}

        //public override void Exit( StateManager manager )
        //{
        //// The things you do when exiting the state.
        //DebugState( manager, "Exiting - State" );
        //}

        //public override void DebugState( StateManager manager, object message )
        //{
        //    base.DebugState( manager, message );
        //}
    }
}