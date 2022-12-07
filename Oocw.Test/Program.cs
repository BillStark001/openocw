// See https://aka.ms/new-console-template for more information

using Oocw.Base;
using Oocw.Crawler.Core;
using Oocw.Crawler.Models;
using Oocw.Query;
using System.Linq.Expressions;
using System.Collections.Generic;
using Oocw.Database;
using Oocw.Cli.Tasks;
using Oocw.Cli.Utils;

var res = Lexer.NaiveMatch("a \\in [ false , true, , 2, \"3\", 4] && b #< ['ffff', 'fffffff']");
foreach (var lex in res)
    Console.WriteLine(lex);

var driver = new DriverWrapper("G:/chromedriver.exe");
var crawler = new Crawler(driver);

Func<int, int> finc = x => x + 1;
Func<int, int> fadd3 = x => finc(finc(finc(x)));
Expression<Func<int, int>> eadd3 = x => fadd3(x);
Expression<Func<int, int>> eadd31 = x => finc(finc(finc(x)));
Expression<Func<int, int>> eadd32 = x => x + 3;
Expression<Func<int, int>> eadd6 = ExpressionUtils.Combine(eadd3, eadd32);


if (true)
{
    Database db = Database.Instance;
    db.Initialize();

    var (courses, codes) = FileUtils.Load<(Dictionary<int, Dictionary<int, (SyllabusRecord?, SyllabusRecord?)>>, List<int>)>(Meta.savepath_details_raw);

    var (cl1, cl2, nr1, nr2) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(Meta.savepath_course_list_raw);

    foreach (var (code, course) in courses)
        foreach (var (year, (courseJa, courseEn)) in course)
        {
            var id = year * 100000 + code % 100000;
            var idStr = id.ToString();
            if (courseJa != null)
                await SingleUpdate.Syllabus(db.Wrapper, courseJa, idStr, "ja");
            if (courseEn != null)
                await SingleUpdate.Syllabus(db.Wrapper, courseEn, idStr, "en");
        }

    // foreach (var course in cl1)
    //     await SingleUpdate.Course(db.Wrapper, course);

    db.Wrapper.RefreshOrganizations();
}
else
{

    driver.Initialize();
    crawler.Task2(Meta.savepath_course_list_raw);
    crawler.Task3(Meta.savepath_course_list_raw, Meta.savepath_details_raw, 2020, 2022);
}