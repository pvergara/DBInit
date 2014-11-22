using NUnit.Framework;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Wire;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;

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
        readonly string _dbName = "sakila";
        readonly string _connectionString;
        readonly MySqlScriptHelper _helper;
        readonly Ecos.DBInit.Core.IDBInit _dbInit;

        public DBInitTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sakila"].ConnectionString;
            _helper = new MySqlScriptHelper(_connectionString);
            _dbName = new MySqlSchemaInfo(_connectionString).DatabaseName;
            _queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM " + _dbName + ".actor;";
            _queryToKnowNumberOfRowsOfAddressTable = "SELECT count(*) FROM " + _dbName + ".address;";

            _queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '" + _dbName + "';";
            _queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '" + _dbName + "';";

            _dbInit = DBInitFactory.
                From(ProviderType.MySql).
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
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(23));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProceduresAndFunctions), Is.EqualTo(6));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(0));
        }


        [Test]
        public void WhenIUseInitDataTheSystemWillAddAllDataIntoSchema()
        {
            //Arrange           

            //Act
            _dbInit.InitData();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfActorsTable), Is.EqualTo(200));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfAddressTable), Is.EqualTo(603));
        }
    }
}