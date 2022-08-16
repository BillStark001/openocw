namespace Oocw.Backend.Database
{
    public class Database
    {
        public static readonly Database Instance = new();

        private DBWrapper? _wrapper;
        public DBWrapper Wrapper => _wrapper ?? throw new InvalidOperationException("Database is not initialized yet.");

        protected Database()
        {
            // TODO read config
        }

        public void Initialize()
        {
            var host = "mongodb://localhost:27017/";
            _wrapper = new DBWrapper(host);
        }
    }
}
