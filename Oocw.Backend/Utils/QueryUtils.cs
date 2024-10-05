using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

using Oocw.Database;
using Oocw.Base;

namespace Oocw.Backend.Utils;

public static class QueryUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inStr"></param>
    public static string FormSearchKeyWords(string inStr)
    {
        // TODO add support of quoted string
        string tokens = string.Join(" ", SearchUtils.TokenizeJapanese(inStr));
        return tokens;
    }

    public static string TryGetLanguage(this ControllerBase ctrl, string def = "ja")
    {
        var cookies = ctrl.Request.Cookies;
        cookies.TryGetValue("lang", out var ret);
        ret = ret ?? def;
        return ret;
    }

    public static void RecordLanguageSettings(this ControllerBase ctrl, string lang = "ja")
    {
        ctrl.Response.Cookies.Append("lang", lang);
    }


    public static (int, int) GetPageInfo(int? dispCount, int? page)
    {
        int dCount = (dispCount ?? 0);
        dCount = dCount > 10 ? dCount : 10;
        dCount = dCount < 100 ? dCount : 100;

        int dPage = (page ?? 0);
        dPage = dPage > 1 ? dPage : 1;

        return (dCount, dPage);
    }

}
