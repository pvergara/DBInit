using Ecos.DBInit.Core.Model;
using Ecos.DBInit.Core;
using Ecos.DBInit.MySql;


namespace Ecos.DBInit.Wire
{
    public class DBInitFactory
    {
        ProviderType _providerType;
        static IDBInit _dbInit;

        public static DBInitFactory From(ProviderType providerType)
        {
            return new DBInitFactory{ _providerType = providerType };
        }

        public DBInitFactory InitWith(string connectionString, string assemblyName)
        {
            _dbInit = new MySqlDBInit(connectionString,assemblyName);
            return this;
        }

        public IDBInit GetDBInit()
        {
            return _dbInit;
        }
    }
}
