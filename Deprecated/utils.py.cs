
using itertools;

using re;

using defaultdict = collections.defaultdict;

using System;

using System.Linq;

using System.Collections.Generic;

using System.Collections;

public static class utils {
    
    
    public static Func<object, object> rev_range = x => Enumerable.Range(0, Convert.ToInt32(Math.Ceiling(Convert.ToDouble(-1 - (x - 1)) / -1))).Select(_x_2 => x - 1 + _x_2 * -1);
    
    public static Func<object, object> lmb_identity = x => x;
    
    public static Func<object, object> lmb_yield_none = x => null;
    
    public static string find_common_prefix(object strs) {
        if (!strs) {
            return "";
        }
        var l = strs[0].Count;
        var n = strs.Count;
        foreach (var i in Enumerable.Range(0, l)) {
            var c = strs[0][i];
            if (any(from j in Enumerable.Range(1, n - 1)
                select i == strs[j].Count || strs[j][i] != c)) {
                return strs[0][::i];
            }
        }
        return strs[0];
    }
    
    public static string find_common_suffix(object strs) {
        if (!strs) {
            return "";
        }
        var l = strs[0].Count;
        var n = strs.Count;
        foreach (var i in Enumerable.Range(0, l)) {
            var c = strs[0][-i - 1];
            if (any(from j in Enumerable.Range(1, n - 1)
                select i == strs[j].Count || strs[j][-i - 1] != c)) {
                return strs[0][i != 0 ? -i : l];
            }
        }
        return strs[0];
    }
    
    public static Func<object, object> list_concat = lists => itertools.chain(lists).ToList();
    
    public static Func<object, object> list_clean_repeats = (x,f) => x.Count > 1 ? new List<object> {
        x[0]
    } + (from i in Enumerable.Range(1, x.Count - 1)
        where f(x[i]) != f(x[i - 1])
        select x[i]).ToList() : x;
    
    public static Func<object, object> list_fill_repeats = (x,f,fill) => x.Count > 1 ? new List<object> {
        x[0]
    } + (from i in Enumerable.Range(1, x.Count - 1)
        select f(x[i]) != f(x[i - 1]) ? x[i] : fill(x[i])).ToList() : x;
    
    public static void first_unrepeated_name(object s, object it, int i_init = 1, Func<object, object> func = (s,i) => "{}({})".format(s, i)) {
        var i = i_init;
        var cur_name = s;
        while (it.Contains(cur_name)) {
            i += 1;
            cur_name = func(s, i);
        }
        return cur_name;
    }
    
    public class node {
        
        public object children;
        
        public object info;
        
        public node(Dictionary<object, object> info = new Dictionary<object, object> {
        }, Dictionary<object, object> children = new Dictionary<object, object> {
        }) {
            this.info = info;
            this.children = children;
        }
        
        public virtual dict @__dict__() {
            var ans_children = new Dictionary<object, object> {
            };
            foreach (var c in this.children) {
                ans_children[c] = this.children[c].@__dict__();
            }
            return new dict(info: this.info, children: ans_children);
        }
    }
    
    public static List<List<object>> iter_tree(object n, Func<object, object> children = n => n.children.values(), string mode = "dfs", bool pass_queue = false) {
        object ncur;
        object queue;
        if (!pass_queue) {
            queue = new List<object> {
                n
            };
        } else {
            queue = n;
        }
        if (mode == "dfs" || mode == "DFS") {
            while (queue) {
                ncur = queue.pop();
                var l = children(ncur).ToList();
                l.reverse();
                queue += l;
                yield return ncur;
            }
        } else {
            while (queue) {
                ncur = queue[0];
                queue = queue[1];
                queue += children(ncur).ToList();
                yield return ncur;
            }
        }
    }
    
    
    public static void find_common_word(List<string> strs) {
        var T = new trietree();
        T.add_words(strs);
        return T.scan_common_word();
    }
    
    public static void scw = find_common_word(new List<string> {
        "abccc",
        "abaadd",
        "abaaaab",
        "abzz"
    });
    
    public static Func<object, object> split_with_sign = (s,sgn) => list_concat((from x in s.split(sgn)
        select new List<object> {
            x,
            sgn
        }).ToList())[:: - 1];
    
    public static void apart_str(object s, object signs, List<string> mono_brackets = new List<string> {
        "\""
    }, List<string> dual_brackets = new List<string> {
        "()",
        "[]"
    }) {
        if (s is str) {
            s = new List<object> {
                s
            };
        }
        // mono_brackets
        foreach (var mb in mono_brackets) {
            s = list_concat((from x in s
                select split_with_sign(x)).ToList());
        }
    }
    
    public static Func<object, object> strip_useless_elem = l => (from x in l
        where x.ToString() != "\n" && x.ToString() != "\xa0"
        select x).ToList();
    
    static utils() {
        if (@__name__ == "__main__") {
        }
    }
}
