
using tqdm = tqdm.tqdm;

public static class put_to_db {
    
    public static object keys = pload(path.savepath_details_keys);
    
    public static object db = init_db();
    
    public static object fsch = form_faculty_scheme();
    
    static put_to_db() {
        fsch[KEY_ID] = fid;
        fsch[KEY_NAME] = fname;
        sess.with_transaction(sess => put_faculty(sess.client, sess, fsch));
        sess.with_transaction(sess => put_course(sess.client, sess, scrs));
        sess.with_transaction(sess => put_class(sess.client, sess, cls));
    }
    
    static put_to_db() {
        if (@__name__ == "__main__") {
            (dd, dt, du) = gather_data(path.savepath_details_raw);
            (details, _) = pload(path.savepath_details_raw);
            foreach (var (fid, fname) in tqdm(dt.items(), desc: "Faculties: ")) {
                using (var sess = db.start_session()) {
                }
            }
            foreach (var oid in tqdm(details, desc: "Courses: ")) {
                if (!(oid is int)) {
                    continue;
                }
                (scrs, scls) = build_db_schemes(oid, details[oid], keys);
                using (var sess = db.start_session()) {
                    foreach (var cls in scls) {
                    }
                }
            }
        }
    }
}
