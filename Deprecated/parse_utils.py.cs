
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
