namespace Ecos.DBInit.Core
{
	public interface IDBOperator
	{
        void CleanDB();
        void InitializeDB();
        void CleanData();
        void AddData();
	}

}