using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

namespace MindHelper.XML
{
    /// <summary>
    /// <para>Reads and Updates XML File in a configuration style format.</para>
    /// <para>Example Format.</para>
    /// <para>-- Root Tag</para>
    /// <para>  -- Namespace Tag</para>
    /// <para>  -- Property Tag</para>
    /// <para>  -- Value Tag</para>
    /// <para> -- Another Property Tag</para>
    /// <para>   -- Value Tag</para>
    /// <para>   -- Value Tag</para>
    /// </summary>
    public class Configuration
    {
        private Dictionary<string, XmlDocument> _Namespaces;
        private const string _XMLFileExtension = "xml";
        private const string _InnerNamespace = "BaseNamespace";
        private const string _InnerProperty = "Property";
        private const string _InnerValue = "Value";
        private const string _DoubleSlash = "//";

        #region LOAD

        /// <summary>
        /// Class Constructor:
        /// </summary>
        public Configuration()
        {
            _Namespaces = new Dictionary<string, XmlDocument>();
        }

        /// <summary>
        /// Loads an XML Configuration File
        /// </summary>
        /// <param name="ConfigurationLocation">XML Configuration File Location.</param>
        /// <param name="LoadInnerValue">Load All of XML Configuration File Location contained in the ConfigurationLocation File specified.</param>
        public void Load(string ConfigurationFileLocation, bool LoadInnerValue = false)
        {
            Load(new string[] { ConfigurationFileLocation }, LoadInnerValue);
        }

        /// <summary>
        /// Loads an XML Configuration File
        /// </summary>
        /// <param name="ConfigurationLocation">List of XML Configuration File Location.</param>
        /// <param name="LoadInnerValue">Load All of XML Configuration File Location contained in the ConfigurationLocation File specified.</param>
        public void Load(string[] ConfigurationFileLocations, bool LoadInnerValue = false)
        {
            if (LoadInnerValue)
            {
                InnerLoad(ConfigurationFileLocations);
            }
            else
            {
                NormalLoad(ConfigurationFileLocations);
            }
        }

        /// <summary>
        /// Get and Loads All the XML Configuration File Locations specified in each Configuration specified.
        /// </summary>
        /// <param name="ConfigurationLocations">List of XML Configuration File Location.</param>
        private void InnerLoad(string[] ConfigurationFileLocations)
        {
            try
            {
                List<string> ConfigurationFileLocationList = new List<string>();
                XmlDocument XmlDocument = new XmlDocument();

                foreach (string ConfigurationFileLocation in ConfigurationFileLocations)
                {
                    if (isXMLFile(ConfigurationFileLocation)) // will only load XML Files
                    {
                        XmlDocument.Load(ConfigurationFileLocation);
                        AddToNamespaceDictionary(_InnerNamespace, XmlDocument);
                        ConfigurationFileLocationList.AddRange(GetValues(_InnerNamespace, _InnerProperty).Cast<string>());
                    }
                }

                NormalLoad(ConfigurationFileLocationList.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all available namespaces.
        /// </summary>
        /// <param name="ConfigurationFileLocations">List of XML Configuration File Location.</param>
        private void NormalLoad(string[] ConfigurationFileLocations)
        {
            try
            {
                foreach (string ConfigurationFileLocation in ConfigurationFileLocations)
                {
                    if (isXMLFile(ConfigurationFileLocation)) // will only load XML Files
                    {
                        XmlDocument XmlDocument = new XmlDocument();
                        XmlDocument.Load(ConfigurationFileLocation);

                        // gets a list of namespaces
                        // look in the last child node since last child node contains the root tag or the main body of the xml file.
                        foreach (XmlNode XMLNode in XmlDocument.LastChild.ChildNodes)
                        {
                           AddToNamespaceDictionary(XMLNode.Name, XmlDocument);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SELECT
        
        /// <summary>
        /// Gets a value in Value tag in a form of namespace and property identifier.
        /// </summary>
        /// <param name="Namespace">Namespace to be looked on</param>
        /// <param name="Property">Property to be looked on.</param>
        /// <param name="Type">Return Type.</param>
        /// <returns>Return value.</returns>
        public object GetValue(string Namespace, string Property, Converter.Types Type = Converter.Types.String)
        {
            object value = null;

            try
            {
                value = Converter.ConvertTo(GetProperty(Namespace, Property).FirstChild.InnerText, Type);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return value;
        }

        /// <summary>
        /// Gets all values in Value tag in a form of namespace and property identifier.
        /// </summary>
        /// <param name="Namespace">Namespace to be looked on</param>
        /// <param name="Property">Property to be looked on.</param>
        /// <param name="Type">Return Type.</param>
        /// <returns>Return List of values.</returns>
        public List<object> GetValues(string Namespace, string Property, Converter.Types Type = Converter.Types.String)
        {
            List<object> ValueList = new List<object>();
            XmlNodeList ChildNodes = null;

            try
            {
                if (Property == _InnerProperty)
                {
                    ChildNodes = _Namespaces[Namespace].SelectNodes(_DoubleSlash + _InnerValue);
                }
                else
                {
                    ChildNodes = GetProperty(Namespace, Property).ChildNodes;
                }

                foreach (XmlNode ChildNode in ChildNodes)
                {
                    ValueList.Add(Converter.ConvertTo(ChildNode.InnerText, Type));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return ValueList;
        }

        #endregion

        #region UTILITY

        /// <summary>
        /// Checks if the file specified is an XML.
        /// </summary>
        /// <param name="XMLFileLocation">XML File Location.</param>
        /// <returns>Return True is File is an XML File otherwise return False.</returns>
        public bool isXMLFile(string XMLFileLocation)
        {
            return (XMLFileLocation.Split('.').Last().ToLower() == _XMLFileExtension);
        }

        /// <summary>
        /// Adds new namespaces to the _Namespaces dictionary, replaces the old one if existing.
        /// </summary>
        /// <param name="Namespace">Namespace string.</param>
        /// <param name="XmlDocument">Xml Document.</param>
        private void AddToNamespaceDictionary(string Namespace, XmlDocument XmlDocument)
        {
            if (_Namespaces.ContainsKey(Namespace))
            {
                _Namespaces[Namespace] = XmlDocument;
            }
            else
            {
                _Namespaces.Add(Namespace, XmlDocument);
            }
        }

        /// <summary>
        /// Gets the property specified in the namespace provided.
        /// </summary>
        /// <param name="Namespace">Namespace to be looked on</param>
        /// <param name="Property">Property to be looked on.</param>
        /// <returns></returns>
        private XmlNode GetProperty(string Namespace, string Property)
        {
            return _Namespaces[Namespace].SelectSingleNode(_DoubleSlash + Property);
        }

        #endregion
    }
}
