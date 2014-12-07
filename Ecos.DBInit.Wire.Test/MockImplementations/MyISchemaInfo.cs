using Ecos.DBInit.Core.Interfaces;
using System.Collections.Generic;

namespace Ecos.DBInit.Wire.Test.MockImplementations
{
    public class MyISchemaInfo : ISchemaInfo
	{
        public IEnumerable<string> GetTables()
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<string> GetViews()
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<string> GetStoredProcedures()
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<string> GetFunctions()
        {
            throw new System.NotImplementedException();
        }
        public string DatabaseName
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
	}

}

