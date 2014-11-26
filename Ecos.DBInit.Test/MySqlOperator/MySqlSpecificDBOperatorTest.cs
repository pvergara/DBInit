using NUnit.Framework;
using Ecos.DBInit.MySql;
using System.Collections.Generic;
using System.Linq;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Test.MySqlOperator
{
    [TestFixture]
    public class MySqlSpecificDBOperatorTest
    {
        [Test]
        public void ComposeScriptsDelete(){
            //Arrange
            var tableNames = new[] {"table1","table2","table3"};
            var specificDbOperation = new SpecificDBOperator();

            //Action
            var deleteScript = specificDbOperation.ComposeScriptsDelete(tableNames);

            //Asserts
            Assert.That(deleteScript, Is.SubsetOf(tableNames.Select(table =>Script.From(string.Format("DELETE FROM {0};", table)))));
        }
    }
}

