///-----------------------------------------------------------------------------
/// <copyright file="QueueSync.cs" company="Iridyn">
///     Copyright (c) Iridyn, Inc.  All rights reserved.
/// </copyright>
///
/// <author email="arjay">
///     Arjay Hawco
/// </author>
///
/// <purpose>
///  Thread safe wrapper for System.Collections.Generic.Queue< T > class 
/// </purpose>
///
/// <contents>
///  SyncQueueCS Class -- Thread safe queue class using a critical section lock
/// </contents>
///
/// <history>
///     <change date="03/25/2006" author="arjay" comment="Module Created."/>
/// </history>
///-----------------------------------------------------------------------------
namespace CG.TrayNotify.Common.Threading
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Using Directives

    #region QueueSync class definition

    /// <summary>
    /// Thread safe queue implementation for in-process queues
    /// </summary>
    /// <typeparam name="L"><see cref="ILock" derived object/></typeparam>
    /// <typeparam name="T">type of Queue object</typeparam>
    public class QueueSync< L, T > where L : ILock, new( ) 
    {
        public QueueSync( )
        {
            _queue = new Queue< T >( );
        }

        public QueueSync( int timeout )
        {
            _queue = new Queue<T>( );
            _timeout = timeout;
        }

        public QueueSync( ICollection< T > col )
        {
            _queue = new Queue< T >( col );
        }

        public QueueSync( ICollection<T> col, int timeout )
        {
            _queue = new Queue<T>( col );
            _timeout = timeout;
        }

        public QueueSync( int capacity, int timeout )
        {
            _queue = new Queue<T>( capacity );
            _timeout = timeout;
        }

        /// <summary>
        /// Queue Count property
        /// </summary>
        public int Count { get { using ( AutoLock.LockToRead( _lock, _timeout ) ) { return _queue.Count; } } }

        /// <summary>
        /// Retrieves the next item in the queue without removing the item.
        /// </summary>
        /// <param name="trimWhenEmpty">true to trim the queue size when empty</param>
        /// <returns>T object or default T object when empty</returns>
        public T GetNextItem( bool trimWhenEmpty )
        {
            using ( AutoLock.LockToRead( _lock, _timeout ) )
            {
                if ( 0 != _queue.Count )
                {
                    return _queue.Peek( );
                }
                else
                {
                    if ( trimWhenEmpty )
                    {
                        // Release the queue memory (by default, once the queue grows,
                        // it won't release any allocated memory even if it's empty)
                        _queue.TrimExcess( );
                    }
                }
            }

            return default( T );
        }

        /// <summary>
        /// Inserts a type T object into the queue
        /// </summary>
        /// <param name="queueItem">T queue object</param>
        public void Enqueue( T queueItem )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                _queue.Enqueue( queueItem );
            }
        }

        /// <summary>
        /// Removes the beginning item from the queue and returns the item
        /// </summary>
        /// <returns>type T item or default T if the queue is empty</returns>
        public T Dequeue( )
        {
            using ( AutoLock.LockToWrite( _lock, _timeout ) )
            {
                if (0 != _queue.Count)
                {
                    return _queue.Dequeue( );
                }
            }

            return default( T );
        }


        private Queue< T > _queue = null;
        private L _lock = new L( );
        private int _timeout = AutoLock.Timeout;
    }

    #endregion QueueSync class definition
}
