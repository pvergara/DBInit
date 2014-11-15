using NUnit.Framework;
using Ecos.DBInit.Core.ScriptsHelpers;
using Ecos.DBInit.Core.Model;
using System.IO;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.Helpers
{
    [TestFixture]
    public class ScriptLoaderOnEmbbededResourceTest
    {
        [Test]
        public void GivenThenAssemblyWhenIInvokeWithTheContainerThenItReturnsMeAStreamReaderCollectionWithAllTheScriptsQueriesFoundedOnTheContainer()
        {
            var loader = new ScriptLoaderOnEmbeddedResource("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase");
            var streams = loader.Load(Container.From("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase.Scripts.Schema"));

            Assert.That(streams,Is.Not.Null);
            Assert.That(streams,Is.InstanceOf<IEnumerable<StreamReader>>());
        }
    }
}