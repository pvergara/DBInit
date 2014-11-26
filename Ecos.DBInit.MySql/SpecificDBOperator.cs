using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class SpecificDBOperator:ISpecificDBOperator
    {
        public IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> tableNames)
        {
            return tableNames.
                Select(tableName => 
                    Script.From(string.Format("DELETE FROM {0};", tableName))
                );
        }
    }
}

