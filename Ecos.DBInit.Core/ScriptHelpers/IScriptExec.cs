using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System;
using System.Data;

namespace Ecos.DBInit.Core.ScriptHelpers
{

    public interface IScriptExec:IDisposable
	{
        void ExecuteAndProcess<TValue>(Script script,ICollection<TValue> result, Func<IDataReader,ICollection<TValue>,ICollection<TValue>> function);
        void ExecuteAndProcess<TKey,TValue>(IDictionary<TKey, Script> indexedQueries, IDictionary<TKey, ICollection<TValue>> indexedResults, Func<IDataReader,TKey,ICollection<TValue>,ICollection<TValue>> functionOnEachQueryToEachResult);
        T ExecuteScalar<T>(Script script);
        void Execute(IEnumerable<Script> scripts);
	}

}

