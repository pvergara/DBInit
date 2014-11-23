using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;
using System.Collections.Generic;
using Ecos.DBInit.Core.ScriptHelpers;
using Moq;
using Ecos.DBInit.Core.Model;
using System.Data;
using System;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlSchemaInfoTest
    {
        private readonly MySqlSchemaInfo _schemaInfo;
        private readonly string _connectionString;

        public MySqlSchemaInfoTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sakilaConStr"].ConnectionString;
            _schemaInfo = new MySqlSchemaInfo(_connectionString, new MySqlScriptHelper(_connectionString));
        }

        [Test]
        public void DatabaseName()
        {
            //Act
            string databaseName = _schemaInfo.DatabaseName;

            //Assert
            Assert.That("sakila", Is.EqualTo(databaseName));
        }

        [Test]
        public void GetTables()
        {
            //Act
            var tables = _schemaInfo.GetTables();

            //Assert
            Assert.That(new []{ "actor", "address", "customer", "film_text", "inventory" }, Is.SubsetOf(tables));
            Assert.That(tables, Has.Count.EqualTo(16));
        }

        [Test]
        public void OnlyTheFirstGetTablesInvokationItWillTryToGetTheDataFromTheDatabase()
        {
            //Arrange
            var helperMock = new Mock<IScriptExec>();

            var schemaInfo = new MySqlSchemaInfo(_connectionString, helperMock.Object);

            //Act
            schemaInfo.GetTables();
            schemaInfo.GetTables();

            //Assert
            helperMock.Verify(m => m.ExecuteAndProcess<string>(It.IsAny<Script>(), It.IsAny<ICollection<string>>(), It.IsAny<Func<IDataReader,ICollection<string>,ICollection<string>>>()), Times.Once);
        }

        [Test]
        public void GetTablesIsIdempotent()
        {
            for (var i = 0; i < 5; i++)
            {
                //Act
                var tables = _schemaInfo.GetTables();

                //Assert
                Assert.That(new []{ "actor", "address", "customer", "film_text", "inventory" }, Is.SubsetOf(tables));
                Assert.That(tables, Has.Count.EqualTo(16));
            }
        }
    }
}