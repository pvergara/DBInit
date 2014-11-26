namespace Ecos.DBInit.Core.Interfaces
{
    public interface ISchemaOperator
    {
        void ActivateReferentialIntegrity();
        void DeactivateReferentialIntegrity();
        void DropDataBaseObjects();
        void CreateDataBaseObjects();
    }
}