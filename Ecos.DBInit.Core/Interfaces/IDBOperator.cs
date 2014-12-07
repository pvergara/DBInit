namespace Ecos.DBInit.Core.Interfaces
{
	public interface IDBOperator
	{
        void CleanDB();
        void InitializeDB();

        void CleanData();
        void AddData();
	}

}