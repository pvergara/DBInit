using System.Collections.Generic;

namespace Ecos.DBInit.Core.ScriptHelpers
{
	public interface IScriptAppender
	{
		void Append(ICollection<string> scripts);
	}
}