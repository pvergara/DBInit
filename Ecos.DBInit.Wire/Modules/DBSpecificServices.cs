using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using MySql = Ecos.DBInit.MySql;

namespace Ecos.DBInit.Wire.Modules
{
    public class DBSpecificServices : NinjectModule
    {
        public override void Load()
        {
            var nameMySql = ProviderType.MySql.ToString();

            Bind<IScriptExec>().To<MySql.ScriptExec>().Named(nameMySql).WithConstructorArgument("connectionString",ModuleLoader.ConnectionString);
            Bind<ISpecificDBComposer>().To<MySql.SpecificDBComposer>().InSingletonScope().Named(nameMySql);
            Bind<ISchemaInfo>().To<MySql.SchemaInfo>().InSingletonScope().Named(nameMySql).WithConstructorArgument("connectionString",ModuleLoader.ConnectionString);
        }
    }
}