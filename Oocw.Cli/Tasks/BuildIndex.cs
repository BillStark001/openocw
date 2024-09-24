using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using Oocw.Database;
using MongoDB.Driver;
using Oocw.Database.Models;
using System.Threading.Tasks;

public static class BuildIndex {
    
    public static async void CreateTextIndex(this OocwDatabase db) {
        await db.Courses.Indexes.CreateOneAsync(new CreateIndexModel<Course>(Builders<Course>.IndexKeys.Text(x => x.Meta.SearchRecord)));
        await db.Classes.Indexes.CreateOneAsync(new CreateIndexModel<Class>(Builders<Class>.IndexKeys.Text(x => x.Meta.SearchRecord)));
        await db.Faculties.Indexes.CreateOneAsync(new CreateIndexModel<Faculty>(Builders<Faculty>.IndexKeys.Text(x => x.Meta.SearchRecord)));
    }
    
    public static async void CreateUniqueIndex(this OocwDatabase db)
    {
        await db.Courses.Indexes.CreateOneAsync(
            new CreateIndexModel<Course>(
                Builders<Course>.IndexKeys.Ascending(x => x.CourseCode), 
                new CreateIndexOptions() { Unique = true }
                )
            );
        await db.Classes.Indexes.CreateOneAsync(
            new CreateIndexModel<Class>(
                Builders<Class>.IndexKeys.Ascending(x => x.Meta.OcwId),
                new CreateIndexOptions() { Unique = true }
                )
            );
        await db.Faculties.Indexes.CreateOneAsync(
            new CreateIndexModel<Faculty>(
                Builders<Faculty>.IndexKeys.Ascending(x => x.Id),
                new CreateIndexOptions() { Unique = true }
                )
            );
    }

    public static async Task<IAsyncCursor<Course>> GetDirtyCoursesAsync(this OocwDatabase db)
    {
        var ret = await db.Courses.FindAsync(x => x.Meta.Dirty);
        return ret;
    }

    
}
