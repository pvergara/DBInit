using System.Configuration;
using NUnit.Framework;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test.Integration
{
    [TestFixture]
    public class MsSqlDBInitTest
    {
        readonly string _assemblyName = "Ecos.DBInit.Samples.NorthwindDataBase";

        readonly string _queryToKnowNumberOfRowsOfOrderDetails;
        readonly string _queryToKnowNumberOfRowsOfProductsTable;
        readonly string _queryToKnowNumberOfTablesAndViews;
        readonly string _queryToKnowNumberOfStoredProcedures;
        readonly string _connectionString;
        readonly IScriptExec _scriptExec;
        readonly IDBInit _dbInit;
        ModuleLoader _moduleLoader;

        public MsSqlDBInitTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[NorthwindDbOM.ConnectionStringName].ConnectionString;

            _moduleLoader = new ModuleLoader(_connectionString, _assemblyName,ProviderType.MsSql);
            _moduleLoader.Wire();

            _dbInit = _moduleLoader.GetDBInit();
            _scriptExec = _moduleLoader.GetScriptExec();

            _queryToKnowNumberOfRowsOfOrderDetails = "SELECT count(*) FROM [Order Details];";
            _queryToKnowNumberOfRowsOfProductsTable = "SELECT count(*) FROM [Products];";

            _queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM Northwind.sys.sysobjects WHERE xtype in ('U','V');";
            _queryToKnowNumberOfStoredProcedures = "SELECT count(*) FROM Northwind.sys.sysobjects WHERE xtype in (N'P', N'PC');";


        }

        private int ExecScalarByUsing(string sqlCommand)
        {
            return _scriptExec.ExecuteScalar<int>(Script.From(sqlCommand));            
        }

        [Test]
        public void WhenIUseInitSchemaAllTheTablesWillBeEmpty()
        {
            //Arrange			

            //Act
            _dbInit.InitSchema();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfTablesAndViews), Is.EqualTo(NorthwindDbOM.TablesCounter + NorthwindDbOM.ViewsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfStoredProcedures), Is.EqualTo(NorthwindDbOM.SPsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfOrderDetails), Is.EqualTo(0));
        }


        [Test]
        public void WhenIUseInitDataTheSystemWillAddAllDataIntoSchema()
        {
            //Arrange           

            //Act
            _dbInit.InitData();

            //Assert
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfOrderDetails), Is.EqualTo(NorthwindDbOM.OrderDetailsCounter));
            Assert.That(ExecScalarByUsing(_queryToKnowNumberOfRowsOfProductsTable), Is.EqualTo(NorthwindDbOM.ProductsCounter));
        }
    }
}