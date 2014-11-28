using NUnit.Framework;
using Ecos.DBInit.Bootstrap;
using Ecos.DBInit.Core.Interfaces;
using Moq;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Test
{
    [TestFixture]
    public class UnitOfWorkBehaviour
    {
        public UnitOfWorkBehaviour()
        {
        }

        [Test]
        public void EveryScriptAddedWillBeExecuteOnlyOnceOnFlush()
        {
            //Arrange
            var scriptExecMock = new Mock<IScriptExec>();
            var uow = new UnitOfWorkOnCollection(scriptExecMock.Object);

            uow.Add(new[]{ Script.From("script 1"), Script.From("script 2") });
            uow.Add(new[] { Script.From("script 3"), Script.From("script 4"), Script.From("script 5") });
            uow.Add(new[] { Script.From("script 6") });

            //Act
            uow.Flush();

            //Assert
            scriptExecMock.Verify(se => se.Execute(It.IsAny<IEnumerable<Script>>()), Times.Once);
        }

    }
}