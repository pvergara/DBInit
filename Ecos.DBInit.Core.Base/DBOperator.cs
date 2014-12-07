using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Core.Base
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

        public virtual void CleanDB()
        {
            _schemaOperator.DeactivateReferentialIntegrity();
            _schemaOperator.DropDataBaseObjects();
        }

        public virtual void InitializeDB()
        {
            _schemaOperator.CreateDataBaseObjects();
            _schemaOperator.ActivateReferentialIntegrity();
        }

        public virtual void CleanData()
        {
            _schemaOperator.DeactivateReferentialIntegrity();
            _dataOperator.CleanEachTable();
        }
            
        public virtual void AddData()
        {
            _dataOperator.LoadDataScripts();
            _schemaOperator.ActivateReferentialIntegrity();
        }
    }
}

