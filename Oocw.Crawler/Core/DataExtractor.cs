using System.Collections.Generic;
using System;
using System.Linq;

using Oocw.Base;
using Oocw.Crawler.Utils;
using System.Threading.Tasks;

using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using System.Collections.Immutable;
using System.Web;
using System.Collections.Specialized;
using Oocw.Crawler.Models;
using System.Text.RegularExpressions;

namespace Oocw.Crawler.Core;

using static System.Net.Mime.MediaTypeNames;
using DictStrArgs = NestedDictionary<string, string?>;

public static class DataExtractor
{

    public static readonly ImmutableHashSet<string> non_exclude = new List<string> {
                    "GakubuCD",
                    "GakkaCD",
                    "KeiCD",
                    "KamokuCD",
                    "course",
                    "SubAction"
                }.ToImmutableHashSet();

    public static (
            List<DictStrArgs>,
            DictStrArgs,
            HashSet<string>
            )
        GetDepartmentList(IDocument bf_base, string url_in = "")
    {

        var lbs = new List<IHtmlDivElement?> {
            bf_base.QuerySelector("div#left-body-1") as IHtmlDivElement,
            bf_base.QuerySelector("div#left-body-2") as IHtmlDivElement,
            bf_base.QuerySelector("div#left-body-3") as IHtmlDivElement,
            bf_base.QuerySelector("div#left-body-4") as IHtmlDivElement,
        };
        var cats = new List<DictStrArgs>();
        var cats_flat = new DictStrArgs();
        var excluded = new HashSet<string>();

        bool sub1(IHtmlListItemElement x, DictStrArgs prev)
        {
            var xa = x.GetElementsByTagName("a").First() as IHtmlAnchorElement;
            if (xa == null)
            {
                return false;
            }
            var xaspan = xa.GetElementsByTagName("span") as IHtmlSpanElement;
            var name = (xaspan == null ? xa.InnerHtml : xaspan.InnerHtml).FixWebString();

            DictStrArgs xah = new();
            if (!string.IsNullOrWhiteSpace(xa.Href))
            {
                xah = HttpUtility.ParseQueryString(xa.Href != "#" ? xa.Href : url_in).ToNestedDictionary();

                foreach (var (k, v) in xah)
                {
                    if (k != null && !non_exclude.Contains(k))
                    {
                        excluded.Add(k);
                        xah.Remove(k);
                        // if k in ['LeftTab']: print(x.a['href'])
                    }
                }
                cats_flat[name] = xah;
            }
            else
            {
                foreach (var y in x.QuerySelectorAll<IHtmlListItemElement>("ul > li"))
                {
                    sub1(y, xah);
                }
            }
            prev[name] = xah;
            return true;
        }

        foreach (var lb in lbs)
        {
            if (lb == null)
                continue;
            var sdmap = new DictStrArgs();

            var li_classes = lb.QuerySelectorAll("ul > li").Select(x => x as IHtmlListItemElement);
            foreach (var sch in li_classes)
            {
                if (sch == null)
                    continue;
                sub1(sch, sdmap);
            }
            cats.Add(sdmap);
        }
        return (cats, cats_flat, excluded);
    }

    private static Regex AcbarRegex = new(@"/images/acbar(-?[0-9]+)\.gif");

    public static IEnumerable<ListedCourseInfo> GetCourseList(IDocument bf_base)
    {
        var tbls = bf_base.QuerySelectorAll<IHtmlTableElement>("table.ranking-list");

        if (tbls.Count() == 0)
        {
            // no available data
            return Enumerable.Empty<ListedCourseInfo>();
        }

        List<ListedCourseInfo> infos = new List<ListedCourseInfo>();

        foreach (var table in tbls)
        {
            var (parsedHead, rows) = table.GetRowSelector();

            foreach (var elem in rows)
            {
                var cells = elem.Cells;
                var (name, url) = cells[1].GetAllAnchors().FirstOrDefault(("", ""));
                var info = new ListedCourseInfo()
                {
                    Code = cells[0].TextContent.FixWebString(),
                    Name = name.FixWebString(),
                    Url = url,
                    Faculties = cells[2].GetAllAnchors()
                    .Select((x) => new FacultyInfo
                    {
                        Name = x.Item1,
                        Code = int.Parse(HttpUtility.ParseQueryString(x.Item2).Get("id") ?? "-1")
                    }),
                    Quarter = cells[3].TextContent.FixWebString(),
                    SyllabusUpdated = cells[4].TextContent.FixWebString(),
                    NotesUpdated = cells[5].TextContent.FixWebString(),
                    AccessRanking = int.Parse(AcbarRegex.Match(cells[6].QuerySelector<IHtmlImageElement>("img")?.Source ?? "/images/acbar-1.gif").Groups[1].Value),
                };
                infos.Add(info);
            }
        }

        return infos;
    }
    /*
    public static object parse_lecture_info(object info_caches) {
        object docs_supp;
        object vtmp;
        object k;
        object lname;
        if (info_caches == null) {
            return null;
        }
        (cnt_lname, cnt_summ, cnt_inner, cnt_note) = info_caches;
        // lname
        var lname_ = cnt_lname.h3.contents[0];
        try {
            lname = lname_.split("\u3000");
            lname[1] = lname[1].split("\xa0\xa0\xa0");
            lname = new List<object> {
                lname[0]
            } + lname[1];
        } catch {
            lname = new List<void> {
                null
            } + lname_.split("\xa0\xa0\xa0");
        }
        var summ = (from x in cnt_summ.find_all("dl")
            where x.dt != null && x.dd != null
            select new List<object> {
                x.dt.contents[0].strip("\n"),
                x.dd.contents
            }).ToList();
        foreach (var summ_key in summ) {
            summ_key[1] = strip_useless_elem(summ_key[1]);
            if (summ_key[1].Count == 1 && summ_key[0] != "担当教員名") {
                summ_key[1] = summ_key[1][0];
                if (summ_key[1] is str) {
                    summ_key[1] = fix_web_str(summ_key[1]);
                    object sk_tmp = null;
                    while (sk_tmp != summ_key[1]) {
                        sk_tmp = summ_key[1];
                        summ_key[1] = summ_key[1].replace("  ", " ");
                    }
                } else {
                    //rankings
                    summ_key[1] = summ_key[1]["src"];
                }
            } else {
                // lecturers
                var sk1 = summ_key[1];
                var sk = new List<object>();
                foreach (var lect in sk1) {
                    if (lect is str) {
                        var lect_tmp = fix_web_str(lect);
                        if (lect_tmp.Count > 0) {
                            Console.WriteLine(lect_tmp.Count, lect_tmp);
                            sk.append((-1, lect_tmp));
                        }
                    } else {
                        var lect_addr = deform_url(lect["href"])[1]["id"];
                        var lect_name = fix_web_str(lect.@string);
                        sk.append((lect_addr, lect_name));
                    }
                }
                summ_key[1] = sk;
            }
        }
        summ = ltod(summ);
        // contents
        var conts_orig = (from y in cnt_inner.find_all(class_: "cont-sec")
            select (from x in y
                where x.ToString() != "\n" && x.ToString() != "\xa0"
                select x).ToList()).ToList();
        var conts = new Dictionary<object, object> {
        };
        foreach (var cont in conts_orig) {
            k = fix_web_str(cont[0].contents[0]).ToString();
            var v = cont[1];
            if (v.name == "table") {
                if (new List<string> {
                    "Competencies that will be developed",
                    "学生が身につける力(ディグリー・ポリシー)"
                }.Contains(k)) {
                    // skills
                    vtmp = (from x in v.tbody.find_all("td")
                        select (from y in x.contents
                            select !(y is str) ? y.contents[0] : y).ToList()).ToList();
                    var skills_dict = new Dictionary<object, object> {
                        {
                            "専門力",
                            0},
                        {
                            "教養力",
                            1},
                        {
                            "コミュニケーション力",
                            2},
                        {
                            "展開力(探究力又は設定力)",
                            3},
                        {
                            "展開力(実践力又は解決力)",
                            4},
                        {
                            "Specialist skills",
                            0},
                        {
                            "Intercultural skills",
                            1},
                        {
                            "Communication skills",
                            2},
                        {
                            "Critical thinking skills",
                            3},
                        {
                            "Practical and/or problem-solving skills",
                            4}};
                    v = new List<bool> {
                        false
                    } * 5;
                    foreach (var skl in vtmp) {
                        if (skl.Count == 2) {
                            v[skills_dict[skl[1]]] = true;
                        }
                    }
                } else if (new List<string> {
                    "Course schedule/Required learning",
                    "授業計画・課題"
                }.Contains(k)) {
                    vtmp = (from x in v.tbody.find_all("tr")
                        select (from y in x.contents
                            where !(y.ToString() == "\n")
                            select y.@string.ToString()).ToList()).ToList();
                    v = vtmp;
                } else if (new List<string> {
                    "Course taught by instructors with work experience",
                    "実務経験のある教員等による授業科目等"
                }.Contains(k)) {
                    v = v.tbody.find_all("tr")[-1].td.@string;
                    v = fix_web_str(v);
                } else {
                    Console.WriteLine(v.name);
                    throw new Exception();
                }
            } else if (v.name == "p") {
                var vc = v.contents;
                var vcs = "";
                foreach (var i in vc) {
                    vcs += i.ToString();
                }
                v = vcs.replace("<br/>", "\n");
            } else if (v.name == "ul") {
                v = (from x in v.find_all("li")
                    select (from y in fix_web_str(x.contents[0]).split("：")
                        select y.strip(" ")).ToList()).ToList();
            } else if (v.name == "div") {
                // TODO what about the god damn video format contents?
                v = v.ToString();
                @"
      vs = []
      for dd in [x for x in v.contents if x.name == 'div']:
        video_src = dd.find('video')['src']
        video_title = dd.find('div', class_='movie_th').string
        video_title = fix_web_str(video_title)
        vs.append((video_src, video_title))
      v = vs
      ";
            } else {
                Console.WriteLine(v.name);
                throw new Exception();
            }
            conts[k] = v;
        }
        // docs
        try {
            docs_supp = cnt_note.dl.dd.find_all("li");
        } catch (AttributeError) {
            docs_supp = new List<object>();
        }
        docs_supp = (from x in docs_supp
            select strip_useless_elem(x.contents)[-2]).ToList();
        docs_supp = (from x in docs_supp
            select (x[0]["href"].ToString(), x[0].contents[0].ToString(), x[1]["src"].ToString())).ToList();
        var docs_supp_meta = (null, null);
        var docs_orig = cnt_note.find_all("div", class_: "cont-sec");
        var docs = new Dictionary<object, object> {
            {
                "supp",
                (docs_supp_meta, docs_supp)}};
        foreach (var doc_orig in docs_orig) {
            var nhead = doc_orig.find(class_: "note-head");
            var nlower = doc_orig.find(class_: "clearfix note-lower");
            if (nhead == null || nlower == null) {
                // adobe pdf related 
                continue;
            }
            k = nhead.h3.@string.ToString();
            var lecture_type = fix_web_str(nhead.p.@string.ToString());
            var start_date = fix_web_str(nlower.p.@string.ToString());
            var notes = (from x in nlower.find_all("div", class_: "pdf-link")
                select (new List<string> {
                    x.a["href"].ToString()
                } + strip_useless_elem(x.a.contents)[1] + x.img != null ? new List<object> {
                    x.img
                } : new List<object>())).ToList();
            notes = (from x in notes
                select (x[0], fix_web_str(x[1].ToString()), x[2]["src"].ToString())).ToList();
            var meta = (lecture_type, start_date);
            docs[k] = (meta, notes);
        }
        //print(lname)  
        return (lname, summ, conts, docs);
    }
    */

}
