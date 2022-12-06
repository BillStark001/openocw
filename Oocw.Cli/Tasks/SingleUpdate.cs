using MongoDB.Driver;
using Oocw.Base;
using Oocw.Crawler.Models;
using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Oocw.Cli.Tasks;

public static class SingleUpdate
{
    public static async Task Faculty(DBWrapper db, FacultyRecord faculty, string lang)
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

    public static async Task Course(DBWrapper db, CourseRecord course, string lang)
    {
        // process faculty
        foreach (var f in course.Faculties)
            await Faculty(db, f, lang);
        await db.UseTransactionAsync(async (dbSess, c) =>
        {
            // ensure object & process name
            var crdb = await dbSess.FindCourseAsync(course.Code);
            if (crdb == null)
            {
                crdb = new();
                crdb.Code = course.Code;
                crdb.Name.Update(course.Name.Trim(), lang);
            } 
            else if (string.IsNullOrWhiteSpace(crdb.Name.Translate(lang)))
            {
                crdb.Name.Update(course.Name.Trim(), lang);
            }
            else
            {
                var cnamet = course.Name.Trim();
                var cnamer = crdb.Name.Translate(lang)?.Trim();
                var commonName = Base.Utils.LCP(cnamer, cnamet).Trim();
                if (string.IsNullOrWhiteSpace(commonName))
                {
                    // do nothing
                }
                else
                {
                    crdb.Name.Update(commonName, lang);
                }
            }

            // process lecture ocw id
            var query = HttpUtility.ParseQueryString(course.Url);
            var courseNumberStr = query.Get("KougiCD") ?? query.Get("JWC");
            var hasCourseNumber = int.TryParse(courseNumberStr, out var courseNumber);
            if (hasCourseNumber && !crdb.Classes.Contains(courseNumber))
                crdb.Classes = crdb.Classes.Append(courseNumber);


            await dbSess.UpdateCourseAsync(crdb);

        }, default);
    }

    public static async Task Syllabus(DBWrapper db, SyllabusRecord syllabus, string ocwId, string lang)
    {
        // process faculty
        foreach (var f in syllabus.Faculties)
            await Faculty(db, f, lang);

        await db.UseTransactionAsync(async (dbSess, c) =>
        {

            var cldb = await dbSess.FindClassAsync(ocwId);
            if (cldb == null)
            {
                cldb = new();
                cldb.Meta.OcwId = ocwId;
            }

            throw new NotImplementedException();

            await dbSess.UpdateClassAsync(cldb);

        }, default);
    }
}
