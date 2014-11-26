using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Bootstrap
{
    public class UnitOfWorkOnCollection: IUnitOfWork
    {
        private readonly List<Script> _scripts;
        private readonly IScriptExec _helper;

        public UnitOfWorkOnCollection(IScriptExec helper)
        {
            _helper = helper;
            _scripts = new List<Script>();
        }

        public void Add(IEnumerable<Script> scripts)
        {
            _scripts.AddRange(scripts);
        }

        public void Flush()
        {
            _helper.Execute(_scripts);
            _scripts.Clear();
        }
    }

}