using NUnit.Framework;
using Ecos.DBInit.MySql;
using Moq;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using Ecos.DBInit.Test.ObjectMothers;

namespace Ecos.DBInit.Test.ExplicitOperations
{
    [TestFixture]
    public class DataOperatorBehaviour
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISchemaInfo> _schemaInfoMock;
        private IDataOperator _dataOperator;

        [SetUp]
        public void BeforeEachTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _schemaInfoMock = new Mock<ISchemaInfo>();
            _dataOperator = new DataOperator(SakilaDbOM.SampleProjectAssemblyName, _unitOfWorkMock.Object, _schemaInfoMock.Object);
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
    }
}

