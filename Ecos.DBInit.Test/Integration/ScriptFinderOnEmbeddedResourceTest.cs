using NUnit.Framework;
using Ecos.DBInit.Core.ScriptHelpers;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test.Integration
{
    [TestFixture]
    public class ScriptFinderOnEmbeddedResourceTest
    {
        [Test]
        public void GivenTheAssemblyNameWhenIInvokeFindMethodIndicatingTheScriptTypeThenItReturnsMeTheContainerWhereTheScriptsAre()
        {
            var container = ScriptFinderFluentFactory.
                    FromEmbeddedResource.
                        InitWith(SakilaDbOM.SampleProjectAssemblyName,ScriptType.Schema).
                    GetContainer();

            Assert.That(container, Is.Not.Null);
            Assert.That(container.Path, Is.EqualTo(string.Format("{0}.Scripts.Schema",SakilaDbOM.SampleProjectAssemblyName)));
        }
    }
}