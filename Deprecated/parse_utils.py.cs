
using re;

using datetime;

using defaultdict = collections.defaultdict;

using collections;

using System.Collections.Generic;

using System.Linq;

using System;
using System.IO;
using Oocw.Base;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Oocw.Database.Models;

namespace Oocw.Cli.Utils;

using ImHS = ImmutableHashSet<string>;

public static class parse_utils
{

    public static Tuple<defaultdict, defaultdict, Dictionary<object, object>> gather_data(string inpath)
    {
        (details, _) = pload(inpath);
        var ans = new defaultdict(() => defaultdict(@int));
        var anst = new defaultdict(dict);
        var ansu = new Dictionary<object, object>
        {
        };
        foreach (var code in details)
        {
            foreach (var year in details[code])
            {
                var subu = new List<void> {
                    null,
                    null
                };
                foreach (var item in details[code][year])
                {
                    if (!item)
                    {
                        continue;
                    }
                    foreach (var k in item[1])
                    {
                        if (item[1][k] is collections.Hashable)
                        {
                            ans[k][item[1][k]] += 1;
                        }
                    }
                    foreach (var k in item[2])
                    {
                        if (item[2][k] is collections.Hashable)
                        {
                            ans[k][item[2][k]] += 1;
                        }
                    }
                    if (item[1].Contains("担当教員名") && type(item[1]["担当教員名"]) == list)
                    {
                        foreach (var (id, tn) in item[1]["担当教員名"])
                        {
                            anst[id]["ja"] = tn;
                        }
                    }
                    if (item[1].Contains("Instructor(s)") && type(item[1]["Instructor(s)"]) == list)
                    {
                        foreach (var (id, tn) in item[1]["Instructor(s)"])
                        {
                            anst[id]["en"] = tn;
                        }
                    }
                    if (item[1].Contains("開講元"))
                    {
                        subu[0] = item[1]["開講元"];
                    }
                    if (item[1].Contains("Academic unit or major"))
                    {
                        subu[1] = item[1]["Academic unit or major"];
                    }
                }
                if (subu[0] != null)
                {
                    ansu[subu[0]] = subu[1];
                }
            }
        }
        return (ans, anst, ansu);
    }


    public static List<object> parse_contacts(object dstr)
    {
        var bad_signs = new List<string> {
            "：",
            "；",
            "，",
            "、",
            "[at]",
            "\u3000",
            "（",
            "）",
            "【",
            "】"
        };
        var good_signs = new List<string> {
            ":",
            ";",
            ",",
            ",",
            "@",
            " ",
            "(",
            ")",
            "[",
            "]"
        };
        foreach (var i in Enumerable.Range(0, bad_signs.Count))
        {
            dstr = dstr.replace(bad_signs[i], good_signs[i]);
        }
        dstr = (from x in dstr.split("\n")
                where x
                select x).ToList();
        dstr = (from x in list_concat((from x in dstr
                                       select x.split(";")).ToList())
                select x.strip(" ")).ToList();
        return dstr;
    }

    public static string parse_book(object bstr)
    {
        var none_mark = bstr.Count <= 10 && (bstr.find("なし") >= 0 || bstr.find("無し") >= 0 || bstr.find("指定しない") >= 0 || bstr.find("定めない") >= 0 || bstr.find("ません") >= 0);
        var lect_word_list = new List<string> {
            "講義",
            "演習",
            "資料",
            "配信",
            "配布",
            "アップロード",
            "OCW",
            "PDF",
            "テキスト"
        };
        var lect_note_prob = (from x in (from w in lect_word_list
                                         select (bstr.find(w) >= 0)).ToList()
                              where x
                              select x).ToList().Count;
        var lect_mark = lect_note_prob >= 2;
        if (none_mark)
        {
            return "None";
        }
        if (lect_mark)
        {
            return "Lecture Note";
        }
        return bstr;
    }
}
