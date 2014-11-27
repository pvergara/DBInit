using NUnit.Framework;
using Ecos.DBInit.MySql;
using System.Collections.Generic;
using System.Linq;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Test.MySqlOperator
{
    [TestFixture]
    public class MySqlSpecificDBOperatorTest
    {
        private readonly ISpecificDBOperator _specificDbOperation;

        public MySqlSpecificDBOperatorTest(){
            _specificDbOperation = new SpecificDBOperator();
        }

        [Test]
        public void ComposeScriptsDelete(){
            //Arrange
            var tableNames = new[] {"table1","table2","table3"};

            //Action
            var deleteScript = _specificDbOperation.ComposeScriptsDelete(tableNames);

            //Asserts
            Assert.That(deleteScript, Is.SubsetOf(tableNames.Select(table =>Script.From(string.Format("DELETE FROM {0};", table)))));
        }

        [Test]
        public void ComposeActivateReferentialIntegrity(){
            //Arrange

            //Action
            var activateReferentialIntegrityScript = _specificDbOperation.ComposeActivateReferentialIntegrity();

            //Asserts
            Assert.That(activateReferentialIntegrityScript, Is.EqualTo(Script.From("SET @@foreign_key_checks = 1;")));
        }
    }
}