using Oocw.Base;
using Oocw.Crawler.Core;
using Oocw.Crawler.Models;
using Oocw.Query;
using System.Linq.Expressions;
using System.Collections.Generic;
using Oocw.Database;
using Oocw.Cli.Tasks;
using Oocw.Cli.Utils;
using Oocw.Database.Models;
using System.Runtime.InteropServices;
using MongoDB.Driver;
using Yaap;
/*


var res = Lexer.NaiveMatch("a \\in [ false , true, , 2, \"3\", 4] && b #< ['ffff', 'fffffff']");
foreach (var lex in res)
Console.WriteLine(lex);
*/
string driverPath;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    driverPath = "G:/chromedriver.exe";
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    driverPath = "/Users/billstark001/Desktop/chromedriver_mac_arm64/chromedriver";
else
    driverPath = "/please/assign/a/valid/path";

var driver = new DriverWrapper(driverPath);
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
    db.Wrapper.CreateUniqueIndex();

    var (courses, codes) = FileUtils.Load<(Dictionary<int, Dictionary<int, (SyllabusRecord?, SyllabusRecord?)>>, List<int>)>(Meta.SAVEPATH_DETAILS_RAW);

    var (cl1, cl2, nr1, nr2) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(Meta.SAVEPATH_COURSE_LIST_RAW);

    List<Task> tasks = new();
    
    foreach (var (code, course) in courses.Yaap().Take(courses.Count()))
        foreach (var (year, (courseJa, courseEn)) in course)
        {
            var id = year * 100000 + code % 100000;
            var idStr = id.ToString();
            if (courseJa != null)
                await (SingleUpdate.Syllabus(db.Wrapper, courseJa, idStr, "ja"));
            if (courseEn != null)
                await (SingleUpdate.Syllabus(db.Wrapper, courseEn, idStr, "en"));
        }
    
    var course1 = new CourseRecord()
    {
        Code = "TST.1145",
        Name = "gansihuangxudong",
    };
    var course2 = new CourseRecord()
    {
        Code = "TST.1145",
        Name = "干死黄旭东",
    };
    // await SingleUpdate.Course(db.Wrapper, course1, lang: "en");
    // await SingleUpdate.Course(db.Wrapper, course2, lang: "zh");

    /* var course = new CourseRecord()
    {
        Code = "TST.1145",
        Name = "gansihuangxudong",
    };
    for (int _ = 0; _ < 114514; ++_)
        tasks.Add(SingleUpdate.Course(db.Wrapper, course, lang: "en"));
        
        tasks.Add(db.Wrapper.UseTransactionAsync(async (s, c) =>
        {
            //var crs = await (await s.Courses.FindAsync(s.Session, x => x.Code == course.Code, cancellationToken: c)).FirstOrDefaultAsync();
            var crs = await s.GetItemAsync(x => x.Courses, x => x.Code == course.Code, c);
            if (crs == null)
                await s.PutOrUpdateItemAsync(s => s.Courses, new Course() { Code = course.Code, Name = new() { En = course.Name } }, x => x.Code == course.Code, c);
                // await s.Courses.InsertOneAsync(s.Session, new Course() { Code = course.Code, Name = new() { En = course.Name } }, cancellationToken: c);
        }, CancellationToken.None));
        */
    // Task.WaitAll(tasks.ToArray());

    // Console.WriteLine("cl1");
    // foreach (var course in cl1)
    //     await SingleUpdate.Course(db.Wrapper, course);

    // Console.WriteLine("cl2");
    // foreach (var course in cl2)
    //     await SingleUpdate.Course(db.Wrapper, course, lang: "en");

    await db.Wrapper.RefreshOrganizations();
}
else
{

    driver.Initialize();
    crawler.Task2(Meta.SAVEPATH_COURSE_LIST_RAW);
    crawler.Task3(Meta.SAVEPATH_COURSE_LIST_RAW, Meta.SAVEPATH_DETAILS_RAW, 2020, 2022);
}