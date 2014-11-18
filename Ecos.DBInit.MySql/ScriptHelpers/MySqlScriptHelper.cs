using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.MySql.ScriptHelpers
{

    public class MySqlScriptHelper:IDisposable
    {
        readonly MySqlConnection _connection;

        Action<IDataReader> _function;

        public MySqlScriptHelper(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);

        }

        public void ExecuteAndProcess(Script script, Action<IDataReader> function)
        {
            _function = function;

            var dictionary = new Dictionary<int, Script>();
            dictionary.Add(0, script);
            ExecuteAndProcess(dictionary,ProcessTheQuery);

        }

        void ProcessTheQuery(IDataReader reader, int scriptIndex)
        {
            _function(reader);
        }

        public void ExecuteAndProcess<T>(IDictionary<T, Script> queries, Action<IDataReader,T> functionOnEachQuery)
        {
            foreach (KeyValuePair<T,Script> element in queries)
            {
                _connection.Open();
                var command = new MySqlCommand(element.Value.Query,_connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        functionOnEachQuery(reader,element.Key);
                    }
                    reader.Close();
                }
                _connection.Close();
            }

        }

        public T ExecuteScalar<T>(Script script)
        {
            T result;
            _connection.Open();

            var command = new MySqlCommand(script.Query, _connection);
            result = (T)command.ExecuteScalar();

            _connection.Close();

            return result;

        }

        public void Execute(IEnumerable<Script> scripts)
        {
            _connection.Open();
            foreach (Script script in scripts)
            {
                var command = new MySqlScript(_connection,script.Query);
                command.Execute();
            }
            _connection.Close();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}

