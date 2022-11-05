using UnityEngine;
using System.Collections;

namespace dnSR_Coding
{
    ///<summary> CharacterState_Idle description <summary>
    public class CharacterState_Idle : CharacterState
    {
        public override void Enter( StateManager manager )
        {
            DebugState( manager, "Entering Idle State" );
        }

        public override void Process( StateManager manager )
        {
            DebugState( manager, "Processing Idle State" );
        }

        public override void Exit( StateManager manager )
        {
            DebugState( manager, "Exiting Idle State" );
        }

        public override void DebugState( StateManager manager, object message )
        {
            base.DebugState( manager, message );
        }
    }
}