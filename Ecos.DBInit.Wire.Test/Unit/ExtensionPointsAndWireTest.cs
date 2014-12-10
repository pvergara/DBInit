using NUnit.Framework;

using Ecos.DBInit.Wire;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Wire.Test.MockImplementations;

using System;

namespace Ecos.DBInit.Wire.Test.Unit
{
    [TestFixture]
    public class ExtensionPointsAndWireTest
    {
        private static ModuleLoader InitModLoader(Action<ModuleLoader> specificImpTypeSetter)
        {
            var moduleLoader = new ModuleLoader("", "", ProviderType.MySql);
            specificImpTypeSetter(moduleLoader);
            moduleLoader.Wire();
            return moduleLoader;
        }

        [Test]
        public void HowToOverrideIScriptExec()
        {
            var moduleLoader = InitModLoader(ml => {ml.ScriptExecImpType = typeof(MyIScriptExecImp);});

            Assert.That(moduleLoader.GetScriptExec(), Is.TypeOf<MyIScriptExecImp>());
        }

        [Test]
        public void HowToOverrideISchemaInfo()
        {
            var moduleLoader = InitModLoader(ml => {ml.SchemaInfoImpType = typeof(MyISchemaInfo);});

            Assert.That(moduleLoader.GetSchemaInfo(), Is.TypeOf<MyISchemaInfo>());
        }

        [Test]
        public void HowToOverrideISpecificDBScriptComposer()
        {
            var moduleLoader = InitModLoader(ml => {ml.SpecificDBComposerImpType = typeof(MyISpecificDBComposer);});

            Assert.That(moduleLoader.GetScriptComposer(), Is.TypeOf<MyISpecificDBComposer>());
        }

        [Test]
        public void HowToOverrideISchemaOperator()
        {
            var moduleLoader = InitModLoader(ml => {ml.SchemaOperatorImpType = typeof(MyISchemaOperator);});

            Assert.That(moduleLoader.GetSchemaOperator(), Is.TypeOf<MyISchemaOperator>());
        }

        [Test]
        public void HowToOverrideIDataOperator()
        {
            var moduleLoader = InitModLoader(ml => {ml.DataOperatorImpType = typeof(MyIDataOperator);});

            Assert.That(moduleLoader.GetDataOperator(), Is.TypeOf<MyIDataOperator>());
        }

        [Test]        
        public void HowToOverrideIDBOperator()
        {
            var moduleLoader = InitModLoader(ml => {ml.DBOperatorImpType = typeof(MyIDBOperator);});

            Assert.That(moduleLoader.GetDBOperator(), Is.TypeOf<MyIDBOperator>());
        }
    }
}

