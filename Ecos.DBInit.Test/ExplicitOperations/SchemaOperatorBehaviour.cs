using NUnit.Framework;
using Ecos.DBInit.Core.Interfaces;
using Moq;
using Ecos.DBInit.MySql;
using Ecos.DBInit.Test.ObjectMothers;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Test.ExplicitOperations
{
    [TestFixture]
    public class SchemaOperatorBehaviour
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ISchemaInfo> _schemaInfoMock;
        private ISchemaOperator _schemaOperator;

        [SetUp]
        public void BeforeEachTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _schemaInfoMock = new Mock<ISchemaInfo>();
            _schemaOperator = new SchemaOperator(SakilaDbOM.SampleProjectAssemblyName, _unitOfWorkMock.Object, _schemaInfoMock.Object);
        }

        [Test]
        public void ActivateReferentialIntegrityAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.ActivateReferentialIntegrity();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }

        [Test]
        public void DeactivateReferentialIntegrityAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.DeactivateReferentialIntegrity();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }

        [Test]
        public void DropDataBaseObjectsAskForTheNameOfAllDataBaseObjectsByUsingSchemaInfo(){
            //Act
            _schemaOperator.DropDataBaseObjects();

            //Assert
            _schemaInfoMock.Verify(m => m.GetTables());
            _schemaInfoMock.Verify(m => m.GetViews());
            _schemaInfoMock.Verify(m => m.GetFunctions());
            _schemaInfoMock.Verify(m => m.GetStoredProcedures());
        }

        [Test]
        public void DropDataBaseObjectsAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.DropDataBaseObjects();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }

        [Test]
        public void CreateDataBaseObjectsAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.CreateDataBaseObjects();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }
    }
}