using UnityEngine;
using ExternalPropertyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public enum Test {}

    public class EnumWriterHelper : MonoBehaviour
    {
        public string Value;
        [ReadOnly] public int Key = 0;

        public int KeyToRemove = 0;
        public string ValueToRemove;

        public Dictionary<int, string> _enumKeyValuePairs = new();

        public void AddEnumEntry( int key, string value )
        {
            if ( value.Equals( string.Empty ) )
            {
                Debug.LogError( "The value you are trying to push is incorrect, it must contains at least one character." );
                return;
            }

            // Check if the key already exists
            if ( _enumKeyValuePairs.ContainsKey( key ) )
            {
                Debug.Log( "This key already exists : " + key + " / " + value  );
                return;
            }

            // Add new key
            _enumKeyValuePairs.Add( key, value );
            Key++;

            // Debug key
            Debug.Log( "New key created : " + key + " / " + value  );
        }

        public void RemoveEnumEntryWithKey( int key )
        {
            // Check if the key already exists
            if ( !_enumKeyValuePairs.ContainsKey( key ) )
            {
                Debug.Log( "This key does not exists : " + key );
                return;
            }

            string valueOfKey = _enumKeyValuePairs [ key ];
            _enumKeyValuePairs.Remove( key );

            Debug.Log( "The key [" + key + "] with value : {" + valueOfKey + "} has been removed !" );
        }

        public void RemoveEnumEntryWithValue( string value )
        {
            // Check if the key already exists
            if ( !_enumKeyValuePairs.ContainsValue( value ) )
            {
                Debug.Log( "This value does not exists : " + value );
                return;
            }

            int keyIndex = 0;
            foreach ( int key in _enumKeyValuePairs.Keys )
            {
                keyIndex = key;
                Debug.Log( keyIndex );
            }

            _enumKeyValuePairs.Remove( keyIndex );

            Debug.Log( "The key [" + keyIndex + "] with value : {" + value + "} has been removed !" );
        }

        [Button]
        public void TestAddEnumEntry()
        {
            AddEnumEntry( Key, Value );
        }

        [Button]
        public void TestRemoveEnumEntryWithKey()
        {
            RemoveEnumEntryWithKey( KeyToRemove );
        }

        [Button]
        public void TestRemoveEnumEntryWithValue()
        {
            RemoveEnumEntryWithValue( ValueToRemove );            
        }
    }
}