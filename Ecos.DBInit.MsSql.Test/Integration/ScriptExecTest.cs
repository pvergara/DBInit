using System.Linq;
using Ecos.DBInit.Test.ObjectMothers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using System.Configuration;
using System.Data;

namespace Ecos.DBInit.MsSql.Test.Integration
{
    [TestFixture]
    public class ScriptExecTest
    {
        private readonly string _connectionString;
        private readonly IEnumerable<String> _someTablesAndViewOfSakilaDB;

        public ScriptExecTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[NorthwindDbOM.ConnectionStringName].ConnectionString;
            _someTablesAndViewOfSakilaDB = NorthwindDbOM.SomeTableNames;
        }

        private static ICollection<string> ProcessIndexedQueriesOnIndexedResults(IDataRecord reader, int index, ICollection<string> collectionForThisIndex)
        {
            collectionForThisIndex.Add(reader.GetString(0));
            return collectionForThisIndex;
        }

        private static ICollection<string> TransformTheReaderAndReturnString(IDataRecord reader, ICollection<string> collection)
        {
            collection.Add(reader.GetString(0));
            return collection;
        }

        private static void DeleteCustomer(IScriptExec scriptExec)
        {
            scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DELETE FROM \"Order Details\";"));
            scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DELETE FROM Orders;"));
            scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DELETE FROM Customers;"));
        }

        private static void AddCustomer(IScriptExec scriptExec, IEnumerable<IList<string>> customers)
        {
            foreach (var script in customers.Select(customer => string.Format("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('{0}','{1}');", customer[0], customer[1])).Select(Script.From))
            {
                scriptExec.TryConnectionAndExecuteInsideTransaction(script);
            }
        }

        [Test]
        public void HowToUseExecuteAndProcessASingleQuery()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                var query = Script.From("SELECT name FROM Northwind.sys.sysobjects WHERE xtype in ('U');");

                //Act
                ICollection<String> results = new List<String>();
                scriptExec.ExecuteAndProcess(query, results, TransformTheReaderAndReturnString);

                //Asserts
                Assert.That(results.Count, Is.EqualTo(NorthwindDbOM.TablesCounter));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(results));
            }
        }

        [Test]
        public void HowToUseExecuteAndProcessQueriesUsingSomeIndexToIdentifyThem()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                IDictionary<int,Script> indexedQueries = new Dictionary<int,Script>();
                IDictionary<int,ICollection<String>> indexedResults = new Dictionary<int,ICollection<String>>();
                const int firsQueryIndex = 0;
                const int secondQueryIndex = 1;

                indexedQueries.Add(firsQueryIndex, Script.From("SELECT name FROM Northwind.sys.sysobjects WHERE xtype in ('U','V');"));
                indexedQueries.Add(secondQueryIndex, Script.From("SELECT name FROM Northwind.sys.sysobjects WHERE xtype in (N'P', N'PC');"));

                //Act
                scriptExec.ExecuteAndProcess(indexedQueries, indexedResults, ProcessIndexedQueriesOnIndexedResults);

                //Asserts
                Assert.That(indexedResults[firsQueryIndex].Count, Is.EqualTo(NorthwindDbOM.TablesCounter + NorthwindDbOM.ViewsCounter));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(indexedResults[firsQueryIndex]));

                Assert.That(indexedResults[secondQueryIndex].Count, Is.EqualTo(NorthwindDbOM.SPsCounter + NorthwindDbOM.FunctionsCounter));
                Assert.That(new[]{ NorthwindDbOM.SPsNames.First() }, Is.SubsetOf(indexedResults[secondQueryIndex]));
            }
        }

        [Test]
        public void HowToUseExecuteScalar()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Act
                var tablesCounter = scriptExec.ExecuteScalar<int>(Script.From("SELECT count(*) FROM Northwind.sys.sysobjects WHERE xtype='U';"));

                //Assert
                Assert.That(tablesCounter, Is.EqualTo(NorthwindDbOM.TablesCounter));
            }
        }

        [Test]
        public void How__TryConnectionAndExecuteInsideTransaction_With_CommitAndClose__Works()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                DeleteCustomer(scriptExec);

                //Act
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('1','1');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('2','2');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('3','3');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('4','4');"));
                scriptExec.CommitAndClose();

                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<int>(Script.From("SELECT COUNT(*) FROM Customers;"));

                //Assert
                Assert.That(actorCounter, Is.EqualTo(4));
            }
        }

        [Test]
        public void HowConHow__TryConnectionAndExecuteInsideTransaction_With_RollbackAndClose__WorksnectionAndExecuteInsideTransactionWithRollbackAndCloseWorks()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                DeleteCustomer(scriptExec);
                AddCustomer(scriptExec, new[]{ new[]{ "1", "1" }, new[]{ "2", "2" } });
                scriptExec.CommitAndClose();

                //Act
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO Customers (CustomerID,CompanyName) VALUES ('3','3');"));
                scriptExec.RollbackAndClose();

                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<int>(Script.From("SELECT COUNT(*) FROM Customers;"));

                //Assert
                Assert.That(actorCounter, Is.EqualTo(2));
            }
        }

        [Test]
        public void TryConnectionAndExecuteInsideTransaction_WithoutExplicitCommit_WillRevertAllChangesOnDisposing_USINGVERSION()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                DeleteCustomer(scriptExec);
                scriptExec.CommitAndClose();

                //Act
                AddCustomer(scriptExec, new[]{ new[]{ "1", "1" }, new []{ "2", "2" } });
            }//"scriptExec.Dispose();" Alias

            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<int>(Script.From("SELECT COUNT(*) FROM Customers;"));

                //Assert
                Assert.That(actorCounter, Is.EqualTo(0));
            }
        }


        [Test]
        public void TryConnectionAndExecuteInsideTransaction_WithoutExplicitCommit_WillRevertAllChangesOnDisposing__EXPLICITVERSION()
        {
            var scriptExec = new ScriptExec(_connectionString);
            //Arrange
            DeleteCustomer(scriptExec);
            scriptExec.CommitAndClose();

            //Act
            AddCustomer(scriptExec, new[] { new[] { "1", "1" }, new[] { "2", "2" } });
            scriptExec.Dispose();

            //Pre-Assert
            scriptExec = new ScriptExec(_connectionString);
            var actorCounter = scriptExec.ExecuteScalar<int>(Script.From("SELECT COUNT(*) FROM Customers;"));

            //Assert
            Assert.That(actorCounter, Is.EqualTo(0));
        }
            
    }
}