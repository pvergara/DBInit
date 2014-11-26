using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Core.Interfaces
{
    public interface IScriptLoader
    {
        IEnumerable<Script> GetScripts();
    }
}

