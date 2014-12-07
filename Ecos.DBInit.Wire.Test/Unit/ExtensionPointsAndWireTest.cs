using NUnit.Framework;

using Ecos.DBInit.Wire;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Wire.Test.MockImplementations;

using System;

namespace Ecos.DBInit.Wire.Test.Unit
{
    [TestFixture]
    public class ExtensionPointsAndWireTest
    {
        static ModuleLoader InitModLoader(Type interfaceType,Type implementationType)
        {
            var moduleLoader = new ModuleLoader("", "", ProviderType.MySql);
            moduleLoader.OverwriteImplementationOf(interfaceType,implementationType);
            moduleLoader.Wire();
            return moduleLoader;
        }

        [Test]
        public void HowToOverrideIScriptExec()
        {
            var moduleLoader = InitModLoader(typeof(IScriptExec), typeof(MyIScriptExecImp));

            Assert.That(moduleLoader.GetScriptExec(), Is.TypeOf<MyIScriptExecImp>());
        }

        [Test]
        public void HowToOverrideISchemaInfo()
        {
            var moduleLoader = InitModLoader(typeof(ISchemaInfo), typeof(MyISchemaInfo));

            Assert.That(moduleLoader.GetSchemaInfo(), Is.TypeOf<MyISchemaInfo>());
        }

        [Test]
        public void HowToOverrideISpecificDBScriptComposer()
        {
            var moduleLoader = InitModLoader(typeof(ISpecificDBComposer), typeof(MyISpecificDBComposer));

            Assert.That(moduleLoader.GetScriptComposer(), Is.TypeOf<MyISpecificDBComposer>());
        }

        [Test]
        public void HowToOverrideISchemaOperator()
        {
            var moduleLoader = InitModLoader(typeof(ISchemaOperator), typeof(MyISchemaOperator));

            Assert.That(moduleLoader.GetSchemaOperator(), Is.TypeOf<MyISchemaOperator>());
        }

        [Test]
        public void HowToOverrideIDataOperator()
        {
            var moduleLoader = InitModLoader(typeof(IDataOperator), typeof(MyIDataOperator));

            Assert.That(moduleLoader.GetDataOperator(), Is.TypeOf<MyIDataOperator>());
        }

        [Test]
        public void HowToOverrideIDBOperator()
        {
            var moduleLoader = InitModLoader(typeof(IDBOperator), typeof(MyIDBOperator));

            Assert.That(moduleLoader.GetDBOperator(), Is.TypeOf<MyIDBOperator>());
        }
    }
}

