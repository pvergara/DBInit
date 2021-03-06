﻿using Ninject.Modules;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using MySql = Ecos.DBInit.MySql;
using MsSql = Ecos.DBInit.MsSql;
using System;

namespace Ecos.DBInit.Wire.Modules
{
    public class DBSpecificServices : NinjectModule
    {
        private readonly string _connectionString;
        private readonly string _namedProviderType;

        public Type SchemaInfoImpType { private get; set; }
        public Type ScriptExecType  { private get; set; }
        public Type SpecificDBComposer { private get; set; }

        public DBSpecificServices(string connectionString,ProviderType providerType){

            _connectionString = connectionString;
            _namedProviderType = providerType.ToString();
            switch(providerType){
                case ProviderType.MySql:{
                        SchemaInfoImpType = typeof(MySql.SchemaInfo);
                        ScriptExecType = typeof(MySql.ScriptExec);
                        SpecificDBComposer = typeof(MySql.SpecificDBComposer);
                    }break;
                case ProviderType.MsSql:
                    {
                        SchemaInfoImpType = typeof(MsSql.SchemaInfo);
                        ScriptExecType = typeof(MsSql.ScriptExec);
                        SpecificDBComposer = typeof(MsSql.SpecificDBComposer);
                    } break;
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