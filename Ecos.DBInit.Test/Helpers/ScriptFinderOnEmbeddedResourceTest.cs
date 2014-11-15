using NUnit.Framework;
using Ecos.DBInit.Core.ScriptsHelpers;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Test.Helpers
{
    [TestFixture]
    public class ScriptFinderOnEmbeddedResourceTest
    {
        [Test]
        public void GivenTheAssemblyNameWhenIInvokeFindMethodIndicatingTheScriptTypeThenItReturnsMeTheContainerWhereTheScriptsAre()
        {
            var finder = new ScriptFinderOnEmbbededResource("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase");
            var container = finder.Find(ScriptType.Schema);

            Assert.That(container, Is.Not.Null);
            Assert.That(container.Path, Is.EqualTo("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase.Scripts.Schema"));
        }
    }
}