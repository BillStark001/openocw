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
using Oocw.Crawler.Models;
using System.Text.RegularExpressions;

namespace Oocw.Crawler.Core;

using DictStrArgs = NestedDictionary<string, string?>;
using DSS = Dictionary<string, string>;

public static class DataExtractor
{

    public static readonly ImmutableHashSet<string> NonExclude = new List<string> {
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
                    if (k != null && !NonExclude.Contains(k))
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

    public static IEnumerable<CourseRecord> GetCourseList(IDocument bf_base)
    {
        var tbls = bf_base.QuerySelectorAll<IHtmlTableElement>("table.ranking-list");

        if (tbls.Count() == 0)
        {
            // no available data
            return Enumerable.Empty<CourseRecord>();
        }

        List<CourseRecord> infos = new List<CourseRecord>();

        foreach (var table in tbls)
        {
            var (parsedHead, rows) = table.GetRowSelector();

            foreach (var elem in rows)
            {
                var cells = elem.Cells;
                var (name, url) = cells[1].GetAllAnchors().FirstOrDefault(("", ""));
                var info = new CourseRecord()
                {
                    Code = cells[0].TextContent.FixWebString(),
                    Name = name.FixWebString(),
                    Url = url,
                    Faculties = cells[2].GetAllAnchors()
                    .Select((x) => new FacultyRecord
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

    private static readonly ImmutableHashSet<string> SkillsTrigger = new HashSet<string>()
    {
        "Competencies that will be developed",
        "学生が身につける力(ディグリー・ポリシー)"
    }.ToImmutableHashSet();

    private static readonly IList<string> SkillsDict = new List<string>()
    {
        "専門力",
        "教養力",
        "コミュニケーション力",
        "展開力(探究力又は設定力)",
        "展開力(実践力又は解決力)",
        "Specialist skills",
        "Intercultural skills",
        "Communication skills",
        "Critical thinking skills",
        "Practical and/or problem-solving skills",
    }.AsReadOnly();


    private static readonly ImmutableHashSet<string> SyllabusTrigger = new HashSet<string>()
    {
        "Course schedule/Required learning",
        "授業計画・課題"
    }.ToImmutableHashSet();

    private static readonly ImmutableHashSet<string> ExperiencedTrigger = new HashSet<string>()
    {
        "Course taught by instructors with work experience",
        "実務経験のある教員等による授業科目等"
    }.ToImmutableHashSet();

    private static readonly Regex NumberExtractor = new Regex(@"(-?[0-9]+)");
    private static readonly Regex ColonSeparator = new Regex(@" *[:：] *");




    public static SyllabusRecord? ParseLectureInfo((IElement?, IElement?, IElement?, IElement?) info_caches)
    {

        var (cnt_lname, cnt_summ, cnt_inner, cnt_note) = info_caches;
        if (cnt_lname == null)
            return null;

        SyllabusRecord ret = new();

        // title
        var lname_ = cnt_lname.GetElementsByTagName("h3").First()?.TextContent ?? "";
        var lname0 = lname_.Split("\u3000");
        var lname1 = lname0[1].Split("\xa0\xa0\xa0");
        ret.YearStr = lname0[0];
        if (lname1.Length > 1)
        {
            ret.NameJa = lname1[0];
            ret.NameEn = lname1[1];
        }
        else
        {
            ret.NameEn = lname1[0];
        }

        // summary
        var dataCells = cnt_summ?.QuerySelectorAll<IHtmlElement>("dl");
        DSS summ = new();
        DSS retDetail = new();
        List<FacultyRecord> faculties = ret.Faculties;

        foreach (var dl in dataCells ?? Enumerable.Empty<IHtmlElement>())
        {
            var dt = dl.GetElementsByTagName("dt").First();
            var dd = dl.GetElementsByTagName("dd").First();

            var summ_key = dt.TextContent.FixWebString();
            string summ_value = "";
            if (summ_key == "担当教員名" || summ_key == "Instructor(s)")
            {
                faculties = dd.GetAllAnchors()
                    .Select((x) => new FacultyRecord
                    {
                        Name = x.Item1,
                        Code = int.Parse(HttpUtility.ParseQueryString(x.Item2).Get("id") ?? "-1")
                    }).ToList();
            }
            else if (summ_key == "アクセスランキング" || summ_key == "Access Index")
            {
                summ_value = AcbarRegex.Match(dd.QuerySelector<IHtmlImageElement>("img")?.Source ?? "/images/acbar-1.gif").Groups[1].Value;
            }
            else
            {
                summ_value = dd.TextContent.FixWebString();
            }
            summ[summ_key] = summ_value;
        }

        ret.Summary.AssignFrom(summ);


        foreach (var cont in cnt_inner?.QuerySelectorAll("div.cont-sec") ?? Enumerable.Empty<IElement>())
        {
            var k = cont.QuerySelector("h3")!.TextContent.FixWebString();
            var v = cont.QuerySelector("h3")!.NextElementSibling!;
            if (v is IHtmlTableElement vt)
            {
                if (SkillsTrigger.Contains(k))
                {
                    var spans = vt.QuerySelectorAll("td > span")!;
                    foreach (var span in spans)
                    {
                        var skill = span.TextContent.FixWebString();
                        var active = span.PreviousSibling != null; // so far so good...
                        var ind = SkillsDict.IndexOf(skill);
                        if (ind >= 0)
                            ind %= 5;
                        else
                            throw new NotImplementedException(skill);
                        ret.Skills[ind] = active;
                    }
                }
                else if (SyllabusTrigger.Contains(k))
                {
                    var (vrow, vtmp) = vt.GetRowSelector();
                    foreach (var row in vtmp)
                    {
                        ret.Schedule.Add((
                            int.Parse(NumberExtractor.Match(row.Cells[0].TextContent).Value),
                            row.Cells[1].TextContent,
                            row.Cells[2].TextContent
                            ));
                    }
                }
                else if (ExperiencedTrigger.Contains(k))
                {
                    var val = vt.GetRowSelector().Item2.Last().Cells[0].TextContent.FixWebString();
                    retDetail[k] = val;
                }
                else
                {
                    throw new NotImplementedException($"{k}, {v.TagName}");
                }
            }
            else if (v is IHtmlParagraphElement vp)
            {
                var vcs = "";
                foreach (var i in vp.ChildNodes)
                {
                    vcs += i is IElement e ? e.InnerHtml : i.TextContent;
                }
                vcs = vcs.Replace("<br/>", "\n");
                retDetail[k] = vcs;
            }
            else if (v is IHtmlUnorderedListElement vul) // related
            {
                foreach (var li in vul.Children.Select(x => x as IHtmlListItemElement).Where(x => x != null))
                {
                    var cls = li!.TextContent.FixWebString();
                    var info = ColonSeparator.Split(cls);
                    ret.Related.Add((info[0], info[1]));
                }
            }
            else if (v is IHtmlDivElement vdiv)
            {
                // TODO what about the god damn video format content?
                retDetail[k] = v.OuterHtml;
                /* 
                   vs = []
                   for dd in [x for x in v.contents if x.name == 'div']:
                     video_src = dd.find('video')['src']
                     video_title = dd.find('div', class_='movie_th').string
                     video_title = fix_web_str(video_title)
                     vs.append((video_src, video_title))
                   v = vs
                   ";
                */
            }
            else
            {
                throw new NotImplementedException($"{k}, {v.TagName}");
            }

            ret.Detail.AssignFrom(retDetail);
        }

        // docs

        var docs_supp = cnt_note?.QuerySelectorAll<IHtmlListItemElement>("dl > dd li")
            ?? Enumerable.Empty<IHtmlListItemElement>();
        if (docs_supp.Count() > 0)
        {
            HashSet<string> links = new();
            foreach (var d in docs_supp)
                foreach (var a in d.GetAllAnchors().Select(x => x.Item2))
                    if (!string.IsNullOrWhiteSpace(a))
                        links.Add(a);
            var supp = new SyllabusRecord.NoteRecord()
            {
                Title = SyllabusRecord.NoteRecord.SUPP_TITLE,
                Links = links.ToArray(),
            };

            ret.Notes.Add(supp);
        }

        var docs_orig = cnt_note?.QuerySelectorAll<IHtmlDivElement>("div.cont-sec")
           ?? Enumerable.Empty<IHtmlDivElement>();
        foreach (var doc_orig in docs_orig)
        {
            var nhead = doc_orig.QuerySelector<IHtmlElement>(".note-head");
            var nlower = doc_orig.QuerySelector<IHtmlElement>(".clearfix.note-lower");
            if (nhead == null || nlower == null)
                continue;// adobe pdf related 

            var note = new SyllabusRecord.NoteRecord();

            note.Title = nhead.GetElementsByTagName("h3").First().TextContent.FixWebString();
            note.LectureType = nhead.GetElementsByTagName("p").First().TextContent.FixWebString();
            note.LectureDate = nlower.GetElementsByTagName("p").First().TextContent.FixWebString();

            // TODO what to do with the img and strip_useless_elem(x.a.contents)[1]?
            note.Links = nlower.QuerySelectorAll<IHtmlAnchorElement>("div.pdf-link a").Select(x => x.Href).ToArray();
            ret.Notes.Add(note);
        }

        return ret;
    }


}
