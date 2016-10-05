// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
//using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;

namespace System.Runtime.Serialization
{


    public class XsdDataContractExporter
    {
        ExportOptions options;
        XmlSchemaSet schemas;
        DataContractSet dataContractSet;

        public XsdDataContractExporter()
        {
        }

        public XsdDataContractExporter(XmlSchemaSet schemas)
        {
            this.schemas = schemas;
        }

        public ExportOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public XmlSchemaSet Schemas
        {
            get
            {
                //XmlSchemaSet schemaSet = GetSchemaSet();
                //SchemaImporter.CompileSchemaSet(schemaSet);
                //return schemaSet;
                // SchemaImporter is not available.
                throw new PlatformNotSupportedException("SchemaImporter");
            }
        }

        XmlSchemaSet GetSchemaSet()
        {
            if (schemas == null)
            {
                schemas = new XmlSchemaSet();
                schemas.XmlResolver = null;
            }
            return schemas;
        }

        DataContractSet DataContractSet
        {
            get
            {
                //if (dataContractSet == null)
                //{
                //    dataContractSet = new DataContractSet((Options == null) ? null : Options.GetSurrogate());
                //}
                return dataContractSet;
            }
        }

        void TraceExportBegin()
        {
            // System.IdentityModel.Services.DiagnosticUtility.ShouldTraceInformation is not available.

            //if (DiagnosticUtility.ShouldTraceInformation)
            //{
            //    TraceUtility.Trace(TraceEventType.Information, TraceCode.XsdExportBegin, SR.Format(SR.TraceCodeXsdExportBegin));
            //}
        }

        void TraceExportEnd()
        {
            //if (DiagnosticUtility.ShouldTraceInformation)
            //{
            //    TraceUtility.Trace(TraceEventType.Information, TraceCode.XsdExportEnd, SR.Format(SR.TraceCodeXsdExportEnd));
            //}
        }

        void TraceExportError(Exception exception)
        {
            //if (DiagnosticUtility.ShouldTraceError)
            //{
            //    TraceUtility.Trace(TraceEventType.Error, TraceCode.XsdExportError, SR.Format(SR.TraceCodeXsdExportError), null, exception);
            //}
        }

        public void Export(ICollection<Assembly> assemblies)
        {
            if (assemblies == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));

            TraceExportBegin();

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullAssembly, "assemblies")));

                    Type[] types = assembly.GetTypes();
                    for (int j = 0; j < types.Length; j++)
                        CheckAndAddType(types[j]);
                }

                Export();
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
            TraceExportEnd();
        }

        public void Export(ICollection<Type> types)
        {
            if (types == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));

            TraceExportBegin();

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                foreach (Type type in types)
                {
                    if (type == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullType, "types")));
                    AddType(type);
                }

                Export();
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
            TraceExportEnd();
        }

        public void Export(Type type)
        {
            if (type == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));

            TraceExportBegin();

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                AddType(type);
                Export();
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
            TraceExportEnd();
        }

        public XmlQualifiedName GetSchemaTypeName(Type type)
        {
            if (type == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            type = GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            XmlDataContract xmlDataContract = dataContract as XmlDataContract;
            if (xmlDataContract != null && xmlDataContract.IsAnonymous)
                return XmlQualifiedName.Empty;
            return dataContract.StableName;
        }

        public XmlSchemaType GetSchemaType(Type type)
        {
            if (type == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            type = GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            XmlDataContract xmlDataContract = dataContract as XmlDataContract;
            if (xmlDataContract != null && xmlDataContract.IsAnonymous)
                return xmlDataContract.XsdType;
            return null;
        }

        public XmlQualifiedName GetRootElementName(Type type)
        {
            if (type == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            type = GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            if (dataContract.HasRoot)
            {
                return new XmlQualifiedName(dataContract.TopLevelElementName.Value, dataContract.TopLevelElementNamespace.Value);
            }
            else
            {
                return null;
            }
        }

        Type GetSurrogatedType(Type type)
        {
            //IDataContractSurrogate dataContractSurrogate;
            //if (options != null && (dataContractSurrogate = Options.GetSurrogate()) != null)
            //    type = DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, type);
            return type;
        }

        void CheckAndAddType(Type type)
        {
            type = GetSurrogatedType(type);
            if (!type.ContainsGenericParameters && DataContract.IsTypeSerializable(type))
                AddType(type);
        }

        void AddType(Type type)
        {
            DataContractSet.Add(type);
        }

        void Export()
        {
            AddKnownTypes();
            SchemaExporter schemaExporter = new SchemaExporter(GetSchemaSet(), DataContractSet);
            schemaExporter.Export();
        }

        void AddKnownTypes()
        {
            if (Options != null)
            {
                Collection<Type> knownTypes = Options.KnownTypes;

                if (knownTypes != null)
                {
                    for (int i = 0; i < knownTypes.Count; i++)
                    {
                        Type type = knownTypes[i];
                        if (type == null)
                            throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullKnownType)));
                        AddType(type);
                    }
                }
            }
        }

        public bool CanExport(ICollection<Assembly> assemblies)
        {
            if (assemblies == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullAssembly, "assemblies")));

                    Type[] types = assembly.GetTypes();
                    for (int j = 0; j < types.Length; j++)
                        CheckAndAddType(types[j]);
                }
                AddKnownTypes();
                return true;
            }
            catch (InvalidDataContractException)
            {
                dataContractSet = oldValue;
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
        }

        public bool CanExport(ICollection<Type> types)
        {
            if (types == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                foreach (Type type in types)
                {
                    if (type == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullType, "types")));
                    AddType(type);
                }
                AddKnownTypes();
                return true;
            }
            catch (InvalidDataContractException)
            {
                dataContractSet = oldValue;
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
        }

        public bool CanExport(Type type)
        {
            if (type == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));

            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                AddType(type);
                AddKnownTypes();
                return true;
            }
            catch (InvalidDataContractException)
            {
                dataContractSet = oldValue;
                return false;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
        }

#if USE_REFEMIT
        //Returns warnings
        public IList<string> GenerateCode(IList<Assembly> assemblies) 
        {
            if (assemblies == null)
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
            List<string> warnings = new List<string>();
 
            DataContractSet oldValue = (dataContractSet == null) ? null : new DataContractSet(dataContractSet);
            try
            {
                for (int i=0; i < assemblies.Count; i++)
                {
                    Assembly assembly = assemblies[i];
                    if (assembly == null)
                        throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.CannotExportNullAssembly, "assemblies")));
 
                    Type[] types = assembly.GetTypes();
                    for (int j=0; j < types.Length; j++)
                    {
                        try
                        {
                            CheckAndAddType(types[j]);
                        }
                        catch (Exception ex)
                        {
                            warnings.Add("Error on exporting Type " + DataContract.GetClrTypeFullName(types[j]) + ". " + ex.Message);
                        }
                        
                    }
                }
 
                foreach (KeyValuePair<XmlQualifiedName, DataContract> pair in dataContractSet)
                {
                    DataContract dataContract = pair.Value;
                    if (dataContract is ClassDataContract)
                    {
                        try
                        {
                            XmlFormatClassWriterDelegate writerMethod = ((ClassDataContract)dataContract).XmlFormatWriterDelegate;
                            XmlFormatClassReaderDelegate readerMethod = ((ClassDataContract)dataContract).XmlFormatReaderDelegate;
                        }
                        catch (Exception ex)
                        {
                            warnings.Add("Error on exporting Type " + dataContract.UnderlyingType + ". " + ex.Message);
                        }
                    }
                    else if (dataContract is CollectionDataContract)
                    {
                        try
                        {
                            XmlFormatCollectionWriterDelegate writerMethod = ((CollectionDataContract)dataContract).XmlFormatWriterDelegate;
                            XmlFormatCollectionReaderDelegate readerMethod = ((CollectionDataContract)dataContract).XmlFormatReaderDelegate;
                        }
                        catch (Exception ex)
                        {
                            warnings.Add("Error on exporting Type " + dataContract.UnderlyingType + ". " + ex.Message);
                        }
                    }
                }
                return warnings;
            }
            catch (Exception ex)
            {
                if (DiagnosticUtility.IsFatal(ex))
                {
                    throw;
                }
                dataContractSet = oldValue;
                TraceExportError(ex);
                throw;
            }
        }
#endif

    }

}