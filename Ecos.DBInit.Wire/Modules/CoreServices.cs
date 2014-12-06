using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Base;

namespace Ecos.DBInit.Wire.Modules
{
    public class CoreServices : NinjectModule
    {
        private readonly IScriptLoader _schemaScriptLoader;
        private readonly IScriptLoader _dataScriptsLoader;

        public CoreServices(IScriptLoader schemaScriptLoader,IScriptLoader dataScriptsLoader)
        {
            _schemaScriptLoader = schemaScriptLoader;
            _dataScriptsLoader = dataScriptsLoader;
        }

        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWorkCurrent>().InSingletonScope();

            Bind<IScriptLoader>().ToConstant(_schemaScriptLoader).WhenInjectedExactlyInto<SchemaOperator>().InSingletonScope();
            Bind<IScriptLoader>().ToConstant(_dataScriptsLoader).WhenInjectedExactlyInto<DataOperator>().InSingletonScope();

            Bind<ISchemaOperator>().To<SchemaOperator>().InSingletonScope();
            Bind<IDataOperator>().To<DataOperator>().InSingletonScope();

            Bind<IDBOperator>().To<DBOperator>().InSingletonScope();

            Bind<IDBInit>().To<Core.Base.DBInit>();
        }
    }
}