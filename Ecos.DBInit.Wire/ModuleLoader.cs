using Ninject;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.ScriptHelpers;
using System;
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

        public void OverwriteImplementationOf(Type interfaceType,Type implementationType){
            if (interfaceType == typeof(ISchemaInfo))
                _dbSpecificservice.SchemaInfoImpType = implementationType;

            if (interfaceType == typeof(ISpecificDBComposer))
                _dbSpecificservice.SpecificDBComposer = implementationType;

            if (interfaceType == typeof(IScriptExec))
                _dbSpecificservice.ScriptExecType = implementationType;

            if (interfaceType == typeof(ISchemaOperator))
                _coreService.SchemaOperatorType = implementationType;

            if (interfaceType == typeof(IDataOperator))
                _coreService.DataOperatorType = implementationType;

            if (interfaceType == typeof(IDBOperator))
                _coreService.DBOperatorType = implementationType;

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