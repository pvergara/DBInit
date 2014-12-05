using NUnit.Framework;
using Ecos.DBInit.Core.Model;
using System.Configuration;
using Ecos.DBInit.Test.ObjectMothers;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Wire;

namespace Ecos.DBInit.Test
{
    [TestFixture]
    public class DBInitTest
    {
        readonly string _assemblyName = SakilaDbOM.SampleProjectAssemblyName;

        readonly string _queryToKnowNumberOfRowsOfActorsTable;
        readonly string _queryToKnowNumberOfRowsOfAddressTable;
        readonly string _queryToKnowNumberOfTablesAndViews;
        readonly string _queryToKnowNumberOfStoredProceduresAndFunctions;
        readonly string _dbName;
        readonly string _connectionString;
        readonly IScriptExec _scriptExec;
        readonly IDBInit _dbInit;
        ModuleLoader _moduleLoader;

        public DBInitTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[SakilaDbOM.ConnectionStringName].ConnectionString;

            _moduleLoader = new ModuleLoader(_connectionString, _assemblyName, ProviderType.MySql);

            _dbInit = _moduleLoader.GetDBInit();
            _scriptExec = _moduleLoader.GetScriptExec();
            _dbName = _moduleLoader.GetSchemaInfo().DatabaseName;

            _queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM " + _dbName + ".actor;";
            _queryToKnowNumberOfRowsOfAddressTable = "SELECT count(*) FROM " + _dbName + ".address;";

            _queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '" + _dbName + "';";
            _queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '" + _dbName + "';";


        }

        private long ExecScalarByUsing(string sqlCommand)
        {
            return _scriptExec.ExecuteScalar<long>(Script.From(sqlCommand));            
        }

        [Test]
        public void WhenIUseInitSchemaAllTheTablesWillBeEmpty()
        {
            //Arrange			

            //Act
            _dbInit.InitSchema();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.TablesCounter + SakilaDbOM.ViewsCounter ));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(SakilaDbOM.SPsCounter + SakilaDbOM.FunctionsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(0));
        }


        [Test]
        public void WhenIUseInitDataTheSystemWillAddAllDataIntoSchema()
        {
            //Arrange           

            //Act
            _dbInit.InitData();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(SakilaDbOM.TablesActorsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(SakilaDbOM.TablesAddressCounter));
        }
    }
}