using System.Collections.Generic;

namespace Ecos.DBInit.Core
{
	public interface ISchemaInfo
	{
        IEnumerable<string> GetTables();
        IEnumerable<string> GetViews();
        IEnumerable<string> GetStoredProcedures();
        IEnumerable<string> GetFunctions();
	}

}

