using Ecos.DBInit.Core.ScriptHelpers;
using System;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptHelpers
{
    public class ScriptFinderFluentFactory
    {

        private ScriptFinderFluentFactory()
        {
        }

        public static ScriptFinderFluentFactory FromEmbeddedResource
        {
            get
            { 
                return new ScriptFinderFluentFactory();
            }
        }

        public ScriptFinderOnEmbbededResource InitWith(string assemblyName, ScriptType scriptType)
        {
            return new ScriptFinderOnEmbbededResource(assemblyName, scriptType);
        }
    }

}