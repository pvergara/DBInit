using Ecos.DBInit.Core.Interfaces;
using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace Ecos.DBInit.MySql
{
    public class SchemaOperator : ISchemaOperator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISchemaInfo _schemaInfo;
        private readonly IScriptLoader _scriptLoader;

        public SchemaOperator(IUnitOfWork unitOfWork,ISchemaInfo schemaInfo,IScriptLoader scriptLoader)
        {
            _unitOfWork = unitOfWork;
            _schemaInfo = schemaInfo;
            _scriptLoader = scriptLoader;
        }

        public void ActivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 1;") };

            _unitOfWork.Add(scripts);
        }

        public void DeactivateReferentialIntegrity()
        {
            var scripts = new []{ Script.From("SET @@foreign_key_checks = 0;") };
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

            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("TABLE", _schemaInfo.GetTables()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("VIEW", _schemaInfo.GetViews()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("PROCEDURE", _schemaInfo.GetStoredProcedures()));
            dropAllObjectsScript.AddRange(ComposeScriptsWithDropUsing("FUNCTION", _schemaInfo.GetFunctions()));

            return dropAllObjectsScript;
        }

        private static IEnumerable<Script> ComposeScriptsWithDropUsing(string typeOfObject, IEnumerable<string> objectNames)
        {
            return objectNames.
                    Select(objectName => 
                        Script.From(string.Format("DROP {0} IF EXISTS {1};", typeOfObject, objectName))
            );

        }

        public void CreateDataBaseObjects()
        {
            var scripts = _scriptLoader.GetScripts();

            _unitOfWork.Add(scripts);
        }
    }
}

