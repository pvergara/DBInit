using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System;
using System.Data;
using System.Collections.Generic;
using Constraints = NUnit.Framework.Constraints;
using Ecos.DBInit.Core.Model;
using System.Configuration;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlScriptHelperTest
    {
        private readonly string _connectionString;
        private readonly IEnumerable<String> _someTablesAndViewOfSakilaDB;

        public MySqlScriptHelperTest(){
            _connectionString = ConfigurationManager.ConnectionStrings["sakila"].ConnectionString;
            _someTablesAndViewOfSakilaDB = new[]{ "actor", "address", "category", "film_category", "actor_info" };
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
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Arrange
                var query = Script.From("SHOW TABLES;");

                //Act
                ICollection<String> results = new List<String>();
                helper.ExecuteAndProcess(query,results, TransformTheReaderAndReturnString);

                //Asserts
                Assert.That(results.Count, Is.EqualTo(23));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(results));
            }
        }

        [Test]
        public void HowToUseExecuteAndProcessQueriesUsingSomeIndexToIdentifyThem()
        {
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Arrange
                IDictionary<int,Script> indexedQueries = new Dictionary<int,Script>();
                IDictionary<int,ICollection<String>> indexedResults = new Dictionary<int,ICollection<String>>();
                const int firsQueryIndex = 0;
                const int secondQueryIndex = 1;

                indexedQueries.Add(firsQueryIndex, Script.From("SHOW TABLES;"));
                indexedQueries.Add(secondQueryIndex, Script.From("SELECT specific_name FROM information_schema.routines WHERE routine_schema = 'sakila';"));

                //Act
                helper.ExecuteAndProcess<int,String>(indexedQueries, indexedResults,ProcessIndexedQueriesOnIndexedResults);

                //Asserts
                Assert.That(indexedResults[firsQueryIndex].Count, Is.EqualTo(23));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(indexedResults[firsQueryIndex]));

                Assert.That(indexedResults[secondQueryIndex].Count, Is.EqualTo(6));
                Assert.That(new[]{ "get_customer_balance", "film_in_stock" }, Is.SubsetOf(indexedResults[secondQueryIndex]));
            }
        }

        [Test]
        public void HowToUseExecuteScalar(){
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Act
                var actorsCount = helper.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));

                //Assert
                Assert.That(actorsCount,Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public void HowToUseExecute(){
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Act
                helper.Execute(new[]{
                    Script.From("DELETE FROM film_actor;"),
                    Script.From("DELETE FROM actor;"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Kevin','Spacey');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Carmelo','Gómez');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Christopher','Lee');"),
                    Script.From("INSERT INTO actor (first_name,last_name) VALUES ('Bruce','Campbell');"),
                });

                //Pre-Assert
                var actorCounter = helper.ExecuteScalar<long>(Script.From("SELECT COUNT(*) FROM actor;"));
                
                //Assert
                Assert.That(actorCounter,Is.EqualTo(4));
            }
        }
            
    }
}

