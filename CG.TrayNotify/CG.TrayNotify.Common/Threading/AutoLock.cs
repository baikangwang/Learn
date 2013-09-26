///-----------------------------------------------------------------------------
/// <copyright file="Threading.cs" company="Iridyn">
///     Copyright (c) Iridyn, Inc.  All rights reserved.
/// </copyright>
///
/// <author email="arjay">
///     Arjay Hawco
/// </author>
///
/// <purpose>
///  Multithreaded object wrappers for process object locking.
///  Uses the RAII ( Resource Acquisition Is Initialization ) pattern
/// </purpose>
///
/// <contents>
///  AutoLock Class
///	    AutoLockT       -- Helper class used to Auto-Lock/Unlock
///					    classes derived from CLockableXXX classes
///					    listed below.
///
///  Lock Classes
///	    LockableCS	    -- Critical Section base class
///		LockableMutex	-- Mutex base class
///      LockableRW     -- Reader Writer base class
/// 
///
/// </contents>
///
/// <disclaimer>
/// </disclaimer>
///
/// <history>
///     <change date="09/15/2005" author="arjay" comment="Module Created."/>
///     <change date="03/21/2006" author="arjay" comment="Added AutoLock ctor asserts."/>
///     <change date="03/22/2006" author="arjay" comment="Updated comments."/>
///     <change date="10/19/2007" author="arjay" comment="Added static helper methods to AutoLock."/>
/// </history>
///-----------------------------------------------------------------------------

namespace CG.TrayNotify.Common.Threading
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    #endregion Using Directives

    /// <summary>
    /// Used by the AutoLockT and LockableRW classes to specify
    /// the type of locking
    /// </summary>
    public enum LockType { Read, Write };

    /// <summary>
    /// Factory class used for 'quick' and concise locking (to be used along with a 'using' block)
    /// </summary>
    public class AutoLock
    {
        public const int Timeout = 30 * 1000;

        /// <summary>
        /// Creates a critical section autolocker 
        /// </summary>
        /// <param name="lockObject">LockableCS object to lock</param>
        /// <returns>AutoLockT< LockableCS > object</returns>
        public static AutoLockT< ILock > LockToRead( ILock lockObject, int timeout )
        {
            var csLock = lockObject as CriticalSectionAutoLock;

            if ( csLock != null )
            {
                return new AutoLockT<ILock>( lockObject );
            }

            var mutexLock = lockObject as MutexAutoLock;

            if ( mutexLock != null )
            {
                return new AutoLockT< ILock >( lockObject, timeout );
            }

            var readerWriterLock = lockObject as ReaderWriterAutoLock;

            if ( readerWriterLock != null )
            {
                return new AutoLockT<ILock>( lockObject, LockType.Read, timeout );
            }

            throw new Exception( "Invalid lock object" );            
        }

        /// <summary>
        /// Creates a critical section autolocker 
        /// </summary>
        /// <param name="lockObject">LockableCS object to lock</param>
        /// <returns>AutoLockT< ILock > object</returns>
        public static AutoLockT<ILock> LockToWrite( ILock lockObject, int timeout )
        {
            var csLock = lockObject as CriticalSectionAutoLock;

            if ( csLock != null )
            {
                return new AutoLockT< ILock >( lockObject );
            }

            var mutexLock = lockObject as MutexAutoLock;

            if ( mutexLock != null )
            {
                return new AutoLockT<ILock>( lockObject, timeout );
            }

            var readerWriterLock = lockObject as ReaderWriterAutoLock;

            if ( readerWriterLock != null )
            {
                return new AutoLockT<ILock>( lockObject, LockType.Write, timeout );
            }

            throw new Exception( "Invalid lock object" );
        }
    }

    #region Interface/Class definitions

    #region ILockable derived classes and Interface definitions

    /// <summary>
    /// Base AutoLock Interface
    /// </summary>
    public interface ILock
    {
        /// <summary>
        /// LockableCS lock method
        /// </summary>
        void Lock( );

        /// <summary>
        /// LockableMutex lock method
        /// </summary>
        /// <param name="timeout"></param>
        void Lock( int timeout );

        /// <summary>
        /// ReaderWriterAutoLock lock method
        /// </summary>
        /// <param name="lockType"></param>
        /// <param name="timeout"></param>
        void Lock( LockType lockType, int timeout );

        /// <summary>
        /// Unlock method for LockableCS, LockableMutex, and LockableRW
        /// implementation classes
        /// </summary>
        void Unlock( );
    }

    /// <summary>
    /// Class used for Critical Section Synchronization
    /// </summary>
    public class CriticalSectionAutoLock : ILock
    {
        /// <summary>
        /// This default ctor does nothing because all
        /// objects have built in critical section 
        /// synchronization.
        /// </summary>
        public CriticalSectionAutoLock( ) { }

        /// <summary>
        /// Call Monitor Enter method to lock the object
        /// </summary>
        public void Lock( )
        {
            Monitor.Enter( this );
        }

        /// <summary>
        /// Call Monitor Exit method to unlock the object
        /// </summary>
        public void Unlock( )
        {
            Monitor.Exit( this );
        }

        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        /// <param name="timeout"></param>
        public void Lock( int timeout )
        {
            Debug.Fail( "This lock method is invalid for LockableCS" );
        }

        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        public void Lock( LockType lockType, int timeout )
        {
            Debug.Fail( "This lock method is invalid for LockableCS" );
        }
    }

    /// <summary>
    /// Class used for named Mutex Synchronization
    /// </summary>
    public class MutexAutoLock : ILock,IDisposable
    {
        /// <summary>
        /// Create the named mutex
        /// </summary>
        /// <param name="name">Name of mutex</param>
        public MutexAutoLock( string name )
        {
            _mutex = new Mutex( false, name, out _alreadyExists );
        }

        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        public void Lock( )
        {
            Debug.Fail( "This lock method is invalid for LockableMutex" );
        }
        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        public void Lock( LockType lockType, int timeout )
        {
            Debug.Fail( "This lock method is invalid for LockableMutex" );
        }

        /// <summary>
        /// Acquire the lock on the mutex
        /// </summary>
        /// <param name="timeout"></param>
        public void Lock( int timeout )
        {
            _mutex.WaitOne( timeout, false );
        }

        /// <summary>
        /// Release the mutex lock
        /// </summary>
        public void Unlock( )
        {
            _mutex.ReleaseMutex( );
        }

        /// <summary>
        /// Checks if the mutex was created new or existed previously
        /// </summary>
        public bool AlreadyExists
        {
            get
            {
                return _alreadyExists;
            }
        }

        /// <summary>
        /// The wrapped mutex object
        /// </summary>
        private Mutex _mutex = null;

        /// <summary>
        /// Tracks whether mutex was created new or previously existed
        /// </summary>
        private bool _alreadyExists = false;

        protected virtual void Dispose(bool all)
        {
            if (all)
            {
                _mutex.Dispose();
            }
        }

        public void Dispose()
        {
           Dispose(true);
        }
    }

    /// <summary>
    /// Class used for Reader Writer synchronization
    /// </summary>
    public class ReaderWriterAutoLock : ILock
    {
        /// <summary>
        /// Create the Reader Writer lock
        /// </summary>
        public ReaderWriterAutoLock( ) { }

        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        public void Lock( )
        {
            Debug.Fail( "This lock method is invalid for LockableRW" );
        }
        /// <summary>
        /// Noop method to satisfy ILockable (not used here)
        /// </summary>
        public void Lock( int timeout )
        {
            Debug.Fail( "This lock method is invalid for LockableRW" );
        }

        /// <summary>
        /// Acquire a read or write lock on the ReaderWriterLock
        /// </summary>
        /// <param name="lockType">Type of lock to acquire - read or write</param>
        /// <param name="timeout">time to wait for lock (seconds)</param>
        public void Lock( LockType lockType, int timeout )
        {
            _lockType = lockType;

            switch ( _lockType )
            {
            case LockType.Read:
                _rwl.AcquireReaderLock( timeout );
                break;
            case LockType.Write:
                _rwl.AcquireWriterLock( timeout );
                break;
            }
        }

        /// <summary>
        /// Releases the ReaderWriterLock
        /// </summary>
        public void Unlock( )
        {
            switch ( _lockType )
            {
            case LockType.Read:
                _rwl.ReleaseReaderLock( );
                break;
            case LockType.Write:
                _rwl.ReleaseWriterLock( );
                break;
            }
        }


        /// <summary>
        /// Contained ReaderWriter lock
        /// </summary>
        private ReaderWriterLock _rwl = new ReaderWriterLock( );
        /// <summary>
        /// Lock type (we track this in Lock, so we can unlock the
        /// appropriate type in Unlock)
        /// </summary>
        private LockType _lockType = LockType.Read;
    }

    #endregion ILock derived classes and Interface definitions

    #region AutoLockT class definition

    /// <summary>
    /// Class used for Auto Locking/Unlocking of LockableXXXX classes
    /// This class is to be used within a 'using' statement and
    /// performs RAII type locking in the ctor and unlocking in
    /// the dtor when the object leaves the using block
    /// 
    /// EX.1 - Using the AutoLock static factory methods
    /// 
    /// // Critical section
    /// using( AutoLock.Lock( _csLockDerived ) )
    /// {
    ///     // Thread safe within this block
    /// }
    /// 
    /// // Mutex
    /// using( AutoLock.Lock( _mutexLockDerived, timeout ) )
    /// {
    ///     // Thread safe within this block
    /// }
    /// 
    /// // ReaderWriterLock - locking to read
    /// /// using( AutoLock.LockToRead( _rwLockDerived, timeout ) )
    /// {
    ///     // Thread safe for reading within this block
    /// }
    /// 
    /// // ReaderWriterLock - locking to write
    /// /// using( AutoLock.LockToWrite( _rwLockDerived, timeout ) )
    /// {
    ///     // Thread safe for writing within this block
    /// } 
    /// 
    /// using( AutoLockT< LockableDerivedObj >
    ///   autoLock = AutoLockT<LockableDerivedObj>.Create( _lockDerived ))
    /// {
    ///     // Thread safe within this block
    /// }
    /// 
    /// Note: Unlocking occurs when leaving using scope.  This is helpful
    /// because rather than having to code the following for each lock/unlock
    /// operation, we can use the using statement above:
    /// try
    /// {
    ///    _lockDerived.Lock(...);
    ///    // Use locked resource
    /// }
    /// catch(...)
    /// {
    /// }
    /// finally( ... )
    /// {
    ///    _lockDerived.Unlock( );
    /// }
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutoLockT< T > : IDisposable where T : ILock
    {
        /// <summary>
        /// LockableCS class ctor
        /// </summary>
        /// <param name="lockObject">LockableCS object to lock</param>
        public AutoLockT( T lockObject )
        {
            if ( !( lockObject is CriticalSectionAutoLock ) )
            {
                Debug.Fail( "This constructor is only to be used with LockableCS" );
            }
            _lockObject = lockObject;
            _lockObject.Lock( );
        }

        /// <summary>
        /// LockableMutex class ctor
        /// </summary>
        /// <param name="lockObject">LockableMutex object to lock</param>
        /// <param name="timeout">timeout to wait to acquire lock</param>
        public AutoLockT( T lockObject, int timeout )
        {
            if ( !( lockObject is MutexAutoLock ) )
            {
                Debug.Fail( "This constructor is only to be used with LockableMutex" );
            }
            _lockObject = lockObject;
            _lockObject.Lock( timeout );
        }

        /// <summary>
        /// LockableRW class ctor
        /// </summary>
        /// <param name="lockObject">LockableRW object to lock</param>
        /// <param name="lockType">Read or Write lock type</param>
        /// <param name="timeout">timeout to wait to acquire the lock</param>
        public AutoLockT( T lockObject, LockType lockType, int timeout )
        {
            if ( !( lockObject is ReaderWriterAutoLock ) )
            {
                Debug.Fail( "This constructor is only to be used with LockableRW" );
            }
            _lockObject = lockObject;
            _lockObject.Lock( lockType, timeout );
        }

        /// <summary>
        /// dtor - calls dispose which unlocks the contained object
        /// </summary>
        ~AutoLockT( )
        {
            Dispose( false );
        }

        #region IDisposable Members

        /// <summary>
        /// Implementation of IDisposable.
        /// Call the virtual Dispose method.
        /// Suppress Finalization.
        /// </summary>
        public void Dispose( )
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Overridden IDisposable method
        /// </summary>
        /// <param name="isDisposing"></param>
        protected virtual void Dispose( bool isDisposing )
        {
            // Don't dispose more than once.
            if ( _isDisposed )
            {
                return;
            }

            if ( isDisposing )
            {
                // Unlock the object
                _lockObject.Unlock( );
            }

            // Set disposed flag:
            _isDisposed = true;
        }

        /// <summary>
        /// Tracks whether object has already been disposed
        /// </summary>
        private bool _isDisposed = false;

        #endregion IDisposable Members

        #region Private fields

        /// <summary>
        /// Contained ILockable derived object
        /// </summary>
        private T _lockObject = default( T );

        #endregion Private fields

    }

    #endregion AutoLockT class definition

    #endregion Interface/Class definitions

}

