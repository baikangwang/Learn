using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Documents;

namespace CG.TrayNotify.Common.Control
{
	// The GridView has access to the ItemSource on the ListView through the dependency property mechanism.
	public class SortableListView : ListView
	{
		private SortableGridViewColumn _lastSortedOnColumn = null;
		private ListSortDirection _lastSortDirection = ListSortDirection.Ascending;
		private SortAdorner _curAdorner = null;
		private GridViewColumnHeader _curSortCol = null;

		
		public string AdornerBrushColor
		{
			get { return ( string ) GetValue( AdornerBrushColorProperty ); }
			set { SetValue( AdornerBrushColorProperty, value ); }
		}

		public static readonly DependencyProperty AdornerBrushColorProperty =
			DependencyProperty.Register( "AdornerBrushColorProperty"
                , typeof( string )
                , typeof( SortableListView )
                , new UIPropertyMetadata( "White" ) );


		public delegate void PreviewSortCompletedEventHandler( object sender, EventArgs e );
		public delegate void SortCompletedEventHandler( object sender, EventArgs e );

		public event PreviewSortCompletedEventHandler PreviewSortCompleted
		{
			remove { _previewSortCompletedEvent -= value; }
			add { _previewSortCompletedEvent += value; }
		}

		public event SortCompletedEventHandler SortCompleted
		{
			remove { _sortCompletedEvent -= value; }
			add { _sortCompletedEvent += value; }
		}

		/// <summary>
		/// Executes when the control is initialized completely the first time through. Runs only once.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInitialized( EventArgs e )
		{
			// add the event handler to the GridViewColumnHeader. This strongly ties this ListView to a GridView.
			this.AddHandler( GridViewColumnHeader.ClickEvent, new RoutedEventHandler( GridViewColumnHeaderClickedHandler ) );

			// cast the ListView's View to a GridView
			GridView gridView = this.View as GridView;
			if( gridView != null )
			{
				// determine which column is marked as IsDefaultSortColumn. Stops on the first column marked this way.
				SortableGridViewColumn sortableGridViewColumn = null;
				foreach( GridViewColumn gridViewColumn in gridView.Columns )
				{
					sortableGridViewColumn = gridViewColumn as SortableGridViewColumn;
					if( sortableGridViewColumn != null )
					{
						if( sortableGridViewColumn.IsDefaultSortColumn )
						{
							break;
						}
						sortableGridViewColumn = null;
					}
				}

				// if the default sort column is defined, sort the data and then update the templates as necessary.
				//if( sortableGridViewColumn != null )
				//{
				//    lastSortedOnColumn = sortableGridViewColumn;
				//    Sort( sortableGridViewColumn.SortPropertyName, ListSortDirection.Ascending );

				//    GridViewColumnHeader header = (GridViewColumnHeader)sortableGridViewColumn; 

				//    SetAdorner( header, ListSortDirection.Ascending );


					//if( !String.IsNullOrEmpty( this.ColumnHeaderSortedAscendingTemplate ) )
					//{
					//    sortableGridViewColumn.HeaderTemplate = this.TryFindResource( ColumnHeaderSortedAscendingTemplate ) as DataTemplate;
					//}

//					this.SelectedIndex = 0;
//				}
			}

			base.OnInitialized( e );
		}

		/// <summary>
		/// Event Handler for the ColumnHeader Click Event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GridViewColumnHeaderClickedHandler( object sender, RoutedEventArgs e )
		{
			GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

			// ensure that we clicked on the column header and not the padding that's added to fill the space.
			if( headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding )
			{
				// attempt to cast to the sortableGridViewColumn object.
				SortableGridViewColumn sortableGridViewColumn = ( headerClicked.Column ) as SortableGridViewColumn;

				if( _curSortCol != null )
				{
					AdornerLayer.GetAdornerLayer( _curSortCol ).Remove( _curAdorner );
				}

				// ensure that the column header is the correct type and a sort property has been set.
				if( sortableGridViewColumn != null && !String.IsNullOrEmpty( sortableGridViewColumn.SortPropertyName ) )
				{

					ListSortDirection direction;

					// determine if this is a new sort, or a switch in sort direction.
					if( _lastSortedOnColumn == null
						|| String.IsNullOrEmpty( _lastSortedOnColumn.SortPropertyName )
						|| !String.Equals( sortableGridViewColumn.SortPropertyName, _lastSortedOnColumn.SortPropertyName, StringComparison.InvariantCultureIgnoreCase ) )
					{
						direction = ListSortDirection.Ascending;
					}
					else
					{
						direction = ( _lastSortDirection == ListSortDirection.Ascending ) ? ListSortDirection.Descending : ListSortDirection.Ascending;
					}

					Cursor oldCursor = Cursor;

					try
					{
						Cursor = Cursors.Wait;

						FirePreviewSortCompletedEvent( );

						Sort( sortableGridViewColumn.SortPropertyName, direction );

						SetAdorner( headerClicked, direction );

						FireSortCompletedEvent( );
					}
					finally
					{
						Cursor = oldCursor;
					}


					_lastSortedOnColumn = sortableGridViewColumn;
				}
			}
		}

		public void FirePreviewSortCompletedEvent( )
		{
			if( _previewSortCompletedEvent != null )
			{
				_previewSortCompletedEvent( this, new EventArgs( ) );
			}
		}

		public void FireSortCompletedEvent( )
		{
			if( _sortCompletedEvent != null )
			{
				_sortCompletedEvent( this, new EventArgs( ) );
			}
		}

		/// <summary>
		/// Helper method that sorts the data.
		/// </summary>
		/// <param name="sortBy"></param>
		/// <param name="direction"></param>
		private void Sort( string sortBy, ListSortDirection direction )
		{
			_lastSortDirection = direction;
			ICollectionView dataView = CollectionViewSource.GetDefaultView( this.ItemsSource );

            if ( dataView == null ) return;

			dataView.SortDescriptions.Clear( );
			SortDescription sd = new SortDescription( sortBy, direction );
			dataView.SortDescriptions.Add( sd );
			dataView.Refresh( );
		}

		private void SetAdorner( GridViewColumnHeader header, ListSortDirection direction )
		{
			_curSortCol = header;
			_curAdorner = new SortAdorner( _curSortCol, direction, AdornerBrushColor );
      
			AdornerLayer.GetAdornerLayer( _curSortCol ).Add( _curAdorner );
		}

		#region Private Fields

		private event PreviewSortCompletedEventHandler _previewSortCompletedEvent;
		private event SortCompletedEventHandler _sortCompletedEvent;

		#endregion Private Fields
	}

	public class SortAdorner : Adorner
	{
		private readonly static Geometry _ascGeometry = Geometry.Parse( "M 0,0 L 10,0 L 5,5 Z" );
		private readonly static Geometry _descGeometry = Geometry.Parse( "M 0,5 L 10,5 L 5,0 Z" );

		public ListSortDirection Direction { get; private set; }
		public string AdornerBrushColor { get; private set; }

		public SortAdorner( UIElement element, ListSortDirection dir, string adornerBrushColor )
			: base( element )
		{
			Direction = dir;
			AdornerBrushColor = adornerBrushColor;
		}

		protected override void OnRender( DrawingContext drawingContext )
		{
			base.OnRender( drawingContext );

			if( AdornedElement.RenderSize.Width < 20 )
			{
				return;
			}

			drawingContext.PushTransform(
				new TranslateTransform( AdornedElement.RenderSize.Width - 15, ( AdornedElement.RenderSize.Height - 5 ) / 2 ) );

			Color color = ( Color ) ColorConverter.ConvertFromString( AdornerBrushColor );
			drawingContext.DrawGeometry( new SolidColorBrush( color ), null, Direction == ListSortDirection.Ascending ? _ascGeometry : _descGeometry );

			drawingContext.Pop( );
		}
	}
}

