namespace Oocw.Database;
public class Definitions
{

    // db related

    public const string DB_SET_NAME = "openocw";

    public const string COL_CNT_NAME = "counter";

    public const string COL_USER_NAME = "user";
    public const string COL_REL_NAME = "relation";

    public const string COL_CLASS_NAME = "classes";
    public const string COL_COURSE_NAME = "courses";
    public const string COL_FACULTY_NAME = "faculties";

    // technical

    public const string KEY_META = "__meta__";
    public const string KEY_COMPLETE = "compl";
    public const string KEY_DIRTY = "dirty";
    public const string KEY_OCW_ID = "ocwId";

    public const string KEY_UPD_TIME = "upd";
    public const string KEY_UPD_TIME_SYL = "updSyl";
    public const string KEY_UPD_TIME_NOTES = "updNts";

    public const string KEY_SEARCH_REC = "srec";
    public const string KEY_ACCESS_RANK = "acrk";


    public const string KEY_ID_RAW = "_id";
    public const string KEY_ID = "id";

    // counter

    public const string KEY_DB_NAME = "dbname";
    public const string KEY_SEQ = "seq";

    // lang related

    public const string KEY_LANG_KEY = "key";


    // universal

    public const string KEY_CODE = "code";
    public const string KEY_YEAR = "year";

    public const string KEY_NAME = "name";
    public const string KEY_CREDIT = "credit";
    public const string KEY_ISLINK = "isLink";
    public const string KEY_UNIT = "unit";

    public const string KEY_CLASSES = "classes";

    public const string KEY_MEDIA_USE = "mediaUse";

    // class-wise

    public const string KEY_CLASS_NAME = "cname";
    public const string KEY_LECTURERS = "lects";
    public const string KEY_FORMAT = "form";
    public const string KEY_QUARTER = "qurt";
    public const string KEY_ADDRESS = "addr";
    public const string KEY_LANGUAGE = "lang";

    public const string KEY_SYLLABUS = "syl";
    public const string KEY_VERSION = "ver";
    public const string VAL_VER_NONE = "none";
    public const string VAL_VER_RAW = "raw";
    public const string VAL_VER_PARSED = "parsed";
    public const string KEY_NOTES = "notes";

    // syllabus related

    public const string KEY_SYL_DESC = "desc";
    public const string KEY_SYL_OUTCOMES = "outcomes";
    public const string KEY_SYL_KEYWORDS = "keywords";
    public const string KEY_SYL_COMPETENCIES = "competencies";
    public const string KEY_SYL_FLOW = "flow";
    public const string KEY_SYL_SCHEDULE = "schedule";
    public const string KEY_SYL_TEXTBOOKS = "textbooks";
    public const string KEY_SYL_REF_MATS = "refmats";
    public const string KEY_SYL_GRADING = "grading";
    public const string KEY_SYL_REL_COURSES = "rels";
    public const string KEY_SYL_PREREQUESITES = "prereqs";
    public const string KEY_SYL_OOC_TIME = "ooctime";
    public const string KEY_SYL_OTHER = "other";
    public const string KEY_SYL_CONTACT = "contact";
    public const string KEY_SYL_EXP_INST = "exp_inst_courses";
    public const string KEY_SYL_OFF_HRS = "offhrs";

    // faculties

    public const string KEY_CONTACT = "contact";


    // user

    public const string KEY_USERNAME = "uname";
    public const string KEY_PASSWORD = "pwd";
    public const string KEY_SEL_PERS = "persona";

    public const string KEY_DELETED = "del";
    public const string KEY_REFRESH_TIME = "refresh";
    public const string KEY_GROUP = "group";

    // persona

    public const string KEY_USER_ID = "uid";
    public const string KEY_PERS_NAME = "name";

    // relationship

    public const string KEY_USER1 = "uid1";
    public const string KEY_USER2 = "uid2";
    public const string KEY_LAST_UPD = "lastUpdate";

    public const string KEY_TARGET = "tgt";
    public const int VAL_USER1 = 1;
    public const int VAL_USER2 = 2;
    public const int VAL_BOTH = 3;

    public const string KEY_STATUS = "status";
    public const int VAL_SENT = 0;
    public const int VAL_ACTIVE = 1;
    public const int VAL_INACTIVE = 2;
    public const int VAL_BLOCKED = 3; // currently useless

    public const string KEY_REASON = "reason";
}