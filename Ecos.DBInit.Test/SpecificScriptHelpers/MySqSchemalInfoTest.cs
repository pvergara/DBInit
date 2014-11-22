using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlSchemaInfoTest
    {
        [Test]
        public void DatabaseName(){
            var schemaInfo = new MySqlSchemaInfo(ConfigurationManager.ConnectionStrings["sakila"].ConnectionString);

            string databaseName = schemaInfo.DatabaseName;

            Assert.That("sakila",Is.EqualTo(databaseName));
        }
    }
}