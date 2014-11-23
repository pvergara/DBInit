using NUnit.Framework;
using Ecos.DBInit.MySql.ScriptHelpers;
using System.Configuration;
using System.Collections.Generic;
using Ecos.DBInit.Core.ScriptHelpers;
using Moq;
using Ecos.DBInit.Core.Model;
using System.Data;
using System;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test.SpecificScriptHelpers
{
    [TestFixture]
    public class MySqlSchemaInfoTest
    {
        private readonly MySqlSchemaInfo _schemaInfo;
        private readonly string _connectionString;
        private Mock<IScriptExec> _helperMock;
        private MySqlSchemaInfo _schemaInfoWithHelperMocked;

        public MySqlSchemaInfoTest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["sakilaConStr"].ConnectionString;
            _schemaInfo = new MySqlSchemaInfo(_connectionString, new MySqlScriptHelper(_connectionString));
        }

        [SetUp]
        public void SetUp(){
            _helperMock = new Mock<IScriptExec>();

            _schemaInfoWithHelperMocked = new MySqlSchemaInfo(_connectionString, _helperMock.Object);
        }

        [TestFixtureTearDown]
        public void RunAtTheEndOfAllFixtureTests(){
            _schemaInfo.Dispose();
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
            Assert.That(SakilaDbOM.SomeTableNames, Is.SubsetOf(tables));
            Assert.That(tables, Has.Count.EqualTo(SakilaDbOM.TablesCounter));
        }

        [Test]
        public void OnlyTheFirstGetTablesInvokationItWillTryToGetTheDataFromTheDatabase()
        {
            //Arrange

            //Act
            _schemaInfoWithHelperMocked.GetTables();
            _schemaInfoWithHelperMocked.GetTables();

            //Assert
            _helperMock.Verify(m => 
                m.ExecuteAndProcess<string>(
                    It.IsAny<Script>(), 
                    It.IsAny<ICollection<string>>(), 
                    It.IsAny<Func<IDataReader,ICollection<string>,ICollection<string>>>()
                ), 
                Times.Once
            );
        }

        [Test]
        public void GetTablesIsIdempotent()
        {
            for (var i = 0; i < 5; i++)
            {
                //Act
                var tables = _schemaInfo.GetTables();

                //Assert
                Assert.That(SakilaDbOM.SomeTableNames, Is.SubsetOf(tables));
                Assert.That(tables, Has.Count.EqualTo(SakilaDbOM.TablesCounter));
            }
        }


        [Test]
        public void GetViews()
        {
            //Act
            IEnumerable<string> views = _schemaInfo.GetViews();

            //Assert
            Assert.That(SakilaDbOM.ViewNames, Is.EquivalentTo(views));
            Assert.That(views, Has.Count.EqualTo(SakilaDbOM.ViewsCounter));
        }

        [Test]
        public void OnlyTheFirstGetViewsInvokationItWillTryToGetTheDataFromTheDatabase()
        {
            //Arrange

            //Act
            _schemaInfoWithHelperMocked.GetViews();
            _schemaInfoWithHelperMocked.GetViews();

            //Assert
            _helperMock.Verify(m => 
                m.ExecuteAndProcess<string>(
                    It.IsAny<Script>(), 
                    It.IsAny<ICollection<string>>(), 
                    It.IsAny<Func<IDataReader,ICollection<string>,ICollection<string>>>()
                ), 
                Times.Once
            );
        }
            
        [Test]
        public void GetViewsIsIdempotent()
        {
            for (var i = 0; i < 5; i++)
            {
                //Act
                var tables = _schemaInfo.GetViews();

                //Assert
                Assert.That(SakilaDbOM.ViewNames, Is.SubsetOf(tables));
                Assert.That(tables, Has.Count.EqualTo(SakilaDbOM.ViewsCounter));
            }
        }

    }
}