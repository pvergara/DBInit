using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
	public interface IScriptFinder
	{
        Container Find(ScriptType type);
	}
}

