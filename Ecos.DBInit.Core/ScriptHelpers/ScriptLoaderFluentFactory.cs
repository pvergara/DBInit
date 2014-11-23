using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.ScriptHelpers
{
    public class ScriptLoaderFluentFactory
    {
        private ScriptLoaderFluentFactory(){}

        public static ScriptLoaderFluentFactory FromEmbeddedResource
        {
            get
            { 
                return new ScriptLoaderFluentFactory(); 
            }
        }

        public ScriptLoaderOnEmbeddedResource InitWith(string assemblyName, Container container)
        {
            return new ScriptLoaderOnEmbeddedResource(assemblyName, container);
        }
    }

}