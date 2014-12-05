using NUnit.Framework;
using System.Linq;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.MySql.Test.Unit
{
    [TestFixture]
    public class SpecificDBComposerTest
    {
        private readonly ISpecificDBComposer _specificDbOperation;

        public SpecificDBComposerTest()
        {
            _specificDbOperation = new SpecificDBComposer();
        }

        [Test]
        public void ComposeScriptsDelete()
        {
            //Arrange
            var tableNames = new[] { "table1", "table2", "table3" };

            //Action
            var deleteScript = _specificDbOperation.ComposeScriptsDelete(tableNames);

            //Asserts
            Assert.That(deleteScript, Is.SubsetOf(tableNames.Select(table => Script.From(string.Format("DELETE FROM {0};", table)))));
            Assert.That(deleteScript.ToList(), Has.Count.EqualTo(tableNames.Count()));
        }

        [Test]
        public void ComposeActivateReferentialIntegrity()
        {
            //Arrange

            //Action
            var activateReferentialIntegrityScript = _specificDbOperation.ComposeActivateReferentialIntegrity();

            //Asserts
            Assert.That(activateReferentialIntegrityScript, Is.EqualTo(Script.From("SET @@foreign_key_checks = 1;")));
        }

        [Test]
        public void ComposeDeactivateReferentialIntegrity()
        {
            //Arrange

            //Action
            var activateReferentialIntegrityScript = _specificDbOperation.ComposeDeactivateReferentialIntegrity();

            //Asserts
            Assert.That(activateReferentialIntegrityScript, Is.EqualTo(Script.From("SET @@foreign_key_checks = 0;")));
        }

        [Test]
        public void ComposeScriptDropTables()
        {
            //Arrange
            var tableNames = new[] { "table1", "table2", "table3" };

            //Action
            var dropTablesScript = _specificDbOperation.ComposeScriptsDropTables(tableNames);

            //Asserts
            Assert.That(dropTablesScript, Is.SubsetOf(tableNames.Select(table => Script.From(string.Format("DROP TABLE IF EXISTS {0};", table)))));
            Assert.That(dropTablesScript.ToList(), Has.Count.EqualTo(tableNames.Count()));
        }

        [Test]
        public void ComposeScriptDropViews()
        {
            //Arrange
            var viewNames = new[] { "view1", "view2" };

            //Action
            var dropViewsScript = _specificDbOperation.ComposeScriptsDropViews(viewNames);

            //Asserts
            Assert.That(dropViewsScript, Is.SubsetOf(viewNames.Select(view => Script.From(string.Format("DROP VIEW IF EXISTS {0};", view)))));
            Assert.That(dropViewsScript.ToList(), Has.Count.EqualTo(viewNames.Count()));
        }

        [Test]
        public void ComposeScriptDropStoredProcedures()
        {
            //Arrange
            var storedProcedureNames = new[] { "sp1", "routine", "storedProcedure" };

            //Action
            var dropSPsScript = _specificDbOperation.ComposeScriptsDropStoredProcedures(storedProcedureNames);

            //Asserts
            Assert.That(dropSPsScript, Is.SubsetOf(storedProcedureNames.Select(sp => Script.From(string.Format("DROP PROCEDURE IF EXISTS {0};", sp)))));
            Assert.That(dropSPsScript.ToList(), Has.Count.EqualTo(storedProcedureNames.Count()));
        }

        [Test]
        public void ComposeScriptDropFunctions()
        {
            //Arrange
            var functionNames = new[] { "FUNC1", "FUNCTION", "fnLerele" };

            //Action
            var dropFuncScript = _specificDbOperation.ComposeScriptsDropFunctions(functionNames);

            //Asserts
            Assert.That(dropFuncScript, Is.SubsetOf(functionNames.Select(func => Script.From(string.Format("DROP FUNCTION IF EXISTS {0};", func)))));
            Assert.That(dropFuncScript.ToList(), Has.Count.EqualTo(functionNames.Count()));
        }
    }
}