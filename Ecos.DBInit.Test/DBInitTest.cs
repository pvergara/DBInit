using NUnit.Framework;
using Core = Ecos.DBInit.Core;
using System;
using System.Data.Common;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Samples.ProjectWithAMySQLDataBase;

namespace Ecos.DBInit.Test
{
	[TestFixture]
	public class DBInitTest
	{
		static long ExecScalarByUsing (string providerInvariantName, string connectionString, string sqlCommand)
		{
			long result;
		
			var dbProviderFactory = DbProviderFactories.GetFactory (providerInvariantName);
			var dbcon = dbProviderFactory.CreateConnection ();
			dbcon.ConnectionString = connectionString;
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
			const string dbName = "sakila";
			const string connectionString =
				"Server=localhost;" +
				"Database="+dbName+";" +
				"User ID=desarrollo;" +
				"Password=3QSo5cff;" +
				"Allow User Variables=True";
			const string queryToKnowNumberOfTablesAndViews = "SELECT count(*) FROM information_schema.tables WHERE table_schema = '"+dbName+"';";
			const string queryToKnowNumberOfStoredProceduresAndFunctions = "SELECT count(*) FROM information_schema.routines WHERE routine_schema = '"+dbName+"';";
			const string queryToKnowNumberOfRowsOfActorsTable = "SELECT count(*) FROM "+dbName+".actor;";
			const string providerInvariantName = "MySql.Data.MySqlClient";

			IScriptFinder finder = new ScriptFinderOnEmbbededResource (typeof(SomeClassThatHasTheAssembly).Assembly,"sakila-schema.sql",ScriptType.Schema);
			var appender = new ScriptAppender(finder);
			var dbInit = new Core.DBInit (providerInvariantName, connectionString,appender);

			//Act
			dbInit.InitSchema ();

			//Pre-Assert
			var numberOfTablesAndViews = ExecScalarByUsing (providerInvariantName, connectionString, queryToKnowNumberOfTablesAndViews);
			var numberOfStoredProcecuresAndFunctions = ExecScalarByUsing (providerInvariantName, connectionString, queryToKnowNumberOfStoredProceduresAndFunctions);
			var numberOfKnowNumberOfRowsOfActorsTable = ExecScalarByUsing (providerInvariantName, connectionString, queryToKnowNumberOfRowsOfActorsTable);

			//Assert
			Assert.That (numberOfTablesAndViews, Is.EqualTo (23));
			Assert.That (numberOfStoredProcecuresAndFunctions, Is.EqualTo (6));
			Assert.That (numberOfKnowNumberOfRowsOfActorsTable, Is.EqualTo (0));
		}			
	}
}