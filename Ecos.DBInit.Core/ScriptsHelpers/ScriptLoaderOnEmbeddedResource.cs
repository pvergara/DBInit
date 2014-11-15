using System.IO;
using System.Reflection;
using Ecos.DBInit.Core.Model;
using System.Linq;
using System.Collections.Generic;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
    public class ScriptLoaderOnEmbeddedResource:IScriptLoader
    {
        private readonly string _assemblyName;

        public ScriptLoaderOnEmbeddedResource(string assemblyName)
        {
            _assemblyName = assemblyName;
        }

        public IEnumerable<StreamReader> Load(Container container)
        {
            var assembly = Assembly.ReflectionOnlyLoad(_assemblyName);
            var resourcesName = assembly.GetManifestResourceNames().Where(s=>s.Contains(container.Path));
            ICollection<StreamReader> streams = new List<StreamReader>();
            foreach (string resourceName in resourcesName)
            {
                streams.Add(new StreamReader(assembly.GetManifestResourceStream(resourceName)));
            }

            return streams.ToArray();
        }
    }
}

