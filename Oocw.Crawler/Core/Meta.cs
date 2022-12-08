using Oocw.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Core;

public static class Meta
{

    

    public static readonly string ROOT_DIR = Path.Combine(FileUtils.BASE_DIR, "operation") + "/";

    public static readonly string SAVEPATH_UNIT_TREE_RAW = ROOT_DIR + "unit_tree_raw.json";
    public static readonly string SAVEPATH_UNIT_TREE = ROOT_DIR + "unit_tree.json";

    public static readonly string SAVEPATH_COURSE_LIST_RAW = ROOT_DIR + "course_list_raw.json";
    public static readonly string SAVEPATH_COURSE_LIST = ROOT_DIR + "course_list.json";

    public static readonly string SAVEPATH_DETAILS_RAW = ROOT_DIR + "details_raw.json";
    public static readonly string SAVEPATH_DETAILS_KEYS = ROOT_DIR + "details_keys.json";

    public static readonly string DATA_DIR = FileUtils.BASE_DIR + "/";

    public static readonly string SCHOOL_DEP_MAP_PATH = DATA_DIR + "school2dep.json";
}
