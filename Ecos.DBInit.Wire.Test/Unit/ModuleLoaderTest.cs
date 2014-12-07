using NUnit.Framework;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Base;
using Mysql = Ecos.DBInit.MySql;


namespace Ecos.DBInit.Wire.Test.Unit
{
    [TestFixture]
    public class ModuleLoaderTest
    {
        [Test]
        public void DefaultComponents()
        {
            var moduleLoader = new ModuleLoader("", "", ProviderType.MySql);
            moduleLoader.Wire();

            Assert.That(moduleLoader.GetDBInit(), Is.TypeOf<Core.Base.DBInit>());

            Assert.That(moduleLoader.GetDataOperator(), Is.TypeOf<DataOperator>());
            Assert.That(moduleLoader.GetSchemaOperator(), Is.TypeOf<SchemaOperator>());
            Assert.That(moduleLoader.GetDBOperator(), Is.TypeOf<DBOperator>());

            Assert.That(moduleLoader.GetSchemaInfo(), Is.TypeOf<MySql.SchemaInfo>());
            Assert.That(moduleLoader.GetScriptComposer(), Is.TypeOf<MySql.SpecificDBComposer>());
            Assert.That(moduleLoader.GetScriptExec(), Is.TypeOf<MySql.ScriptExec>());

        }
    }
}