namespace Ecos.DBInit.Core
{
    public interface ISchemaOperator
    {
        void ActivateReferentialIntegrity();
        void DeactivateReferentialIntegrity();
        void DropDataBaseObjects();
        void CreateDataBaseObjects();
    }
}

