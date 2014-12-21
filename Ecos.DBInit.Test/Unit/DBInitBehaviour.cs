using Ecos.DBInit.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace Ecos.DBInit.Test.Unit
{
    [TestFixture]
    public class DBInitBehaviour
    {
        private static byte _testCounter;

        [Test]
        public void SmartInitRunsOnlyOnceInitSchemaEvenBetweenDifferentInvocations()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var dbOperatorMock = new Mock<IDBOperator>();
            var dbInit = new Core.Base.DBInit(uowMock.Object, dbOperatorMock.Object);

            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();

            if(_testCounter==0)
                dbOperatorMock.Verify(db => db.CleanDB(), Times.Once);
            else
                dbOperatorMock.Verify(db => db.CleanDB(), Times.Never);

            UpdateTheTestCounter();
        }

        private static void UpdateTheTestCounter()
        {
            _testCounter++;
        }

        [Test]
        public void SmartInitRunsInitDataAsTimesAsItHasInvoked()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var dbOperatorMock = new Mock<IDBOperator>();
            var dbInit = new Core.Base.DBInit(uowMock.Object, dbOperatorMock.Object);

            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();

            dbOperatorMock.Verify(db => db.AddData(), Times.Exactly(4));

            UpdateTheTestCounter();
        }

        [Test]
        public void OnEverySmartInitInvokationTheresOnlyOneFlushMethodCall()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var dbOperatorMock = new Mock<IDBOperator>();
            var dbInit = new Core.Base.DBInit(uowMock.Object, dbOperatorMock.Object);

            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();
            dbInit.SmartInit();

            uowMock.Verify(uo => uo.Flush(), Times.Exactly(4));

            UpdateTheTestCounter();
        }

    }
}
