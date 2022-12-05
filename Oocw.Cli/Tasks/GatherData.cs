using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Cli.Tasks;

public static class GatherData
{

    public static Tuple<defaultdict, List<>, Dictionary<object, object>> gather_data(string inpath)
    {
        (details, _) = pload(inpath);
        var ans = new defaultdict(() => defaultdict(@int));
        var anst = new defaultdict(dict);
        var ansu = new Dictionary<object, object>
        {
        };
        foreach (var code in details)
        {
            foreach (var year in details[code])
            {
                var subu = new List<void> {
                    null,
                    null
                };
                foreach (var item in details[code][year])
                {
                    if (!item)
                    {
                        continue;
                    }
                    foreach (var k in item[1])
                    {
                        if (item[1][k] is collections.Hashable)
                        {
                            ans[k][item[1][k]] += 1;
                        }
                    }
                    foreach (var k in item[2])
                    {
                        if (item[2][k] is collections.Hashable)
                        {
                            ans[k][item[2][k]] += 1;
                        }
                    }
                    if (item[1].Contains("担当教員名") && type(item[1]["担当教員名"]) == list)
                    {
                        foreach (var (id, tn) in item[1]["担当教員名"])
                        {
                            anst[id]["ja"] = tn;
                        }
                    }
                    if (item[1].Contains("Instructor(s)") && type(item[1]["Instructor(s)"]) == list)
                    {
                        foreach (var (id, tn) in item[1]["Instructor(s)"])
                        {
                            anst[id]["en"] = tn;
                        }
                    }
                    if (item[1].Contains("開講元"))
                    {
                        subu[0] = item[1]["開講元"];
                    }
                    if (item[1].Contains("Academic unit or major"))
                    {
                        subu[1] = item[1]["Academic unit or major"];
                    }
                }
                if (subu[0] != null)
                {
                    ansu[subu[0]] = subu[1];
                }
            }
        }
        return (ans, anst, ansu);
    }

}
