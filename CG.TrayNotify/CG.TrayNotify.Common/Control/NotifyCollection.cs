namespace CG.TrayNotify.Common.Control
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Collections.ObjectModel;
	using System.ComponentModel;

	#endregion Using Directives

	public interface INotifyItem< T > : INotifyPropertyChanged
	{
		T ID { get; set; }
		bool IsEditing { get; set; }
		bool IsNew { get; }
	}

	public abstract partial class NotifyItem< T > : INotifyItem< T >
	{
		#region Public Properties

		public T ID { get { return _id; } set { _id = value; } }  

		public bool IsEditing { get { return _isEditing; } set { _isEditing = value; } }
		public bool IsNew { get { return _isNew; } }

		#endregion Public Properties

		#region Protected Fields

		protected T _id = default( T );
		protected bool _isEditing = false;
		protected bool _isNew = false;

		#endregion Protected Fields

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Fires the event for the property when it changes.
		/// </summary>
		protected virtual void OnPropertyChanged( string propertyName )
		{
			PropertyChangedEventArgs args = new PropertyChangedEventArgs( propertyName );
			
			if( null != PropertyChanged )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion INotifyPropertyChanged

		protected void SetProperty( ref string target, string value )
		{
			if( null == value )
			{
				value = string.Empty;
			}

			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

        protected void SetProperty<T>( ref T target, T value )
        {
            target = value;
            OnPropertyChanged( GetPropertyName( ) );
        }

		protected void SetProperty( ref bool target, bool value )
		{
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		protected void SetProperty( ref Double target, Double value )
		{
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		protected void SetProperty( ref int target, int value )
		{
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		protected void SetProperty( ref DateTime target, DateTime value )
		{
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		protected void SetProperty( ref byte[ ] target, byte[ ] value )
		{
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		protected void SetProperty( ref Guid target, Guid value )
		{
			if( null == value )
			{
				value = Guid.Empty;
			}
			
			if( target != value )
			{
				target = value;
				OnPropertyChanged( GetPropertyName( ) );
			}
		}

		private string GetPropertyName( )
		{
			string name = new System.Diagnostics.StackTrace( ).GetFrame( 2 ).GetMethod( ).Name;
			return name.Substring( 4 );
		}

		protected void TrimTrailingColon( ref string target )
		{
			if( String.IsNullOrEmpty( target ) ) return;

			if( target.Length - 1 == target.LastIndexOf( ":" ) )
			{
				target.Remove( target.Length - 1 );
			}
		}
	}

	/// <summary>
	/// Argument that is passed with the ContentChanged event. Contains the
	/// item that was added to the list.
	/// </summary>
	public class ContentChangedEventArgs< T > : EventArgs
	{
		private T _newItem;
		private int _index = -1;

		public T NewItem { get { return _newItem; } }

		public int Index { get { return _index; } }

		public ContentChangedEventArgs( T newItem, int index )
		{
			_newItem = newItem;
			_index = index;
		}
	}
	
	/// <summary>
    /// Generic collection with notify and observable properties
    /// </summary>
    [Serializable]
	public partial class NotifyCollection<T, P> 
        : ObservableCollection<T>
        , INotifyPropertyChanged
		where T : INotifyItem<P>
    {
		public NotifyCollection( ) { }

		private T _current;
		private T _restoreCurrent;
        private bool _dirty;
        private int _index = 0;
		private bool _enableCurrentChangedEvent = true;

		public bool EnableCurrentChangedEvent
		{
			get
			{
				return _enableCurrentChangedEvent;
			}
			set
			{
				_enableCurrentChangedEvent = value;

				// Save the existing current while the change event is disabled
				if( !_enableCurrentChangedEvent )
				{
					_restoreCurrent = _current;
				}
				// Restore the current and fire a current change event
				else
				{
					//OnCurrentChanged( _restoreCurrent );
					_restoreCurrent = default( T );
				}
			}
		}

		/// <summary>
        /// currently selected item
        /// </summary>
		public T Current
        {
            get { return _current; }
            set
            {
                if( null == _current || null != _current && !_current.Equals( value ) )
                {
                    _current = value;
                    OnPropertyChanged( "Current" );
                    OnCurrentChanged( value );
                }
            }
        }

		public T UpdateCurrent
        {
            set
            {
                if( !_current.Equals( value ) )
                {
                    this[ CurrentIndex ] = value;
                    Current = value;
                }
            }
        }

        /// <summary>
		/// Index of currently selected index
        /// </summary>
        public int CurrentIndex
        {
            get { return _index; }
        }

        public bool IsCurrentEditingOrNew
        {
            get
            {
                if( null == Current )
                    return false;

				return _current.IsEditing || _current.IsNew;
            }
        }

        /// <summary>
        /// Get or set if the list has been modified.
        /// </summary>
        public bool IsDirty
        {
            get { return _dirty; }
            set { _dirty = value; }
        }

        /// <summary>
        /// An item was added, removed or modified in the list.
        /// </summary>
        public event EventHandler< ContentChangedEventArgs<T> > ContentChanged;

        /// <summary>
        /// The details of a item changed.
        /// </summary>
        public void OnContentChanged( )
        {
            _dirty = true;
            if( ContentChanged != null )
                ContentChanged( this, new ContentChangedEventArgs< T >( default( T ), -1 ) );
        }

        /// <summary>
        /// The details of a item changed, and a new item was added to the collection.
        /// </summary>
		public void OnContentChanged( T newItem )
        {
            _dirty = true;

            int index = 0;
			foreach( T item in this )
            {
                if( null != Current && Current.Equals( item ) )
                {
                    Current = item;
                    _index = index;
                    break;
                }
                index++;
            }

            if( ContentChanged != null )
                ContentChanged( this, new ContentChangedEventArgs<T>( newItem, index ) );
        }

        /// <summary> 
        /// The primary item changed in the list.
        /// </summary>
        public event EventHandler CurrentChanged;
		protected void OnCurrentChanged( T newItem )
        {
			if( !_enableCurrentChangedEvent || ( null == Current && null == newItem ) )
			{
				return;
			}
            if( CurrentChanged != null )
            {
                int index = 0;
				foreach( T item in this )
                {
					if( Current.Equals( item ) )
                    {
                        Current = item;
                        _index = index;
                        break;
                    }
                    index++;
                }
                CurrentChanged( this, new ContentChangedEventArgs<T>( newItem, index ) );
            }
        }

        #region Add New 

        /// <summary>
        /// Adds a new item to the list
        /// </summary>
		public T AddNew( T item )
        {
            //add the child to the item list
            if( !this.Contains( item ) )
                this.Add( item );

            Current = item;

			return item;
        }

        #endregion

        public void Delete( P id )
        {
			T itemPrevious = default( T );
			T itemDelete = default( T );

            // Retrieve the item to delete based off of the id
            // Track the previous item to that we can set it to the current item after deletion
			foreach( T item in this )
            {
                if( item.ID.Equals( id ) )
                {
                    itemDelete = item;
                    break;
                }

                itemPrevious = item;
            }

            // Delete the item
            if( null != itemDelete )
            {
                if( this.Contains( itemDelete ) )
					this.Remove( itemDelete );

				OnContentChanged( itemDelete );
            }

            // Set the previous item to current 
            if( null != itemPrevious )
            {
                Current = itemPrevious;
                _index = _index--;
            }
        }

		public T Find( P id )
        {
			foreach( T item in this )
            {
                if( item.ID. Equals( id ) )
                    return item;
            }

            return default( T );
        }

        #region INotifyPropertyChanged Members

	    protected override event PropertyChangedEventHandler PropertyChanged;
         
        protected virtual void OnPropertyChanged( string propertyName )
        {
            if( PropertyChanged != null )
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
        }

        #endregion
    }
}
