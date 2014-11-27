using NUnit.Framework;
using Moq;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using Ecos.DBInit.Bootstrap;

namespace Ecos.DBInit.Test.ExplicitOperations
{
    [TestFixture]
    public class DataOperatorBehaviour
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISchemaInfo> _schemaInfoMock;
        private IDataOperator _dataOperator;
        private Mock<IScriptLoader> _scriptLoaderMock;
        private Mock<ISpecificDBOperator> _specificDBOperatorMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _schemaInfoMock = new Mock<ISchemaInfo>();
            _scriptLoaderMock = new Mock<IScriptLoader>();
            _specificDBOperatorMock = new Mock<ISpecificDBOperator>();
            _dataOperator = new DataOperator(_unitOfWorkMock.Object, _schemaInfoMock.Object, _scriptLoaderMock.Object,_specificDBOperatorMock.Object);
        }

        [Test]
        public void CleanEachTableAskForDatabaseTablesByUsingSchemaInfo()
        {
            //Act
            _dataOperator.CleanEachTable();

            //Assert
            _schemaInfoMock.Verify(m => m.GetTables());
        }

        [Test]
        public void CleanEachTableDelegatesTheSpecificCompositionOfTheDeletingEachDataByUsingISpecificDBOperator()
        {
            //Arrange
            var tables = new[]{"table1","table2","table3"};
            _schemaInfoMock.Setup(m => m.GetTables()).Returns(tables);

            //Act
            _dataOperator.CleanEachTable();

            //Assert
            _specificDBOperatorMock.Verify(m => m.ComposeScriptsDelete(tables));
        }

        [Test]
        public void CleanEachTableAddsTheResultedScriptsToTheUnitOfWork()
        {
            //Act
            _dataOperator.CleanEachTable();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }


        [Test]
        public void LoadDataScriptsAddsTheResultedScriptsToTheUnitOfWork()
        {
            //Act
            _dataOperator.LoadDataScripts();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }    

        [Test]
        public void LoadDataScriptsAskForTheScriptsThatAreNeededToLoadTheDataByUsingIScriptLoader()
        {
            //Act
            _dataOperator.LoadDataScripts();

            //Assert
            _scriptLoaderMock.Verify(m => m.GetScripts());
        }    
    }
}

