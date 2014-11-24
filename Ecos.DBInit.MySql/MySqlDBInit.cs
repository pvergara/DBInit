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
        private readonly string _assemblyName;
        private readonly List<Script> _scripts = new List<Script>();
        private readonly MySqlScriptHelper _helper;
        private readonly MySqlSchemaInfo _schemaInfo;

        public MySqlDBInit(string connectionString,string assemblyName){
            _assemblyName = assemblyName;
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _helper = new MySqlScriptHelper(connectionString);
            //TODO: WHAT ABOUT THE INJECTION!!!!
            _schemaInfo = new MySqlSchemaInfo(connectionString,_helper);
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

        private void CleanData()
        {
            DeactivateReferentialIntegrity();
            CleanEachTable();
        }

        private void CleanEachTable()
        {
            var scripts = new List<Script>();

            scripts.AddRange(ComposeScriptsDeleteEachTable());

            AddToUoW(scripts);
        }

        private IEnumerable<Script> ComposeScriptsDeleteEachTable()
        {
            return _schemaInfo.GetTables().
                Select(tableName => 
                    Script.From(string.Format("DELETE FROM {0};", tableName))
                );
        }
            
        private static Dictionary<int, Script> FromCollectionToDictionary(IEnumerable<Script> collection)
        {
            return collection.
                Select((element, index) => 
                    new {
                        element,
                        index
                    }).
                ToDictionary(keySelector => keySelector.index, valueSelector => valueSelector.element);
        }
            
        private ICollection<string> AddFirstFieldIntoStringCollectionField(IDataRecord reader,int index,ICollection<string> tableNames){
            tableNames.Add(reader.GetString(0));
            return tableNames;
        }

        private void AddData()
        {
            LoadDataScripts();
            ActivateReferentialIntegrity();
        }

        private void LoadDataScripts()
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

        private void DeactivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 0;") };

            AddToUoW(scripts);
        }

        private void DropDataBaseObjects()
        {
            var scripts = new List<Script>();

            scripts.AddRange(GetDropAllSchemaObjectsScripts());

            AddToUoW(scripts);
        }
                 
        private IEnumerable<Script> GetDropAllSchemaObjectsScripts()
        {
            var dropAllObjectsScript = new List<Script>();

            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("TABLE",_schemaInfo.GetTables()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("VIEW",_schemaInfo.GetViews()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("PROCEDURE",_schemaInfo.GetStoredProcedures()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("FUNCTION",_schemaInfo.GetFunctions()));

            return dropAllObjectsScript;
        }

        private IEnumerable<Script> ComposeScriptsWithDropUsing(string typeOfObject, IEnumerable<string> objectNames)
        {
            return objectNames.
                Select(objectName => 
                    Script.From(string.Format("DROP {0} IF EXISTS {1};", typeOfObject,objectName))
                );

        }

        private static ICollection<Script> AddFirstFieldIntoCollection(IDataRecord reader, int index,ICollection<Script> collection)
        {
            collection.Add(Script.From(reader.GetString(0)));
            return collection;
        }
            
        private void ActivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 1;") };

            AddToUoW(scripts);
        }

        private void CreateDataBaseObjects()
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

        private void AddToUoW(IEnumerable<Script> scripts)
        {
            _scripts.AddRange(scripts);
        }

        private void ExecuteFromUoW()
        {
            _helper.Execute(_scripts);
            _scripts.Clear();
        }
	}
}