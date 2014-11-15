using System.Collections.Generic;
using System.IO;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
	public class ScriptAppender:IScriptAppender
	{
        public IEnumerable<Script> GetScriptsFrom (IEnumerable<StreamReader> streams)
		{
            var scripts = new List<Script>();
            foreach(StreamReader stream in streams)
                scripts.Add(Script.From(stream.ReadToEnd()));
            return scripts;
		}
    }
}

