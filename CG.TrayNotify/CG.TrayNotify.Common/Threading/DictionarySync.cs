namespace CG.TrayNotify.Common.Threading
{
    #region Using Directives

    using System;
    using System.Collections.Generic;

    #endregion Using Directives

    public class DictionarySync<L, K, V> where L : ILock, new( )
    {
        public DictionarySync( )
        {
            _lock = new L( );
        }

        public DictionarySync( int timeout )
        {
            _lock = new L( );
            _timeout = timeout;
        }

        public V this [ K key ]
        {
            get { using ( AutoLock.LockToRead( _lock, _timeout ) ) { return _dictionary [ key ]; } }
            set { using ( AutoLock.LockToWrite( _lock, _timeout ) ) { _dictionary [ key ] = value; } }
        }

        public int Count
        {
            get { using ( AutoLock.LockToRead( _lock, _timeout ) ) { return _dictionary.Count; } }
        }

        /// WARNING, the caller must externally lock the class by retrieving the Lock property of
        /// this class and using the AutoLock.LockToRead or AutoLock.LockToWrite methods.
        #region Warning: External Locks Required

        public Dictionary<K, V> Dictionary
        {
            get { return _dictionary; }
        }

        public Dictionary<K, V>.KeyCollection Keys
        {
            get { return _dictionary.Keys; }
        }

        public Dictionary<K, V>.ValueCollection Values
        {
            get { return _dictionary.Values; }
        }

        #endregion Warning: External Locks Required


        public L Lock
        {
            get { return _lock; }
        }

        public void Add( K key, V value )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                _dictionary.Add( key, value );
            }
        }

        public void Clear( )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                _dictionary.Clear( );
            }
        }

        public bool ContainsKey( K key )
        {
            using ( AutoLock.LockToRead( _lock, _timeout ) )
            {
                return _dictionary.ContainsKey( key );
            }
        }

        public bool ContainsValue( V value )
        {
            using ( AutoLock.LockToRead( _lock, _timeout ) )
            {
                return _dictionary.ContainsValue( value );
            }
        }

        /// <summary>
        /// Returns Dictionary Enumerator.  WARNING, must externally lock the class
        /// using the Lock Property before calling this method and enumerating the 
        /// contents.
        /// </summary>
        /// <returns></returns>
        public Dictionary<K, V>.Enumerator GetEnumerator( )
        {
            return _dictionary.GetEnumerator( );
        }

        public void Remove( K key )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                _dictionary.Remove( key );
            }
        }

        public bool TryGetValue( K key, out V value )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                return _dictionary.TryGetValue( key, out value );
            }
        }

        private Dictionary<K, V> _dictionary = new Dictionary<K, V>( );
        private L _lock;
        private int _timeout = AutoLock.Timeout;
    }
}
