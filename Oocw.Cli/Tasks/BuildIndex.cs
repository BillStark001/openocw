using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using Oocw.Database;
using MongoDB.Driver;
using Oocw.Database.Models;
using System.Threading.Tasks;

public static class BuildIndex {
    
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

    
}
