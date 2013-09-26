namespace CG.TrayNotify.Common
{
	#region Using Directives

	using System.ServiceModel;
	using System;

	#endregion Using Directives	

	public class ChannelProxy< TChannel > : IDisposable where TChannel : class
	{
		public static ChannelProxy<TChannel> Open( string config )
		{
			return new ChannelProxy< TChannel >( config );
		}

		public static ChannelProxy<TChannel> Open( InstanceContext context, string config )
		{
			return new ChannelProxy<TChannel>( context, config );
		}
		
		private ChannelProxy( string config )
		{
			ChannelFactory< TChannel > factory = new ChannelFactory< TChannel >( config );
			_channel = factory.CreateChannel( );

			( ( IClientChannel ) _channel ).Open( );
		}

		private ChannelProxy( InstanceContext context, string config )
		{
			DuplexChannelFactory<TChannel> factory = new DuplexChannelFactory<TChannel>( context, config );
			_channel = factory.CreateChannel( );

			( ( IClientChannel ) _channel ).Open( );
		}

		public TChannel  Channel { get { return _channel; } }

		private TChannel _channel = default( TChannel );

		#region IDisposable Members

		/// <summary>
		/// Finalizer
		/// </summary>
		~ChannelProxy( )
		{
			Dispose( false );
		}

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
			if( isDisposing )
			{
				if( null != _channel )
				{
                    try
                    {
                        ( ( IClientChannel ) _channel ).Close( );
                    }
                    catch (CommunicationException )
                    {
                        ( ( IClientChannel ) _channel ).Abort( );
                    }
                    catch (TimeoutException )
                    {
                        ( ( IClientChannel ) _channel ).Abort( );
                    }
                    catch ( Exception )
                    {
                        ( ( IClientChannel ) _channel ).Abort( );
                        throw;
                    }
				}
			}
		}

		#endregion IDisposable Members
	}
}
