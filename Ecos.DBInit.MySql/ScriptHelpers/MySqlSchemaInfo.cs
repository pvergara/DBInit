using MySql.Data.MySqlClient;

namespace Ecos.DBInit.MySql.ScriptHelpers
{
    public class MySqlSchemaInfo
    {
        readonly MySqlConnection _connection;

        public MySqlSchemaInfo(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public string DatabaseName
        {
            get { return _connection.Database; }
        }
    }
}

