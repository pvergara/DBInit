using NUnit.Framework;
using Ecos.DBInit.Core.ScriptsHelpers;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.Helpers
{
    [TestFixture]
    public class ScriptLoaderOnEmbbededResourceTest
    {
        [Test]
        public void GivenThenAssemblyWhenIInvokeWithTheContainerThenItReturnsMeAStreamReaderCollectionWithAllTheScriptsQueriesFoundedOnTheContainer()
        {
            const string assemblyName = "Ecos.DBInit.Samples.ProjectWithAMySQLDataBase";
            var container = ScriptFinderFluentFactory.
                FromEmbeddedResource.
                    InitWith(assemblyName, ScriptType.Schema).
                GetContainer();

            var scripts = ScriptLoaderFluentFactory.
                    FromEmbeddedResource.
                        InitWith(assemblyName,container).
                    GetScripts();


            Assert.That(scripts,Is.Not.Null);
            Assert.That(scripts,Is.InstanceOf<IEnumerable<Script>>());
        }
    }
}