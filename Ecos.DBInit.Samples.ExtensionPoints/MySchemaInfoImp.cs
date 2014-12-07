using Ecos.DBInit.MySql;
using Ecos.DBInit.Core.Interfaces;
using System.Collections.Generic;

namespace Ecos.DBInit.Samples.ExtensionPoints
{
    public class SchemaInfoNoDropSomeDBObjects:SchemaInfo
    {
        public SchemaInfoNoDropSomeDBObjects(string connectionString, IScriptExec exec)
            : base(connectionString, exec)
        {
        }

        public override IEnumerable<string> GetViews()
        {
            return new string[0];
        }

        public override IEnumerable<string> GetStoredProcedures()
        {
            return new string[0];
        }

        public override IEnumerable<string> GetFunctions()
        {
            return new string[0];
        }
        
    }
}