namespace CG.TrayNotify.Common.Control
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;
    using System.Windows.Media.Animation;
    using System.Windows.Forms;

    #endregion Using Directives

    public class TrayNotifier : Window, INotifyPropertyChanged
    {
        public TrayNotifier( )
        {
            this.Loaded += new RoutedEventHandler( OnLoaded );
        }

        private void OnLoaded( object sender, RoutedEventArgs e )
        {
            // Set initial settings based on the current screen working area.
            SetInitialLocations( false );

            // Start the window in the Hidden state.
            DisplayState = DisplayStates.Hidden;

            // Prepare the timer for how long the window should stay open.
            _stayOpenTimer = new DispatcherTimer( );
            _stayOpenTimer.Interval = TimeSpan.FromMilliseconds( _stayOpenMilliseconds );
            _stayOpenTimer.Tick += new EventHandler( stayOpenTimer_Elapsed );

            // Prepare the animation to change the Top property.
            _animation = new DoubleAnimation( );
            Storyboard.SetTargetProperty( _animation, new PropertyPath( Window.TopProperty ) );
            _storyboard = new Storyboard( );
            _storyboard.Children.Add( _animation );
            _storyboard.FillBehavior = FillBehavior.Stop;

            // Create the event handlers for when the animation finishes.
            _arrivedHidden = new EventHandler( Storyboard_ArrivedHidden );
            _arrivedOpened = new EventHandler( Storyboard_ArrivedOpened );
        }

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );

            // For lack of a better way, bring the notifier window to the top whenever
            // Top changes.  Let me know if you have a better way.
            if ( e.Property.Name == "Top" )
            {
                if ( ( ( double ) e.NewValue != ( double) e.OldValue )
                    && ( ( double ) e.OldValue != _hiddenTop ) )
                {
                    BringToTop( );
                }
            }
        }

        /// <summary>
        /// The time the TaskbarNotifier window should take to open in milliseconds.
        /// </summary>
        public int OpeningMilliseconds
        {
            get { return _openingMilliseconds; }
            set
            {
                _openingMilliseconds = value;
                OnPropertyChanged( "OpeningMilliseconds" );
            }
        }

        /// <summary>
        /// The time the TaskbarNotifier window should take to hide in milliseconds.
        /// </summary>
        public int HidingMilliseconds
        {
            get { return _hidingMilliseconds; }
            set
            {
                _hidingMilliseconds = value;
                OnPropertyChanged( "HidingMilliseconds" );
            }
        }


        /// <summary>
        /// The time the TaskbarNotifier window should stay open in milliseconds.
        /// </summary>
        public int StayOpenMilliseconds
        {
            get { return _stayOpenMilliseconds; }
            set
            {
                _stayOpenMilliseconds = value;
                
                if ( _stayOpenTimer != null)
                    _stayOpenTimer.Interval = TimeSpan.FromMilliseconds( _stayOpenMilliseconds );
                
                OnPropertyChanged( "StayOpenMilliseconds" );
            }
        }

        /// <summary>
        /// The space, if any, between the left side of the TaskNotifer window and the right side of the screen.
        /// </summary>
        public int LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value;
                
                OnPropertyChanged( "LeftOffset" );
            }
        }

        /// <summary>
        /// The current DisplayState
        /// </summary>
        protected DisplayStates DisplayState
        {
            get
            {
                return _displayState;
            }
            set
            {
                if ( value != _displayState )
                {
                    _displayState = value;

                    // Handle the new state.
                    OnDisplayStateChanged( );
                }
            }
        }

        private void SetInitialLocations( bool showOpened )
        {
            // Determine screen working area.
            System.Drawing.Rectangle workingArea 
                = new System.Drawing.Rectangle( ( int ) Left
                                                , ( int ) Top
                                                , ( int ) ActualWidth
                                                , ( int ) ActualHeight );

            workingArea = Screen.GetWorkingArea( workingArea );

            // Initialize the window location to the bottom right corner.
            Left = workingArea.Right - ActualWidth - _leftOffset;

            // Set the opened and hidden locations.
            _hiddenTop = workingArea.Bottom;
            _openedTop = workingArea.Bottom - this.ActualHeight;

            // Set Top based on whether opened or hidden is desired
            Top = ( showOpened ) ? _openedTop : _hiddenTop;
        }

        private void BringToTop()
        {
            // Bring this window to the top without making it active.
            Topmost = true;
            Topmost = false;
            Visibility = Visibility.Visible;
        }

        private void OnDisplayStateChanged()
        {
            // The display state has changed.

            // Unless the stortboard as already been created, nothing can be done yet.
            if ( _storyboard == null )
                return;

            // Stop the current animation.
            _storyboard.Stop( this );

            // Since the storyboard is reused for opening and closing, both possible
            // completed event handlers need to be removed.  It is not a problem if
            // either of them was not previously set.
            _storyboard.Completed -= _arrivedHidden;
            _storyboard.Completed -= _arrivedOpened;

            if ( _displayState != DisplayStates.Hidden )
            {
                // Unless the window has just arrived at the hidden state, it must be
                // moving, and should be shown.
                BringToTop();
            }

            if ( _displayState == DisplayStates.Opened )
            {
                // The window has just arrived at the opened state.

                // Because the inital settings of this TaskNotifier depend on the screen's working area,
                // it is best to reset these occasionally in case the screen size has been adjusted.
                SetInitialLocations( true );

                if ( !IsMouseOver )
                {
                    // The mouse is not within the window, so start the countdown to hide it.
                    _stayOpenTimer.Stop();
                    _stayOpenTimer.Start();
                }
            }
            else if ( _displayState == DisplayStates.Opening )
            {
                // The window should start opening.

                // Make the window visible.
                Visibility = Visibility.Visible;
                BringToTop( );

                // Because the window may already be partially open, the rate at which
                // it opens may be a fraction of the normal rate.
                // This must be calculated.
                int milliseconds = this.CalculateMillseconds( _openingMilliseconds, _openedTop );

                if ( milliseconds < 1 )
                {
                    // This window must already be open.
                    DisplayState = DisplayStates.Opened;
                    return;
                }

                // Reconfigure the animation.
                _animation.To = _openedTop;
                _animation.Duration = new Duration( new TimeSpan( 0, 0, 0, 0, milliseconds ) );

                // Set the specific completed event handler.
                _storyboard.Completed += _arrivedOpened;

                // Start the animation.
                _storyboard.Begin( this, true );
            }
            else if ( _displayState == DisplayStates.Hiding )
            {
                // The window should start hiding.

                // Because the window may already be partially hidden, the rate at which
                // it hides may be a fraction of the normal rate.
                // This must be calculated.
                int milliseconds = CalculateMillseconds( _hidingMilliseconds, _hiddenTop );

                if ( milliseconds < 1 )
                {
                    // This window must already be hidden.
                    DisplayState = DisplayStates.Hidden;
                    return;
                }

                // Reconfigure the animation.
                _animation.To = _hiddenTop;
                _animation.Duration = new Duration( new TimeSpan( 0, 0, 0, 0, milliseconds ) );

                // Set the specific completed event handler.
                _storyboard.Completed += _arrivedHidden;

                // Start the animation.
                _storyboard.Begin(this, true);
            }
            else if ( _displayState == DisplayStates.Hidden )
            {
                // Ensure the window is in the hidden position.
                SetInitialLocations( false );

                // Hide the window.
                this.Visibility = Visibility.Hidden;
            }
        }

        private int CalculateMillseconds(int totalMillsecondsNormally, double destination)
        {
            if ( Top == destination )
            {
                // The window is already at its destination.  Nothing to do.
                return 0;
            }

            double distanceRemaining = Math.Abs( Top - destination );
            double percentDone = distanceRemaining / ActualHeight;

            // Determine the percentage of normal milliseconds that are actually required.
            return ( int )( totalMillsecondsNormally * percentDone );
        }

        protected virtual void Storyboard_ArrivedHidden(object sender, EventArgs e)
        {
            // Setting the display state will result in any needed actions.
            DisplayState = DisplayStates.Hidden;
        }

        protected virtual void Storyboard_ArrivedOpened(object sender, EventArgs e)
        {
            // Setting the display state will result in any needed actions.
            DisplayState = DisplayStates.Opened;
        }

        private void stayOpenTimer_Elapsed(Object sender, EventArgs args)
        {
            // Stop the timer because this should not be an ongoing event.
            _stayOpenTimer.Stop();

            if ( !this.IsMouseOver )
            {
                // Only start closing the window if the mouse is not over it.
                DisplayState = DisplayStates.Hiding;
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if ( DisplayState == DisplayStates.Opened )
            {
                // When the user mouses over and the window is already open, it should stay open.
                // Stop the timer that would have otherwise hidden it.
                _stayOpenTimer.Stop();
            }
            else if ( ( DisplayState == DisplayStates.Hidden )
                || ( DisplayState == DisplayStates.Hiding ) )
            {
                // When the user mouses over and the window is hidden or hiding, it should open. 
                DisplayState = DisplayStates.Opening;
            }

            base.OnMouseEnter( e );
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if ( DisplayState == DisplayStates.Opened )
            {
                // When the user moves the mouse out of the window, the timer to hide the window
                // should be started.
                _stayOpenTimer.Stop();
                _stayOpenTimer.Start();
            }

            base.OnMouseEnter( e );
        }

        public void Notify( )
        {
            if ( Visibility != Visibility.Visible )
            {
                Visibility = Visibility.Visible;
            }
            if ( DisplayState == DisplayStates.Opened )
            {
                // The window is already open, and should now remain open for another count.
                _stayOpenTimer.Stop();
                _stayOpenTimer.Start();
            }
            else
            {
                DisplayState = DisplayStates.Opening;
            }
        }

        /// <summary>
        /// Force the window to immediately move to the hidden state.
        /// </summary>
        public void ForceHidden()
        {
            DisplayState = DisplayStates.Hidden;
        }

        #region Protected Methods

        protected override void OnInitialized( EventArgs e )
        {
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;

            base.OnInitialized( e );
        }

        #endregion Protected Methods


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }

        #endregion  INotifyPropertyChanged Members


        #region Private Fields

        private int _openingMilliseconds = 1000;


        protected enum DisplayStates { Opening, Opened, Hiding, Hidden }

        private DispatcherTimer _stayOpenTimer = null;
        private Storyboard _storyboard;
        private DoubleAnimation _animation;

        private double _hiddenTop;
        private double _openedTop;
        private EventHandler _arrivedHidden;
        private EventHandler _arrivedOpened;
        private DisplayStates _displayState;

        private int _hidingMilliseconds = 1000;
        private int _stayOpenMilliseconds = 1000;

        private object _lock = new object( );

        private int _leftOffset = 0;

        #endregion Private Fields
    }
}
