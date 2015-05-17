using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MindHelper.Reflection
{
    public class ClassReflector
    {
        private Type _ClassType;
        private Dictionary<string, object> _FieldValueConstantList;
        private Dictionary<string, string> _ValueConstantFieldList;
        private Dictionary<string, Type> _FieldList;
        private Dictionary<string, Type> _PropertyList;
        private Dictionary<string, Dictionary<string, Type>> _MethodList;
        private Dictionary<string, Dictionary<string, int>> _EnumList;
        private BindingFlags OriginalBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        private BindingFlags ModifiedBindingFlags;
        private object _InstantiatedClass;

        #region LOAD/CLASS

        /// <summary>
        /// Class Constructor:
        /// </summary>
        public ClassReflector() { }

        /// <summary>
        /// Class Constructor:
        /// </summary>
        /// <param name="ClassType">Type to be Loaded.</param>
        public ClassReflector(Type ClassType)
        {
            LoadClass(ClassType);
        }

        /// <summary>
        /// Loads a Class, specifying the DLL location to be looked for.
        /// </summary>
        /// <param name="DLLLocation">DLL location to be loaded.</param>
        /// <param name="ClassFullName">Class Type to be invoked/used.</param>
        public void LoadClass(string DLLLocation, string ClassFullName)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Assembly DynamicLinkLibrary = Assembly.LoadFile(DLLLocation);
            _InstantiatedClass = DynamicLinkLibrary.CreateInstance(ClassFullName);
            LoadClass(_InstantiatedClass.GetType(), false);
        }
       
        /// <summary>
        /// Loads a Class through Type.
        /// </summary>
        /// <param name="ClassType">Class Type.</param>
        public void LoadClass(Type ClassType, bool DoInstantiate = true )
        {
            ClearFields();
            _ClassType = ClassType;
            if ((!_ClassType.IsSealed && !_ClassType.IsAbstract) && DoInstantiate)
            {
                _InstantiatedClass = Activator.CreateInstance(_ClassType);
            }
        }

        /// <summary>
        /// Gets the intansiated Class.
        /// </summary>
        /// <returns>Instantiated Class.</returns>
        public object GetInstantiatedClass()
        {
            return _InstantiatedClass;
        }

        /// <summary>
        /// Clears the Class Fields when loaded with new Class.
        /// </summary>
        private void ClearFields()
        {
            _ClassType = null;
            _FieldValueConstantList = null;
            _ValueConstantFieldList = null;
            _FieldList = null;
            _PropertyList = null;
            _MethodList = null;
            _EnumList = null;
        }

        /// <summary>
        /// Resolve missing assemblies.
        /// </summary>
        /// <param name="sender">Object who initiates this event.</param>
        /// <param name="args">ResolveEventArgs</param>
        /// <returns>Assembly to be added as referenced.</returns>
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFile(string.Format("{0}\\{1}.dll",
                                    args.RequestingAssembly.Location.Substring(0, args.RequestingAssembly.Location.LastIndexOf('\\')),
                                    args.Name.Split(',')[0]));
        }

        #endregion

        #region FIELDLIST

        /// <summary>
        /// Gets Class's Constant and Readonly Fields and its Values
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a dictionary of Constant and Readonly Fields and its values.</returns>
        public Dictionary<string, object> GetFieldValueConstantList(bool IncludeNonPublic = false)
        {
            if (_FieldValueConstantList == null)
            {
                GetConstantFields(IncludeNonPublic);
            }
            return _FieldValueConstantList;
        }

        /// <summary>
        /// Gets Class's Constant and Readonly Fields and its Values, where its values are being treated as keys.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a dictionary of Constant and Readonly Fields and its values.</returns>
        public Dictionary<string, string> GetValueFieldConstantList(bool IncludeNonPublic = false)
        {
            if (_ValueConstantFieldList == null)
            {
                GetConstantFields(IncludeNonPublic);
            }
            return _ValueConstantFieldList;
        }

        /// <summary>
        /// Gets Class's Fields and its Type.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a Dictionary of Fields and its Type.</returns>
        public Dictionary<string, Type> GetFieldList(bool IncludeNonPublic = false)
        {
            if (_FieldList == null)
            {
                GetNonConstantFields(IncludeNonPublic);
            }
            return _FieldList;
        }

        /// <summary>
        /// Sets the specified field with the specified value.
        /// </summary>
        /// <param name="FieldName">Field Name to be modified.</param>
        /// <param name="Data">Data to be stored on the specified Field Name.</param>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>The instantiated class object with the updated data.</returns>
        public object SetField(string FieldName, object Data, object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                _ClassType.GetField(FieldName, OriginalBindingFlags | BindingFlags.NonPublic).SetValue(_InstantiatedClass, Data);
            }
            else
            {
                _ClassType.GetField(FieldName, OriginalBindingFlags | BindingFlags.NonPublic).SetValue(InstantiatedClass, Data);
            }            

            return InstantiatedClass;
        }

        /// <summary>
        /// Gets the data of the specified field.
        /// </summary>
        /// <param name="FieldName">Field Name to retrieve data.</param>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>The data of the specified Field Name.</returns>
        public object GetFieldData(string FieldName, object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                return _ClassType.GetField(FieldName, OriginalBindingFlags).GetValue(_InstantiatedClass);
            }
            else
            {
                return _ClassType.GetField(FieldName, OriginalBindingFlags).GetValue(InstantiatedClass);
            }
        }

        /// <summary>
        /// Gets a collection of Class's Fields in dictionary format with its data.
        /// </summary>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>a Dictionary of Field name with its data.</returns>
        public Dictionary<string, object> GetFieldValueList(object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                InstantiatedClass = _InstantiatedClass;
            }
            
            Dictionary<string, object> FieldValueList = new Dictionary<string, object>();
            FieldInfo[] Fields = _ClassType.GetFields(OriginalBindingFlags);

            foreach (FieldInfo FI in Fields)
            {
                FieldValueList.Add(FI.Name, FI.GetValue(InstantiatedClass));
            }

            return FieldValueList;
        }

        /// <summary>
        /// Populates the _FieldValueConstantList and _ValueConstantFieldList collection with Constant and Readonly Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        private void GetConstantFields(bool IncludeNonPublic = false)
        {
            if (IncludeNonPublic) { ModifiedBindingFlags = OriginalBindingFlags | System.Reflection.BindingFlags.NonPublic; }
            else { ModifiedBindingFlags = OriginalBindingFlags; }
            FieldInfo[] Fields = _ClassType.GetFields(ModifiedBindingFlags).Where(FI => FI.IsLiteral || FI.IsInitOnly).ToArray<FieldInfo>();
            _FieldValueConstantList = new Dictionary<string, object>();
            _ValueConstantFieldList = new Dictionary<string, string>();
            foreach (FieldInfo FI in Fields)
            {
                _FieldValueConstantList.Add(FI.Name, FI.GetValue(null));
                _ValueConstantFieldList.Add(FI.GetValue(null).ToString(), FI.Name);
            }
        }

        /// <summary>
        /// Populates the _FieldList collection with all Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        private void GetNonConstantFields(bool IncludeNonPublic = false)
        {
            if (IncludeNonPublic) { ModifiedBindingFlags = OriginalBindingFlags | System.Reflection.BindingFlags.NonPublic; }
            else { ModifiedBindingFlags = OriginalBindingFlags; }
            FieldInfo[] Fields = _ClassType.GetFields(ModifiedBindingFlags).Where(FI => !FI.IsLiteral || !FI.IsInitOnly).ToArray<FieldInfo>();
            _FieldList = new Dictionary<string, Type>();
            foreach (FieldInfo FI in Fields)
            {
                _FieldList.Add(FI.Name, FI.FieldType);
            }
        }

        #endregion

        #region PROPERTYLIST

        /// <summary>
        /// Gets Class's Propety Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a Dictionary of Property Fields and its Type.</returns>
        public Dictionary<string, Type> GetPropertyList(bool IncludeNonPublic = false)
        {
            if (_PropertyList == null)
            {
                GetPropertyFields(IncludeNonPublic);
            }
            return _PropertyList;
        }

        /// <summary>
        /// Sets the specified property with the specified value.
        /// </summary>
        /// <param name="PropertyName">Property Name to be modified.</param>
        /// <param name="Data">Data to be stored on the specified Property Name.</param>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>The instantiated class object with the updated data.</returns>
        public object SetPropertyData(string PropertyName, object Data, object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                _ClassType.GetProperty(PropertyName, OriginalBindingFlags | BindingFlags.NonPublic).SetValue(_InstantiatedClass, Data, null);
            }
            else
            {
                _ClassType.GetProperty(PropertyName, OriginalBindingFlags | BindingFlags.NonPublic).SetValue(InstantiatedClass, Data, null);
            }

            return InstantiatedClass;
        }

        /// <summary>
        /// Gets the data of the specified property.
        /// </summary>
        /// <param name="PropertyName">Property Name to retrieve data.</param>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>The data of the specified Property Name.</returns>
        public object GetPropertyData(string PropertyName, object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                return _ClassType.GetProperty(PropertyName, OriginalBindingFlags).GetValue(_InstantiatedClass, null);
            }
            else
            {
                return _ClassType.GetProperty(PropertyName, OriginalBindingFlags).GetValue(InstantiatedClass, null);
            }
        }

        /// <summary>
        /// Gets a collection of Class's Properties in dictionary format with its data.
        /// </summary>
        /// <param name="InstantiatedClass">Instantiated class object to be used for Member modification. Not specifying this parameter will use the class object Instantiated during load.</param>
        /// <returns>a Dictionary of Property name with its data.</returns>
        public Dictionary<string, object> GetPropertyValueList(object InstantiatedClass = null)
        {
            if (InstantiatedClass == null)
            {
                InstantiatedClass = _InstantiatedClass;
            }

            Dictionary<string, object> PropertyValueList = new Dictionary<string, object>();
            PropertyInfo[] Properties = _ClassType.GetProperties(OriginalBindingFlags);

            foreach (PropertyInfo PI in Properties)
            {
                PropertyValueList.Add(PI.Name, PI.GetValue(InstantiatedClass, null));
            }

            return PropertyValueList;
        }

        /// <summary>
        /// Populates the _PropertyList collection with all Property Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        private void GetPropertyFields(bool IncludeNonPublic = false)
        {
            if (IncludeNonPublic) { ModifiedBindingFlags = OriginalBindingFlags | System.Reflection.BindingFlags.NonPublic; }
            else { ModifiedBindingFlags = OriginalBindingFlags; }
            PropertyInfo[] Properties = _ClassType.GetProperties(ModifiedBindingFlags);
            _PropertyList = new Dictionary<string, Type>();
            foreach (PropertyInfo PI in Properties)
            {
                _PropertyList.Add(PI.Name, PI.PropertyType);
            }
        }

        #endregion

        #region METHODLIST

        /// <summary>
        /// Gets Class's Methods.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a Dictionary of Method and its parameters.</returns>
        public Dictionary<string, Dictionary<string, Type>> GetMethodList(bool IncludeNonPublic = false)
        {
            if (_MethodList == null)
            {
                GetMethods(IncludeNonPublic);
            }
            return _MethodList;
        }

        /// <summary>
        /// Populate the _MethodList with Methods.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        private void GetMethods(bool IncludeNonPublic = false)
        {
            if (IncludeNonPublic) { ModifiedBindingFlags = OriginalBindingFlags | System.Reflection.BindingFlags.NonPublic; }
            else { ModifiedBindingFlags = OriginalBindingFlags; }
            MethodInfo[] Methods = _ClassType.GetMethods(ModifiedBindingFlags);
            _MethodList = new Dictionary<string, Dictionary<string, Type>>();
            foreach (MethodInfo MI in Methods)
            {
                if (!_MethodList.ContainsKey(MI.Name)) { _MethodList.Add(MI.Name, new Dictionary<string, Type>()); }
                foreach (ParameterInfo PI in MI.GetParameters())
                {
                    _MethodList[MI.Name].Add(PI.Name, PI.ParameterType);
                }
            }
        }

        /// <summary>
        /// Run/Invoke Specified Method found in a class. This Method will always assumes the method being invoke always return a value.
        /// </summary>
        /// <param name="MethodName">Method Name to invoke.</param>
        /// <param name="Type">Output Type.</param>
        /// <param name="Parameters">Parameters for the Method being invoked.</param>
        /// <returns>The returned Value from the invoked method.</returns>
        public object MethodInvoker(string MethodName, Converter.Types Type, object[] Parameters = null)
        {
            // Gets the total number of optional parameters, if missing, add it to the Parameters object.
            MethodInfo MI = _ClassType.GetMethod(MethodName);
            int MethodParameterMissingCount = MI.GetParameters().Length - Parameters.Length;
            if(MethodParameterMissingCount > 0)
            {
                List<object> ParameterList = new List<object>(Parameters);
                for (int i = 0; i < MethodParameterMissingCount; i++)
                {
                    ParameterList.Add(System.Type.Missing);
                }
                Parameters = ParameterList.ToArray<object>();
            }
            return Converter.ConvertTo(MI.Invoke(null, Parameters), Type);
        }

        #endregion

        #region ENUMS

        /// <summary>
        /// Gets Class's Enum Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        /// <returns>a Dictionary of Enums containing there members/items and its constant values.</returns>
        public Dictionary<string, Dictionary<string, int>> GetEnumList(bool IncludeNonPublic = false)
        {
            if (_EnumList == null)
            {
                GetEnumFields(IncludeNonPublic);
            }
            return _EnumList;
        }

        /// <summary>
        /// Gets a dictionary of Enum Members by specifying the Enum Type/Field. Can be separated from the Class type being supplied.
        /// </summary>
        /// <param name="EnumType">Enum Type/Field to have its item iterated/searched/loop/collect.</param>
        /// <returns>a Dictionary containing the Enum item/name/member and its constant value.</returns>
        public Dictionary<string, int> GetEnumMembers(Type EnumType)
        {
            Dictionary<string, int> EnumMembers = new Dictionary<string, int>();

            foreach (string enumitem in EnumType.GetEnumNames())
            {
                EnumMembers.Add(enumitem, (int)Enum.Parse(EnumType, enumitem));
            }

            return EnumMembers;
        }

        /// <summary>
        /// Populates the _EnumList collection with Enum Fields.
        /// </summary>
        /// <param name="IncludeNonPublic">Indicator whether to include non public identifiers or not.</param>
        private void GetEnumFields(bool IncludeNonPublic = false)
        {
            if (IncludeNonPublic) { ModifiedBindingFlags = OriginalBindingFlags | System.Reflection.BindingFlags.NonPublic; }
            else { ModifiedBindingFlags = OriginalBindingFlags; }
            Type[] Enums = _ClassType.GetNestedTypes(ModifiedBindingFlags).Where(MI => MI.IsEnum).ToArray<Type>();
            _EnumList = new Dictionary<string, Dictionary<string, int>>();
            foreach (Type E in Enums)
            {
                if (!_EnumList.ContainsKey(E.Name)) { _EnumList.Add(E.Name, new Dictionary<string, int>()); }
                foreach (string enumitem in E.GetEnumNames())
                {
                    _EnumList[E.Name] = CollectionUtility.MergeDictionary<string, int>(_EnumList[E.Name], GetEnumMembers(E));
                }
            }
        }

        #endregion
    }
}
