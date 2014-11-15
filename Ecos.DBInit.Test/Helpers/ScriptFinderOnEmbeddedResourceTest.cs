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
            var container = ScriptFinderFluentFactory.
                    FromEmbeddedResource.
                        InitWith("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase",ScriptType.Schema).
                    GetContainer();

            Assert.That(container, Is.Not.Null);
            Assert.That(container.Path, Is.EqualTo("Ecos.DBInit.Samples.ProjectWithAMySQLDataBase.Scripts.Schema"));
        }
    }
}