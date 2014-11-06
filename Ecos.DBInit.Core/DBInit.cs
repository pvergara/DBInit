using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.IO;

//TODO: Maybe not all the developers need to clean ALL the schema, maybe they only need to clean some specific tables (or maybe have limited permissions)
//TODO: Each DataBase Engine could use different command to clean the database/schema!!!!
//TODO: BEWARE WITH REFERENTIAL INTEGRITY
using System.Text;
using MySql.Data.MySqlClient;


namespace Ecos.DBInit.Core
{
    public class DBInit: IDBInit
    {
        private IDbConnection _dbConnection;
        private string _dbName;

        private static void SingleExecuteNonQuery(IDbCommand dbCommand, string script)
        {
            dbCommand.CommandText = script;
            dbCommand.ExecuteNonQuery();
        }

        private static IEnumerable<string> LoadSchemaScripts(string path)
        {
            var files = new DirectoryInfo(path).GetFiles();
            var result = new List<string>();
            foreach (var file in files)
            {
                var stream = file.OpenText();
                result.Add(stream.ReadToEnd());
            }
            return result;
        }

        private void SingleExecuteQuery(string queryWithDropTablesClauses)
        {
            _dbConnection.Open();
            using (var dbCommand = _dbConnection.CreateCommand ())
            {
                dbCommand.CommandText = queryWithDropTablesClauses;
                var reader = dbCommand.ExecuteReader();
                var sb = new StringBuilder();

                while (reader.Read())
                {
                    sb.Append(reader.GetString(0));
                }
            }
            _dbConnection.Close();

        }

		private List<string> SingleExecuteQueryGettingFirstFieldAsString (string queryWithDropTablesClauses,List<string> scripts)
		{
			_dbConnection.Open ();
			using (var dbCommand = _dbConnection.CreateCommand ()) {
				dbCommand.CommandText = queryWithDropTablesClauses;
				var reader = dbCommand.ExecuteReader ();
				while (reader.Read ()) {
					scripts.Add (reader.GetString (0));
				}
			}
			_dbConnection.Close ();
			return scripts;
		}

        private IEnumerable<string> LoadDropTablesScripts()
        {
            var scripts = new List<string>();
			
			IEnumerable<string> queries = LoadSchemaScripts(@"/home/pvergara/Documents/MonoWorkspace/DBInit/Ecos.DBInit.MySql/Scripts/Schema");

			foreach (string query in queries) {
				var aux = query.Replace ("_DATABASE_NAME_", _dbName);
				scripts = SingleExecuteQueryGettingFirstFieldAsString (aux, scripts);
			}

			return scripts;
        }

        private void CleanDB()
        {
            IEnumerable<string> scripts = LoadDropTablesScripts();
            _dbConnection.Open();
            using (var dbCommand = _dbConnection.CreateCommand ())
            {
                SingleExecuteNonQuery(dbCommand, "SET @@foreign_key_checks = 0;");               
                foreach (string script in scripts)
                {
                    SingleExecuteNonQuery(dbCommand, script);
                }
            }
            _dbConnection.Close();
        }

		private string _connectionString;

		public DBInit(string providerInvariantName, string connectionString)
        {
			_connectionString = connectionString;
            var dbProviderFactory = DbProviderFactories.GetFactory(providerInvariantName);
            _dbConnection = dbProviderFactory.CreateConnection();

            _dbConnection.ConnectionString = connectionString;
            _dbName = _dbConnection.Database;
        }

        private void LoadSchema()
        {
			IEnumerable<string> scripts = LoadSchemaScripts(@"/home/pvergara/Documents/MonoWorkspace/DBInit/Ecos.DBInit.Samples.ProjectWithAMySQLDataBase/Scripts/Schema");

			MySqlConnection cn = new MySqlConnection(_connectionString);
			foreach (var sql in scripts)
			{
				var script = new MySqlScript(cn, sql);
				script.Execute ();
			}

        }

        public void InitData()
        {
        }

        public void InitSchema()
        {
            CleanDB();
            LoadSchema();
        }
    }
}
