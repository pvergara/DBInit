using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System;
using System.Data;
using System.Collections.Generic;
using Constraints = NUnit.Framework.Constraints;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlScriptHelperTest
    {
        readonly string _connectionString;
        readonly ICollection<String> _collection = new List<String>();
        readonly Dictionary<int,ICollection<String>> _setOfCollection = new Dictionary<int,ICollection<String>>();
        IEnumerable<String> _someTablesAndViewOfSakilaDB;

        public MySqlScriptHelperTest(){
            _connectionString = "Server = localhost;" +
                "Database = sakila;" +
                "User ID = desarrollo;" +
                "Password = 3QSo5cff;";
            _someTablesAndViewOfSakilaDB = new[]{ "actor", "address", "category", "film_category", "actor_info" };
        }

        [Test]
        public void HowToUseExecuteAndProcessASingleQuery()
        {
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Arrange
                var query = Script.From("SHOW TABLES;");

                //Act
                helper.ExecuteAndProcess(query, TransformTheReatherOnString);

                //Asserts
                Assert.That(_collection.Count, Is.EqualTo(23));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(_collection));
            }
        }

        void TransformTheReatherOnString(IDataRecord reader)
        {
            _collection.Add(reader.GetString(0));
        }

        [Test]
        public void HowToUseExecuteAndProcessQueriesUsingSomeIndexToIdentifyThem()
        {
            using (var helper = new MySqlScriptHelper(_connectionString))
            {
                //Arrange
                IDictionary<int,Script> queries = new Dictionary<int,Script>();
                const int firsQueryIndex = 0;
                const int secondQueryIndex = 1;

                queries.Add(firsQueryIndex, Script.From("SHOW TABLES;"));
                queries.Add(secondQueryIndex, Script.From("SELECT specific_name FROM information_schema.routines WHERE routine_schema = 'sakila';"));

                //Act
                helper.ExecuteAndProcess<int>(queries, ProcessQueries);

                //Asserts
                Assert.That(_setOfCollection[firsQueryIndex].Count, Is.EqualTo(23));
                Assert.That(_someTablesAndViewOfSakilaDB, Is.SubsetOf(_setOfCollection[firsQueryIndex]));

                Assert.That(_setOfCollection[secondQueryIndex].Count, Is.EqualTo(6));
                Assert.That(new[]{ "get_customer_balance", "film_in_stock" }, Is.SubsetOf(_setOfCollection[secondQueryIndex]));
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
            
        void ProcessQueries(IDataRecord reader, int index)
        {
            ICollection<String> collection = new List<String>();
            if (!_setOfCollection.ContainsKey(index)){
                _setOfCollection.Add(index,collection);
            }
            collection = _setOfCollection[index];
            collection.Add(reader.GetString(0));
            _setOfCollection[index] = collection;
        }

    }
}

