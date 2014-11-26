using NUnit.Framework;
using Ecos.DBInit.MySql;
using Moq;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.ExplicitOperations
{
    [TestFixture]
    public class DataOperatorBehaviour
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISchemaInfo> _schemaInfoMock;
        private IDataOperator _dataOperator;
        private Mock<IScriptLoader> _scriptLoaderMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _schemaInfoMock = new Mock<ISchemaInfo>();
            _scriptLoaderMock = new Mock<IScriptLoader>();
            _dataOperator = new DataOperator( _unitOfWorkMock.Object, _schemaInfoMock.Object, _scriptLoaderMock.Object);
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

