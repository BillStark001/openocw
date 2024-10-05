using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using AngleSharp;
using Oocw.Crawler.Utils;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using Oocw.Crawler.Models;
using OpenQA.Selenium.DevTools;
using Oocw.Base;
using System.Web;
using Yaap;
using System.IO;

namespace Oocw.Crawler.Core;

using IDSS = IDictionary<string, string>;
using DSS = Dictionary<string, string>;
using IMDSS = ImmutableDictionary<string, string>;

using DictStrArgs = NestedDictionary<string, string?>;
using DictIntSyl = Dictionary<int, (SyllabusRecord?, SyllabusRecord?)>;

public class Crawler
{

    public const string ADDR_DEFAULT = "http://www.ocw.titech.ac.jp/index.php";

    public static readonly int THIS_YEAR = DateTime.Now.Year;

    public const int INNOVATION_YEAR = 2016;



    public readonly DriverWrapper Driver;

    public Crawler(DriverWrapper driver)
    {
        Driver = driver;
    }


    public static Func<string, DSS> args_lang = lang => new() { { "lang", lang } };

    public static Func<int, DSS> args_year = year => new() { { "Nendo", year.ToString() } };

    public static Func<bool, DSS> args_term = zenki => new() { { "Gakki", zenki ? "1" : "2" } };

    public static Func<bool, DSS> args_lnote = lnote => new() { { "vid", lnote ? "05" : "03" } };

    public static readonly IMDSS args_general_list = new DSS
    {
        ["module"] = "General",
        ["tab"] = "2"
    }.ToImmutableDictionary();

    public static readonly IMDSS args_general_course = new DSS
    {
        {"module", "General"},
        { "action","T0300"}
    }.ToImmutableDictionary();

    public static readonly IMDSS args_archive_list = new DSS
    {
        { "module", "Archive"},
        {  "action",  "ArchiveIndex"},
        {  "tab", "2"},
        { "vid",  "4"}
    }.ToImmutableDictionary();

    public static readonly IMDSS args_archive_course = new DSS
    {
        { "module", "Archive"},
        { "action","ArchiveIndex"},
        {"SubAction", "T0300"}
    }.ToImmutableDictionary();

    public static readonly IMDSS args_archive_course_old = new DSS
    {
        { "module", "Archive"},
        { "action", "KougiInfo"},
        { "GakubuCD", "100"},
        { "GakkaCD", "12"}
    }.ToImmutableDictionary();

    // funcs
    public (List<DictStrArgs>, DictStrArgs) GetDepartmentList(int year = 2020, string lang = "JA")
    {
        var url = $"http://www.ocw.titech.ac.jp/index.php?module=Archive&action=ArchiveIndex&GakubuCD=1&GakkaCD=311100&KeiCD=11&tab=2&focus=200&lang={lang}&Nendo={year}&SubAction=T0200";
        var html = Driver.GetHtmlAfterLoaded(url);
        var (cats, cats_flat, excluded) = DataExtractor.GetDepartmentList(html.Dom());
        Console.WriteLine(excluded);
        return (cats, cats_flat);
    }


    public IEnumerable<CourseRecord> GetCourseList(IDSS info, int year = 114514, int retry_limit = 5)
    {
        var args_course = year >= THIS_YEAR ? args_general_list : args_archive_list;

        if (year >= THIS_YEAR)
        {
            year = THIS_YEAR;
            if (info.ContainsKey("SubAction"))
            {
                var _info = new DSS(info)
                {
                    ["action"] = info["SubAction"]
                };
                _info.Remove("SubAction");
                info = _info;
            }
        }
        var url = ADDR_DEFAULT.AddUrlEncodedParameters(args_course, args_year(year), info);
        //print(url)
        var tbls_ans = new List<object>();
        foreach (var _ in Enumerable.Range(0, retry_limit))
        {
            var dom = Driver.GetHtmlAfterLoaded(url).Dom();
            var isCurrentYear = dom.QuerySelectorAll<IHtmlTableElement>("table.ranking-list").Where(
                x => x.Rows.Where(x => x.Cells[4].TextContent.NormalizeWebString().StartsWith(year.ToString())).Count() > 0)
                .Count() > 0;

            if (isCurrentYear)
            {
                return DataExtractor.GetCourseList(dom);
            }
        }
        return Enumerable.Empty<CourseRecord>();
    }

    // 
    //   Get the html return according to lecture id (and other information).
    //   lecture_info: id or (id, year) or (id, year, term)
    //   
    public (IElement?, IElement?, IElement?, IElement?) GetLectureInfo(int id_seed, int year, bool en = false)
    {
        // request
        var lang = !en ? "JA" : "EN";
        var url1 = $"http://www.ocw.titech.ac.jp/index.php?module=General&action=T0300&JWC={year}{id_seed.ToString()!.Substring(4)}&lang={lang}";
        var url2 = url1 + "&vid=05";
        var html = Driver.GetHtmlAfterLoaded(url1);
        if (html == "404")
        {
            return (null, null, null, null);
        }
        var dom = html.Dom();
        var nameTitleContainer = dom.QuerySelector(".page-title-area.clearfix");
        var summaryContainer = dom.QuerySelector(".gaiyo-data");
        var innerContainer = dom.QuerySelector(".right-inner");
        IElement? notesContainer = null;

        var hasSummary = dom.QuerySelector("li a.summary") != null;
        var hasNotes = dom.QuerySelector("li a.notes") != null;

        if (hasNotes) {
            html = Driver.GetHtmlAfterLoaded(url2);
            dom = html.Dom();
            notesContainer = dom.QuerySelector(".right-inner") ?? dom.QuerySelector("#right-inner");
        }
        
        var ans = (nameTitleContainer, summaryContainer, innerContainer, notesContainer);
        return ans;
    }

    /// <summary>
    /// 開講元のリスト（つまり左側のあの赤いまたは青いもの）を取得する
    /// </summary>
    /// <param name="outpath"></param>
    public void Task1(string outpath)
    {
        var d = GetDepartmentList();
        var e = GetDepartmentList(lang: "EN");
        FileUtils.Dump((d, e), outpath);
    }


    private static IDSS _k(int i) => new DSS() { ["GakubuCD"] = i.ToString() }.ToImmutableDictionary();
    public static readonly ImmutableList<(string, IDSS)> DepartmentCodes = new List<(string, IDSS)>()
    {
        ("理学院", _k(1)),
        ("工学院", _k(2)),
        ("物質理工学院", _k(3)),
        ("情報理工学院", _k(4)),
        ("生命理工学院", _k(5)),
        ("環境・社会理工学院", _k(6)),
        ("工系３学院", new DSS() { ["GakubuCD"]= "11", ["KamokuCD"]= "300001" }.ToImmutableDictionary()),
        ("教養科目群", new DSS() { ["GakubuCD"]= "7", ["GakkaCD"]= "370000" }.ToImmutableDictionary()),
        ("初年次専門科目", _k(10)),
    }.ToImmutableList();

    public static readonly IDSS args_default = new DSS
        {
            {"module","General"},
            {  "action","T0100"}
        }.ToImmutableDictionary();

    public static readonly IDSS args_la = new DSS
        {
            {"module", "General"},
            {"action", "T0200"},
            { "tab", "2"},
            {"focus", "100"}
        }.ToImmutableDictionary();

    public static readonly IDSS args_e3 = new DSS
        {
            {"module", "General"},
            {"action", "T0210"},
            { "tab", "2"},
            {"focus", "200"}
        }.ToImmutableDictionary();

    /// <summary>
    /// Get course list
    /// </summary>
    /// <param name="outPath"></param>
    public void Task2(string outPath)
    {
        List<CourseRecord> cl1, cl2;
        int nr1, nr2;

        // load cache
        try
        {
            (cl1, cl2, nr1, nr2) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(outPath);
        }
        catch
        {
            cl1 = [];
            cl2 = [];
            nr1 = 0;
            nr2 = 0;
            FileUtils.Dump((cl1, cl2, nr1, nr2), outPath);
        }

        for (int i = 0; i < DepartmentCodes.Count; ++i)
        {
            var (target, dt) = DepartmentCodes[i];

            if (i < nr1)
                continue;
            var args = args_default;
            if (target == "教養科目群")
                args = args_la;
            else if (target == "工系３学院")
                args = args_e3;

            var url = ADDR_DEFAULT.AddUrlEncodedParameters(dt, args, args_lang("JA"));
            var urlEn = ADDR_DEFAULT.AddUrlEncodedParameters(dt, args, args_lang("EN"));
            Console.WriteLine($"{target} => {url}");

            var html = Driver.GetHtmlAfterLoaded(url);
            var lists = DataExtractor.GetCourseList(html.Dom());

            var htmlEn = Driver.GetHtmlAfterLoaded(urlEn);
            var listsEn = DataExtractor.GetCourseList(htmlEn.Dom());

            nr1 = i + 1;
            nr2 = i + 1;
            cl1.AddRange(lists);
            cl2.AddRange(listsEn);

            FileUtils.BackupFile(outPath);
            FileUtils.Dump((cl1, cl2, nr1, nr2), outPath);
        }
    }

    bool Task3Sub1(DictIntSyl detail, int courseCode, int year)
    {
        if (detail.ContainsKey(year))
        {
            return false;
        }
        var detail_jp = DataExtractor.ParseLectureInfo(GetLectureInfo(courseCode, year));
        var detail_en = DataExtractor.ParseLectureInfo(GetLectureInfo(courseCode, year, en: true));
        detail[year] = (detail_jp, detail_en);
        return true;
    }



    /// <summary>
    /// get course data
    /// </summary>
    /// <param name="inPath"></param>
    /// <param name="outPath"></param>
    /// <param name="startYear"></param>
    /// <param name="endYear"></param>
    /// <exception cref="Exception"></exception>
    public void Task3(string inPath, string outPath, int startYear = 2016, int endYear = 2022)
    {
        var (courseList, _, _, _) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(inPath);
        var targets = courseList.Select(x =>
        {
            var idStr = HttpUtility.ParseQueryString(x.Url).Get("KougiCD", "JWC");
            var parsed = int.TryParse(idStr, out var idInt);
            if (!parsed)
            {
                Console.WriteLine($"Warning: Wrong format: {x.Url}");
                return -1;
            }
            return idInt;
        })
        .Where(x => x > 0)
        .ToList();
        // initialize storage

        Dictionary<int, DictIntSyl> details;
        List<int> erroredCodes;
        try
        {
            (details, erroredCodes) = FileUtils.Load<(Dictionary<int, DictIntSyl>, List<int>)>(outPath);
        }
        catch
        {
            details = [];
            erroredCodes = [];
            FileUtils.Dump((details, erroredCodes), outPath);
        }


        // error correction
        while (erroredCodes.Count > 0)
        {
            var errCode = erroredCodes.First();
            erroredCodes.RemoveAt(0);

            Console.WriteLine($"Retrying errored record {errCode} ({erroredCodes.Count + 1} remains)");

            if (!details.ContainsKey(errCode))
                details[errCode] = [];

            var detail = details[errCode];

            for (var year = startYear; year <= endYear; ++year)
            {
                Task3Sub1(detail, errCode, year);
            }

            FileUtils.BackupFile(outPath);
            FileUtils.Dump((details, erroredCodes), outPath);
        }

        // normal process
        var exitRequired = false;
        foreach (var courseCode in targets.Yaap())
        {
            if (!details.ContainsKey(courseCode))
                details[courseCode] = [];

            var detail = details[courseCode];
            var isCurrentCodeRecorded = false;
            var needDump = true;
            for (var year = startYear; year <= endYear; ++year)
            {
                try
                {
                    needDump = Task3Sub1(detail, courseCode, year);
                }
                catch (Exception e)
                {
                    if (!isCurrentCodeRecorded)
                    {
                        erroredCodes.Add(courseCode);
                        isCurrentCodeRecorded = true;
                    }
                    needDump = true;
                    Console.WriteLine($"ERROR {courseCode} in year {year}: {e}");
                }
            }
            // to avoid incomplete records
            if (exitRequired)
            {
                Console.WriteLine("Interrupted by user.");
                break;
            }
            details[courseCode] = detail;
            if (needDump) {
                FileUtils.BackupFile(outPath);
                FileUtils.Dump((details, erroredCodes), outPath);
            }
        }
    }


}
