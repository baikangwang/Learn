namespace CG.TrayNotify.Common
{
	#region Using Directives

	using System;
	using System.Configuration;
	using System.ServiceModel;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Diagnostics;
    using System.IO;

	#endregion Using Directives

	public class WcfServiceHost
	{
		public static WcfServiceHost Create( EventLog eventLog )
		{
            return new WcfServiceHost( ) { EventLog = eventLog };
		}
        
        /// <summary>
        /// Hosts the wcf services in a console app for debugging.
        /// </summary>
        /// <param name="eventLog"></param>
        public static void RunServiceAsConsoleApp( string serviceAppName, EventLog eventLog )
        {
            Console.WriteLine( String.Format( "Running {0} Service.\n\n", serviceAppName ) );

            var serviceHost = WcfServiceHost.Create( eventLog );
            serviceHost.Start( );

            Console.WriteLine( String.Format( "\n{0} WCF services have successfully started!\n\nPress ENTER to exit.", serviceAppName ) );
            Console.Read(); //ReadKey( false );
        }

		/// <summary>
		/// Starts WCF Services
		/// </summary>
		public void Start( )
		{
			Stop( );

            Console.WriteLine( "Loading WCF Services..." );

			_serviceHostList = new List<ServiceHost>( );

			string appFolder = GetAppFolder( );

			WcfServices wcfServices = WcfServices.Create( GetAppConfig( ) );

			// Load WCF service assemblies and create a service host for each service
			foreach( var wcfService in wcfServices.ServiceModel.Services )
			{
				// If the assembly is duplicated, it is only loaded once
                var assemblyName = Path.Combine( appFolder, wcfService.AssemblyName );

				var assembly = Assembly.LoadFile( assemblyName );

				// Get the type from the class name
				var type = assembly.GetType( wcfService.ClassName );

				var host = new ServiceHost( type );
				host.Faulted += new EventHandler( OnServiceHostFaulted );

				// Create a service host based on the type and add it to our list
				_serviceHostList.Add( host );
			}

            Console.WriteLine( "Starting WCF Services...\n" );

			// Start up each WCF ServiceHost in our list
			foreach( ServiceHost serviceHost in _serviceHostList )
			{
                Console.Write( String.Format( "\t{0}...", serviceHost.Description.Name ) );

				serviceHost.Open( );

                Console.WriteLine( String.Format( " {0}", serviceHost.State ) );
			}
		}

		void OnServiceHostFaulted( object sender, EventArgs e )
		{
            foreach ( var serviceHost in _serviceHostList )
            {
                if ( serviceHost.State == CommunicationState.Faulted )
                {
                    EventLog.WriteEntry( 
                        String.Format( "The {0} service has faulted."
                        , serviceHost.Description.Name )
                        , EventLogEntryType.Error );
                }
            }
		}

		/// <summary>
		/// Stops WCF Services
		/// </summary>
		public void Stop( )
		{
			if( null == _serviceHostList ) return;

			// Stop each open WCF service in our list
			foreach( var serviceHost in _serviceHostList )
			{
				if( serviceHost.State != CommunicationState.Closed )
				{
					serviceHost.Close( );
				}
			}

			// Clear the list, so we're clean when we call OnStart again.
			_serviceHostList.Clear( );

			_serviceHostList = null;
		}

        /// <summary>
        /// Retrieves the location of the service application
        /// </summary>
        /// <returns></returns>
		private string GetAppFolder( )
		{
            string appFolder = GetAppConfig( );

			return appFolder.Substring( 0, appFolder.LastIndexOf( '\\' ) );
		}

        /// <summary>
        /// Retrieves the app.config file
        /// </summary>
        /// <returns></returns>
		private string GetAppConfig( )
		{
            return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        #region Private Fields/Properties

        /// <summary>
		/// List used to hold ServiceHost instances
		/// </summary>
		private List<ServiceHost> _serviceHostList = null;

        /// <summary>
        /// Event log
        /// </summary>
		private EventLog EventLog { get; set; }

        #endregion Private Fields/Properties
    }
}
