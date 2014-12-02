using Ecos.DBInit.Core.Base;
using NUnit.Framework;
using Ecos.DBInit.Core.Interfaces;
using Moq;
using Ecos.DBInit.Core.Model;
using System;

namespace Ecos.DBInit.Test
{
    [TestFixture]
    public class UnitOfWorkBehaviour
    {
        private readonly Script _firstScript = Script.From("script 1");
        private readonly Script _secondScript = Script.From("script 2");
        private readonly Script _thirdScript = Script.From("script 3");
        private Mock<IScriptExec> _scriptExecMock;
        private IUnitOfWork _uow;

        [SetUp]
        public void BeforeEachTest()
        {
            _scriptExecMock = new Mock<IScriptExec>();
            _uow = new UnitOfWorkCurrent(_scriptExecMock.Object);
        }

        [Test]
        public void EveryScriptAddedWillBeExecuteOnlyOnceOnFlush()
        {
            //Arrange
            _uow.Add(new[] { _firstScript, _secondScript });
            _uow.Add(new[] { _thirdScript });

            //Act
            _uow.Flush();

            //Assert
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_firstScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_secondScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_thirdScript), Times.Once);
        }

        [Test]
        public void EveryScriptAddedWillBeExecuteOnlyOnceOnFlushEvenIfTheFlushIsExecutedMoreThatOnce()
        {
            //Arrange
            _uow.Add(new[] { _firstScript, _secondScript });
            _uow.Add(new[] { _thirdScript });

            //Act
            _uow.Flush();
            _uow.Flush();
            _uow.Flush();
            _uow.Flush();
            _uow.Flush();

            //Assert
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_firstScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_secondScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_thirdScript), Times.Once);
        }

        [Test]
        public void OnEachFlushItWillEmptyTheScriptsPreviouslyAdded()
        {
            //Arrange
            _uow.Add(new[] { _firstScript, _secondScript });
            _uow.Flush();
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_firstScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_secondScript), Times.Once);

            //Act
            _uow.Add(new[] { _thirdScript });
            _uow.Flush();

            //Assert
            //ONCE IS NOT TWICE!!!!
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_firstScript), Times.Once);
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_secondScript), Times.Once);
            //_________________
            _scriptExecMock.Verify(se => se.TryConnectionAndExecuteInsideTransaction(_thirdScript), Times.Once);
        }

        [Test]
        public void OnEachFlushTheTransactionWillBeCommitedIfThereIsNoError()
        {
            //Arrange
            _uow.Add(new[] { _firstScript, _secondScript, _thirdScript });

            //Act
            _uow.Flush();

            //Assert
            _scriptExecMock.Verify(se => se.CommitAndClose(), Times.Once);
            _scriptExecMock.Verify(se => se.RollbackAndClose(), Times.Never);
        }


        [Test]
        public void OnEachFlushTheTransactionWillBeRolledBackOnException()
        {
            //Arrange
            _scriptExecMock.Setup(se => se.TryConnectionAndExecuteInsideTransaction(_thirdScript)).Throws(new Exception("yeeeaahh"));
            _uow.Add(new[] { _firstScript, _secondScript, _thirdScript });

            //Act
            try
            {
                _uow.Flush();
            }
            catch (Exception)
            {
                //Assert
                _scriptExecMock.Verify(se => se.CommitAndClose(), Times.Never);
                _scriptExecMock.Verify(se => se.RollbackAndClose(), Times.Once);
            }
        }
    }
}