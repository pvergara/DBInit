using Ecos.DBInit.Core;
using System.Collections.Generic;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core.Model;
using MySql.Data.MySqlClient;

namespace Ecos.DBInit.MySql
{
    public class MySqlDBInit:IDBInit
	{
        readonly string _assemblyName;
        readonly MySqlConnection _connection;

        public MySqlDBInit(string connectionString,string assemblyName){
            _assemblyName = assemblyName;
            _connection = new MySqlConnection(connectionString);
        }

        public void InitData()
        {
        }

        public void InitSchema()
        {
            CleanDB();
            InitializeDB();
            ExecuteFromUoW();
        }


        private void CleanDB()
        {
            DeactivateReferentialIntegrity();
            DropDataBaseObjects();
        }

        void ExecuteFromUoW()
        {
            foreach (var sql in _scripts)
            {
                var script = new MySqlScript(_connection, sql.Query);
                script.Execute();
            }
            _scripts.Clear();
        }

        private void InitializeDB()
        {
            CreateDataBaseObjects();
            ActivateReferentialIntegrity();
        }

        void DeactivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 0;") };

            AddToUoW(scripts);
        }

        void DropDataBaseObjects()
        {
            var scripts = new List<Script>();

            scripts.AddRange(LoadDropAllSchemaObjectsScripts());

            AddToUoW(scripts);
        }
                 
        IEnumerable<Script> LoadDropAllSchemaObjectsScripts()
        {
            IEnumerable<Script> scriptsWithDropFunction = ComposeScriptsWithDroptAllSchemaObjects();
            return  ExecuteAndReturnFirstField(scriptsWithDropFunction);
        }

        IEnumerable<Script> ComposeScriptsWithDroptAllSchemaObjects()
        {
            var container = ScriptFinderFluentFactory.
                FromEmbeddedResource.
                    InitWith(GetType().Assembly.GetName().Name, ScriptType.Schema).
                GetContainer();

            return ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                    InitWith(GetType().Assembly.GetName().Name, container).
                GetScripts("_DATABASE_NAME_",_connection.Database);
        }


        void ActivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 1;") };

            AddToUoW(scripts);
        }

        void CreateDataBaseObjects()
        {
            var container = 
                ScriptFinderFluentFactory.
                FromEmbeddedResource.
                InitWith(_assemblyName, ScriptType.Schema).
                GetContainer();

            var scripts = 
                ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                InitWith(_assemblyName, container).
                GetScripts();

            AddToUoW(scripts);
        }

        List<Script> _scripts = new List<Script>();

        void AddToUoW(IEnumerable<Script> scripts)
        {
            _scripts.AddRange(scripts);
        }


        IEnumerable<Script> ExecuteAndReturnFirstField(IEnumerable<Script> scripts)
        {
            var result = new List<Script>();
            foreach (Script script in scripts)
            {
                _connection.Open();
                var command = new MySqlCommand(script.Query,_connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(Script.From(reader.GetString(0)));
                }
                _connection.Close();
            }
            return result;
        }

	}

}