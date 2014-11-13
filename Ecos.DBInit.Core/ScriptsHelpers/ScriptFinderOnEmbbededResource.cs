using System;
using System.Reflection;
using System.IO;

namespace Ecos.DBInit.Core.ScriptHelpers
{
	public class ScriptFinderOnEmbbededResource: IScriptFinder
	{
		private readonly Assembly _assembly;
		private readonly string _resourceName;
		private readonly ScriptType _scriptType;

		public ScriptFinderOnEmbbededResource(Assembly assembly,string resourceName,ScriptType type)
		{
			_scriptType = type;
			_resourceName = resourceName;
			_assembly = assembly;
		}

		public StreamReader Find ()
		{
			var resourcePath = String.Format ("{0}.Scripts.{1}.{2}", _assembly.GetName().Name, _scriptType, _resourceName);
			var stream = _assembly.GetManifestResourceStream (resourcePath);
			return new StreamReader (stream);
		}
	}
}

