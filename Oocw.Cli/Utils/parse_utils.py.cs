
using re;

using datetime;

using defaultdict = collections.defaultdict;

using collections;

using System.Collections.Generic;

using System.Linq;

using System;

public static class parse_utils {
    
    public static Tuple<defaultdict, defaultdict, Dictionary<object, object>> gather_data(string inpath) {
        (details, _) = pload(inpath);
        var ans = new defaultdict(() => defaultdict(@int));
        var anst = new defaultdict(dict);
        var ansu = new Dictionary<object, object> {
        };
        foreach (var code in details) {
            foreach (var year in details[code]) {
                var subu = new List<void> {
                    null,
                    null
                };
                foreach (var item in details[code][year]) {
                    if (!item) {
                        continue;
                    }
                    foreach (var k in item[1]) {
                        if (item[1][k] is collections.Hashable) {
                            ans[k][item[1][k]] += 1;
                        }
                    }
                    foreach (var k in item[2]) {
                        if (item[2][k] is collections.Hashable) {
                            ans[k][item[2][k]] += 1;
                        }
                    }
                    if (item[1].Contains("担当教員名") && type(item[1]["担当教員名"]) == list) {
                        foreach (var (id, tn) in item[1]["担当教員名"]) {
                            anst[id]["ja"] = tn;
                        }
                    }
                    if (item[1].Contains("Instructor(s)") && type(item[1]["Instructor(s)"]) == list) {
                        foreach (var (id, tn) in item[1]["Instructor(s)"]) {
                            anst[id]["en"] = tn;
                        }
                    }
                    if (item[1].Contains("開講元")) {
                        subu[0] = item[1]["開講元"];
                    }
                    if (item[1].Contains("Academic unit or major")) {
                        subu[1] = item[1]["Academic unit or major"];
                    }
                }
                if (subu[0] != null) {
                    ansu[subu[0]] = subu[1];
                }
            }
        }
        return (ans, anst, ansu);
    }
    
    public static string normalize_brackets(string dstr) {
        return dstr.replace("（", "(").replace("）", ")").replace("【", "[").replace("】", "]");
    }
    
    public static bool detect_kw(string dstr, object kws) {
        foreach (var kw in kws) {
            if (dstr.find(kw) >= 0) {
                return true;
            }
        }
        return false;
    }
    
    public static int bool2bin(object arr) {
        var ans = 0;
        foreach (var (i, b) in arr.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            ans += (1 << i) * b;
        }
        return ans;
    }
    
    public static object bin2bool(int bin, int digit) {
        var ans = new List<object>();
        foreach (var i in Enumerable.Range(0, digit)) {
            ans.append((bin & 1 << i) > 0);
        }
        return ans;
    }
    
    public static object find_brackets(
        string passage,
        int start = 0,
        string brl = "(",
        string brr = ")",
        bool strict = false,
        object exclude = ("\"", "\\\"")) {
        var pointer = start;
        var endpoint = passage.Count;
        var lbrl = brl.Count;
        var lbrr = brr.Count;
        var lex0 = exclude ? exclude[0].Count : -1;
        var lex1 = exclude ? exclude[1].Count : -1;
        var layer = 0;
        var outer_brl = -1;
        while (pointer < endpoint) {
            var next_brl = passage.find(brl, pointer);
            var next_brr = passage.find(brr, pointer);
            var next_exclude = exclude ? passage.find(exclude[0], pointer) : -1;
            // print(pointer, next_brl, next_brr, next_exclude, layer)
            if (next_exclude >= 0 && (next_brl < 0 || next_exclude < next_brl) && (next_brr < 0 || next_exclude < next_brr)) {
                pointer = next_exclude + lex0;
                while (true) {
                    var next_exclude_2 = passage.find(exclude[0], pointer);
                    var next_ignore_exclude_2 = passage.find(exclude[1], pointer);
                    if (next_ignore_exclude_2 >= 0 && next_exclude_2 > next_ignore_exclude_2 && next_exclude_2 <= next_ignore_exclude_2 + lex1) {
                        // this exclude mark is ignored
                        pointer = next_ignore_exclude_2 + lex1;
                    } else if (next_exclude_2 >= 0) {
                        // this mark is real
                        pointer = next_exclude_2 + lex0;
                        break;
                    } else if (strict) {
                        throw new ValueError("Excluding section matching failed");
                    } else {
                        return (outer_brl, -1);
                    }
                }
            } else if (next_brl >= 0 && (next_brl < next_brr || next_brr < 0)) {
                pointer = next_brl + lbrl;
                if (layer == 0) {
                    outer_brl = next_brl;
                }
                layer += 1;
            } else if (next_brr >= 0 && (next_brr < next_brl || next_brl < 0)) {
                if (layer == 0) {
                    if (strict) {
                        throw new ValueError("Brackets matching failed (no left bracket)");
                    } else {
                        pointer = next_brr + lbrr;
                        continue;
                    }
                } else {
                    pointer = next_brr + lbrr;
                    layer -= 1;
                    if (layer == 0) {
                        return (outer_brl, next_brr);
                    }
                }
            } else if (next_brl < 0 && next_brr < 0) {
                return (-1, -1);
            } else {
                throw new ValueError("what the hell?");
            }
        }
        if (strict) {
            throw new ValueError("Brackets matching failed (no right bracket)");
        }
        return (outer_brl, -1);
    }
    
    public static void parse_form(string dstr) {
        var ans = 0;
        dstr = normalize_brackets(dstr).lower();
        var fl_lect = detect_kw(dstr, new List<string> {
            "講義",
            "lecture"
        });
        var fl_exer = detect_kw(dstr, new List<string> {
            "演習",
            "練習",
            "exercise",
            "recitation"
        });
        var fl_expr = detect_kw(dstr, new List<string> {
            "実験",
            "experiment"
        });
        ans += bool2bin(new List<object> {
            fl_lect,
            fl_exer,
            fl_expr
        });
        (brl, brr) = find_brackets(dstr);
        if (brl >= 0) {
            var dstr_br = brr < 0 ? dstr[brl + 1] : dstr[(brl  +  1)::brr];
            var fl_hybrid = detect_kw(dstr_br, new List<string> {
                "ハイフレックス",
                "ブレンド",
                "blend",
                "hyflex",
                "hybrid"
            });
            var fl_online = detect_kw(dstr_br, new List<string> {
                "zoom",
                "livestream",
                "online",
                "ライブ",
                "オンライン"
            });
            var fl_offline = detect_kw(dstr_br, new List<string> {
                "対面",
                "face-to-face",
                "offline",
                "オンライン"
            });
            ans += bool2bin(new List<object> {
                fl_online || fl_hybrid,
                fl_offline || fl_hybrid
            }) * 16;
        }
        return ans;
    }
    
    public static int parse_day(string dstr) {
        dstr = dstr.lower();
        foreach (var (i, groups) in new List<List<string>> {
            new List<string> {
                "月",
                "mon"
            },
            new List<string> {
                "火",
                "tue"
            },
            new List<string> {
                "水",
                "wed"
            },
            new List<string> {
                "木",
                "thu"
            },
            new List<string> {
                "金",
                "fri"
            },
            new List<string> {
                "土",
                "sat"
            },
            new List<string> {
                "日",
                "sun"
            }
        }.Select((_p_1,_p_2) => Tuple.Create(_p_2, _p_1))) {
            if (detect_kw(dstr, groups)) {
                return i + 1;
            }
        }
        return 0;
    }
    
    public static List<object> parse_addr(string dstr) {
        object ansd;
        object loc;
        object loc_pos;
        object end;
        object start;
        var dstrp = normalize_brackets(dstr.lower());
        var ans = new List<object>();
        var pat_time = @" *(月|火|水|木|金|土|日|mon|tue|wed|thur?|fri|sat|sun) *(\d{1,2})[-~]?(\d{1,2})? *";
        var pat_spec = @" *(集中講義等?|オン・?デマンド|講究等?|ゼミ|セミナー|intensive|on\-?demand|seminar) *(\d{1,2})?[-~]?(\d{1,2})? *";
        object parse_loc(int pos) {
            (brl, brr) = find_brackets(dstrp, pos);
            if (brl < 0) {
                return "";
            } else if (brr < 0) {
                return dstr[brl + 1];
            }
            return dstr[(brl  +  1)::brr];
        }
        var res_time = re.finditer(pat_time, dstrp).ToList();
        var res_spec = re.finditer(pat_spec, dstrp).ToList();
        foreach (var res in res_time) {
            var day = parse_day(res.group(1));
            start = Convert.ToInt32(res.group(2));
            end = res.group(3) ? Convert.ToInt32(res.group(3)) : start;
            loc_pos = res.span()[1];
            loc = loc_pos < dstrp.Count && dstrp[loc_pos] == "(" ? parse_loc(loc_pos) : "";
            ansd = form_address_scheme();
            ansd[KEY_ADDR_TYPE] = VAL_TYPE_NORMAL;
            ansd[KEY_ADDR_TIME] = new Dictionary<object, object> {
                {
                    KEY_ADDR_DAY,
                    day},
                {
                    KEY_ADDR_START,
                    start},
                {
                    KEY_ADDR_END,
                    end}};
            ansd[KEY_ADDR_LOCATION] = loc;
            ans.append(ansd);
        }
        foreach (var res in res_spec) {
            (ins, ine) = res.span(1);
            loc_pos = res.span()[1];
            start = res.group(2) ? Convert.ToInt32(res.group(2)) : 0;
            end = res.group(3) ? Convert.ToInt32(res.group(3)) : start;
            loc = loc_pos < dstrp.Count && dstrp[loc_pos] == "(" ? parse_loc(loc_pos) : "";
            ansd = form_address_scheme();
            ansd[KEY_ADDR_TYPE] = VAL_TYPE_SPECIAL;
            ansd[KEY_ADDR_TIME] = new Dictionary<object, object> {
                {
                    KEY_ADDR_DESC,
                    dstr[ins::ine]},
                {
                    KEY_ADDR_START,
                    start},
                {
                    KEY_ADDR_END,
                    end}};
            ansd[KEY_ADDR_LOCATION] = loc;
            ans.append(ansd);
        }
        if (!ans) {
            ansd = form_address_scheme();
            ansd[KEY_ADDR_TYPE] = VAL_TYPE_UNKNOWN;
            ansd[KEY_ADDR_TIME][KEY_ADDR_DESC] = dstr;
            ans.append(ansd);
        }
        return ans;
    }
    
    // 
    //   return: (year, month, date) or None
    //   
    public static datetime parse_date(object dstr) {
        var patterns = new List<string> {
            @"( *\d+ *)/( *\d+ *)/( *\d+ *)",
            @"( *\d+ *)-( *\d+ *)-( *\d+ *)",
            @"( *\d+ *)年( *\d+ *)月( *\d+ *)日"
        };
        var ans = 0;
        foreach (var p in patterns) {
            var rs = re.search(p, dstr);
            try {
                var anst = (from i in Enumerable.Range(1, 4 - 1)
                    select Convert.ToInt32(rs.group(i).strip(" "))).ToList();
                ans = to_timestamp(anst);
            } catch (Exception) {
                // print(e)
            }
        }
        // return ans
        return datetime.datetime.fromtimestamp(ans / 1000, tz: datetime.timezone.utc);
    }
    
    public static tuple parse_ay_and_q(object dstr) {
        var pattern = @" *(H?R?\d{1,4}) *年?度? *(.+) *Q";
        var rs = re.search(pattern, dstr);
        object ans = null;
        try {
            var anst = (from i in Enumerable.Range(1, 3 - 1)
                select rs.group(i).strip(" ")).ToList();
            anst[0] = Convert.ToInt32(anst[0]);
            anst[1] = parse_quarter(anst[1]);
            ans = tuple(anst);
        } catch {
        }
        return ans;
    }
    
    public static int parse_quarter(List<int> dstr) {
        var ans = 0;
        try {
            var anst = dstr.replace("Q", "").strip();
            var anst1 = new List<bool> {
                false,
                false,
                false,
                false
            };
            var last_d = 0;
            var bar = false;
            foreach (var d in anst) {
                try {
                    var cur_d = Convert.ToInt32(d) - 1;
                    anst1[cur_d] = true;
                    if (bar) {
                        foreach (var dd in Enumerable.Range(last_d, cur_d - last_d)) {
                            anst1[dd] = true;
                        }
                    }
                    last_d = cur_d;
                } catch {
                    if (new List<string> {
                        "-",
                        "~"
                    }.Contains(d)) {
                        bar = true;
                    }
                }
            }
            ans = bool2bin(anst1);
        } catch (Exception) {
            throw new Exception();
        }
        return ans;
    }
    
    public static List<object> parse_contacts(object dstr) {
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
        foreach (var i in Enumerable.Range(0, bad_signs.Count)) {
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
    
    public static string parse_book(object bstr) {
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
        if (none_mark) {
            return "None";
        }
        if (lect_mark) {
            return "Lecture Note";
        }
        return bstr;
    }
    
    public static object parse_acrk(object ar) {
        return float(re.search("acbar(\d+).gif", ar).group(1));
    }
    
    public static Dictionary<string, string> lang_dict = new Dictionary<object, object> {
        {
            "日本語",
            "ja"},
        {
            "英語",
            "en"},
        {
            "Japanese",
            "ja"},
        {
            "English",
            "en"}};
    
    public static Func<object, object> parse_lang = x => lang_dict.Contains(x) ? lang_dict[x] : x;
    
    static parse_utils() {
        if (@__name__ == "__main__") {
            Console.WriteLine(parse_addr("集中講義等 5-8(Zoom)"));
            Console.WriteLine(parse_addr("月5-8(物理学生実験室（石川台6・南５）) 木5-8(物理学生実験室（石川台6・南５）)"));
        }
    }
}
