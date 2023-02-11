using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/OnlyOneInstance/Character/Stats/New StatType List")]
    public class StatType : ScriptableObject
    {
        [SerializeField] private List<StatInfos> _types = new();

        public StatInfos GetStatInfoByNameOrAbbreviation( string lookedForNameOrAbbreviation )
        {
            if ( _types.IsEmpty() ) 
            {
                Debug.LogError( "No type defined." );
                return null; 
            }

            for ( int i = 0; i < _types.Count; i++ )
            {
                if ( !_types [ i ].Name.Equals( lookedForNameOrAbbreviation ) ) { continue; }

                return _types [ i ];
            }

            return null;
        }

        [Serializable]
        public class StatInfos
        {
            [Header( "Details" )]

            public string Name;
            public string Abbreviation;

            [Header( "UI" )]

            public bool HasAnIcon = true;
            [AllowNesting, ShowIf( "HasAnIcon" ), ShowAssetPreview] public Sprite Icon;
        }
    }
}