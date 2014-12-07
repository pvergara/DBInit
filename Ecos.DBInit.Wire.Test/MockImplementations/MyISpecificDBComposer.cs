using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.MockImplementations
{
    public class MyISpecificDBComposer : ISpecificDBComposer
	{
        public IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames)
        {
            throw new System.NotImplementedException();
        }
        public Script ComposeActivateReferentialIntegrity()
        {
            throw new System.NotImplementedException();
        }
        public Script ComposeDeactivateReferentialIntegrity()
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<Script> ComposeScriptsDropTables(IEnumerable<string> tableNames)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<Script> ComposeScriptsDropViews(IEnumerable<string> viewNames)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<Script> ComposeScriptsDropStoredProcedures(IEnumerable<string> storedProcedureNames)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<Script> ComposeScriptsDropFunctions(IEnumerable<string> functionNames)
        {
            throw new System.NotImplementedException();
        }
	}

}

