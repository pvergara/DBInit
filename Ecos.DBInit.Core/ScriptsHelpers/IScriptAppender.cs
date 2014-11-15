using System.Collections.Generic;
using System.IO;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
	public interface IScriptAppender
	{
        IEnumerable<Script> GetScriptsFrom(IEnumerable<StreamReader> streams);
	}
}