namespace Ecos.DBInit.Core
{
    public interface IDataOperator
    {
        void CleanEachTable();
        void LoadDataScripts();
    }
}

