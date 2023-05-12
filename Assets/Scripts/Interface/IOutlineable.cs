namespace dnSR_Coding.Interfaces.Gameplay
{
    ///<summary> IOutlineable description <summary>
    public interface IOutlineable
    {
        public Outline OutlineComponent { get; }
        public bool IsOutlined { get; }
        public float OutlineWidth { get; set; }

        public abstract void DisplayOutline( float width = 1 );
        public abstract void HideOutline();
    }
}