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
                var _info = new DSS(info);
                _info["action"] = info["SubAction"];
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
                x => x.Rows.Where(x => x.Cells[4].TextContent.FixWebString().StartsWith(year.ToString())).Count() > 0)
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
        var bf_base_page = html.Dom();
        var cnt_lname = bf_base_page.QuerySelector(".page-title-area.clearfix");
        var cnt_summ = bf_base_page.QuerySelector(".gaiyo-data");
        var cnt_inner = bf_base_page.QuerySelector(".right-inner");

        html = Driver.GetHtmlAfterLoaded(url2);
        bf_base_page = html.Dom();
        var cnt_note = bf_base_page.QuerySelector(".right-inner");
        if (cnt_note == null)
        {
            cnt_note = bf_base_page.QuerySelector("#right-inner");
        }
        var ans = (cnt_lname, cnt_summ, cnt_inner, cnt_note);
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
    /// <param name="outpath"></param>
    public void Task2(string outpath)
    {
        List<CourseRecord> cl1, cl2;
        int nr1, nr2;

        // load cache
        try
        {
            (cl1, cl2, nr1, nr2) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(outpath);
        }
        catch
        {
            cl1 = new();
            cl2 = new();
            nr1 = 0;
            nr2 = 0;
            FileUtils.Dump((cl1, cl2, nr1, nr2), outpath);
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

            nr1 = i;
            nr2 = i;
            cl1.AddRange(lists);
            cl2.AddRange(listsEn);

            FileUtils.BackupFile(outpath);
            FileUtils.Dump((cl1, cl2, nr1, nr2), outpath);
        }
    }

    void Task3Sub1(DictIntSyl detail, int dcode, int year)
    {
        if (detail.ContainsKey(year))
        {
            return;
        }
        var detail_jp = DataExtractor.ParseLectureInfo(GetLectureInfo(dcode, year));
        var detail_en = DataExtractor.ParseLectureInfo(GetLectureInfo(dcode, year, en: true));
        detail[year] = (detail_jp, detail_en);
    }

    

    /// <summary>
    /// get course data
    /// </summary>
    /// <param name="inpath"></param>
    /// <param name="outpath"></param>
    /// <param name="start_year"></param>
    /// <param name="end_year"></param>
    /// <exception cref="Exception"></exception>
    public void Task3(string inpath, string outpath, int start_year = 2016, int end_year = 2022)
    {
        var (clist, _, _, _) = FileUtils.Load<(List<CourseRecord>, List<CourseRecord>, int, int)>(inpath);
        var targets = clist.Select(x => HttpUtility.ParseQueryString(x.Url).Get("KougiCD", "JWC")).Select(x => int.Parse(x!));
        // initialize storage

        Dictionary<int, DictIntSyl> details;
        List<int> gshxd_code;
        try
        {
            (details, gshxd_code) = FileUtils.Load<(Dictionary<int, DictIntSyl>, List<int>)>(outpath);
        }
        catch
        {
            details = new();
            gshxd_code = new();
            FileUtils.Dump((details, gshxd_code), outpath);
        }


        // error correction
        while (gshxd_code.Count > 0)
        {
            var dcode = gshxd_code.First();
            gshxd_code.RemoveAt(0);

            Console.WriteLine($"Retrying errored record {dcode} {gshxd_code.Count + 1} remains)");

            if (!details.ContainsKey(dcode))
                details[dcode] = new();

            var detail = details[dcode];

            for (var year = start_year; year <= end_year; ++year)
            {
                Task3Sub1(detail, dcode, year);
                try
                {
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{dcode}, {year}: {e}");
                    throw;
                }
            }

            FileUtils.BackupFile(outpath);
            FileUtils.Dump((details, gshxd_code), outpath);
        }

        // normal process
        var exit_tag = false;
        foreach (var dcode in targets.Yaap())
        {
            if (!details.ContainsKey(dcode))
                details[dcode] = new();

            var detail = details[dcode];
            var err_rec = false;
            for (var year = start_year; year <= end_year; ++year)
            {

                // if cancelled: exit_tag = true; break;
                try
                {
                    Task3Sub1(detail, dcode, year);
                }
                catch (Exception e)
                {
                    if (!err_rec)
                    {
                        gshxd_code.Add(dcode);
                        err_rec = true;
                    }
                    Console.WriteLine($"ERROR {dcode} in year {year}: {e} {e.Message}");
                }
            }
            // to avoid incomplete records
            if (exit_tag)
            {
                Console.WriteLine("Interrupted by user.");
                break;
            }
            details[dcode] = detail;
            FileUtils.BackupFile(outpath);
            FileUtils.Dump((details, gshxd_code), outpath);
        }
    }


}
