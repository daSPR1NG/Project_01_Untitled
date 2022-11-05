namespace dnSR_Coding
{
    public class CharacterState_Move : CharacterState
    {
        public override void Enter( StateManager manager )
        {
            DebugState( manager, "Entering Move State" );
        }

        public override void Process( StateManager manager )
        {
            DebugState( manager, "Processing Move State" );
        }

        public override void Exit( StateManager manager )
        {
            DebugState( manager, "Exiting Move State" );
        }

        public override void DebugState( StateManager manager, object message )
        {
            base.DebugState( manager, message );
        }
    }
}