using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.MySql.ScriptHelpers;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Wire
{
    public class DBSpecificServices : NinjectModule
    {
        public override void Load()
        {
            var nameMySql = ProviderType.MySql.ToString();
            Bind<IScriptExec>().To<MySqlScriptExec>().Named(nameMySql).WithConstructorArgument("connectionString",ModuleLoader.ConnectionString);
            Bind<ISpecificDBOperator>().To<MySql.SpecificDBOperator>().InSingletonScope().Named(nameMySql);
            Bind<ISchemaInfo>().To<MySqlSchemaInfo>().InSingletonScope().Named(nameMySql).WithConstructorArgument("connectionString",ModuleLoader.ConnectionString);
        }
    }
}