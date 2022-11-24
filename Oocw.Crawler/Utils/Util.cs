﻿using Oocw.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Oocw.Crawler.Utils;

using IDSS = IDictionary<string, string>;

public static class Util
{


    // dict

    public static string? Get(this NameValueCollection col, string key)
    {
        int ind = -1;
        for (int i = 0; i < col.AllKeys.Length; ++i)
        {
            if (col.AllKeys[i] == key)
            {
                ind = i;
                break;
            }
        }
        return ind > 0 ? col[ind] : null;
    }

    public static NestedDictionary<string, string?> ToNestedDictionary(this NameValueCollection col)
    {
        NestedDictionary<string, string?> result = new NestedDictionary<string, string?>();
        foreach (var k in col.AllKeys)
        {   
            if (k == null)
                continue;
            result[k] = new() { Value = col[k] };
        }
        return result;
    }

    public static string AddUrlEncodedParameters(this string rawUrl, params IDSS[] parameters)
    {
        StringBuilder sb = new();
        sb.Append(rawUrl);
        bool flag = rawUrl.Contains('?');
        foreach (var param in parameters)
        {
            foreach (var (k, v) in param)
            {
                if (flag)
                    sb.Append('&');
                else
                    sb.Append('?');
                flag = true;
                sb.Append(HttpUtility.UrlEncodeUnicode(k));
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncodeUnicode(v));
            }
        }
        return sb.ToString();
    }

    public static string FixWebString(this string strIn)
    {
        var strOut = strIn
            .Replace("\t", " ").Replace("\n", " ")
            .Replace("\xa0", " ").Replace("\u3000", " ")
            .Trim(' ');
        return strOut;
    }
}