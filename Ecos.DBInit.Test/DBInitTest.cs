using NUnit.Framework;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Bootstrap;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test
{
    [TestFixture]
    public class DBInitTest
    {
        const string AssemblyName = "Ecos.DBInit.Samples.ProjectWithAMySQLDataBase";

        readonly string _queryToKnowNumberOfRowsOfActorsTable;
        readonly string _queryToKnowNumberOfRowsOfAddressTable;
        readonly string _queryToKnowNumberOfTablesAndViews;
        readonly string _queryToKnowNumberOfStoredProceduresAndFunctions;
        readonly string _dbName;
        readonly string _connectionString;
        readonly MySqlScriptHelper _helper;
        readonly Ecos.DBInit.Core.IDBInit _dbInit;

        public DBInitTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sakilaConStr"].ConnectionString;
            _helper = new MySqlScriptHelper(_connectionString);
            _dbName = new MySqlSchemaInfo(_connectionString,_helper).DatabaseName;
            _queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM " + _dbName + ".actor;";
            _queryToKnowNumberOfRowsOfAddressTable = "SELECT count(*) FROM " + _dbName + ".address;";

            _queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '" + _dbName + "';";
            _queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '" + _dbName + "';";

            _dbInit = DBInitFactory.
                From().
                InitWith(_connectionString, AssemblyName).
                GetDBInit();
        }

        private long ExecScalarByUsing(string sqlCommand)
        {
            return _helper.ExecuteScalar<long>(Script.From(sqlCommand));            
        }

        [Test]
        public void WhenIUseInitSchemaAllTheTablesWillBeEmpty()
        {
            //Arrange			

            //Act
            _dbInit.InitSchema();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(SakilaDbOM.ViewsCounter + SakilaDbOM.ViewsCounter ));
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