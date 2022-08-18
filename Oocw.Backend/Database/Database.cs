using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Backend.Utils;

namespace Oocw.Backend.Database
{
    public class Database
    {
        public static readonly Database Instance = new();

        private readonly FilterDefinitionBuilder<BsonDocument> _f;

        private DBWrapper? _wrapper;
        public DBWrapper Wrapper => _wrapper ?? throw new InvalidOperationException("Database is not initialized yet.");

        public IMongoCollection<BsonDocument> Classes => Wrapper.Classes;
        public IMongoCollection<BsonDocument> Courses => Wrapper.Courses;
        public IMongoCollection<BsonDocument> Faculties => Wrapper.Faculties;

        protected Database()
        {
            // TODO read config
            _f = Builders<BsonDocument>.Filter;
        }

        public void Initialize(string host = "mongodb://localhost:27017/")
        {
            _wrapper = new DBWrapper(host);
        }

        // utils
        // TODO add cache (dict & watch)

        public BsonDocument? GetCourseInfo(string classId)
        {
            return Courses.Find(_f.Eq(Definitions.KEY_CODE, classId)).FirstOrDefault();
        }

        public IEnumerable<(int, string)> GetFacultyNames(IEnumerable<int> fs, string lang = "ja")
        {
            List<(int, string)> ans = new();
            var qs = _f.In(Definitions.KEY_ID, fs);
            Dictionary<int, string?> dt = new();
            foreach (var f in Faculties.Find(qs).ToList())
            {
                f.TryGetElement(Definitions.KEY_NAME, out var nameRaw);
                f.TryGetElement(Definitions.KEY_ID, out var idRaw);
                dt[(int)idRaw.Value] = nameRaw.Value.TryGetTranslation(lang);
            }
            foreach (var id in fs)
            {
                ans.Add((id, dt[id] ?? $"Not Found ({id})"));
            }
            return ans;
        }
    }
}
