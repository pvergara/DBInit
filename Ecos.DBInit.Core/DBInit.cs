using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.IO;

//TODO: Maybe not all the developers need to clean ALL the schema, maybe they only need to clean some specific tables (or maybe have limited permissions)
//TODO: Each DataBase Engine could use different command to clean the database/schema!!!!
//TODO: BEWARE WITH REFERENTIAL INTEGRITY
using System.Text;
using Ecos.DBInit.Core.ScriptsHelpers;
using Ecos.DBInit.Core.Model;
using MySql.Data.MySqlClient;


namespace Ecos.DBInit.Core
{
    public class DBInit: IDBInit
    {
        private IDbConnection _dbConnection;
        private string _dbName;
        private string _connectionString;
        private string _assemblyName;        

        public DBInit(string providerInvariantName, string connectionString,string assemblyName)
        {
            _assemblyName = assemblyName;
            _connectionString = connectionString;
            var dbProviderFactory = DbProviderFactories.GetFactory(providerInvariantName);
            _dbConnection = dbProviderFactory.CreateConnection();

            _dbConnection.ConnectionString = connectionString;
            _dbName = _dbConnection.Database;
        }

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

        private ICollection<string> SingleExecuteQueryGettingFirstFieldAsString (string queryWithDropTablesClauses,ICollection<string> scripts)
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
            ICollection<string> scripts = new List<string>();
            
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

        private void LoadSchema()
        {
            var container = ScriptFinderFluentFactory.FromEmbeddedResource.InitWith(_assemblyName,ScriptType.Schema).GetContainer();
            var scripts = ScriptLoaderFluentFactory.FromEmbeddedResource.InitWith(_assemblyName,container).GetScripts();

            var cn = new MySqlConnection(_connectionString);
            foreach (var sql in scripts)
            {
                var script = new MySqlScript(cn, sql.Query);
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
