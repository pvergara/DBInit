using NUnit.Framework;
using System;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Wire;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;

namespace Ecos.DBInit.Test
{
	[TestFixture]
	public class DBInitTest
	{
        const string DBName = "sakila";
        const string AssemblyName = "Ecos.DBInit.Samples.ProjectWithAMySQLDataBase";
        const string QueryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM "+DBName+".actor;";
        const string QueryToKnowNumberOfRowsOfAddressTable = "SELECT count(*) FROM "+DBName+".address;";

        readonly string _connectionString;
        readonly MySqlScriptHelper _helper;

        public DBInitTest(){
            _connectionString = ConfigurationManager.ConnectionStrings["sakila"].ConnectionString;
            _helper = new MySqlScriptHelper(_connectionString);
        }

		long ExecScalarByUsing (string sqlCommand)
		{
            return _helper.ExecuteScalar<long>(Script.From(sqlCommand));			
		}

		[Test]
		public void WhenIUseInitSchemaAllTheTablesWillBeEmpty ()
		{
			//Arrange			
			const string queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '"+DBName+"';";
			const string queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '"+DBName+"';";

            var dbInit = DBInitFactory.
                From(ProviderType.MySql).
                    InitWith(_connectionString, AssemblyName).
                GetDBInit();

			//Act
			dbInit.InitSchema ();

			//Pre-Assert
			var numberOfTablesAndViews = ExecScalarByUsing (queryToKnowNumberOfTablesAndViews);
			var numberOfStoredProcecuresAndFunctions = ExecScalarByUsing (queryToKnowNumberOfStoredProceduresAndFunctions);
			var numberOfKnowNumberOfRowsOfActorsTable = ExecScalarByUsing (QueryToKnowNumberOfRowsOfActorsTable);

			//Assert
			Assert.That (numberOfTablesAndViews, Is.EqualTo (23));
			Assert.That (numberOfStoredProcecuresAndFunctions, Is.EqualTo (6));
			Assert.That (numberOfKnowNumberOfRowsOfActorsTable, Is.EqualTo (0));
		}			


        [Test]
        public void WhenIUseInitDataTheSystemWillAddAllDataIntoSchema ()
        {
            //Arrange           
            var dbInit = DBInitFactory.
                From(ProviderType.MySql).
                    InitWith(_connectionString, AssemblyName).
                GetDBInit();

            //Act
            dbInit.InitData ();

            //Assert
            Assert.That (ExecScalarByUsing(QueryToKnowNumberOfRowsOfActorsTable), Is.EqualTo (200));
            Assert.That (ExecScalarByUsing(QueryToKnowNumberOfRowsOfAddressTable), Is.EqualTo (603));
        }
	}
}