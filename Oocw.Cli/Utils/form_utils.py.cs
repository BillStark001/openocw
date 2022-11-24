
using re;

using defaultdict = collections.defaultdict;

using System.Linq;

using System.Collections.Generic;

using System;

public static class form_utils {
    
    static form_utils() {
        @"
Created on Sun Jun 13 00:02:27 2021

@author: billstark
";
    }
    
    // -*- coding: utf-8 -*-
    public static void build_tree(string inpath, string outpath, string dep_path) {
        // 開講元のID
        var sd_num_map = jload(dep_path);
        var sd_num_map_ja = new dict((from y in list_concat((from x in sd_num_map
            select x["department"]).ToList())
            select new List<object> {
                y["nameJa"],
                y["id"]
            }).ToList());
        var sd_num_map_en = new dict((from y in list_concat((from x in sd_num_map
            select x["department"]).ToList())
            select new List<object> {
                y["nameEn"],
                y["id"]
            }).ToList());
        // オリジナルなリスト
        (d, e) = pload(inpath);
        (jt, jl) = d;
        (et, el) = e;
        // 異なるurlの共通的なquery parametersを得る
        object max_common_dict(List<object> l) {
            if (l.Count == 0) {
                return new Dictionary<object, object> {
                };
            }
            ans = new dict(l[0]);
            foreach (var d in l[1]) {
                foreach (var k in ans.keys().ToList()) {
                    if (!d.Contains(k) || ans[k] != d[k]) {
                        ans.Remove(k);
                    }
                }
            }
            return ans;
        }
        // リストはツリー状なものである
        // 辞書型のリストの要素をノードに変える
        object reform(object n) {
            if (n.Count == 0) {
                return node();
            }
            if (!(first_elem(n) is dict)) {
                return node(info: new dict(n));
            }
            ans_k = new List<object>();
            ans_d = new List<object>();
            foreach (var k in n) {
                r = reform(n[k]);
                ans_k.append((k, r));
                ans_d.append(r.info);
            }
            return node(info: max_common_dict(ans_d), children: ltod(ans_k));
        }
        // OCWには正くないパラメータを含むリンクがある
        // これを直す
        object fix_subaction(List<void> n) {
            if (n.info.Count == 1 && n.info.Contains("GakubuCD") || n.info.Count == 2 && n.info.Contains("GakubuCD") && n.info.Contains("SubAction")) {
                //print(n.info)
                if (new List<int> {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6
                }.Contains(n.info["GakubuCD"])) {
                    n.info["SubAction"] = "T0210";
                } else if (new List<int> {
                    7
                }.Contains(n.info["GakubuCD"])) {
                    n.info["SubAction"] = "T0200";
                    //print(n.info)
                }
            }
            foreach (var nn in n.children.values()) {
                fix_subaction(nn);
            }
        }
        var jtt = (from i in Enumerable.Range(0, jt.Count)
            select reform(jt[i])).ToList();
        fix_subaction(jtt[0]);
        var ett = (from i in Enumerable.Range(0, et.Count)
            select reform(et[i])).ToList();
        fix_subaction(ett[0]);
        // この前作ったノードツリーを再びリストに変える
        // あるノートに対してそのアドレスを表示する関数
        object c(object x) {
            return (from y in x[1].children.items()
                select new List<string> {
                    (x[0] + "." + y[0]),
                    y[1]
                }).ToList();
        }
        // ソートのための比較関数
        object d(object x) {
            return (x[1].ToString(), x[0]);
        }
        object d2(object x) {
            return x[1].ToString();
        }
        // 新課程
        var used_dicts = (from x in iter_tree(new List<object> {
            new List<object> {
                "学部",
                jtt[0]
            },
            new List<object> {
                "大学院",
                jtt[1]
            },
            new List<object> {
                "Undergraduate",
                ett[0]
            },
            new List<object> {
                "Graduate",
                ett[1]
            }
        }, children: c, pass_queue: true)
            select (x[0], x[1].info)).ToList();
        used_dicts.sort(key: d, reverse: true);
        used_dicts = list_clean_repeats(used_dicts, d2);
        used_dicts.sort(key: x => (sd_num_map_ja.Contains(x[0]) ? sd_num_map_ja[x[0]] : 114514, x[0], x[1].ToString()));
        // 旧課程
        var used_dicts_old = (from x in iter_tree(new List<object> {
            new List<object> {
                "学部",
                jtt[2]
            },
            new List<object> {
                "大学院",
                jtt[3]
            },
            new List<object> {
                "Undergraduate",
                ett[2]
            },
            new List<object> {
                "Graduate",
                ett[3]
            }
        }, children: c, pass_queue: true)
            select (x[0], x[1].info)).ToList();
        used_dicts_old.sort(key: d, reverse: true);
        used_dicts_old = list_clean_repeats(used_dicts_old, d2);
        used_dicts.sort(key: x => (x[0], x[1].ToString()));
        @"
  jl = ltod(list_concat([[[x[0], x[1].info] for x in iter_tree(['ルート', y], children=c)] for y in jtt]), ignore_repeat=False)
  el = ltod(list_concat([[[x[0], x[1].info] for x in iter_tree(['root', y], children=c)] for y in ett]), ignore_repeat=False)

  sdmap = ltod([[str(x.info), x.info] for x in iter_tree(jtt[0])])
  djmap = ltod(list_concat([[[str(x[1].info), x[0]] for x in iter_tree(['ルート', y], children=lambda x: x[1].children.items())] for y in jtt]))
  demap = ltod(list_concat([[[str(x[1].info), x[0]] for x in iter_tree(['root', y], children=lambda x: x[1].children.items())] for y in ett]))
  jemap = [[djmap[k], (demap[k] if k in demap else '')] for k in djmap] + [[(djmap[k] if k in djmap else ''), demap[k]] for k in demap]
  ejmap = [[x[1], x[0]] for x in jemap]
  jemap = ltod(jemap, ignore_repeat=False)
  ejmap = ltod(ejmap, ignore_repeat=False)

  jk = [djmap[str(x[1])] for x in used_dicts]
  ek = [demap[str(x[1])] for x in used_dicts]
  ";
        // 出力のために辞書型に変える
        jtt = (from x in jtt
            select x.@__dict__()).ToList();
        ett = (from x in ett
            select x.@__dict__()).ToList();
        // 出力
        var additional = (jtt, ett);
        pdump((used_dicts, used_dicts_old, additional), outpath);
    }
    
    public static Dictionary<object, object> build_keys(string inpath, string outpath) {
        (details, _) = pload(inpath);
        var keys_jp = new defaultdict(list);
        var keys_en = new defaultdict(list);
        foreach (var code in details) {
            foreach (var year in details[code]) {
                (detail_jp, detail_en) = details[code][year];
                if (!detail_jp) {
                    continue;
                }
                if (detail_jp) {
                    foreach (var k in detail_jp[1]) {
                        keys_jp[k].append(detail_jp[1][k]);
                    }
                    foreach (var k in detail_jp[2]) {
                        keys_jp[k].append(detail_jp[2][k]);
                    }
                }
                if (detail_en) {
                    foreach (var k in detail_en[1]) {
                        keys_en[k].append(detail_en[1][k]);
                    }
                    foreach (var k in detail_en[2]) {
                        keys_en[k].append(detail_en[2][k]);
                    }
                }
            }
        }
        var kj = keys_jp.keys();
        var ke = keys_en.keys();
        kj = kj.ToList();
        ke = ke.ToList();
        // 2022/8/6
        var keys_general = new List<object> {
            KEY_UNIT,
            KEY_LECTURERS,
            KEY_FORMAT,
            KEY_MEDIA_USE,
            KEY_ADDRESS,
            KEY_CLASS_NAME,
            KEY_CODE,
            KEY_CREDIT,
            KEY_YEAR,
            KEY_QUARTER,
            KEY_UPD_TIME_SYL,
            KEY_UPD_TIME_NOTES,
            KEY_LANGUAGE,
            KEY_ACCESS_RANK
        } + new List<object> {
            KEY_SYL_DESC,
            KEY_SYL_OUTCOMES,
            KEY_SYL_KEYWORDS,
            KEY_SYL_COMPETENCIES,
            KEY_SYL_FLOW,
            KEY_SYL_SCHEDULE,
            KEY_SYL_TEXTBOOKS,
            KEY_SYL_REF_MATS,
            KEY_SYL_GRADING,
            KEY_SYL_REL_COURSES,
            KEY_SYL_PREREQUESITES,
            KEY_SYL_OOC_TIME,
            KEY_SYL_OTHER,
            KEY_SYL_CONTACT,
            KEY_SYL_EXP_INST,
            KEY_SYL_OFF_HRS
        };
        var kkk = new Dictionary<object, object> {
        };
        foreach (var i in Enumerable.Range(0, keys_general.Count)) {
            kkk[keys_general[i]] = new Dictionary<object, object> {
                {
                    "ja",
                    kj[i]},
                {
                    "en",
                    ke[i]}};
        }
        pdump(kkk, outpath);
        return kkk;
    }
    
    public static void form_record(object recs_raw, object code, int year = 2022, string lang = "ja") {
        // scan common words
        var names = (from x in recs_raw
            select x[0][0]).ToList();
        var common_name = find_common_word(names);
        // build basic record
        var ans = form_basic_record_scheme();
        ans[KEY_CODE] = code;
        ans[KEY_YEAR] = year;
        ans[KEY_ISLINK] = false;
        ans[KEY_NAME] = new Dictionary<object, object> {
            {
                lang,
                common_name}};
        // build full record
        foreach (var rec in recs_raw) {
            var cls = form_class_record_scheme();
            var cls_meta = cls[KEY_META];
            cls_meta[KEY_OCW_ID] = rec[0][1][0];
            // determine class name
            var cname = rec[0][0];
            cname = cname[find_common_prefix(new List<object> {
                cname,
                common_name
            }).Count].strip();
            cls[KEY_NAME] = cname;
            cls[KEY_LECTURERS] = (from x in rec[1]
                select x[1]).ToList();
        }
        ans[KEY_CLASSES].append(cls);
        return ans;
    }
    
    public static void form_records(string inpath, string outpath) {
        (cl1, _, _, _) = pload(inpath);
        var clist = new defaultdict(list);
        // merge code
        foreach (var course in cl1) {
            var ccode = course[0];
            clist[ccode].append(course[1]);
        }
        // clean repeats
        foreach (var code in clist) {
            var info = clist[code];
            info.sort(key: key1);
            info = list_clean_repeats(info, key1);
            clist[code] = info;
        }
        object key1(object x) {
            return x[0][1];
        }
        var built_list = new Dictionary<object, object> {
        };
        var lecturers = new Dictionary<object, object> {
        };
        foreach (var code in clist) {
            var cur_course = clist[code];
            // add lecturer info
            foreach (var rec in cur_course) {
                var lects = rec[1];
                foreach (var (lname, lid) in lects) {
                    if (!lecturers.Contains(lid)) {
                        lecturers[lid] = new Dictionary<object, object> {
                            {
                                "ja",
                                lname}};
                    }
                }
            }
            // form record
            var course_rec = form_record(cur_course, code, 2022, "ja");
            built_list[code] = course_rec;
        }
        pdump((built_list, lecturers), outpath);
    }
    
    static form_utils() {
        if (@__name__ == "__main__") {
        }
    }
}
