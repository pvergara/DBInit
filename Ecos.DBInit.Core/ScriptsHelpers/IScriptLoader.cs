using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.IO;

namespace Ecos.DBInit.Core.ScriptsHelpers
{
    public interface IScriptLoader
    {
        IEnumerable<StreamReader> Load(Container container);
    }
}

