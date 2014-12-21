using NUnit.Framework;
using Ecos.DBInit.Core.Model;
using System.Configuration;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Test.ObjectMothers;
using System.Linq;

namespace Ecos.DBInit.Test.Integration
{
    [TestFixture]
    public class MySqlDBInitTest
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
        readonly ISchemaInfo _schemaInfo;
        readonly ISpecificDBComposer _composer;

        public MySqlDBInitTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[SakilaDbOM.ConnectionStringName].ConnectionString;

            _moduleLoader = new ModuleLoader(_connectionString, _assemblyName,ProviderType.MySql);
            _moduleLoader.Wire();

            _dbInit = _moduleLoader.GetDBInit();
            _scriptExec = _moduleLoader.GetScriptExec();
            _schemaInfo = _moduleLoader.GetSchemaInfo();
            _dbName = _schemaInfo.DatabaseName;
            _composer = _moduleLoader.GetScriptComposer();

            _queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM " + _dbName + ".actor;";
            _queryToKnowNumberOfRowsOfAddressTable = "SELECT count(*) FROM " + _dbName + ".address;";

            _queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '" + _dbName + "';";
            _queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '" + _dbName + "';";


        }

        private long ExecScalarByUsing(string sqlCommand)
        {
            return _scriptExec.ExecuteScalar<long>(Script.From(sqlCommand));            
        }

        void CleanFunctionsViewsAndSPs()
        {
            foreach (var functionName in _schemaInfo.GetFunctions())
            {
                _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From(string.Format("DROP FUNCTION `{0}`.`{1}`;", _dbName, functionName)));
            }
            foreach (var viewName in _schemaInfo.GetViews())
            {
                _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From(string.Format("DROP VIEW `{0}`.`{1}`;", _dbName, viewName)));
            }
            foreach (var storedProcedureName in _schemaInfo.GetStoredProcedures())
            {
                _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From(string.Format("DROP PROCEDURE `{0}`.`{1}`;", _dbName, storedProcedureName)));
            }
            _scriptExec.CommitAndClose();
        }

        [Test]
        public void WhenIUseInitSchemaAllTheTablesWillBeEmpty()
        {
            //Arrange			
            CleanFunctionsViewsAndSPs();
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.TablesCounter + 0));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(0));

            //Act
            _dbInit.InitSchema();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.TablesCounter + SakilaDbOM.ViewsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(SakilaDbOM.SPsCounter + SakilaDbOM.FunctionsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(0));
        }

        private void CleanAllData()
        {
            _scriptExec.TryConnectionAndExecuteInsideTransaction(_composer.ComposeDeactivateReferentialIntegrity());
            foreach (var tableName in _schemaInfo.GetTables())
            {
                _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From(string.Format("TRUNCATE `{0}`.`{1}`;", _dbName, tableName)));
            }
            _scriptExec.TryConnectionAndExecuteInsideTransaction(_composer.ComposeActivateReferentialIntegrity());
            _scriptExec.CommitAndClose();
        }

        [Test]
        public void WhenIUseInitDataTheSystemWillAddAllDataIntoSchema()
        {
            //Arrange           
            CleanAllData();
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(0));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(0));

            //Act
            _dbInit.InitData();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(SakilaDbOM.TablesActorsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(SakilaDbOM.TablesAddressCounter));
        }

        [Test]
        public void SmartInitWillInitTheSchemaAndTheData()
        {
            //Arrange           
            CleanAllData();
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(0));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(0));

            CleanFunctionsViewsAndSPs();
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.TablesCounter + 0));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(0));

            //Act
            _dbInit.SmartInit();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.TablesCounter + SakilaDbOM.ViewsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(SakilaDbOM.SPsCounter + SakilaDbOM.FunctionsCounter));

            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(SakilaDbOM.TablesActorsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(SakilaDbOM.TablesAddressCounter));
        }
    }
}