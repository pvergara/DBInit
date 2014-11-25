using Ecos.DBInit.MySql.ScriptHelpers;
using Ecos.DBInit.Core.ScriptHelpers;

namespace Ecos.DBInit.Bootstrap
{
    public class ScriptExecFactory
    {
        IScriptExec _scriptExec;

        private  ScriptExecFactory()
        {
        }

        public static ScriptExecFactory From()
        {
            return new ScriptExecFactory();
        }

        public ScriptExecFactory InitWith(string connectionString)
        {
            _scriptExec = new MySqlScriptExec(connectionString);

            return this;
        }

        public IScriptExec GetScriptExec()
        {
            return _scriptExec;
        }
    }
}

