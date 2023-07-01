namespace dnSR_Coding.Interfaces.Gameplay
{
    ///<summary> IInteractable description <summary>
    public interface IInteractable
    {
        public bool IsInteractive { get; set; }
        public bool IsItAlwaysSelectable { get; set; }
        public object Interactor { get; set; }

        public abstract void BeginInteraction( object interactor );
        public abstract void EndInteraction();
    }
}