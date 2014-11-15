using System;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
	public class ScriptFinderOnEmbbededResource: IScriptFinder
	{
        private readonly String _assemblyName;

        public ScriptFinderOnEmbbededResource(String assemblyName)
		{
            _assemblyName = assemblyName;
		}

        public Container Find (ScriptType type)
		{
            return Container.From(String.Format ("{0}.Scripts.{1}", _assemblyName, type));
		}
	}
}

