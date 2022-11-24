
using Any = typing.Any;

using Dict = typing.Dict;

using Optional = typing.Optional;

using Tuple = typing.Tuple;

using MongoClient = pymongo.MongoClient;

using WriteConcern = pymongo.WriteConcern;

using ReadPreference = pymongo.ReadPreference;

using ClientSession = pymongo.client_session.ClientSession;

using ReadConcern = pymongo.read_concern.ReadConcern;

using Collection = pymongo.collection.Collection;

using System.Collections.Generic;

public static class db_oprs {
    
    public static object init_db() {
        var uri = "mongodb://localhost:27017/";
        var cl = MongoClient(host: uri);
        return cl;
    }
    
    public static object get_cols(object cl) {
        var db = cl["openocw"];
        var col_crs = db["courses"];
        var col_cls = db["classes"];
        var col_fct = db["faculties"];
        return (col_crs, col_cls, col_fct);
    }
    
    public static void put_course(object db, object sess, Dictionary<string, object> crs) {
        (ccrs, _, _) = get_cols(db);
        var cid = crs[KEY_CODE];
        var crs_old = ccrs.find_one(new Dictionary<object, object> {
            {
                KEY_CODE,
                cid}}, session: sess);
        var crs_new = new Dictionary<object, object> {
        };
        if (crs_old != null) {
            // TODO timestamp concern
            crs_new = new dict(crs_old);
            foreach (var k in crs) {
                if (k == KEY_CLASSES || k == KEY_META) {
                    continue;
                }
                crs_new[k] = crs[k];
            }
            crs_new[KEY_CLASSES] += crs[KEY_CLASSES];
            list.sort(crs_new[KEY_CLASSES]);
            crs_new[KEY_CLASSES] = list_clean_repeats(crs_new[KEY_CLASSES], lmb_identity);
        } else {
            crs_new = new dict(crs);
        }
        crs_new[KEY_META][KEY_UPD_TIME] = datetime.datetime.utcnow();
        crs_new[KEY_META][KEY_DIRTY] = true;
        if (crs_old != null) {
            ccrs.update_one(new Dictionary<object, object> {
                {
                    KEY_CODE,
                    cid}}, new Dictionary<object, object> {
                {
                    "$set",
                    crs_new}}, session: sess);
        } else {
            ccrs.insert_one(crs_new, session: sess);
        }
    }
    
    public static void put_class(object db, object sess, Dictionary<string, object> cls) {
        (ccrs, ccls, _) = get_cols(db);
        var cond_scheme = new Dictionary<object, object> {
            {
                KEY_CODE,
                cls[KEY_CODE]},
            {
                KEY_YEAR,
                cls[KEY_YEAR]},
            {
                KEY_CLASS_NAME,
                cls[KEY_CLASS_NAME]}};
        var cls_old = ccls.find_one(cond_scheme, session: sess);
        var cls_new = new Dictionary<object, object> {
        };
        if (cls_old == null) {
            cls_new = new dict(cls);
        } else {
            cls_new = new dict(cls_old);
            // TODO compare timestamp
            foreach (var k in cls) {
                if (new List<object> {
                    KEY_META,
                    KEY_SYLLABUS,
                    KEY_NOTES
                }.Contains(k)) {
                    continue;
                }
                cls_new[k] = cls[k];
            }
            var oid = cls_old[KEY_META][KEY_OCW_ID];
            if (oid != cls[KEY_META][KEY_OCW_ID]) {
                // TODO may cause bug, try to put first
                ccrs.update_one(cond_scheme, new Dictionary<object, object> {
                    {
                        "$set",
                        new Dictionary<object, object> {
                            {
                                KEY_CLASS_NAME,
                                cls_old[KEY_CLASS_NAME] + oid.ToString()}}}}, session: sess);
            }
            // maintain key_meta
            // TODO
            // maintain syllabus and note
            foreach (var k in cls[KEY_SYLLABUS]) {
                cls_new[KEY_SYLLABUS][k] = cls[KEY_SYLLABUS][k];
            }
            foreach (var k in cls[KEY_NOTES]) {
                cls_new[KEY_NOTES][k] = cls[KEY_NOTES][k];
            }
        }
        cls_new[KEY_META][KEY_UPD_TIME] = datetime.datetime.utcnow();
        cls_new[KEY_META][KEY_DIRTY] = true;
        var cls_found = ccls.find_one_and_update(cond_scheme, new Dictionary<object, object> {
            {
                "$set",
                cls_new}}, session: sess);
        if (!cls_found) {
            ccls.insert_one(cls_new, session: sess);
        }
    }
    
    public static void put_faculty(object db, object sess, Dictionary<string, object> scheme) {
        (_, _, cfct) = get_cols(db);
        var fid = scheme[KEY_ID];
        var cond_scheme = new Dictionary<object, object> {
            {
                KEY_ID,
                fid}};
        var fct_old = cfct.find_one(cond_scheme, session: sess);
        if (fct_old == null) {
            cfct.insert_one(scheme, session: sess);
        } else {
            cfct.update_one(cond_scheme, new Dictionary<object, object> {
                {
                    "$set",
                    scheme}});
        }
    }
}
