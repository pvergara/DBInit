using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;

namespace Ecos.DBInit.Wire.Test.MockImplementations
{

    public class MyIScriptExecImp : IScriptExec
    {
        public bool ExecuteAndProcessTValueInvoked { get; private set; }
        public bool ExecuteAndProcessTKeyTValueInvoked { get; private set; }
        public bool ExecuteScalarInvoked { get; private set; }
        public bool TryConnectionAndExecuteInsideTransactionInvoked { get; private set; }
        public bool CommitAndCloseInvoked { get; private set; }
        public bool RollbackAndCloseInvoked { get; private set; }
        public bool DisposeInvoked { get; private set; }

        public MyIScriptExecImp()
        {
            ExecuteAndProcessTValueInvoked = false;
            ExecuteAndProcessTKeyTValueInvoked = false;
            ExecuteScalarInvoked = false;
            TryConnectionAndExecuteInsideTransactionInvoked = false;
            CommitAndCloseInvoked = false;
            RollbackAndCloseInvoked = false;
            DisposeInvoked = false;
        }

        public void ExecuteAndProcess<TValue>(Script script, ICollection<TValue> result, Func<IDataReader, ICollection<TValue>, ICollection<TValue>> function)
        {
            ExecuteAndProcessTValueInvoked = true;
        }

        public void ExecuteAndProcess<TKey, TValue>(IDictionary<TKey, Script> indexedQueries, IDictionary<TKey, ICollection<TValue>> indexedResults, Func<IDataReader, TKey, ICollection<TValue>, ICollection<TValue>> functionOnEachQueryToEachResult)
        {
            ExecuteAndProcessTKeyTValueInvoked = true;
        }

        public T ExecuteScalar<T>(Script script)
        {
            ExecuteScalarInvoked = true;
            object result = 0;
            return (T)result;
        }

        public void TryConnectionAndExecuteInsideTransaction(Script scripts)
        {
            TryConnectionAndExecuteInsideTransactionInvoked = true;
        }

        public void CommitAndClose()
        {
            CommitAndCloseInvoked = true;
        }

        public void RollbackAndClose()
        {
            RollbackAndCloseInvoked = true;
        }

        public void Dispose()
        {
            DisposeInvoked = true;
        }
    }

}