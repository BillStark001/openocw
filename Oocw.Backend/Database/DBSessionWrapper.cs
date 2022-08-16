using MongoDB.Driver;
using MongoDB.Bson;

namespace Oocw.Backend.Database
{
    public class DBSessionWrapper: DBWrapper, IDisposable
    {
        protected IClientSessionHandle _sess;

        public IClientSessionHandle Session => _sess;


        public DBSessionWrapper(IClientSessionHandle sess): base(sess.Client)
        {
            _sess = sess;
        }

        public void Dispose()
        {
            _sess.Dispose();
        }
    }
}
