using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/One Instance/Character/_statSheet/New StatDataReference List")]
    public class StatDataReference : ScriptableObject
    {
        [SerializeField] private List<StatInfos> _types = new();

        public StatInfos GetStatInfoByNameOrAbbreviation( string lookedForNameOrAbbreviation )
        {
            if ( _types.IsEmpty() ) 
            {
                Debug.LogError( "No _type defined." );
                return null; 
            }

            for ( int i = 0; i < _types.Count; i++ )
            {
                if ( !_types [ i ].Name.Equals( lookedForNameOrAbbreviation ) ) { continue; }

                return _types [ i ];
            }

            return null;
        }        

        public List<StatInfos> GetTypes() { return _types; }        

        [Serializable]
        public class StatInfos
        {
            [Header( "Details" )]

            [HideInInspector] public string Name;
            public string Abbreviation;
            public Enums.StatType Type;

            [Header( "UI" )]

            public bool HasAnIcon = true;
            [AllowNesting, ShowIf( "HasAnIcon" ), ShowAssetPreview] public Sprite Icon;
        }

#if UNITY_EDITOR

        public void SetTypesNameInEditor()
        {
            if ( _types.IsEmpty() ) { return; }

            for ( int i = 0; i < _types.Count; i++ )
            {
                _types [ i ].Name = _types [ i ].Type.ToString();
            }
        }

#endif
    }
}