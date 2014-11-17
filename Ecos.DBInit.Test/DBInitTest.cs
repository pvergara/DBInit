using NUnit.Framework;
using System;
using System.Data.Common;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Wire;

namespace Ecos.DBInit.Test
{
	[TestFixture]
	public class DBInitTest
	{
        const string DBName = "sakila";

        readonly string _connectionString;

        public DBInitTest(){
            _connectionString =
                "Server=localhost;" +
                "Database=" + DBName + ";" +
                "User ID=desarrollo;" +
                "Password=3QSo5cff;" +
                "Allow User Variables=True";
        }

		long ExecScalarByUsing (string sqlCommand)
		{
            const string providerInvariantName = "MySql.Data.MySqlClient";
			long result;
		
			var dbProviderFactory = DbProviderFactories.GetFactory (providerInvariantName);
			var dbcon = dbProviderFactory.CreateConnection ();
			dbcon.ConnectionString = _connectionString;
			dbcon.Open ();
			using (var dbcmd = dbcon.CreateCommand ()) {
				dbcmd.CommandText = sqlCommand;
				result = (long)dbcmd.ExecuteScalar ();
			}
			dbcon.Close ();
			return result;
		}

		[Test]
		public void WhenIUseInitSchemaAllTheTablesWillBeEmpty ()
		{
			//Arrange			
			const string queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '"+DBName+"';";
			const string queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '"+DBName+"';";
            const string queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM "+DBName+".actor;";

            const string assemblyName = "Ecos.DBInit.Samples.ProjectWithAMySQLDataBase";
            var dbInit = DBInitFactory.
                From(ProviderType.MySql).
                    InitWith(_connectionString, assemblyName).
                GetDBInit();

			//Act
			dbInit.InitSchema ();

			//Pre-Assert
			var numberOfTablesAndViews = ExecScalarByUsing (queryToKnowNumberOfTablesAndViews);
			var numberOfStoredProcecuresAndFunctions = ExecScalarByUsing (queryToKnowNumberOfStoredProceduresAndFunctions);
			var numberOfKnowNumberOfRowsOfActorsTable = ExecScalarByUsing (queryToKnowNumberOfRowsOfActorsTable);

			//Assert
			Assert.That (numberOfTablesAndViews, Is.EqualTo (23));
			Assert.That (numberOfStoredProcecuresAndFunctions, Is.EqualTo (6));
			Assert.That (numberOfKnowNumberOfRowsOfActorsTable, Is.EqualTo (0));
		}			
	}
}