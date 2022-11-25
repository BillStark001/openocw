using Oocw.Base;
using Oocw.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oocw.Cli.Utils;

public static class ParseUtil
{




    /// <summary>
    /// 
    /// </summary>
    /// <param name="dstr">
    /// 0b 00000000 00000000 00000000 00ED0CBA <br></br>
    /// A: lect B: exer C: expr D: has online E: has offline
    /// </param>
    /// <returns></returns>
    public static int ParseForm(string dstr)
    {
        var ans = 0;

        dstr = dstr.NormalizeBrackets().ToLower();
        var fl_lect = dstr.HasKeyword("講義", "lecture");
        var fl_exer = dstr.HasKeyword("演習", "練習", "exercise", "recitation");
        var fl_expr = dstr.HasKeyword("実験", "experiment");

        ans += Base.Utils.BoolToBinary(
            fl_lect,
            fl_exer,
            fl_expr
        );

        var (brl, brr) = dstr.FindBrackets();
        if (brl >= 0)
        {
            var dstr_br = brr < 0 ? dstr.Substring(brl + 1) : dstr.Substring(brl + 1, brr - brl - 1);
            var fl_hybrid = dstr_br.HasKeyword(
                "ハイフレックス",
                "ブレンド",
                "blend",
                "hyflex",
                "hybrid"
            );
            var fl_online = dstr_br.HasKeyword(
                "zoom",
                "livestream",
                "online",
                "ライブ",
                "オンライン"
            );
            var fl_offline = dstr_br.HasKeyword(
                "対面",
                "face-to-face",
                "offline",
                "オンライン"
            );
            ans += Base.Utils.BoolToBinary(
                fl_online || fl_hybrid,
                fl_offline || fl_hybrid
            ) * 16;
        }
        return ans;
    }

    private static readonly IList<IList<string>> WeekTable = new List<IList<string>> {
            new List<string> {
                "月",
                "mon"
            }.AsReadOnly(),
            new List<string> {
                "火",
                "tue"
            }.AsReadOnly(),
            new List<string> {
                "水",
                "wed"
            }.AsReadOnly(),
            new List<string> {
                "木",
                "thu"
            }.AsReadOnly(),
            new List<string> {
                "金",
                "fri"
            }.AsReadOnly(),
            new List<string> {
                "土",
                "sat"
            }.AsReadOnly(),
            new List<string> {
                "日",
                "sun"
            }.AsReadOnly()
        }.AsReadOnly();

    public static int ParseDay(string dstr)
    {
        dstr = dstr.ToLower();
        foreach (var (i, groups) in WeekTable.GetIndexedEnumerator())
        {
            if (dstr.HasKeyword(groups))
            {
                return i + 1;
            }
        }
        return 0;
    }

    public static readonly Regex TimePattern = new(
        @" *(月|火|水|木|金|土|日|mon|tue|wed|thur?|fri|sat|sun) *(\d{1,2})[-~]?(\d{1,2})? *");
    public static readonly Regex SpecPattern = new(
        @" *(集中講義等?|オン・?デマンド|講究等?|ゼミ|セミナー|intensive|on\-?demand|seminar) *(\d{1,2})?[-~]?(\d{1,2})? *");

    public static List<AddressInfo> ParseAddress(string dstr)
    {
        AddressInfo ansd;

        string loc;

        int loc_pos;
        int end;
        int start;

        var dstrp = dstr.NormalizeBrackets().ToLower();
        var ans = new List<AddressInfo>();

        string ParseLocation(int pos)
        {
            var (brl, brr) = dstrp.FindBrackets(pos);
            if (brl < 0)
            {
                return "";
            }
            else if (brr < 0)
            {
                return dstr.Substring(brl + 1);
            }
            return dstr.Substring(brl + 1, brr - brl - 1);
        }

        foreach (var _res in TimePattern.Matches(dstrp))
        {
            var res = (Match)_res;
            var day = ParseDay(res.Groups[1].Value);
            start = Convert.ToInt32(res.Groups[2].Value);
            end = res.Groups.Count > 3 ? Convert.ToInt32(res.Groups[3].Value) : start;
            loc_pos = res.Groups[1].Index;
            loc = loc_pos < dstrp.Length && dstrp[loc_pos] == '(' ? ParseLocation(loc_pos) : "";
            ansd = new AddressInfo();
            ansd.Type = AddressInfo.AddressType.Normal;
            ansd.Time = new()
            {
                Day = day,
                Start = start,
                End = end,
            };
            ansd.Location = loc;
            ans.Add(ansd);
        }
        foreach (var _res in SpecPattern.Matches(dstrp))
        {
            var res = (Match)_res;
            var ins = res.Groups[1].Index;
            var dine = res.Groups[1].Length;
            loc_pos = res.Index + res.Length;
            start = res.Groups.Count > 2 ? Convert.ToInt32(res.Groups[2].Value) : 0;
            end = res.Groups.Count > 3 ? Convert.ToInt32(res.Groups[3].Value) : start;
            loc = loc_pos < dstrp.Length && dstrp[loc_pos] == '(' ? ParseLocation(loc_pos) : "";

            ansd = new AddressInfo();
            ansd.Type = AddressInfo.AddressType.Special;
            ansd.Time = new()
            {
                Description = dstr.Substring(ins, dine),
                Start = start,
                End = end,
            };
            ansd.Location = loc;
            ans.Add(ansd);
        }
        if (ans.Count == 0)
        {
            ansd = new AddressInfo();
            ansd.Type = AddressInfo.AddressType.Unknown;
            ansd.Time = new()
            {
                Description = dstr
            };
            ans.Add(ansd);
        }
        return ans;
    }

    // 
    //   return: (year, month, date) or None
    //   

    public static readonly Regex DatePatternSlash = new(@"( *\d+ *)/( *\d+ *)/( *\d+ *)");
    public static readonly Regex DatePatternBar = new(@"( *\d+ *)-( *\d+ *)-( *\d+ *)");
    public static readonly Regex DatePatternAsian = new(@"( *\d+ *)年( *\d+ *)月( *\d+ *)日");
    private static readonly IList<Regex> DatePatterns = new List<Regex> { DatePatternBar, DatePatternSlash, DatePatternAsian }.AsReadOnly();
    public static DateTime ParseDate(string dstr)
    {
        DateTime ans;
        foreach (var p in DatePatterns)
        {
            var rs = p.Match(dstr);
            if (rs.Success)
            {
                int[] info = (from i in Enumerable.Range(1, 4 - 1)
                              select Convert.ToInt32(rs.Groups[i].Value.Trim())).ToArray();
                ans = new(info[0], info[1], info[2]);
                // aybe we have a better approach in c#?
                return ans;
            }
        }
        // return ans
        return DateTime.MinValue;
    }


    public static readonly Regex AcademicDivisionPattern = new(@" *(H?R?\d{1,4}) *年?度? *(.+) *Q");
    public static (int, int) ParseAcademicDivision(string dstr)
    {
        var rs = AcademicDivisionPattern.Match(dstr);
        int ay = 0;
        int q = 0;
        if (rs.Success)
        {
            ay = int.Parse(rs.Groups[1].Value.Trim());
            q = int.Parse(rs.Groups[2].Value.Trim());
        }
        return (ay, q);
    }

    public static int ParseAcademicQuarter(string dstr)
    {
        var ans = 0;
        var anst = dstr.Replace("Q", "").Trim();
        var anst1 = new List<bool> {
                false,
                false,
                false,
                false
            };
        var last_d = 0;
        var bar = false;
        foreach (var d in anst)
        {
            try
            {
                var cur_d = Convert.ToInt32(d) - 1;
                anst1[cur_d] = true;
                if (bar)
                {
                    foreach (var dd in Enumerable.Range(last_d, cur_d - last_d))
                    {
                        anst1[dd] = true;
                    }
                }
                last_d = cur_d;
            }
            catch
            {
                if (d == '-' || d == '~')
                {
                    bar = true;
                }
            }
        }
        ans = Base.Utils.BoolToBinary(anst1);
        return ans;
    }


    public static string ParseLanguage(string langIn)
    {
        return langIn.ToLower().HasKeyword("english", "英語") ? "en" : "ja";
    }
}
