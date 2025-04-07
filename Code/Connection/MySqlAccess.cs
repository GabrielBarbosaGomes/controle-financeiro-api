using MySql.Data.MySqlClient;

namespace financeiroApi.Code.Connection
{
    public class MySqlAccess : IDisposable
    {
        private readonly IConfiguration _configuration;
        private MySqlConnection _db {  get; set; }

        public MySqlAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected MySqlConnection Db {
            get
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                _db ??= new MySqlConnection(connectionString);
                return _db;
            }
        }

        public void Dispose()
        {
            if(_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        ~MySqlAccess()
        {
            Dispose();
        }
    }
}
