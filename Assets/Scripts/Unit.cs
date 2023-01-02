using ExternalPropertyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/Combat/Unit")]
    public class Unit : ScriptableObject
    {
        [Header( "Details" )]
        [SerializeField] private string _name;
        [SerializeField] private StatSheet _stats;
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private List<Competences> _competences = new();

        [Header( "Appearence settings" )]
        [SerializeField, ShowAssetPreview] private Sprite _icon;
        [SerializeField, ShowAssetPreview( 128, 128 )] private GameObject _prefab;
    }
}