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