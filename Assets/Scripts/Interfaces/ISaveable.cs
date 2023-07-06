using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> ISaveable description <summary>
    public interface ISaveable
    {
        public string ID { get; set; }
        public object GetData();
        public void Load( object data );

        public class ISaveableExtensions
        {
            public static string GetID( string inputID )
            {
                if ( inputID.IsNull<string>() || inputID.Equals( string.Empty ) )
                {
                    inputID = System.Guid.NewGuid().ToString();
                    UnityEngine.Debug.Log( $"Id wasn't found, a new one has been set. {inputID}" );
                }

                return inputID;
            }
        }
    }
}