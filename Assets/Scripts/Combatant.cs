using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Combat/New Combatant" )]
    public class Combatant : ScriptableObject, IDamageable<int>, IDeathable
    {
        [Header( "Details" )]

        [SerializeField] private string _name;
        [SerializeField] private Enums.Team _team;
        [SerializeField] private StatSheet _statSheet;
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private List<Competence> _competences = new();

        [Header( "Appearence settings" )]

        [SerializeField, ShowAssetPreview] private Sprite _icon;
        [SerializeField, ShowAssetPreview( 128, 128 )] private GameObject _prefab;

        public string GetName() => _name;
        public StatSheet GetStatSheet() => _statSheet;
        public List<Item> GetItems() => _items;
        public List<Competence> GetCompetences() => _competences;

        public bool IsDamageable { get; private set; }
        public bool CanDie => true;

        public void OnDamageTaken( object target, int damageTaken, object assailant )
        {
            Debug.Log( "On damage taken " + target );

        }

        public void OnDeath()
        {
            Debug.Log( "Is Dead" );

        }

        public Enums.Team GetTeam() => _team;
        public void SetTeam( Enums.Team newTeam )
        {
            if ( _team != newTeam ) { _team = newTeam; }
        }

        public void Reset()
        {
            _name = "[TYPE HERE]";

            _statSheet = null;
            _items.Clear();
            _competences.Clear();

            _icon = null;
            _prefab = null;
        }
    }
}