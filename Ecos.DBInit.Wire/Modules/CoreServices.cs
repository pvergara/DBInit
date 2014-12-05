using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Base;

namespace Ecos.DBInit.Wire.Modules
{
    public class CoreServices : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWorkCurrent>().InSingletonScope();

            Bind<IScriptLoader>().ToConstant(ModuleLoader.SchemaScriptLoader).WhenInjectedExactlyInto<SchemaOperator>().InSingletonScope();
            Bind<IScriptLoader>().ToConstant(ModuleLoader.DataScriptsLoader).WhenInjectedExactlyInto<DataOperator>().InSingletonScope();

            Bind<ISchemaOperator>().To<SchemaOperator>().InSingletonScope();
            Bind<IDataOperator>().To<DataOperator>().InSingletonScope();

            Bind<IDBOperator>().To<DBOperator>().InSingletonScope();

            Bind<IDBInit>().To<Core.Base.DBInit>();
        }
    }
}