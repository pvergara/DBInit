using System.IO;

namespace Ecos.DBInit.Core.ScriptHelpers
{
	public interface IScriptFinder
	{
		StreamReader Find ();
	}
}

