using NUnit.Framework;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test.Integration
{
    [TestFixture]
    public class ScriptLoaderOnEmbbededResourceTest
    {
        [Test]
        public void GivenThenAssemblyWhenIInvokeWithTheContainerThenItReturnsMeAStreamReaderCollectionWithAllTheScriptsQueriesFoundedOnTheContainer()
        {
            string assemblyName = SakilaDbOM.SampleProjectAssemblyName;
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