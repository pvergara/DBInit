using Ecos.DBInit.Core.Interfaces;

namespace Ecos.DBInit.Test.MockImplementations
{
    public class MyISchemaOperator : ISchemaOperator
	{
        public void ActivateReferentialIntegrity()
        {
            throw new System.NotImplementedException();
        }
        public void DeactivateReferentialIntegrity()
        {
            throw new System.NotImplementedException();
        }
        public void DropDataBaseObjects()
        {
            throw new System.NotImplementedException();
        }
        public void CreateDataBaseObjects()
        {
            throw new System.NotImplementedException();
        }
	}

}

