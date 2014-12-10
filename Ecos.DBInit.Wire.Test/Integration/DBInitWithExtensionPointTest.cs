using System.Configuration;

using NUnit.Framework;

using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Test.ObjectMothers;
using Ecos.DBInit.Samples.ExtensionPoints;
using Ecos.DBInit.Core.Model;
using System.Linq;
using System.Collections.Generic;

namespace Ecos.DBInit.Wire.Test.Integration
{
    [TestFixture]
    public class DBInitWithExtensionPointTest
    {
        private IDBInit _dbInit;
        private ModuleLoader _moduleLoader;
        private string _connectionString;
        private readonly string _assemblyName = typeof(SchemaInfoNoDropSomeDBObjects).Assembly.GetName().Name;

        [Test]
        public void HowToOverwritePartiallySchemaInfo()
        {
            //Arrange           
            _connectionString = ConfigurationManager.ConnectionStrings[SakilaDbOM.ConnectionStringName].ConnectionString;

            _moduleLoader = new ModuleLoader(_connectionString, _assemblyName,ProviderType.MySql);
            _moduleLoader.SchemaInfoImpType = typeof(SchemaInfoNoDropSomeDBObjects);
            _moduleLoader.Wire();

            var _scriptExec = _moduleLoader.GetScriptExec();
            _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DROP VIEW IF EXISTS vw_actor;"));
            _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("CREATE VIEW vw_actor AS SELECT * FROM actor;"));
            _scriptExec.CommitAndClose();
            _dbInit = _moduleLoader.GetDBInit();
            var _schemaInfo = _moduleLoader.GetSchemaInfo();

            //Act
            _dbInit.InitSchema();

            //Pre-Assert
            var views = new List<string>();
            _scriptExec.ExecuteAndProcess(Script.From("SELECT table_name FROM information_schema.tables WHERE table_schema = '"+_schemaInfo.DatabaseName+"' AND TABLE_TYPE = 'VIEW';"),views,(reader,collection)=>{
                collection.Add(reader.GetString(0));
                    return collection;
                    });

            //Assert
            Assert.That(views.FirstOrDefault(v => v.Equals("vw_actor")), Is.Not.Null);
        }
    }
}