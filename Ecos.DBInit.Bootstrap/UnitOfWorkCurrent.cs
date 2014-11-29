using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using System;

namespace Ecos.DBInit.Bootstrap
{
    public class UnitOfWorkCurrent: IUnitOfWork
    {
        private readonly List<Script> _scripts;
        private readonly IScriptExec _specificExec;

        public UnitOfWorkCurrent(IScriptExec helper)
        {
            _specificExec = helper;
            _scripts = new List<Script>();
        }

        public void Add(IEnumerable<Script> scripts)
        {
            _scripts.AddRange(scripts);
        }

        public void Flush()
        {
            try{
                AtomicExecution();
            }catch(Exception e){
                _specificExec.RollbackAndClose();
                throw e;
            }finally{
                _scripts.Clear();
            }
        }

        void AtomicExecution()
        {
            foreach (Script script in _scripts)
            {
                _specificExec.TryConnectionAndExecuteInsideTransaction(script);
            }
            _specificExec.CommitAndClose();
        }

        public void Dispose()
        {
            _specificExec.Dispose();
        }
    }

}