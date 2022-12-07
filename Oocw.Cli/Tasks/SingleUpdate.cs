using MongoDB.Bson;
using MongoDB.Driver;
using Oocw.Base;
using Oocw.Cli.Utils;
using Oocw.Crawler.Models;
using Oocw.Crawler.Utils;
using Oocw.Database;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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

    public static async Task Course(DBWrapper db, CourseRecord course, CourseRecord.Additional? add = null, string lang = "ja")
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
            var courseNumberStr = query.Get("KougiCD", "JWC");
            var hasCourseNumber = int.TryParse(courseNumberStr, out var courseNumber);
            if (hasCourseNumber && !crdb.Classes.Contains(courseNumber))
                crdb.Classes = crdb.Classes.Append(courseNumber);

            if (add != null)
            {
                if (add.Credit2.HasValue)
                    crdb.Credit = add.Credit2.Value;
                else if (add.Credit > 0)
                    crdb.Credit = (crdb.Credit.Item1, crdb.Credit.Item2, crdb.Credit.Item3, add.Credit);
            }

            await dbSess.UpdateCourseAsync(crdb);

        }, default);
    }

    public static async Task Syllabus(DBWrapper db, SyllabusRecord syllabus, string ocwId, string lang = "ja", bool forceUpdate = false, bool forceUpdateNotes = false)
    {
        // since it is processed in course, omit
        /*
        // process faculty
        foreach (var f in syllabus.Faculties)
            await Faculty(db, f, lang);
        */
        // ensure the course exists (and also ensure the faculties exist)
        await Course(
            db, 
            new(syllabus, lang), 
            new() {
                Unit = syllabus.Summary.Unit, 
                Credit = int.TryParse(syllabus.Summary.Credit, out var c) ? c : -1
            }, 
            lang);

        await db.UseTransactionAsync(async (dbSess, c) =>
        {
            // ensure the model
            var cldb = await dbSess.FindClassAsync(ocwId);
            if (cldb == null)
            {
                cldb = new();
                cldb.Meta.OcwId = ocwId;
            }
            cldb.Code = syllabus.Summary.Code;

            // update time
            var updateTimeSyllabus = ParseUtils.ParseDate(syllabus.Summary.SyllabusUpdated);
            var updateTimeNotes = ParseUtils.ParseDate(syllabus.Summary.NotesUpdated);

            if (forceUpdate || updateTimeSyllabus > cldb.UpdateTimeSyllabus)
            {
                cldb.UpdateTimeSyllabus = updateTimeSyllabus;
                if (int.TryParse(syllabus.YearStr.ToHalfWidth().Replace("年度", "").Trim(), out var year))
                    cldb.Year = year;
                cldb.ClassName = syllabus.Summary.Class;
                cldb.Lecturers = new HashSet<int>(cldb.Lecturers.Concat(syllabus.Faculties.Select(x => x.Code))).OrderBy(x => x);
                cldb.Format = ParseUtils.ParseForm(syllabus.Summary.Format);
                cldb.Quarter = ParseUtils.ParseAcademicQuarter(syllabus.Summary.Quarter);
                cldb.Addresses = ParseUtils.ParseAddress(syllabus.Summary.Address);
                cldb.Language = ParseUtils.ParseLanguage(syllabus.Summary.Language);

                cldb.Skills = syllabus.Skills;
                // TODO media-enhanced courses?

                // handle schedule
                cldb.Syllabus.PutItem(syllabus.Detail.ToBsonDocument(), "raw");
            }
            else if (cldb.Syllabus.GetItem("raw") == null)
                cldb.Syllabus.PutItem(syllabus.Detail.ToBsonDocument(), "raw");


            // handle lecture 
            foreach (var (number, title, detail) in syllabus.Schedule)
            {
                if (!cldb.Lecturers.Contains(number))
                    cldb.Lectures[number] = new() { Number = number, };

                var lect = cldb.Lectures[number];
                if (forceUpdate || updateTimeSyllabus > cldb.UpdateTimeSyllabus)
                {
                    lect.Title.Update(title, lang);
                    lect.Detail.Update(detail, lang);
                }
                else if (string.IsNullOrWhiteSpace(lect.Title.Translate(lang)))
                    lect.Title.Update(title, lang);
                else if (string.IsNullOrWhiteSpace(lect.Detail.Translate(lang)))
                    lect.Detail.Update(detail, lang);
            }


            // handle notes

            // preprocessing
            foreach (var note in syllabus.Notes)
            {
                if (note.Title == SyllabusRecord.NoteRecord.SUPP_TITLE)
                    note.Number = -2;
                else
                    note.Number = note.Title.ExtractInteger(out var n) ? n : -1;

                if (!cldb.Lecturers.Contains(note.Number))
                    cldb.Lectures[note.Number] = new() { Number = note.Number, };
            }

            // update update time
            if (forceUpdateNotes || updateTimeNotes > cldb.UpdateTimeNotes)
                cldb.UpdateTimeNotes = updateTimeNotes;

            foreach (var note in syllabus.Notes)
            {
                var nrec = cldb.Lectures[note.Number];
                var updateDate = nrec.Date.HasValue || forceUpdateNotes || updateTimeNotes > cldb.UpdateTimeNotes;
                var updateType = nrec.Type == 0 || forceUpdateNotes || updateTimeNotes > cldb.UpdateTimeNotes;

                if (updateDate)
                    nrec.Date = ParseUtils.ParseDate(note.LectureDate);
                if (updateType)
                    nrec.Type = ParseUtils.ParseForm(note.LectureType);

                if (nrec.Links != null)
                    nrec.Links = new HashSet<string>(nrec.Links.Concat(note.Links));
                else
                    nrec.Links = new HashSet<string>(note.Links);
            }

            await dbSess.UpdateClassAsync(cldb);

        }, default);
    }
}
