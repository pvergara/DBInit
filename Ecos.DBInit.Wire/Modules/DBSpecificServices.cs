using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using MySql = Ecos.DBInit.MySql;
using System;

namespace Ecos.DBInit.Wire.Modules
{
    public class DBSpecificServices : NinjectModule
    {
        private readonly string _connectionString;
        private readonly string _namedProviderType;

        public Type SchemaInfoImpType { get; set; }
        public Type ScriptExecType  { get; set; }
        public Type SpecificDBComposer { get; set; }

        public DBSpecificServices(string connectionString,ProviderType providerType){

            _connectionString = connectionString;
            _namedProviderType = providerType.ToString();
            switch(providerType){
                case ProviderType.MySql:{
                        SchemaInfoImpType = typeof(MySql.SchemaInfo);
                        ScriptExecType = typeof(MySql.ScriptExec);
                        SpecificDBComposer = typeof(MySql.SpecificDBComposer);
                    }break;
            }
        }
            
        public override void Load()
        {
            Bind<IScriptExec>().To(ScriptExecType).Named(_namedProviderType).WithConstructorArgument("connectionString", _connectionString);
            Bind<ISpecificDBComposer>().To(SpecificDBComposer).InSingletonScope().Named(_namedProviderType);
            Bind<ISchemaInfo>().To(SchemaInfoImpType).InSingletonScope().Named(_namedProviderType).WithConstructorArgument("connectionString", _connectionString);

        }
    }
}