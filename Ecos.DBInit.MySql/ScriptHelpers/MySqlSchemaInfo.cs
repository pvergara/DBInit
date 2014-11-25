using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.Data;
using System;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core;

namespace Ecos.DBInit.MySql.ScriptHelpers
{
    public class MySqlSchemaInfo:ISchemaInfo,IDisposable
    {
        const int FirstField = 0;

        readonly MySqlConnection _connection;
        readonly IScriptExec _exec;

        ICollection<string> _tables;
        ICollection<string> _views;
        ICollection<string> _storedProcedures;
        ICollection<string> _functions;

        private static string TablesScript
        {
            get { return "SELECT table_name FROM information_schema.tables WHERE table_schema = '{0}' AND TABLE_TYPE = 'BASE TABLE';";}
        }

        private static string ViewsScript
        {
            get { return "SELECT table_name FROM information_schema.tables WHERE table_schema = '{0}' AND TABLE_TYPE = 'VIEW';";}
        }

        private static string StoredProceduresScript
        {
            get { return "SELECT routine_name FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '{0}' AND ROUTINE_TYPE = 'PROCEDURE';";}
        }

        private static string FunctionsScript
        {
            get { return "SELECT routine_name FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '{0}' AND ROUTINE_TYPE = 'FUNCTION';";}
        }

        public MySqlSchemaInfo(string connectionString,IScriptExec exec)
        {
            _connection = new MySqlConnection(connectionString);
            _exec = exec;
        }

        private static bool IsSetted(ICollection<string> collection)
        {
            return collection!=null;
        }

        private static ICollection<string> Init()
        {
            return new List<string>();
        }

        private static ICollection<string> ReturnFirstFieldAsString(IDataRecord reader, ICollection<string> collection)
        {
            collection.Add(reader.GetString(FirstField));
            return collection;
        }

        private Script ComposeScript(string script)
        {
            var str = string.Format(script, DatabaseName);
            return Script.From(str);
        }

        private ICollection<string> SetOnlyOnceTheCollectionByUsingFunction(ICollection<string> collectionIn,Func<string,Script> function,string script)
        {
            if (!IsSetted(collectionIn))
            {
                collectionIn = Init();
                var composedScipt = function(script);
                _exec.ExecuteAndProcess<string>(composedScipt, collectionIn, ReturnFirstFieldAsString);
            }
            return collectionIn;
        }

        public string DatabaseName
        {
            get { return _connection.Database; }
        }

        public IEnumerable<string> GetTables()
        {
            _tables = SetOnlyOnceTheCollectionByUsingFunction(_tables, ComposeScript,TablesScript);
            return _tables;
        }

        public IEnumerable<string> GetViews()
        {
            _views = SetOnlyOnceTheCollectionByUsingFunction(_views,ComposeScript,ViewsScript);
            return _views;
        }

        public IEnumerable<string> GetStoredProcedures()
        {
            _storedProcedures = SetOnlyOnceTheCollectionByUsingFunction(_storedProcedures,ComposeScript,StoredProceduresScript);
            return _storedProcedures;
        }

        public IEnumerable<string> GetFunctions()
        {
            _functions = SetOnlyOnceTheCollectionByUsingFunction(_functions,ComposeScript,FunctionsScript);
            return _functions;
        }
            
        public void Dispose()
        {
            _exec.Dispose();
        }
    }
}

