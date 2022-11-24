
using List = typing.List;

using tqdm = tqdm.tqdm;

using Tagger = fugashi.Tagger;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;

using System;

public static class build_search_index {
    
    public static string TEXT_INDEX_TARGET = $"{KEY_META}.{KEY_SEARCH_REC}";
    
    public static string TEXT_DIRTY_TARGET = $"{KEY_META}.{KEY_DIRTY}";
    
    public static Dictionary<string, bool> TEXT_INDEX_DIRTY_SCHEME = new Dictionary<object, object> {
        {
            TEXT_DIRTY_TARGET,
            true}};
    
    public static object tagger = Tagger("-Owakati");
    
    public static object build_stopwords() {
        var sw = new Dictionary<object, object> {
        };
        foreach (var fn in (from x in new List<object> {
            "en",
            "ja",
            "zh",
            "py"
        }
            select "./data/stopwords_{}.txt".format(x)).ToList()) {
            using (var f = open(fn, "r", encoding: "utf-8")) {
                while (true) {
                    fl = f.readline();
                    if (!fl) {
                        break;
                    }
                    word = fl.replace("\n", "").replace("\r", "");
                    sw[word] = true;
                }
            }
        }
        return sw;
    }
    
    public static object stopwords = build_stopwords();
    
    public static void create_index(object db) {
        (ccrs, ccls, cfct) = get_cols(db);
        var scheme = new List<Tuple<string, string>> {
            (TEXT_INDEX_TARGET, "text")
        };
        ccrs.create_index(scheme);
        ccls.create_index(scheme);
        cfct.create_index(scheme);
    }
    
    public static object get_random_dirty_doc(object col, object sess = null) {
        return col.find_one(TEXT_INDEX_DIRTY_SCHEME, session: sess);
    }
    
    public static bool update_text_index(object col, Dictionary<string, object> doc, List<string> index, object sess = null) {
        var res = col.find_one_and_update(new Dictionary<object, object> {
            {
                "_id",
                doc["_id"]}}, new Dictionary<object, object> {
            {
                "$set",
                new Dictionary<object, object> {
                    {
                        TEXT_INDEX_TARGET,
                        index},
                    {
                        TEXT_DIRTY_TARGET,
                        false}}}}, session: sess);
        return res != null;
    }
    
    public static object iter_dirty_doc(object col, object sess = null) {
        var doc = get_random_dirty_doc(col, sess);
        while (doc != null) {
            yield return doc;
            doc = get_random_dirty_doc(col, sess);
        }
    }
    
    public static object db = init_db();
    
    static build_search_index() {
        create_index(db);
        s[wstr] = true;
        s[wfeat] = true;
    }
    
    public static Dictionary<object, object> s = new Dictionary<object, object> {
    };
    
    public static List<string> wpos = word.pos.ToString().split(",")[0];
    
    public static string wstr = word.ToString();
    
    public static string wfeat = word.feature.lemma.ToString();
    
    public static list s = s.keys().ToList();
    
    public static bool dres = update_text_index(ccls, doc, s);
    
    static build_search_index() {
        try {
            Debug.Assert(tagger);
        } catch {
        }
        try {
            Debug.Assert(stopwords);
        } catch {
        }
        if (@__name__ == "__main__") {
            (ccrs, ccls, cfct) = get_cols(db);
            foreach (var doc in tqdm(iter_dirty_doc(ccls))) {
                foreach (var word in tagger(doc.ToString())) {
                    if (wpos.Contains("記号")) {
                        continue;
                    }
                    if (stopwords.Contains(wstr) || stopwords.Contains(wfeat)) {
                        continue;
                    }
                }
            }
        }
    }
}
