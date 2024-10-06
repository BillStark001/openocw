using Microsoft.Extensions.Options;
using Oocw.Database;

namespace Oocw.Backend.Services;

public class DatabaseService(IOptions<DatabaseService.Settings> settings)
{
    public class Settings
    {
        public string ConnectionHost { get; set; } = OocwDatabase.DEFAULT_HOST;
    }

    private readonly OocwDatabase _db = new(settings.Value.ConnectionHost);
    public OocwDatabase Wrapper => _db;
}