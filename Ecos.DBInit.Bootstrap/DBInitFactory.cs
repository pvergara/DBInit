using Ecos.DBInit.Core;
using Ecos.DBInit.MySql;


namespace Ecos.DBInit.Bootstrap
{
    public class DBInitFactory
    {
        static IDBInit _dbInit;

        public static DBInitFactory From()
        {
            return new DBInitFactory{ };
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
