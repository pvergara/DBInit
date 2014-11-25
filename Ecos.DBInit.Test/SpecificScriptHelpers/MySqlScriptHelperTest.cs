using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System;
using System.Data;
using System.Collections.Generic;
using Constraints = NUnit.Framework.Constraints;
using Ecos.DBInit.Core.Model;
using System.Configuration;
using Ecos.DBInit.Test.ObjectMothers;
using System.Linq;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlScriptHelperTest
    {
        private readonly string _connectionString;
        private readonly IEnumerable<String> _someTablesAndViewOfSakilaDB;

        public MySqlScriptHelperTest(){
            _connectionString = ConfigurationManager.ConnectionStrings[SakilaDbOM.ConnectionStringName].ConnectionString;
            _someTablesAndViewOfSakilaDB = SakilaDbOM.SomeTableNames;
        }

        private static ICollection<string> ProcessIndexedQueriesOnIndexedResults(IDataRecord reader, int index,ICollection<string> collectionForThisIndex){
            collectionForThisIndex.Add(reader.GetString(0));
            return collectionForThisIndex;
        }

        private static ICollection<string> TransformTheReaderAndReturnString(IDataRecord reader,ICollection<string> collection)
        {
            collection.Add(reader.GetString(0));
            return collection;
        }

        [Test]
        public void HowToUseExecuteAndProcessASingleQuery()
        {
            using (var scriptExec = new MySqlScriptExec(_connectionString))
            {
                //Arrange
                var query = Script.From("SHOW TABLES;");

                //Act
                ICollection<String> results = new List<String>();
                scriptExec.ExecuteAndProcess(query,results, TransformTheReaderAndReturnString);

                //Asserts
                Assert.That(results.Count, Is.EqualTo(SakilaDbOM.TablesCounter+SakilaDbOM.ViewsCounter));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(results));
            }
        }

        [Test]
        public void HowToUseExecuteAndProcessQueriesUsingSomeIndexToIdentifyThem()
        {
            using (var scriptExec = new MySqlScriptExec(_connectionString))
            {
                //Arrange
                IDictionary<int,Script> indexedQueries = new Dictionary<int,Script>();
                IDictionary<int,ICollection<String>> indexedResults = new Dictionary<int,ICollection<String>>();
                const int firsQueryIndex = 0;
                const int secondQueryIndex = 1;

                indexedQueries.Add(firsQueryIndex, Script.From("SHOW TABLES;"));
                indexedQueries.Add(secondQueryIndex, Script.From("SELECT specific_name FROM information_schema.routines WHERE routine_schema = 'sakila';"));

                //Act
                scriptExec.ExecuteAndProcess<int,String>(indexedQueries, indexedResults,ProcessIndexedQueriesOnIndexedResults);

                //Asserts
                Assert.That(indexedResults[firsQueryIndex].Count, Is.EqualTo(SakilaDbOM.TablesCounter+SakilaDbOM.ViewsCounter));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(indexedResults[firsQueryIndex]));

                Assert.That(indexedResults[secondQueryIndex].Count, Is.EqualTo(SakilaDbOM.SPsCounter+SakilaDbOM.FunctionsCounter));
                Assert.That(new[]{ SakilaDbOM.FunctionNames.First(), SakilaDbOM.SPsNames.First() }, Is.SubsetOf(indexedResults[secondQueryIndex]));
            }
        }

        [Test]
        public void HowToUseExecuteScalar(){
            using (var scriptExec = new MySqlScriptExec(_connectionString))
            {
                //Act
                var actorsCount = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

                //Assert
                Assert.That(actorsCount,Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public void HowToUseExecute(){
            using (var scriptExec = new MySqlScriptExec(_connectionString))
            {
                //Act
                scriptExec.Execute(new[]{
                    Script.From("DELETE FROM film_actor;"),
                    Script.From("DELETE FROM actor;"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Kevin','Spacey');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Carmelo','Gómez');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Christopher','Lee');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Bruce','Campbell');"),
                });

                //Pre-Assert
                var actorCounter = scriptExec.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));
                
                //Assert
                Assert.That(actorCounter,Is.EqualTo(4));
            }
        }
            
    }
}