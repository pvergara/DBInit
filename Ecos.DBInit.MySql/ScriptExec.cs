using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.MySql
{
    public class ScriptExec:IScriptExec
    {
        private readonly MySqlConnection _connection;
        private readonly string _connectionString;

        private static MySqlConnection _executionConnection;
        private static MySqlTransaction _transaction;

        public ScriptExec(string connectionString)
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

        private static MySqlConnection GetExecutionConnection()
        {
            return ScriptExec._executionConnection;
        }

        private static bool IsOpennedExecutionConnection()
        {
            return ScriptExec._executionConnection != null;
        }

        private void OpenExecutionConnection()
        {
            ScriptExec._executionConnection = new MySqlConnection(_connectionString);
            ScriptExec._executionConnection.Open();
        }

        private static void BeginTransaction()
        {
            ScriptExec._transaction = ScriptExec._executionConnection.BeginTransaction();
        }

        private static void CommitTransaction()
        {
            ScriptExec._transaction.Commit();
        }

        private static void RollbackTransaction()
        {
            ScriptExec._transaction.Rollback();
        }

        private static void TryCloseExecutionConnection()
        {
            if (IsOpennedExecutionConnection())
                CloseExecutionConnection();
        }

        private static void CloseExecutionConnection()
        {
            ScriptExec._executionConnection.Close();
            ScriptExec._executionConnection.Dispose();
            ScriptExec._executionConnection = null;
        }

        public virtual void ExecuteAndProcess<TValue>(Script script, ICollection<TValue> result, Func<IDataReader,ICollection<TValue>,ICollection<TValue>> function)
        {
            const int firstItem = 0;

            var indexedScript = new Dictionary<int, Script>();
            indexedScript.Add(firstItem, script);

            var indexedResult = new Dictionary<int, ICollection<TValue>>();
            indexedResult.Add(firstItem, result);

            Func<IDataReader,int,ICollection<TValue>,ICollection<TValue>> par = (reader, index, ir) => function(reader, ir);
            ExecuteAndProcess<int,TValue>(indexedScript, indexedResult, par);
        }

        public virtual void ExecuteAndProcess<TKey,TValue>(IDictionary<TKey, Script> indexedQueries, IDictionary<TKey, ICollection<TValue>> indexedResults, Func<IDataReader,TKey,ICollection<TValue>,ICollection<TValue>> functionOnEachQueryToEachResult)
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

        public virtual T ExecuteScalar<T>(Script script)
        {
            T result;
            _connection.Open();

            var command = new MySqlCommand(script.Query, _connection);
            result = (T)command.ExecuteScalar();

            _connection.Close();

            return result;

        }

        public virtual void TryConnectionAndExecuteInsideTransaction(Script script)
        {
            if (!IsOpennedExecutionConnection())
            {
                OpenExecutionConnection();
                BeginTransaction();
            }
            var command = new MySqlScript(GetExecutionConnection(), script.Query);
            command.Execute();

        }

        public virtual void CommitAndClose()
        {
            CommitTransaction();
            TryCloseExecutionConnection();
        }

        public virtual void RollbackAndClose()
        {
            RollbackTransaction();
            TryCloseExecutionConnection();
        }

        public virtual void Dispose()
        {
            TryCloseExecutionConnection();
            _connection.Dispose();
        }
    }
}