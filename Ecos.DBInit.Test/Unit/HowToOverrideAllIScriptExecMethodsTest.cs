using NUnit.Framework;
using Ecos.DBInit.Wire;
using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;

namespace Ecos.DBInit.Test.Unit
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
        public void OverwritingTheImplementationMakesThatModuleLoaderUsesTheDesiredImplementation()
        {
            //Arrange

            //Act
            var scriptExec = _moduleLoader.GetScriptExec();

            //Assert
            Assert.That(scriptExec, Is.TypeOf<MyIScriptExecImp>());
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

    public class MyIScriptExecImp : IScriptExec
    {
        public bool ExecuteAndProcessTValueInvoked        { get; private set; }

        public bool ExecuteAndProcessTKeyTValueInvoked        { get; private set; }

        public bool ExecuteScalarInvoked        { get; private set; }

        public bool TryConnectionAndExecuteInsideTransactionInvoked        { get; private set; }

        public bool CommitAndCloseInvoked        { get; private set; }

        public bool RollbackAndCloseInvoked        { get; private set; }

        public bool DisposeInvoked        { get; private set; }

        public MyIScriptExecImp()
        {
            ExecuteAndProcessTValueInvoked = false;
            ExecuteAndProcessTKeyTValueInvoked = false;
            ExecuteScalarInvoked = false;
            TryConnectionAndExecuteInsideTransactionInvoked = false;
            CommitAndCloseInvoked = false;
            RollbackAndCloseInvoked = false;
            DisposeInvoked = false;
        }

        public void ExecuteAndProcess<TValue>(Script script, ICollection<TValue> result, Func<IDataReader, ICollection<TValue>, ICollection<TValue>> function)
        {
            ExecuteAndProcessTValueInvoked = true;
        }

        public void ExecuteAndProcess<TKey, TValue>(IDictionary<TKey, Script> indexedQueries, IDictionary<TKey, ICollection<TValue>> indexedResults, Func<IDataReader, TKey, ICollection<TValue>, ICollection<TValue>> functionOnEachQueryToEachResult)
        {
            ExecuteAndProcessTKeyTValueInvoked = true;
        }

        public T ExecuteScalar<T>(Script script)
        {
            ExecuteScalarInvoked = true;
            object result = 0;
            return (T)result;
        }

        public void TryConnectionAndExecuteInsideTransaction(Script scripts)
        {
            TryConnectionAndExecuteInsideTransactionInvoked = true;
        }

        public void CommitAndClose()
        {
            CommitAndCloseInvoked = true;
        }

        public void RollbackAndClose()
        {
            RollbackAndCloseInvoked = true;
        }

        public void Dispose()
        {
            DisposeInvoked = true;
        }
    }

}