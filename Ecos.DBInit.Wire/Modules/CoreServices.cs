using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Base;
using System;

namespace Ecos.DBInit.Wire.Modules
{
    public class CoreServices : NinjectModule
    {
        private readonly IScriptLoader _schemaScriptLoader;
        private readonly IScriptLoader _dataScriptsLoader;

        public Type SchemaOperatorType { private get; set; }
        public Type DataOperatorType { private get; set; }
        public Type DBOperatorType { private get; set; }

        public CoreServices(IScriptLoader schemaScriptLoader, IScriptLoader dataScriptsLoader)
        {
            _schemaScriptLoader = schemaScriptLoader;
            _dataScriptsLoader = dataScriptsLoader;

            SchemaOperatorType = typeof(SchemaOperator);
            DataOperatorType = typeof(DataOperator);
            DBOperatorType = typeof(DBOperator);
        }

        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWorkCurrent>().InSingletonScope();

            Bind<IScriptLoader>().ToConstant(_schemaScriptLoader).WhenInjectedExactlyInto<SchemaOperator>().InSingletonScope();
            Bind<IScriptLoader>().ToConstant(_dataScriptsLoader).WhenInjectedExactlyInto<DataOperator>().InSingletonScope();

            Bind<ISchemaOperator>().To(SchemaOperatorType).InSingletonScope();
            Bind<IDataOperator>().To(DataOperatorType).InSingletonScope();
            Bind<IDBOperator>().To(DBOperatorType).InSingletonScope();

            Bind<IDBInit>().To<Core.Base.DBInit>();
        }
    }
}