using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Bootstrap
{
    public class DBOperator: IDBOperator
    {
        readonly ISchemaOperator _schemaOperator;
        readonly IDataOperator _dataOperator;

        public DBOperator(ISchemaOperator schemaOperator,IDataOperator dataOperator)
        {
           _schemaOperator = schemaOperator;
            _dataOperator = dataOperator;
        }

        private DBOperator(){}

        public void CleanDB()
        {
            _schemaOperator.DeactivateReferentialIntegrity();
            _schemaOperator.DropDataBaseObjects();
        }

        public void InitializeDB()
        {
            _schemaOperator.CreateDataBaseObjects();
            _schemaOperator.ActivateReferentialIntegrity();
        }

        public void CleanData()
        {
            _schemaOperator.DeactivateReferentialIntegrity();
            _dataOperator.CleanEachTable();
        }


        public void AddData()
        {
            _dataOperator.LoadDataScripts();
            _schemaOperator.ActivateReferentialIntegrity();
        }
    }
}

