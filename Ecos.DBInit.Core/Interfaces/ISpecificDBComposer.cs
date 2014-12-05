using System.Collections.Generic;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.Interfaces
{
    public interface ISpecificDBComposer
    {
        IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames);       

        Script ComposeActivateReferentialIntegrity();
        Script ComposeDeactivateReferentialIntegrity();

        IEnumerable<Script> ComposeScriptsDropTables(IEnumerable<string> tableNames);       
        IEnumerable<Script> ComposeScriptsDropViews(IEnumerable<string> viewNames);       
        IEnumerable<Script> ComposeScriptsDropStoredProcedures(IEnumerable<string> storedProcedureNames);
        IEnumerable<Script> ComposeScriptsDropFunctions(IEnumerable<string> functionNames);       
    }
}