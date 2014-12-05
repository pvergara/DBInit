using Ninject;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.ScriptHelpers;
using System.Reflection;

namespace Ecos.DBInit.Wire
{
    public class ModuleLoader
    {
        readonly StandardKernel _kernel;

        public static string ConnectionString { get; private set; }
        public static string AssemblyName { get; private set; }
        public static ProviderType ProviderType { get; private set; }

        public static IScriptLoader SchemaScriptLoader { get; private set; }
        public static IScriptLoader DataScriptsLoader { get; private set; }

        public ModuleLoader(string connectionString, string assemblyName, ProviderType providerType)
        {

            ConnectionString = connectionString;
            AssemblyName = assemblyName;
            ProviderType = providerType;

            var schemaContainer = 
                ScriptFinderFluentFactory.
                FromEmbeddedResource.
                    InitWith(AssemblyName, ScriptType.Schema).
                GetContainer();

            SchemaScriptLoader = 
                ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                    InitWith(AssemblyName, schemaContainer);

            var dataContainer = 
                ScriptFinderFluentFactory.
                FromEmbeddedResource.
                    InitWith(AssemblyName, ScriptType.Data).
                GetContainer();

            DataScriptsLoader = 
                ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                    InitWith(AssemblyName, dataContainer);

            _kernel = new StandardKernel();
            _kernel.Load(Assembly.GetExecutingAssembly());
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
    }
}

