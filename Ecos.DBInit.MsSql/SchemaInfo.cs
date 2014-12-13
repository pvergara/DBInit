using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.MsSql
{
    public class SchemaInfo : ISchemaInfo
    {
        const int FirstField = 0;

        private readonly SqlConnection _connection;
        private readonly IScriptExec _exec;

        private ICollection<string> _tables;
        private ICollection<string> _views;
        private ICollection<string> _procedures;
        private ICollection<string> _functions;

        public SchemaInfo(string connectionString, IScriptExec exec)
        {
            _connection = new SqlConnection(connectionString);
            _exec = exec;
        }

        private static string TablesScript
        {
            get { return "SELECT name FROM {0}.sys.sysobjects WHERE xtype='U';"; }
        }

        private static string ViewsScript
        {
            get { return "SELECT name FROM {0}.sys.sysobjects WHERE xtype='V';"; }
        }

        private static string SPsScript
        {
            get { return "SELECT name FROM {0}.sys.sysobjects WHERE xtype='P';"; }
        }

        private static string FunctionsScript
        {
            get { return "SELECT name FROM {0}.sys.sql_modules m INNER JOIN {0}.sys.objects o ON m.object_id=o.object_id where type_desc like '%function%';"; }
        }    

        //TODO: Refactor repited code
        private static bool IsSetted(ICollection<string> collection)
        {
            return collection != null;
        }

        //TODO: Refactor repited code
        private static ICollection<string> Init()
        {
            return new List<string>();
        }

        //TODO: Refactor repited code
        private static ICollection<string> ReturnFirstFieldAsString(IDataRecord reader, ICollection<string> collection)
        {
            collection.Add(reader.GetString(FirstField));
            return collection;
        }

        //TODO: Refactor repited code
        private Script ComposeScript(string script)
        {
            var str = string.Format(script, DatabaseName);
            return Script.From(str);
        }

        //TODO: Refactor repited code
        private ICollection<string> SetOnlyOnceTheCollectionByUsingFunction(ICollection<string> collectionIn, Func<string, Script> function, string script)
        {
            if (IsSetted(collectionIn)) return collectionIn;
            collectionIn = Init();
            var composedScipt = function(script);
            _exec.ExecuteAndProcess(composedScipt, collectionIn, ReturnFirstFieldAsString);
            return collectionIn;
        }


        public string DatabaseName
        {
            get { return _connection.Database; }
        }

        public IEnumerable<string> GetTables()
        {
            _tables = SetOnlyOnceTheCollectionByUsingFunction(_tables, ComposeScript, TablesScript);
            return _tables;
        }

        public IEnumerable<string> GetViews()
        {
            _views = SetOnlyOnceTheCollectionByUsingFunction(_views, ComposeScript, ViewsScript);
            return _views;
        }

        public IEnumerable<string> GetStoredProcedures()
        {
            _procedures = SetOnlyOnceTheCollectionByUsingFunction(_procedures, ComposeScript, SPsScript);
            return _procedures;
        }

        public IEnumerable<string> GetFunctions()
        {
            _functions = SetOnlyOnceTheCollectionByUsingFunction(_functions, ComposeScript, FunctionsScript);
            return _functions;

        }

    }
}
