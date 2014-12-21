using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Core.Base
{
    public class DBInit: IDBInit
    {
        private readonly IDBOperator _operator;
        private readonly IUnitOfWork _unitOfWork;

        public DBInit(IUnitOfWork unitOfWork,IDBOperator oper)
        {
            _unitOfWork = unitOfWork;
            _operator = oper;
        }

        public void InitSchema()
        {
            InitSchemaNoFlush();
            _unitOfWork.Flush();
        }

        private void InitSchemaNoFlush()
        {
            _operator.CleanDB();
            _operator.InitializeDB();
        }

        public void InitData()
        {
            InitDataNoFlush();
            _unitOfWork.Flush();
        }

        private void InitDataNoFlush()
        {
            _operator.CleanData();
            _operator.AddData();
        }

        public void SmartInit()
        {
            if(Global.IsFirstTime)
                InitSchemaNoFlush();
            InitDataNoFlush();
            ExecuteAllScripts();
        }

        private void ExecuteAllScripts()
        {
            _unitOfWork.Flush();
        }
    }
}