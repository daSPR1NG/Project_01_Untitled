using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Combat/New Combatant Data" )]
    public class CombatantData : ScriptableObject
    {
        [Header( "Details" )]

        [SerializeField] private string _name;
        [SerializeField] private Enums.Team _team;

        [Header( "Dependencies" )]

        [SerializeField, InfoBox( "This stat sheet needs to be referenced.", EInfoBoxType.Normal )]
        private StatSheet _statSheet;
        [SerializeField, InfoBox( "Items need to be referenced.", EInfoBoxType.Normal )]
        private List<Item> _items = new();
        [SerializeField, InfoBox( "Competences need to be referenced.", EInfoBoxType.Normal )]
        private List<Competence> _competences = new();

        [Header( "Appearence settings" )]

        [SerializeField, ShowAssetPreview] private Sprite _icon;
        [SerializeField, ShowAssetPreview( 128, 128 )] private GameObject _prefab;

        public string GetName() => _name;
        public StatSheet GetStatSheet() => _statSheet;
        public List<Item> GetItems() => _items; // => Inventory
        public List<Competence> GetCompetences() => _competences; // => Competences holder

        public void SetName( string input )
        {
            if ( !_name.Equals( input ) ) { _name = input; }
        }

        public void SetTeam( Enums.Team newTeam )
        {
            if ( _team != newTeam ) { _team = newTeam; }
        }
        public bool IsTeamAlly() => _team == Enums.Team.Ally;
        public bool IsTeamEnemy() => _team == Enums.Team.Enemy;

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