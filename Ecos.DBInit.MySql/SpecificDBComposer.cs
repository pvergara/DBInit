using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class SpecificDBComposer : ISpecificDBComposer
    {
        public virtual IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames)
        {
            return tableNames.
                Select(tableName =>
                    Script.From(string.Format("DELETE FROM {0};", tableName))
                );
        }

        public virtual Script ComposeActivateReferentialIntegrity()
        {
            return Script.From("SET @@foreign_key_checks = 1;");
        }

        public virtual Script ComposeDeactivateReferentialIntegrity()
        {
            return Script.From("SET @@foreign_key_checks = 0;");
        }

        public virtual IEnumerable<Script> ComposeScriptsDropTables(IEnumerable<string> tableNames)
        {
            return ComposeScriptsWithDropUsing("TABLE", tableNames);
        }

        public virtual IEnumerable<Script> ComposeScriptsDropViews(IEnumerable<string> viewNames)
        {
            return ComposeScriptsWithDropUsing("VIEW", viewNames);
        }

        public virtual IEnumerable<Script> ComposeScriptsDropStoredProcedures(IEnumerable<string> storedProcedureNames)
        {
            return ComposeScriptsWithDropUsing("PROCEDURE", storedProcedureNames);
        }

        public virtual IEnumerable<Script> ComposeScriptsDropFunctions(IEnumerable<string> functionNames)
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

