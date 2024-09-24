using Microsoft.Extensions.Options;
using Oocw.Database;

namespace Oocw.Backend.Services;

public class DatabaseService
{
    public class Settings
    {
        public string ConnectionHost { get; set; } = OocwDatabase.DEFAULT_HOST;
    }

    private OocwDatabase _db;
    public OocwDatabase Wrapper => _db;

    public DatabaseService(IOptions<Settings> settings)
    {
        _db = new OocwDatabase(settings.Value.ConnectionHost);
    }
}