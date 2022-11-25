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

    

    public static readonly string root_path = Path.Combine(FileUtil.BASE_DIR, "operation") + "/";

    public static readonly string savepath_unit_tree_raw = root_path + "unit_tree_raw.json";
    public static readonly string savepath_unit_tree = root_path + "unit_tree.json";

    public static readonly string savepath_course_list_raw = root_path + "course_list_raw.json";
    public static readonly string savepath_course_list = root_path + "course_list.json";

    public static readonly string savepath_details_raw = root_path + "details_raw.json";
    public static readonly string savepath_details_keys = root_path + "details_keys.json";

    public static readonly string data_path = FileUtil.BASE_DIR + "/";

    public static readonly string school_dep_map_path = data_path + "school2dep.json";
}
