using System.Collections.Generic;
using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;

namespace Ecos.DBInit.Core.Base
{
    public class SchemaOperator : ISchemaOperator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISchemaInfo _schemaInfo;
        private readonly IScriptLoader _scriptLoader;
        private readonly ISpecificDBOperator _specificDBOperator;

        public SchemaOperator(IUnitOfWork unitOfWork,ISchemaInfo schemaInfo,IScriptLoader scriptLoader,ISpecificDBOperator specificDBOperator)
        {
            _unitOfWork = unitOfWork;
            _schemaInfo = schemaInfo;
            _scriptLoader = scriptLoader;
            _specificDBOperator = specificDBOperator;
        }

        public void ActivateReferentialIntegrity()
        {
            var scripts = new []{ _specificDBOperator.ComposeActivateReferentialIntegrity() };

            _unitOfWork.Add(scripts);
        }

        public void DeactivateReferentialIntegrity()
        {
            var scripts = new []{ _specificDBOperator.ComposeDeactivateReferentialIntegrity() };
            _unitOfWork.Add(scripts);
        }

        public void DropDataBaseObjects()
        {
            var scripts = new List<Script>();

            scripts.AddRange(GetDropAllSchemaObjectsScripts());

            _unitOfWork.Add(scripts);

        }

        private IEnumerable<Script> GetDropAllSchemaObjectsScripts()
        {
            var dropAllObjectsScript = new List<Script>();

            dropAllObjectsScript.AddRange(_specificDBOperator.ComposeScriptsDropTables(_schemaInfo.GetTables()));
            dropAllObjectsScript.AddRange(_specificDBOperator.ComposeScriptsDropViews(_schemaInfo.GetViews()));
            dropAllObjectsScript.AddRange(_specificDBOperator.ComposeScriptsDropStoredProcedures(_schemaInfo.GetStoredProcedures()));
            dropAllObjectsScript.AddRange(_specificDBOperator.ComposeScriptsDropFunctions(_schemaInfo.GetFunctions()));

            return dropAllObjectsScript;
        }
            

        public void CreateDataBaseObjects()
        {
            var scripts = _scriptLoader.GetScripts();

            _unitOfWork.Add(scripts);
        }
    }
}

