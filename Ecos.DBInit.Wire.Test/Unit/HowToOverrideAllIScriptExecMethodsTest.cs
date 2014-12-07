using NUnit.Framework;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using System;
using System.Collections.Generic;
using Ecos.DBInit.Wire.Test.MockImplementations;

namespace Ecos.DBInit.Wire.Test.Unit
{
    [TestFixture]
    public class HowToOverrideAllIScriptExecMethodsTest
    {
        private ModuleLoader _moduleLoader;
        private IScriptExec _scriptExec;
        private MyIScriptExecImp _specificScriptExec;

        [SetUp]
        public void BeforeEachTest()
        {
            _moduleLoader = new ModuleLoader("", "", ProviderType.MySql);

            _moduleLoader.OverwriteImplementationOf(typeof(IScriptExec), typeof(MyIScriptExecImp));
            _moduleLoader.Wire();
            _scriptExec = _moduleLoader.GetScriptExec();
            _specificScriptExec = _scriptExec as MyIScriptExecImp;
        }
            
        [Test]
        public void TheSystemUsesMy_ExecuteScalar_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.ExecuteScalarInvoked, Is.False);
            _scriptExec.ExecuteScalar<Int32>(Script.From(""));

            //Assert
            Assert.That(_specificScriptExec.ExecuteScalarInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_ExecuteAndProcessTValue_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.ExecuteAndProcessTValueInvoked, Is.False);
            _scriptExec.ExecuteAndProcess<string>(Script.From(""), new string[0], (a, e) => new string[0]);

            //Assert
            Assert.That(_specificScriptExec.ExecuteAndProcessTValueInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_ExecuteAndProcessTKeyTValue_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.ExecuteAndProcessTKeyTValueInvoked, Is.False);
            _scriptExec.ExecuteAndProcess<int,string>(new Dictionary<int,Script>(), new Dictionary<int,ICollection<string>>(), (a, i, e) => new string[0]);

            //Assert
            Assert.That(_specificScriptExec.ExecuteAndProcessTKeyTValueInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_TryConnectionAndExecuteInsideTransactionInvoked_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.TryConnectionAndExecuteInsideTransactionInvoked, Is.False);
            _scriptExec.TryConnectionAndExecuteInsideTransaction(Script.From(""));

            //Assert
            Assert.That(_specificScriptExec.TryConnectionAndExecuteInsideTransactionInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_CommitAndClose_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.CommitAndCloseInvoked, Is.False);
            _scriptExec.CommitAndClose();

            //Assert
            Assert.That(_specificScriptExec.CommitAndCloseInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_RollbackAndClose_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.RollbackAndCloseInvoked, Is.False);
            _scriptExec.RollbackAndClose();

            //Assert
            Assert.That(_specificScriptExec.RollbackAndCloseInvoked, Is.True);
        }

        [Test]
        public void TheSystemUsesMy_Dispose_Implementation()
        {
            //Act
            Assert.That(_specificScriptExec.DisposeInvoked, Is.False);
            _scriptExec.Dispose();

            //Assert
            Assert.That(_specificScriptExec.DisposeInvoked, Is.True);
        }
    }        
}