using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using Oocw.Database;
using MongoDB.Driver;
using Oocw.Database.Models;
using System.Threading.Tasks;

public static class BuildSearchIndex {
    
    public static void CreateTextIndex(this DBWrapper db) {
        db.Courses.Indexes.CreateOneAsync(new CreateIndexModel<Course>(Builders<Course>.IndexKeys.Text(x => x.Meta.SearchRecord)));
        db.Classes.Indexes.CreateOneAsync(new CreateIndexModel<Class>(Builders<Class>.IndexKeys.Text(x => x.Meta.SearchRecord)));
        db.Faculties.Indexes.CreateOneAsync(new CreateIndexModel<Faculty>(Builders<Faculty>.IndexKeys.Text(x => x.Meta.SearchRecord)));
    }
    
    public static async Task<IAsyncCursor<Course>> GetDirtyCoursesAsync(this DBWrapper db)
    {
        var ret = await db.Courses.FindAsync(x => x.Meta.Dirty);
        return ret;
    }


    public static bool update_text_index(object col, Dictionary<string, object> doc, List<string> index, object sess = null) {
        var res = col.find_one_and_update(new Dictionary<object, object> {
            {
                "_id",
                doc["_id"]}}, new Dictionary<object, object> {
            {
                "$set",
                new Dictionary<object, object> {
                    {
                        TEXT_INDEX_TARGET,
                        index},
                    {
                        TEXT_DIRTY_TARGET,
                        false}}}}, session: sess);
        return res != null;
    }
    
    public static object iter_dirty_doc(object col, object sess = null) {
        var doc = get_random_dirty_doc(col, sess);
        while (doc != null) {
            yield return doc;
            doc = get_random_dirty_doc(col, sess);
        }
    }
    
    public static object db = init_db();
    
    static build_search_index() {
        create_index(db);
        s[wstr] = true;
        s[wfeat] = true;
    }
    
    public static Dictionary<object, object> s = new Dictionary<object, object> {
    };
    
    public static List<string> wpos = word.pos.ToString().split(",")[0];
    
    public static string wstr = word.ToString();
    
    public static string wfeat = word.feature.lemma.ToString();
    
    public static list s = s.keys().ToList();
    
    public static bool dres = update_text_index(ccls, doc, s);
    
}
