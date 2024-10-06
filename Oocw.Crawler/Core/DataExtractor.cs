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
        GetDepartmentList(IDocument document, string urlIn = "")
    {

        var lbs = new List<IHtmlDivElement?> {
            document.QuerySelector("div#left-body-1") as IHtmlDivElement,
            document.QuerySelector("div#left-body-2") as IHtmlDivElement,
            document.QuerySelector("div#left-body-3") as IHtmlDivElement,
            document.QuerySelector("div#left-body-4") as IHtmlDivElement,
        };
        var cats = new List<DictStrArgs>();
        var catsFlat = new DictStrArgs();
        var excluded = new HashSet<string>();

        bool Sub1(IHtmlListItemElement x, DictStrArgs prev)
        {
            var anchor = x.GetElementsByTagName("a").First() as IHtmlAnchorElement;
            if (anchor == null)
            {
                return false;
            }
            var anchorSpan = anchor.GetElementsByTagName("span") as IHtmlSpanElement;
            var name = (anchorSpan == null ? anchor.InnerHtml : anchorSpan.InnerHtml).NormalizeWebString();

            DictStrArgs xah = [];
            if (!string.IsNullOrWhiteSpace(anchor.Href))
            {
                xah = HttpUtility.ParseQueryString(anchor.Href != "#" ? anchor.Href : urlIn).ToNestedDictionary();

                foreach (var (k, v) in xah)
                {
                    if (k != null && !NonExclude.Contains(k))
                    {
                        excluded.Add(k);
                        xah.Remove(k);
                        // if k in ['LeftTab']: print(x.a['href'])
                    }
                }
                catsFlat[name] = xah;
            }
            else
            {
                foreach (var y in x.QuerySelectorAll<IHtmlListItemElement>("ul > li"))
                {
                    Sub1(y, xah);
                }
            }
            prev[name] = xah;
            return true;
        }

        foreach (var label in lbs)
        {
            if (label == null)
                continue;
            var sdMap = new DictStrArgs();

            var liClasses = label.QuerySelectorAll("ul > li").Select(x => x as IHtmlListItemElement);
            foreach (var sch in liClasses)
            {
                if (sch == null)
                    continue;
                Sub1(sch, sdMap);
            }
            cats.Add(sdMap);
        }
        return (cats, catsFlat, excluded);
    }

    private static Regex AcBarRegex = new(@"/images/acbar(-?[0-9]+)\.gif");

    public static IEnumerable<CourseRecord> GetCourseList(IDocument document)
    {
        var tables = document.QuerySelectorAll<IHtmlTableElement>("table.ranking-list");

        if (tables.Count() == 0)
        {
            // no available data
            return [];
        }

        List<CourseRecord> infos = [];

        foreach (var table in tables)
        {
            var (parsedHead, rows) = table.GetRowSelector();

            foreach (var elem in rows)
            {
                var cells = elem.Cells;
                var (name, url) = cells[1].GetAllAnchors().FirstOrDefault(("", ""));

                var k = cells.Length == 7 ? 0 : 1;

                var info = new CourseRecord()
                {
                    Code = cells[0].TextContent.NormalizeWebString(),
                    Name = name.NormalizeWebString(),
                    Url = url,
                    Faculties = cells[2].GetAllAnchors()
                    .Select((x) => new FacultyRecord
                    {
                        Name = x.Item1,
                        Code = int.Parse(HttpUtility.ParseQueryString(x.Item2).Get("id") ?? "-1")
                    }),
                    Quarter = cells[3 + k].TextContent.NormalizeWebString(),
                    SyllabusUpdated = cells[4 + k].TextContent.NormalizeWebString(),
                    NotesUpdated = cells[5 + k].TextContent.NormalizeWebString(),
                    AccessRanking = int.Parse(AcBarRegex.Match(cells[6 + k].QuerySelector<IHtmlImageElement>("img")?.Source ?? "/images/acbar-1.gif").Groups[1].Value),
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




    public static SyllabusRecord? ParseLectureInfo((IElement?, IElement?, IElement?, IElement?) infoCaches)
    {

        var (nameContainer, summaryContainer, innerContainer, noteContainer) = infoCaches;
        if (nameContainer == null)
            return null;

        SyllabusRecord ret = new();

        // title
        var nameRaw = nameContainer.GetElementsByTagName("h3").First()?.TextContent ?? "";
        var nameRaw2 = nameRaw.Split("\u3000");
        var nameRaw21 = nameRaw2[1].Split("\xa0\xa0\xa0");
        ret.YearStr = nameRaw2[0];
        if (nameRaw21.Length > 1)
        {
            ret.NameJa = nameRaw21[0];
            ret.NameEn = nameRaw21[1];
        }
        else
        {
            ret.NameEn = nameRaw21[0];
        }

        // summary
        var dataCells = summaryContainer?.QuerySelectorAll<IHtmlElement>("dl");
        DSS retSummary = [];
        DSS retDetail = new() { ["skills"] = "" };
        List<FacultyRecord> faculties = ret.Faculties;

        foreach (var dl in dataCells ?? [])
        {
            var dt = dl.GetElementsByTagName("dt").First();
            var dd = dl.GetElementsByTagName("dd").First();

            var sKey = dt.TextContent.NormalizeWebString();
            string sValue = "";
            if (sKey == "担当教員名" || sKey == "Instructor(s)")
            {
                faculties.AddRange(dd.GetAllAnchors()
                    .Select((x) => new FacultyRecord
                    {
                        Name = x.Item1,
                        Code = int.Parse(HttpUtility.ParseQueryString(x.Item2).Get("id") ?? "-1")
                    }));
            }
            else if (sKey == "アクセスランキング" || sKey == "Access Index")
            {
                sValue = AcBarRegex.Match(dd.QuerySelector<IHtmlImageElement>("img")?.Source ?? "/images/acbar-1.gif").Groups[1].Value;
            }
            else
            {
                sValue = dd.TextContent.NormalizeWebString();
            }
            retSummary[sKey] = sValue;
        }

        ret.Summary.AssignFrom(retSummary);


        foreach (var contSec in innerContainer?.QuerySelectorAll("div.cont-sec") ?? Enumerable.Empty<IElement>())
        {
            var k = contSec.QuerySelector("h3")!.TextContent.NormalizeWebString();
            var v = contSec.QuerySelector("h3")!.NextElementSibling;
            if (v is IHtmlTableElement vt)
            {
                if (SkillsTrigger.Contains(k))
                {
                    var spans = vt.QuerySelectorAll("td > span")!;
                    foreach (var span in spans)
                    {
                        var skill = span.TextContent.NormalizeWebString();
                        var active = span.PreviousSibling != null; // so far so good...
                        var ind = SkillsDict.IndexOf(skill);
                        if (ind >= 0 && ind < 5)
                        {
                            ret.Skills[ind] = active;
                        }
                        else{
                            var skillActive = $"{skill} : {active}";
                            retDetail["skills"] =
                            string.IsNullOrWhiteSpace(retDetail["skills"]) ?
                            skillActive :
                            retDetail["skills"] + " " + skillActive;
                        }
                    }
                }
                else if (SyllabusTrigger.Contains(k))
                {
                    var (vHeader, vElement) = vt.GetRowSelector();
                    foreach (var row in vElement)
                    {
                        ret.Schedule.Add((
                            int.Parse(NumberExtractor.Match(row.Cells[0].TextContent).Value),
                            row.Cells[1].TextContent.NormalizeWebString(replaceReturns: false).RemoveUnnecessarySpaces(),
                            row.Cells[2].TextContent.NormalizeWebString(replaceReturns: false).RemoveUnnecessarySpaces()
                            ));
                    }
                }
                else if (ExperiencedTrigger.Contains(k))
                {
                    var val = vt.GetRowSelector().Item2.Last().Cells[0].TextContent.NormalizeWebString();
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
                    var cls = li!.TextContent.NormalizeWebString();
                    var info = ColonSeparator.Split(cls);
                    if (info.Length < 2)
                        ret.Related.Add(("", cls));
                    else
                        ret.Related.Add((info[0], info[1]));
                }
            }
            else if (v is IHtmlDivElement vdiv)
            {
                // TODO what about the god damn video format content?
                retDetail[k] = v.OuterHtml
                    .NormalizeWebString(replaceReturns: false)
                    .RemoveUnnecessarySpaces();
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
            else if (v == null) {
                // http://www.ocw.titech.ac.jp/index.php?module=General&action=T0300&JWC=202131265&lang=EN
                var detail = string.Join('\n', contSec.ChildNodes
                    .Where(x => (x is IElement elem && elem.QuerySelector("h3") == null) || x is not IElement)
                    .Select(x => x.TextContent))
                    .NormalizeWebString(replaceReturns: false)
                    .RemoveUnnecessarySpaces();
                retDetail[k] = detail;
                
            }
            else
            {
                throw new NotImplementedException($"{k}, {v.TagName}");
            }

            ret.Detail.AssignFrom(retDetail);
        }

        // docs

        var docsSupp = noteContainer?.QuerySelectorAll<IHtmlListItemElement>("dl > dd li")
            ?? [];
        if (docsSupp.Count() > 0)
        {
            HashSet<string> links = [];
            foreach (var d in docsSupp)
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

        var docsOrig = noteContainer?.QuerySelectorAll<IHtmlDivElement>("div.cont-sec")
           ?? [];
        foreach (var docOrig in docsOrig)
        {
            var nHead = docOrig.QuerySelector<IHtmlElement>(".note-head");
            var nLower = docOrig.QuerySelector<IHtmlElement>(".clearfix.note-lower");
            if (nHead == null || nLower == null)
                continue;// adobe pdf related 

            var note = new SyllabusRecord.NoteRecord();

            note.Title = nHead.GetElementsByTagName("h3").First().TextContent.NormalizeWebString();
            note.LectureType = nHead.GetElementsByTagName("p").First().TextContent.NormalizeWebString();
            note.LectureDate = nLower.GetElementsByTagName("p").First().TextContent.NormalizeWebString();

            // TODO what to do with the img and strip_useless_elem(x.a.contents)[1]?
            note.Links = nLower.QuerySelectorAll<IHtmlAnchorElement>("div.pdf-link a").Select(x => x.Href).ToArray();
            ret.Notes.Add(note);
        }

        return ret;
    }


}
