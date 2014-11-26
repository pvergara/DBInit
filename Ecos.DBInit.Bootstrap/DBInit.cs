using Ecos.DBInit.Bootstrap;
using Ecos.DBInit.Core;
using Ecos.DBInit.MySql;
using Ecos.DBInit.MySql.ScriptHelpers;
using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Bootstrap
{
    public class DBInit: IDBInit
    {
        private readonly IDBOperator _operator;
        private readonly IUnitOfWork _unitOfWork;

        public DBInit(string connectionString,string assemblyName){
            //TODO: SINGLETON!!!!
            var scriptExec = ScriptExecFactory.From().InitWith(connectionString).GetScriptExec();

            //TODO: SINGLETON!!!!
            _unitOfWork = new UnitOfWorkOnCollection(scriptExec);
            //SPECIFIC
            var mySqlSchemaInfo = new MySqlSchemaInfo(connectionString, scriptExec);
            var schemaOperator = new SchemaOperator(assemblyName, _unitOfWork, mySqlSchemaInfo);
            var dataOperator = new DataOperator(assemblyName, _unitOfWork, mySqlSchemaInfo);
            _operator = new DBOperator(schemaOperator, dataOperator);
        }

        public void InitSchema()
        {
            _operator.CleanDB();
            _operator.InitializeDB();
            _unitOfWork.Flush();
        }

        public void InitData()
        {
            _operator.CleanData();
            _operator.AddData();
            _unitOfWork.Flush();
        }
    }
}