namespace Oocw.Backend.Database
{
    public class Definitions
    {

        // db related

        public const string DB_SET_NAME = "openocw";
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

        public const string KEY_ID = "id";
        public const string KEY_CONTACT = "contact";
    }
}
