using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.MySql.ScriptHelpers;

namespace Ecos.DBInit.Wire
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

