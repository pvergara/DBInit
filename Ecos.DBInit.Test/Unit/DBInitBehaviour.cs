using Ecos.DBInit.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace Ecos.DBInit.Test.Unit
{
    [TestFixture]
    public class DBInitBehaviour
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IDBOperator> _dbOperatorMock;
        private readonly IDBInit _dbInit;

        private static byte _testCounterThatUsesSmartInit;

        public DBInitBehaviour()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _dbOperatorMock = new Mock<IDBOperator>();
            _dbInit = new Core.Base.DBInit(_uowMock.Object, _dbOperatorMock.Object);
        }

        private static void UpdateTheTestCounterThatUsesSmartInit()
        {
            _testCounterThatUsesSmartInit++;
        }

        [Test]
        public void InitSchemaConsistsToInvokeToCleanDBInitializeDBAndFlush(){
            //Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var dbOperatorMock = new Mock<IDBOperator>();
            var dbInit = new Core.Base.DBInit(uowMock.Object, dbOperatorMock.Object);

            //Act
            dbInit.InitSchema();

            //Assert
            dbOperatorMock.Verify(db => db.CleanDB(), Times.Once);
            dbOperatorMock.Verify(db => db.InitializeDB(), Times.Once);
            uowMock.Verify(uo => uo.Flush(), Times.Once);
        }


        [Test]
        public void InitDataConsistsToInvokeToCleanDBInitializeDBAndFlush(){
            //Arrange
            var uowMock = new Mock<IUnitOfWork>();
            var dbOperatorMock = new Mock<IDBOperator>();
            var dbInit = new Core.Base.DBInit(uowMock.Object, dbOperatorMock.Object);

            //Act
            dbInit.InitData();

            //Assert
            dbOperatorMock.Verify(db => db.CleanData(), Times.Once);
            dbOperatorMock.Verify(db => db.AddData(), Times.Once);
            uowMock.Verify(uo => uo.Flush(), Times.Once);
        }

        [Test]
        public void SmartInitRunsOnlyOnceInitSchemaEvenBetweenDifferentInvocations()
        {
            //Act
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();

            //Assert
            _dbOperatorMock.Verify(db => db.CleanDB(), Times.Once);
            _dbOperatorMock.Verify(db => db.InitializeDB(), Times.Once);

            //Post-Assert
            UpdateTheTestCounterThatUsesSmartInit();
        }

        [Test]
        public void SmartInitRunsInitDataAsTimesAsItHasInvoked()
        {
            //Arrange
            const int numberOfSmartInitLocalInvocations = 4;

            //Act
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();

            //Assert
            _dbOperatorMock.Verify(db => db.AddData(), Times.Exactly((_testCounterThatUsesSmartInit + 1) * numberOfSmartInitLocalInvocations));

            //Post-Assert
            UpdateTheTestCounterThatUsesSmartInit();
        }

        [Test]
        public void OnEverySmartInitInvokationTheresOnlyOneFlushMethodCall()
        {
            //Arrange
            const int numberOfSmartInitLocalInvocations = 4;

            //Act
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();
            _dbInit.SmartInit();

            //Assert
            _uowMock.Verify(uo => uo.Flush(), Times.Exactly((_testCounterThatUsesSmartInit+1) * numberOfSmartInitLocalInvocations));

            //Post-Assert
            UpdateTheTestCounterThatUsesSmartInit();
        }

    }
}
