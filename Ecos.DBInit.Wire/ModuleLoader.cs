using Ninject;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.ScriptHelpers;
using System;
using System.Linq;
using Ninject.Modules;
using Ecos.DBInit.Wire.Modules;

namespace Ecos.DBInit.Wire
{
    public class ModuleLoader
    {
        readonly StandardKernel _kernel;

        private readonly DBSpecificServices _dbSpecificservice;
        private readonly CoreServices _coreService;

        public ModuleLoader(string connectionString, string assemblyName,ProviderType providerType)
        {
            var schemaScriptLoader = InitializeDefaultSchemaScriptLoader(assemblyName);
            var dataScriptsLoader = InitializeDefaultDataScriptLoader(assemblyName);

            _kernel = new StandardKernel();

            _dbSpecificservice = new DBSpecificServices(connectionString,providerType);
            _coreService = new CoreServices(schemaScriptLoader,dataScriptsLoader);
        }

        private static ScriptLoaderOnEmbeddedResource InitializeDefaultSchemaScriptLoader(string assemblyName)
        {
            var schemaContainer = ScriptFinderFluentFactory.FromEmbeddedResource.InitWith(assemblyName, ScriptType.Schema).GetContainer();
            var schemaScriptLoader = ScriptLoaderFluentFactory.FromEmbeddedResource.InitWith(assemblyName, schemaContainer);
            return schemaScriptLoader;
        }

        private static ScriptLoaderOnEmbeddedResource InitializeDefaultDataScriptLoader(string assemblyName)
        {
            var dataContainer = ScriptFinderFluentFactory.FromEmbeddedResource.InitWith(assemblyName, ScriptType.Data).GetContainer();
            var dataScriptsLoader = ScriptLoaderFluentFactory.FromEmbeddedResource.InitWith(assemblyName, dataContainer);
            return dataScriptsLoader;
        }

        private static void ValidateTypeWithExpectedInterface(Type interfaceType,Type implType)
        {
            if (implType.GetInterfaces().All(t => t != interfaceType))
                throw new ArgumentException(string.Format("The expected type must implement {0}.", interfaceType.Name));
        }

        public Type SchemaInfoImpType 
        { 
            set
            {
                ValidateTypeWithExpectedInterface(typeof(ISchemaInfo),value);
                _dbSpecificservice.SchemaInfoImpType = value;
            }
        }

        public Type SpecificDBComposerImpType 
        { 
            set
            {
                ValidateTypeWithExpectedInterface(typeof(ISpecificDBComposer),value);
                _dbSpecificservice.SpecificDBComposer = value;
            }
        }

        public Type ScriptExecImpType
        {
            set
            {
                ValidateTypeWithExpectedInterface(typeof(IScriptExec),value);
                _dbSpecificservice.ScriptExecType = value;
            }
        }

        public Type SchemaOperatorImpType
        {
            set
            {
                ValidateTypeWithExpectedInterface(typeof(ISchemaOperator),value);
                _coreService.SchemaOperatorType = value;
            }
        }

        public Type DataOperatorImpType
        {
            set
            {
                ValidateTypeWithExpectedInterface(typeof(IDataOperator),value);
                _coreService.DataOperatorType = value;
            }
        }

        public Type DBOperatorImpType
        {
            set
            {
                ValidateTypeWithExpectedInterface(typeof(IDBOperator),value);
                _coreService.DBOperatorType = value;
            }
        }

        public void Wire(){
            _kernel.Load(new INinjectModule[] {_dbSpecificservice,_coreService});
        }


        public IDBInit GetDBInit()
        {
            return _kernel.Get<IDBInit>();
        }

        public IScriptExec GetScriptExec()
        {
            return _kernel.Get<IScriptExec>();
        }

        public ISchemaInfo GetSchemaInfo()
        {
            return _kernel.Get<ISchemaInfo>();
        }
    
        public ISpecificDBComposer GetScriptComposer()
        {
            return _kernel.Get<ISpecificDBComposer>();
        }

        public ISchemaOperator GetSchemaOperator()
        {
            return _kernel.Get<ISchemaOperator>();
        }

        public object GetDataOperator()
        {
            return _kernel.Get<IDataOperator>();
        }

        public object GetDBOperator()
        {
            return _kernel.Get<IDBOperator>();
        }
    }
}