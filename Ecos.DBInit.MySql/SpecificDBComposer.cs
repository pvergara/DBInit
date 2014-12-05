using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class SpecificDBComposer:ISpecificDBComposer
    {
        public IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames)
        {
            return tableNames.
                Select(tableName => 
                    Script.From(string.Format("DELETE FROM {0};", tableName))
                );
        }

        public Script ComposeActivateReferentialIntegrity()
        {
            return Script.From("SET @@foreign_key_checks = 1;");
        }

        public Script ComposeDeactivateReferentialIntegrity()
        {
            return Script.From("SET @@foreign_key_checks = 0;");
        }

        public IEnumerable<Script> ComposeScriptsDropTables(IEnumerable<string> tableNames)
        {
            return ComposeScriptsWithDropUsing("TABLE", tableNames);
        }

        public IEnumerable<Script> ComposeScriptsDropViews(IEnumerable<string> viewNames)
        {
            return ComposeScriptsWithDropUsing("VIEW", viewNames);
        }

        public IEnumerable<Script> ComposeScriptsDropStoredProcedures(IEnumerable<string> storedProcedureNames)
        {
            return ComposeScriptsWithDropUsing("PROCEDURE", storedProcedureNames);
        }

        public IEnumerable<Script> ComposeScriptsDropFunctions(IEnumerable<string> functionNames)
        {
            return ComposeScriptsWithDropUsing("FUNCTION", functionNames);
        }

        private static IEnumerable<Script> ComposeScriptsWithDropUsing(string typeOfObject, IEnumerable<string> objectNames)
        {
            return objectNames.
                Select(objectName => 
                    Script.From(string.Format("DROP {0} IF EXISTS {1};", typeOfObject, objectName))
                );

        }
    }
}

