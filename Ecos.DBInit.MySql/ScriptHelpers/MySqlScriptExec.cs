using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.MySql.ScriptHelpers
{

    public class MySqlScriptExec:IScriptExec
    {
        readonly MySqlConnection _connection;

        readonly string _connectionString;

        static MySqlConnection _executionConnection;
        static MySqlTransaction _transaction;

        public MySqlScriptExec(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new MySqlConnection(connectionString);
        }

        private static IDictionary<TKey, ICollection<TValue>> AddNewItemIfNotExists<TKey,TValue>(TKey index, IDictionary<TKey, ICollection<TValue>> setOfCollection)
        {
            ICollection<TValue> collection = new List<TValue>();
            if (!setOfCollection.ContainsKey(index))
            {
                setOfCollection.Add(index, collection);
            }
            return setOfCollection;
        }

        public void ExecuteAndProcess<TValue>(Script script, ICollection<TValue> result, Func<IDataReader,ICollection<TValue>,ICollection<TValue>> function)
        {
            const int firstItem = 0;

            var indexedScript = new Dictionary<int, Script>();
            indexedScript.Add(firstItem, script);

            var indexedResult = new Dictionary<int, ICollection<TValue>>();
            indexedResult.Add(firstItem, result);

            Func<IDataReader,int,ICollection<TValue>,ICollection<TValue>> par = (reader, index, ir) => function(reader, ir);
            ExecuteAndProcess<int,TValue>(indexedScript, indexedResult, par);
        }

        public void ExecuteAndProcess<TKey,TValue>(IDictionary<TKey, Script> indexedQueries, IDictionary<TKey, ICollection<TValue>> indexedResults, Func<IDataReader,TKey,ICollection<TValue>,ICollection<TValue>> functionOnEachQueryToEachResult)
        {
            foreach (KeyValuePair<TKey,Script> element in indexedQueries)
            {
                _connection.Open();
                var command = new MySqlCommand(element.Value.Query, _connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TKey index = element.Key;
                        indexedResults = AddNewItemIfNotExists(index, indexedResults);
                        indexedResults[index] = functionOnEachQueryToEachResult(reader, index, indexedResults[index]);
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
            var transaction = _connection.BeginTransaction();
            try {
                foreach (Script script in scripts)
                {
                    var command = new MySqlScript(_connection, script.Query);
                    command.Execute();
                }
                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
            } finally {
                _connection.Close();
            }
        }

        public void TryConnectionAndExecuteInsideTransaction(Script script)
        {
            if (!IsOpennedExecutionConnection())
            {
                OpenExecutionConnection();
                BeginTransaction();
            }
            var command = new MySqlScript(GetExecutionConnection(), script.Query);
            command.Execute();

        }

        static MySqlConnection GetExecutionConnection()
        {
            return MySqlScriptExec._executionConnection;
        }

        static bool IsOpennedExecutionConnection()
        {
            return MySqlScriptExec._executionConnection != null;
        }

        void OpenExecutionConnection()
        {
            MySqlScriptExec._executionConnection = new MySqlConnection(_connectionString);
            MySqlScriptExec._executionConnection.Open();
        }

        static void BeginTransaction()
        {
            MySqlScriptExec._transaction =  MySqlScriptExec._executionConnection.BeginTransaction();
        }

        public void CommitAndClose()
        {
            CommitTransaction();
            TryCloseExecutionConnection();
        }

        static void CommitTransaction()
        {
            MySqlScriptExec._transaction.Commit();
        }

        public void RollbackAndClose()
        {
            RollbackTransaction();
            TryCloseExecutionConnection();
        }

        void RollbackTransaction()
        {
            MySqlScriptExec._transaction.Rollback();
        }

        static void TryCloseExecutionConnection()
        {
            if (IsOpennedExecutionConnection())
                CloseExecutionConnection();
        }

        static void CloseExecutionConnection()
        {
            MySqlScriptExec._executionConnection.Close();
            MySqlScriptExec._executionConnection.Dispose();
            MySqlScriptExec._executionConnection = null;
        }

        public void Dispose()
        {
            TryCloseExecutionConnection();
            _connection.Dispose();
        }
    }
}

