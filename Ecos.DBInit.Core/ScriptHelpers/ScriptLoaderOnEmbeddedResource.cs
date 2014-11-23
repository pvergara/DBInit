using System.IO;
using System.Reflection;
using Ecos.DBInit.Core.Model;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Ecos.DBInit.Core.ScriptHelpers
{
    public class ScriptLoaderOnEmbeddedResource
    {
        private readonly string _assemblyName;
        private readonly Container _container;

        public ScriptLoaderOnEmbeddedResource(string assemblyName,Container container)
        {
            _assemblyName = assemblyName;
            _container = container;
        }

        public IEnumerable<Script> GetScripts()
        {
            return GetScripts("", "");
        }

        public IEnumerable<Script> GetScripts(string oldValue,string newValue)
        {
            var assembly = Assembly.ReflectionOnlyLoad(_assemblyName);
            var resourcesName = assembly.GetManifestResourceNames().Where(s=>s.Contains(_container.Path));
            ICollection<Script> scripts = new List<Script>();
            foreach (string resourceName in resourcesName)
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                var text = new StreamReader(stream).ReadToEnd();
                if (!String.IsNullOrEmpty(oldValue) && !String.IsNullOrEmpty(newValue))
                    text = text.Replace(oldValue, newValue);
                var script = Script.From(text);
                scripts.Add(script);
            }

            return scripts;
        }
    }
}

