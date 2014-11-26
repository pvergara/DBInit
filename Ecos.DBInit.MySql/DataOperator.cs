using Ecos.DBInit.Core.Interfaces;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class DataOperator:IDataOperator
    {
        readonly ISchemaInfo _schemaInfo;
        readonly IUnitOfWork _unitOfWork;
        readonly IScriptLoader _loader;

        public DataOperator(IUnitOfWork unitOfWork, ISchemaInfo schemaInfo,IScriptLoader loader)
        {
            _unitOfWork = unitOfWork;
            _schemaInfo = schemaInfo;
            _loader = loader;
        }

        public void CleanEachTable()
        {
            var scripts = new List<Script>();

            scripts.AddRange(ComposeScriptsDeleteEachTable());

            _unitOfWork.Add(scripts);
        }

        private IEnumerable<Script> ComposeScriptsDeleteEachTable()
        {
            return _schemaInfo.GetTables().
                Select(tableName => 
                    Script.From(string.Format("DELETE FROM {0};", tableName))
            );
        }

        public void LoadDataScripts()
        {
            var scripts = _loader.GetScripts();

            _unitOfWork.Add(scripts);
        }
    }
}

