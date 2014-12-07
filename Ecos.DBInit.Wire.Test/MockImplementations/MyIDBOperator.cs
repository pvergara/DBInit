using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Wire.Test.MockImplementations
{
    public class MyIDBOperator : IDBOperator
	{
        public void CleanDB()
        {
            throw new System.NotImplementedException();
        }
        public void InitializeDB()
        {
            throw new System.NotImplementedException();
        }
        public void CleanData()
        {
            throw new System.NotImplementedException();
        }
        public void AddData()
        {
            throw new System.NotImplementedException();
        }
	}

}