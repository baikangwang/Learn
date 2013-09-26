
namespace CG.TrayNotify.Common.Control
{
    #region Using Directives

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Drawing = System.Drawing;
    using Forms = System.Windows.Forms;

    #endregion Using Directives

    public enum BalloonTipIconTypes
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }


//    [ContentProperty( "Text" )]
//    [DefaultEvent( "MouseDoubleClick" )]
    public class NotifyIcon : FrameworkElement,IDisposable
    {

        #region Dependency Properties

        public static readonly DependencyProperty BalloonTipIconTypeProperty =
            DependencyProperty.Register( "BalloonTipIconType", typeof( BalloonTipIconTypes ), typeof( NotifyIcon ) );

        public BalloonTipIconTypes BalloonTipIconType
        {
            get { return ( BalloonTipIconTypes ) GetValue( BalloonTipIconTypeProperty ); }
            set { SetValue( BalloonTipIconTypeProperty, value ); }
        }

        public static readonly DependencyProperty BalloonTipTextProperty =
            DependencyProperty.Register( "BalloonTipText", typeof( string ), typeof( NotifyIcon ) );

        public string BalloonTipText
        {
            get { return ( string ) GetValue( BalloonTipTextProperty ); }
            set { SetValue( BalloonTipTextProperty, value ); }
        }

        public static readonly DependencyProperty BalloonTipTitleProperty =
            DependencyProperty.Register( "BalloonTipTitle", typeof( string ), typeof( NotifyIcon ) );

        public string BalloonTipTitle
        {
            get { return ( string ) GetValue( BalloonTipTitleProperty ); }
            set { SetValue( BalloonTipTitleProperty, value ); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register( "Icon", typeof( ImageSource ), typeof( NotifyIcon ) );

        public ImageSource Icon
        {
            get { return ( ImageSource ) GetValue( IconProperty ); }
            set { SetValue( IconProperty, value ); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register( "Text", typeof( string ), typeof( NotifyIcon ) );

        public string Text
        {
            get { return ( string ) GetValue( TextProperty ); }
            set { SetValue( TextProperty, value ); }
        }

        #endregion Dependency Properties

        #region Public Events

        public static readonly RoutedEvent MouseDownEvent = EventManager.RegisterRoutedEvent(
            "MouseDown", RoutingStrategy.Bubble, typeof( MouseButtonEventHandler ), typeof( NotifyIcon ) );

        public event MouseButtonEventHandler MouseDown 
        {
            add { AddHandler( MouseDownEvent, value ); }
            remove { RemoveHandler( MouseDownEvent, value ); }
        }

        public static readonly RoutedEvent MouseUpEvent = EventManager.RegisterRoutedEvent(
            "MouseUp", RoutingStrategy.Bubble, typeof( MouseButtonEventHandler ), typeof( NotifyIcon ) );

        public event MouseButtonEventHandler MouseUp
        {
            add { AddHandler( MouseUpEvent, value ); }
            remove { RemoveHandler( MouseUpEvent, value ); }
        }

        public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
            "MouseClick", RoutingStrategy.Bubble, typeof( MouseButtonEventHandler ), typeof( NotifyIcon ) );

        public event MouseButtonEventHandler MouseClick
        {
            add { AddHandler( MouseClickEvent, value ); }
            remove { RemoveHandler( MouseClickEvent, value ); }
        }

        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "MouseDoubleClick", RoutingStrategy.Bubble, typeof( MouseButtonEventHandler ), typeof( NotifyIcon ) );

        public event MouseButtonEventHandler MouseDoubleClick
        {
            add { AddHandler( MouseDoubleClickEvent, value ); }
            remove { RemoveHandler( MouseDoubleClickEvent, value ); }
        }

        #endregion Public Events

        #region Protected Methods

        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

			_notifyIcon.Text = Text;
			_notifyIcon.Icon = FromImageSource(Icon);
            _notifyIcon.Visible = Visibility == Visibility.Visible;

			_notifyIcon.MouseDown += OnMouseDown;
			_notifyIcon.MouseUp += OnMouseUp;
			_notifyIcon.MouseClick += OnMouseClick;
			_notifyIcon.MouseDoubleClick += OnMouseDoubleClick;

            Dispatcher.ShutdownStarted += new EventHandler( OnDispatcherShutdownStarted );

            _isInitialized = true;
        }

        #endregion Protected Methods

        #region Private Methods

        private MouseButton ConvertMouseButton( Forms.MouseButtons button )
        {
            MouseButton btn = MouseButton.Left;

			switch ( button )
			{
            case Forms.MouseButtons.Left: btn = MouseButton.Left; break;
			case Forms.MouseButtons.Right:  btn = MouseButton.Right; break;
            case Forms.MouseButtons.Middle: btn = MouseButton.Middle; break;
            case Forms.MouseButtons.XButton1: btn = MouseButton.XButton1; break;
            case Forms.MouseButtons.XButton2: btn = MouseButton.XButton2; break;
			}

            return btn;
		}

        private void DisplayContextMenu( )
        {
            if ( ContextMenu != null )
            {
                ContextMenuService.SetPlacement( ContextMenu, PlacementMode.MousePoint );
                ContextMenu.IsOpen = true;
            }
        }

        private static Drawing.Icon FromImageSource( ImageSource icon )
        {
            if ( icon == null )
            {
                return null;
            }
            Uri iconUri = new Uri( icon.ToString( ) );
            return new Drawing.Icon( Application.GetResourceStream( iconUri ).Stream );
        }

        private void OnDispatcherShutdownStarted( object sender, EventArgs e )
        {
            _notifyIcon.Dispose( );
        }

        #region Mouse Handlers

        private void OnMouseClick( object sender, Forms.MouseEventArgs e )
        {
            OnRaiseMouseEvent( MouseClickEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ConvertMouseButton( e.Button ) ) );
        }

        private void OnMouseDoubleClick( object sender, Forms.MouseEventArgs e )
        {
            OnRaiseMouseEvent( MouseDoubleClickEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ConvertMouseButton( e.Button ) ) );
        }

        private void OnMouseDown( object sender, Forms.MouseEventArgs e )
        {
            OnRaiseMouseEvent( MouseDownEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ConvertMouseButton( e.Button ) ) );
        }

        private void OnMouseUp( object sender, Forms.MouseEventArgs e )
        {
            if ( Forms.MouseButtons.Right == e.Button )
            {
                DisplayContextMenu( );
            }
            OnRaiseMouseEvent( MouseUpEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ConvertMouseButton( e.Button ) ) );
        }

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );

            if ( !_isInitialized ) return;
            
            switch ( e.Property.Name )
            {
            case "Icon":
                _notifyIcon.Icon = FromImageSource( Icon );
                break;
            case "Text":
                _notifyIcon.Text = Text;
                break;
            case "Visibility":
                _notifyIcon.Visible = Visibility == Visibility.Visible;
                break;
            }
        }

        private void OnRaiseMouseEvent( RoutedEvent handler, MouseButtonEventArgs e )
        {
            e.RoutedEvent = handler;
            RaiseEvent( e );
        }

        #endregion Handlers

        #endregion Private Methods

        #region Private Fields

        Forms.NotifyIcon _notifyIcon = new Forms.NotifyIcon( );

        private bool _isInitialized = false;

        #endregion Private Fields

        protected virtual void Dispose(bool all)
        {
            if (all)
            {
                _notifyIcon.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
