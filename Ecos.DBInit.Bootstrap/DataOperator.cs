using Ecos.DBInit.Core.Interfaces;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core
{
    public class DataOperator:IDataOperator
    {
        readonly ISchemaInfo _schemaInfo;
        readonly IUnitOfWork _unitOfWork;
        readonly IScriptLoader _loader;
        readonly ISpecificDBOperator _specificDBOperator;

        public DataOperator(IUnitOfWork unitOfWork, ISchemaInfo schemaInfo,IScriptLoader loader,ISpecificDBOperator specificDBOperator)
        {
            _unitOfWork = unitOfWork;
            _schemaInfo = schemaInfo;
            _loader = loader;
            _specificDBOperator = specificDBOperator;
        }

        public void CleanEachTable()
        {
            var scripts = new List<Script>();

            scripts.AddRange(ComposeScriptsDeleteEachTable());

            _unitOfWork.Add(scripts);
        }

        private IEnumerable<Script> ComposeScriptsDeleteEachTable()
        {
            return _specificDBOperator.ComposeScriptsDelete(_schemaInfo.GetTables());
        }

        public void LoadDataScripts()
        {
            var scripts = _loader.GetScripts();

            _unitOfWork.Add(scripts);
        }
    }
}

