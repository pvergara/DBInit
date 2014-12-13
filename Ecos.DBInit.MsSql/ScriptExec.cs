using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.MsSql
{
    public class ScriptExec : IScriptExec
    {
        private static DbConnection _executionConnection;
        private static DbTransaction _transaction;
        private readonly SqlConnection _connection;
        private readonly string _connectionString;
        private DbProviderFactory _factory;

        public ScriptExec(String connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        }

        //TODO: Refactor repited code
        public virtual void ExecuteAndProcess<TValue>(Script script, ICollection<TValue> result,
            Func<IDataReader, ICollection<TValue>, ICollection<TValue>> function)
        {
            const int firstItem = 0;

            var indexedScript = new Dictionary<int, Script> {{firstItem, script}};

            var indexedResult = new Dictionary<int, ICollection<TValue>> {{firstItem, result}};

            Func<IDataReader, int, ICollection<TValue>, ICollection<TValue>> par =
                (reader, index, ir) => function(reader, ir);
            ExecuteAndProcess(indexedScript, indexedResult, par);
        }

        public virtual void ExecuteAndProcess<TKey, TValue>(IDictionary<TKey, Script> indexedQueries,
            IDictionary<TKey, ICollection<TValue>> indexedResults,
            Func<IDataReader, TKey, ICollection<TValue>, ICollection<TValue>> functionOnEachQueryToEachResult)
        {
            foreach (var element in indexedQueries)
            {
                _connection.Open();
                var command = new SqlCommand(element.Value.Query, _connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var index = element.Key;
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
            _connection.Open();

            var command = new SqlCommand(script.Query, _connection);
            var result = (T) command.ExecuteScalar();

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
            var command = _factory.CreateCommand();
            command.CommandType = CommandType.Text;
            command.Connection = GetExecutionConnection();
            var qry = script.Query;
            var strCommands = Regex.Split(qry, @"\nGO");
            foreach (var strCommand in strCommands.Where(cmd => !String.IsNullOrEmpty(cmd.Trim())))
            {

                command.CommandType = CommandType.Text;
                command.CommandText = strCommand;
                command.Transaction = _transaction;
                command.ExecuteNonQuery();
            }
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

        public void Dispose()
        {
            TryCloseExecutionConnection();
            _connection.Dispose();
        }

        //TODO: Refactor repited code
        private static IDictionary<TKey, ICollection<TValue>> AddNewItemIfNotExists<TKey, TValue>(TKey index,
            IDictionary<TKey, ICollection<TValue>> setOfCollection)
        {
            ICollection<TValue> collection = new List<TValue>();
            if (!setOfCollection.ContainsKey(index))
            {
                setOfCollection.Add(index, collection);
            }
            return setOfCollection;
        }

        //TODO: Refactor repited code
        private static DbConnection GetExecutionConnection()
        {
            return _executionConnection;
        }

        //TODO: Refactor repited code
        private static bool IsOpennedExecutionConnection()
        {
            return _executionConnection != null;
        }

        //TODO: Refactor repited code
        private void OpenExecutionConnection()
        {
            _executionConnection = _factory.CreateConnection();
            _executionConnection.ConnectionString = _connectionString;
            _executionConnection.Open();
        }

        //TODO: Refactor repited code
        private static void BeginTransaction()
        {
            _transaction = _executionConnection.BeginTransaction();
        }

        //TODO: Refactor repited code
        private static void CommitTransaction()
        {
            _transaction.Commit();
        }

        //TODO: Refactor repited code
        private static void RollbackTransaction()
        {
            _transaction.Rollback();
        }

        //TODO: Refactor repited code
        private static void TryCloseExecutionConnection()
        {
            if (IsOpennedExecutionConnection())
                CloseExecutionConnection();
        }

        //TODO: Refactor repited code
        private static void CloseExecutionConnection()
        {
            _executionConnection.Close();
            _executionConnection.Dispose();
            _executionConnection = null;
        }
    }
}