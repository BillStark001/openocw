using Microsoft.Extensions.Options;
using Oocw.Database;

namespace Oocw.Backend.Services;

public class DatabaseService
{
    public class Settings
    {
        public string ConnectionHost { get; set; } = DBWrapper.DEFAULT_HOST;
    }

    private DBWrapper _db;
    public DBWrapper Wrapper => _db;

    public DatabaseService(IOptions<Settings> settings)
    {
        _db = new DBWrapper(settings.Value.ConnectionHost);
    }
}