using Ecos.DBInit.Core.Interfaces;
using System.Collections.Generic;
using Ecos.DBInit.Core.Model;
using System.Linq;
using Ecos.DBInit.Core.ScriptHelpers;

namespace Ecos.DBInit.MySql
{
    public class DataOperator:IDataOperator
    {
        readonly ISchemaInfo _schemaInfo;
        readonly IUnitOfWork _unitOfWork;
        readonly string _assemblyName;

        public DataOperator(string assemblyName, IUnitOfWork unitOfWork, ISchemaInfo schemaInfo)
        {
            _unitOfWork = unitOfWork;
            _schemaInfo = schemaInfo;
            _assemblyName = assemblyName;
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
            var container = 
                ScriptFinderFluentFactory.
                    FromEmbeddedResource.
                        InitWith(_assemblyName, ScriptType.Data).
                    GetContainer();

            var scripts = 
                ScriptLoaderFluentFactory.
                    FromEmbeddedResource.
                        InitWith(_assemblyName, container).
                    GetScripts();

            _unitOfWork.Add(scripts);
        }
    }
}

