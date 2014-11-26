using System.Collections.Generic;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.Interfaces
{
    public interface ISpecificDBOperator
    {
        IEnumerable<Script> ComposeScriptsDelete(IEnumerable<string> enumerable);
    }
}