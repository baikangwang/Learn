using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace XMLValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://www.contoso.com/books", "contosoBooks.xsd");
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += ValidationEventHandler;

                XmlReader reader = XmlReader.Create("contosoBooks.xml", settings);
                //XmlDocument document = new XmlDocument();
                //document.Load(reader);
                while (reader.Read())
                {
                    
                }

                //ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

                // the following call to Validate succeeds.
                //document.Validate(eventHandler);

                // add a node so that the document is no longer valid
                //XPathNavigator navigator = document.CreateNavigator();
                //navigator.MoveToFollowing("price", "http://www.contoso.com/books");
                //XmlWriter writer = navigator.InsertAfter();
                //writer.WriteStartElement("anotherNode", "http://www.contoso.com/books");
                //writer.WriteEndElement();
                //writer.Close();

                // the document will now fail to successfully validate
                //document.Validate(eventHandler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error (line {1} Col {2}): {0}", e.Message,e.Exception.LineNumber,e.Exception.LinePosition);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning (line {1} Col {2}): {0}", e.Message,e.Exception.LineNumber,e.Exception.LinePosition);
                    break;
            }
        }
    }
}
