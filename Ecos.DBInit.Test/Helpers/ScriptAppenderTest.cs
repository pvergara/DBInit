using NUnit.Framework;
using Ecos.DBInit.Core.ScriptsHelpers;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.Helpers
{
    [TestFixture]
    public class ScriptAppenderTest
    {
        [Test]
        public void WhenIInvokeGetScriptsFromWithStreamReaderCollectionThenItReturnsMeAllTheScripts()
        {
            var loader = new ScriptLoaderOnEmbeddedResource("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase");
            var streams = loader.Load(Container.From("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase.Scripts.Schema"));
            var appender = new ScriptAppender();

            var scripts = appender.GetScriptsFrom(streams);

            Assert.That(scripts,Is.Not.Null);
            Assert.That(scripts,Is.InstanceOf<IEnumerable<Script>>());

        }
    }
}