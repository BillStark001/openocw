using MongoDB.Driver;
using Oocw.Crawler.Models;
using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Cli.Tasks;

public static class SingleUpdate
{
    public static async void Faculty(DBWrapper db, FacultyRecord faculty, string lang)
    {
        await db.UseTransactionAsync(async (dbSess, c) =>
        {

            var fdb = await dbSess.FindFacultyAsync(faculty.Code);
            if (fdb == null)
            {
                fdb = new();
                fdb.Id = faculty.Code;
            }
            fdb.Name.Update(faculty.Name, lang);

            return await dbSess.UpdateFacultyAsync(fdb);

        }, default);
    }

    public static async void Course(DBWrapper db, CourseRecord course, string lang)
    {
        await db.UseTransactionAsync(async (dbSess, c) =>
        {

            var crdb = await dbSess.FindCourseAsync(course.Code);
            if (crdb == null)
            {
                crdb = new();
                crdb.Code = course.Code;
            }



            crdb.Name.Update(faculty.Name, lang);

            return await dbSess.UpdateFacultyAsync(crdb);

        }, default);
    }
}
