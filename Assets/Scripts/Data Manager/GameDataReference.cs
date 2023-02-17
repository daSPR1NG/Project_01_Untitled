using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/One Instance/Datas/New Data Manager Reference")]
    public class GameDataReference : ScriptableObject
    {
        [field: SerializeField] public StatDataReference StatDataReference { get; set; }
    }
}