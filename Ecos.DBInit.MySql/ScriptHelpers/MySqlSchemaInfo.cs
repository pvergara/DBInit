using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.Data;
using System;
using Ecos.DBInit.Core.ScriptHelpers;

namespace Ecos.DBInit.MySql.ScriptHelpers
{
    public class MySqlSchemaInfo:IDisposable
    {
        const int FirstField = 0;

        readonly MySqlConnection _connection;
        readonly IScriptExec _exec;
        ICollection<string> _tables;

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

        Script ComposeGetTablesScript()
        {
            var str = string.Format("SELECT table_name FROM information_schema.tables WHERE table_schema = '{0}' AND TABLE_TYPE = 'BASE TABLE';", DatabaseName);
            var script = Script.From(str);
            return script;
        }

        public string DatabaseName
        {
            get { return _connection.Database; }
        }

        public IEnumerable<string> GetTables()
        {
            if (!IsSetted(_tables))
            {
                _tables = Init();

                var script = ComposeGetTablesScript();

                _exec.ExecuteAndProcess<string>(script, _tables, ReturnFirstFieldAsString);
            }
            return _tables;
        }
            
        public void Dispose()
        {
            _exec.Dispose();
        }
    }
}

