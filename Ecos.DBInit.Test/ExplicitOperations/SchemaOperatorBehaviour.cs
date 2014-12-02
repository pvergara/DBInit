using Ecos.DBInit.Core.Base;
using NUnit.Framework;
using Ecos.DBInit.Core.Interfaces;
using Moq;
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
        private Mock<IScriptLoader> _scriptLoaderMock;
        private Mock<ISpecificDBOperator> _specificDBOperatorMock;

        [SetUp]
        public void BeforeEachTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _schemaInfoMock = new Mock<ISchemaInfo>();
            _scriptLoaderMock = new Mock<IScriptLoader>();
            _specificDBOperatorMock = new Mock<ISpecificDBOperator>();
            _schemaOperator = new SchemaOperator(_unitOfWorkMock.Object, _schemaInfoMock.Object,_scriptLoaderMock.Object,_specificDBOperatorMock.Object);
        }

        [Test]
        public void ActivateReferentialIntegrityAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.ActivateReferentialIntegrity();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }

        [Test]
        public void ActivateReferentialIntegrityDelegatesTheSpecificCompositionOfTheScriptToISpecificDBOperator(){
            //Act
            _schemaOperator.ActivateReferentialIntegrity();

            //Assert
            _specificDBOperatorMock.Verify(m => m.ComposeActivateReferentialIntegrity());
        }

        [Test]
        public void DeactivateReferentialIntegrityAddsTheResultedScriptsToTheUnitOfWork(){
            //Act
            _schemaOperator.DeactivateReferentialIntegrity();

            //Assert
            _unitOfWorkMock.Verify(m => m.Add(It.IsAny<IEnumerable<Script>>()));
        }

        [Test]
        public void DeactivateReferentialIntegrityDelegatesTheSpecificCompositionOfTheScriptToISpecificDBOperator(){
            //Act
            _schemaOperator.DeactivateReferentialIntegrity();

            //Assert
            _specificDBOperatorMock.Verify(m => m.ComposeDeactivateReferentialIntegrity());
        }

        [Test]
        public void DropDataBaseObjectsAskForTheNameOfAllDataBaseObjectsByUsingSchemaInfo(){
            //Act
            _schemaOperator.DropDataBaseObjects();

            //Assert
            _schemaInfoMock.Verify(m => m.GetTables());
            _schemaInfoMock.Verify(m => m.GetViews());
            _schemaInfoMock.Verify(m => m.GetStoredProcedures());
            _schemaInfoMock.Verify(m => m.GetFunctions());
        }

        [Test]
        public void DropDataBaseObjectsDelegatesTheSpecificCompositionOfTheScriptToISpecificDBOperator(){
            //Arrange
            var tableNames = new[]{ "t1", "t2", "t3", "t4", "t5" };
            _schemaInfoMock.Setup(mn => mn.GetTables()).Returns(tableNames);
            var viewNames = new[]{ "v1", "v2", "v3" };
            _schemaInfoMock.Setup(mn => mn.GetViews()).Returns(viewNames);
            var storedProcedureNames = new[]{ "sp1", "sp2" };
            _schemaInfoMock.Setup(mn => mn.GetStoredProcedures()).Returns(storedProcedureNames);
            var functionNames = new[]{ "f1", "f2", "f3", "f4" };
            _schemaInfoMock.Setup(mn => mn.GetFunctions()).Returns(functionNames);

            //Act
            _schemaOperator.DropDataBaseObjects();

            //Assert
            _specificDBOperatorMock.Verify(m => m.ComposeScriptsDropTables(tableNames));
            _specificDBOperatorMock.Verify(m => m.ComposeScriptsDropViews(viewNames));
            _specificDBOperatorMock.Verify(m => m.ComposeScriptsDropStoredProcedures(storedProcedureNames));
            _specificDBOperatorMock.Verify(m => m.ComposeScriptsDropFunctions(functionNames));
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

        [Test]
        public void CreateDataBaseObjectsAskForTheScriptNeededToRecreateTheSchemaByUsingIScriptLoader(){
            //Act
            _schemaOperator.CreateDataBaseObjects();

            //Assert
            _scriptLoaderMock.Verify(m => m.GetScripts());
        }
    }
}