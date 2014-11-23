using Ecos.DBInit.Core;
using System.Collections.Generic;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Data;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class MySqlDBInit:IDBInit
	{
        readonly string _databaseName;
        readonly string _assemblyName;
        readonly List<Script> _scripts = new List<Script>();
        readonly MySqlScriptHelper _helper;
        readonly MySqlSchemaInfo _schemaInfo;

        public MySqlDBInit(string connectionString,string assemblyName){
            _assemblyName = assemblyName;
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _helper = new MySqlScriptHelper(connectionString);
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _schemaInfo = new MySqlSchemaInfo(connectionString,_helper);
            _databaseName = _schemaInfo.DatabaseName;
        }

        public void InitData()
        {
            CleanData();
            AddData();
            ExecuteFromUoW();
        }

        public void InitSchema()
        {
            CleanDB();
            InitializeDB();
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

            scripts.AddRange(ComposeScriptsDeleteEachTable());

            AddToUoW(scripts);
        }

        IEnumerable<Script> ComposeScriptsDeleteEachTable()
        {
            return GetEachTableName().
                Select(tableName => 
                    Script.From(string.Format("DELETE FROM {0};", tableName))
                );
        }

        IEnumerable<string> GetEachTableName()
        {
            return _schemaInfo.GetTables();
        }

        static Dictionary<int, Script> FromCollectionToDictionary(IEnumerable<Script> collection)
        {
            return collection.
                Select((element, index) => 
                    new {
                        element,
                        index
                    }).
                ToDictionary(keySelector => keySelector.index, valueSelector => valueSelector.element);
        }
            
        ICollection<string> AddFirstFieldIntoStringCollectionField(IDataRecord reader,int index,ICollection<string> tableNames){
            tableNames.Add(reader.GetString(0));
            return tableNames;
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

            scripts.AddRange(GetDropAllSchemaObjectsScripts());

            AddToUoW(scripts);
        }
                 
        IEnumerable<Script> GetDropAllSchemaObjectsScripts()
        {
            var dropAllObjectsScript = new List<Script>();

            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("TABLE",_schemaInfo.GetTables()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("VIEW",_schemaInfo.GetViews()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("PROCEDURE",_schemaInfo.GetStoredProcedures()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("FUNCTION",_schemaInfo.GetFunctions()));

            return dropAllObjectsScript;
        }

        IEnumerable<Script> ComposeScriptsWithDropUsing(string typeOfObject, IEnumerable<string> objectNames)
        {
            return objectNames.
                Select(objectName => 
                    Script.From(string.Format("DROP {0} IF EXISTS {1};", typeOfObject,objectName))
                );

        }

        static ICollection<Script> AddFirstFieldIntoCollection(IDataRecord reader, int index,ICollection<Script> collection)
        {
            collection.Add(Script.From(reader.GetString(0)));
            return collection;
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