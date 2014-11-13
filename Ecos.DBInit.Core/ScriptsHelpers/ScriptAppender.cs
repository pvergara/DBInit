using System.Collections.Generic;

namespace Ecos.DBInit.Core.ScriptHelpers
{
	public class ScriptAppender:IScriptAppender
	{
		readonly IScriptFinder _finder;

		public ScriptAppender (IScriptFinder finder)
		{
			_finder = finder;
		}

		public void Append (ICollection<string> scripts)
		{
			var stream = _finder.Find ();
			scripts.Add(stream.ReadToEnd());
		}
	}
}

