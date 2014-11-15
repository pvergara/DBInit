using System;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
	public class ScriptFinderOnEmbbededResource
	{
        private readonly String _assemblyName;
        private readonly ScriptType _scriptType;

        public ScriptFinderOnEmbbededResource(String assemblyName,ScriptType scriptType)
		{
            _scriptType = scriptType;
            _assemblyName = assemblyName;
		}

        public Container GetContainer()
		{
            return Container.From(String.Format ("{0}.Scripts.{1}", _assemblyName, _scriptType));
		}
	}
}

