using NUnit.Framework;
using System;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.Linq;
using Ecos.DBInit.Core.Interfaces;
using System.Configuration;
using Ecos.DBInit.Test.ObjectMothers;
using System.Data;

namespace Ecos.DBInit.MySql.Test.Integration
{
    [TestFixture]
    public class ScriptExecTest
    {
        private readonly string _connectionString;
        private readonly IEnumerable<String> _someTablesAndViewOfSakilaDB;

        public ScriptExecTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings[SakilaDbOM.ConnectionStringName].ConnectionString;
            _someTablesAndViewOfSakilaDB = SakilaDbOM.SomeTableNames;
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

        private static void DeleteFilmActorAndActorTables(IScriptExec scriptExec)
        {
            scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DELETE FROM film_actor;"));
            scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("DELETE FROM actor;"));
        }

        private static void AddActor(IScriptExec scriptExec, IEnumerable<IList<string>> actors)
        {
            foreach (var actor in actors)
            {
                var scriptOnString = string.Format("INSERT INTO actor (first_name,last_name) VALUES ('{0}','{1}');", actor[0], actor[1]);
                var script = Script.From(scriptOnString);
                scriptExec.TryConnectionAndExecuteInsideTransaction(script);
            }
        }

        [Test]
        public void HowToUseExecuteAndProcessASingleQuery()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                var query = Script.From("SHOW TABLES;");

                //Act
                ICollection<String> results = new List<String>();
                scriptExec.ExecuteAndProcess(query, results, TransformTheReaderAndReturnString);

                //Asserts
                Assert.That(results.Count, Is.EqualTo(SakilaDbOM.TablesCounter + SakilaDbOM.ViewsCounter));
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

                indexedQueries.Add(firsQueryIndex, Script.From("SHOW TABLES;"));
                indexedQueries.Add(secondQueryIndex, Script.From("SELECT specific_name FROM information_schema.routines WHERE routine_schema = 'sakila';"));

                //Act
                scriptExec.ExecuteAndProcess<int,String>(indexedQueries, indexedResults, ProcessIndexedQueriesOnIndexedResults);

                //Asserts
                Assert.That(indexedResults[firsQueryIndex].Count, Is.EqualTo(SakilaDbOM.TablesCounter + SakilaDbOM.ViewsCounter));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(indexedResults[firsQueryIndex]));

                Assert.That(indexedResults[secondQueryIndex].Count, Is.EqualTo(SakilaDbOM.SPsCounter + SakilaDbOM.FunctionsCounter));
                Assert.That(new[]{ SakilaDbOM.FunctionNames.First(), SakilaDbOM.SPsNames.First() }, Is.SubsetOf(indexedResults[secondQueryIndex]));
            }
        }

        [Test]
        public void HowToUseExecuteScalar()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Act
                var actorsCount = scriptExec.ExecuteScalar<long>(Script.From("SELECT count(*) FROM information_schema.tables WHERE table_schema = 'sakila' AND TABLE_TYPE = 'BASE TABLE';"));

                //Assert
                Assert.That(actorsCount, Is.EqualTo(SakilaDbOM.TablesCounter));
            }
        }

        [Test]
        public void How__TryConnectionAndExecuteInsideTransaction_With_CommitAndClose__Works()
        {
            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Arrange
                DeleteFilmActorAndActorTables(scriptExec);

                //Act
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Kevin','Spacey');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Carmelo','Gómez');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Christopher','Lee');"));
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Bruce','Campbell');"));
                scriptExec.CommitAndClose();

                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

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
                DeleteFilmActorAndActorTables(scriptExec);
                AddActor(scriptExec, new[]{ new string [2]{ "Kevin", "Spacey" }, new string [2]{ "Carmelo", "Gómez" } });
                scriptExec.CommitAndClose();

                //Act
                scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Bruce','Campbell');"));
                scriptExec.RollbackAndClose();

                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

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
                DeleteFilmActorAndActorTables(scriptExec);
                scriptExec.CommitAndClose();

                //Act
                AddActor(scriptExec, new[]{ new string [2]{ "Kevin", "Spacey" }, new string [2]{ "Carmelo", "Gómez" } });
            }//"scriptExec.Dispose();" Alias

            using (var scriptExec = new ScriptExec(_connectionString))
            {
                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

                //Assert
                Assert.That(actorCounter, Is.EqualTo(0));
            }
        }


        [Test]
        public void TryConnectionAndExecuteInsideTransaction_WithoutExplicitCommit_WillRevertAllChangesOnDisposing__EXPLICITVERSION()
        {
            var scriptExec = new ScriptExec(_connectionString);
            //Arrange
            DeleteFilmActorAndActorTables(scriptExec);
            scriptExec.CommitAndClose();

            //Act
            AddActor(scriptExec, new[]{ new string [2]{ "Kevin", "Spacey" }, new string [2]{ "Carmelo", "Gómez" } });
            scriptExec.Dispose();

            //Pre-Assert
            var actorCounter = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

            //Assert
            Assert.That(actorCounter, Is.EqualTo(0));
        }
            
    }
}