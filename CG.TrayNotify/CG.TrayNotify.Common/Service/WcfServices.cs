namespace CG.TrayNotify.Common
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using System;

    #endregion Using Directives

    /// <remarks/>
    [Serializable]
    [XmlType(AnonymousType=true)]
    [XmlRoot( ElementName="configuration", Namespace="", IsNullable=false)]
    public partial class WcfServices
	{
		/// <summary/>
		public static WcfServices Create( string configFile )
		{
            var serializer = new XmlSerializer( typeof( WcfServices ) );

			using( var reader = XmlReader.Create( configFile ) )
			{
                return ( WcfServices ) serializer.Deserialize( reader );
			}
		}

        /// <remarks/>
        [XmlElement("system.serviceModel")]
        public ServiceModel ServiceModel { get { return _serviceModel; } set { _serviceModel = value; } }

		private ServiceModel _serviceModel = null;
    }
    
    /// <remarks/>
    [Serializable]
    [XmlType(AnonymousType=true)]
    public partial class ServiceModel
	{
        /// <remarks/>
		[XmlArray( "services", IsNullable = false )]
		[XmlArrayItem( "service", IsNullable = false )]
		public Service[] Services
		{
            get
			{
                return _serviceList.ToArray( );
            }
            set
			{
				if( null == _serviceList )
				{
					_serviceList = new List<Service>( );
				}
				_serviceList.AddRange( value );
            }
        }

		private List<Service> _serviceList = new List<Service>( );
    }
    
    /// <remarks/>
    [Serializable()]
    [XmlType(AnonymousType=true)]
    public partial class Service
	{
        /// <remarks/>
        [XmlAttribute( AttributeName="name" )]
		public string Name 
        {
            get
            {
                return _name;
            }
            
            set
            {
                // The service name parameter is ':' delimited with the following format
                // name="<assemblyName>:<className>"
                if ( !String.IsNullOrEmpty( value ) )
                {
                    _name = value;

                    string [] components = _name.Split( new [ ] { ':' } );

                    switch( components.Length )
                    {
                    case 2:
                        // Assembly name and class name specified.  This allows us to have the class located in a differently
                        // named assembly.
                        _assemblyName = components [ 0 ];
                        _className = components [ 1 ];
                        break;
                    default:
                        Console.WriteLine( "Invalid appConfig <system.serviceModel\\services\\service> node name param."
                            + "The name param must follow the name=\"<assemblyName>:<className\" naming convention\n" );
                        break;
                    }
                }
            }
        }

        [XmlIgnore]
        public string AssemblyName
        {
            get
            {
                // Append .Dll if required
                return _assemblyName.IndexOf( ".dll", StringComparison.CurrentCultureIgnoreCase ) == -1 ? _assemblyName + ".dll" : _assemblyName;
            }
            
            set { _assemblyName = value; } }

        [XmlIgnore]
        public string ClassName { get { return _className; } set { _className = value; } }

        private string _assemblyName;
        private string _className;
        private string _name;
    }
}
