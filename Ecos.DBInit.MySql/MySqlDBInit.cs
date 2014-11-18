using Ecos.DBInit.Core;
using System.Collections.Generic;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core.Model;
using MySql.Data.MySqlClient;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Data;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class MySqlDBInit:IDBInit
	{
        readonly string _assemblyName;
        readonly MySqlConnection _connection;
        readonly List<Script> _scripts = new List<Script>();
        readonly ICollection<Script> _dropAllSchemaObjectsScripts = new List<Script>();
        readonly MySqlScriptHelper _helper;
        readonly ICollection<string> _tableNames = new List<string>();

        public MySqlDBInit(string connectionString,string assemblyName){
            _assemblyName = assemblyName;
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _connection = new MySqlConnection(connectionString);
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _helper = new MySqlScriptHelper(connectionString);
        }

        public void InitData()
        {
            CleanData();
            AddData();
            ExecuteFromUoW();
        }

        void CleanData()
        {
            DeactivateReferentialIntegrity();
            CleanEachTable();
        }

        void CleanEachTable()
        {
            var scripts = new List<Script>();

            scripts.AddRange(GetDeleteEachTableScripts());

            AddToUoW(scripts);
        }

        IEnumerable<Script> GetDeleteEachTableScripts()
        {
            IEnumerable<Script> enumerable = GetEachTableName().
                Select(tableName => Script.From(string.Format("DELETE FROM {0};", tableName)));
            return enumerable;
        }

        IEnumerable<string> GetEachTableName()
        {
            var container = ScriptFinderFluentFactory.
                FromEmbeddedResource.
                InitWith(GetType().Assembly.GetName().Name, ScriptType.Data).
                GetContainer();

            var scriptsWithTableNames = ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                InitWith(GetType().Assembly.GetName().Name, container).
                GetScripts("_DATABASE_NAME_",_connection.Database);

            IDictionary<int, Script> scriptMap = scriptsWithTableNames.Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => x.s);
            _helper.ExecuteAndProcess(scriptMap,AddFirstFieldIntoStringCollectionField);
            return _tableNames;
        }
            
        void AddFirstFieldIntoStringCollectionField(IDataRecord reader,int index){
            _tableNames.Add(reader.GetString(0));
        }

        void AddData()
        {
            LoadDataScripts();
            ActivateReferentialIntegrity();
        }

        void LoadDataScripts()
        {
            var container = 
                ScriptFinderFluentFactory.
                FromEmbeddedResource.
                InitWith(_assemblyName, ScriptType.Data).
                GetContainer();

            var scripts = 
                ScriptLoaderFluentFactory.
                FromEmbeddedResource.
                InitWith(_assemblyName, container).
                GetScripts();

            AddToUoW(scripts);
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
            IDictionary<int, Script> dictionary = scriptsWithDropFunction.Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => x.s);
            _helper.ExecuteAndProcess(dictionary,AddFirstFieldIntoScriptMap);
            return _dropAllSchemaObjectsScripts;
        }

        void AddFirstFieldIntoScriptMap(IDataRecord reader, int index)
        {
            _dropAllSchemaObjectsScripts.Add(Script.From(reader.GetString(0)));
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

        void AddToUoW(IEnumerable<Script> scripts)
        {
            _scripts.AddRange(scripts);
        }

        void ExecuteFromUoW()
        {
            _helper.Execute(_scripts);
            _scripts.Clear();
        }
	}

}