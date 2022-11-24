using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;


public static class Utils
{
    


    // reference: https://www.cnblogs.com/xu-yi/p/10525090.html
    public static string ToSBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new string(c);
    }

    public static string ToDBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }

    public static string Cleanout(this string str)
    {
        var ans = str
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Trim().ToDBC();
        return ans;
    }

    public static V? TryGetValue<K, V>(this IDictionary<K, V> dict, params K[] keys)
    {
        V? val = default(V);
        foreach (var key in keys)
        {
            if (dict.TryGetValue(key, out val))
                return val;
        }
        return val;
    }

    public static string ToStringConcat(this IEnumerable<string> strs, string? sep = null)
    {
        return string.Join(sep ?? "", strs);
    }

    // string operations

    public static string EnsurePrefix(this string str, string prefix)
    {
        if (str.StartsWith(prefix))
            return str;
        return prefix + str;
    }

    public static string RemovePrefix(this string str, string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return str;
        while (str.StartsWith(prefix))
            str = str.Substring(prefix.Length);
        return str;
    }

    public static string EnsureSuffix(this string str, string suffix)
    {
        if (str.EndsWith(suffix))
            return str;
        return str + suffix;
    }

    public static string RemoveSuffix(this string str, string suffix)
    {
        if (string.IsNullOrEmpty(suffix))
            return str;
        while (str.EndsWith(suffix))
            str = str.Substring(0, str.Length - suffix.Length);
        return str;
    }


    public static string LCP(string a, string b)
    {
        for (int i = 0; i < a.Length; ++i)
        {
            if (b.Length <= i)
                return b;
            if (a[i] != b[i])
                return a.Substring(0, i);
        }
        return a;
    }

    private static string _lcp(in string[] strs, int l, int r)
    {
        if (r - l < 2)
            return strs[l];
        else if (r - l == 2)
            return LCP(strs[l], strs[l + 1]);
        int m = (r + l) / 2;
        return LCP(_lcp(strs, l, m), _lcp(strs, m, r));
    }

    public static string LCP(params string[] strs)
    {
        if (strs.Length == 0)
            return string.Empty;
        if (strs.Length == 1)
            return strs[0];
        return _lcp(strs, 0, strs.Length);
    }

    public static string LCP<T>(this IEnumerable<T> strs, Func<T, string> selector)
    {
        return LCP(strs.Select(selector).ToArray());
    }

    public static string LCP(this IEnumerable<string?> strs)
    {
        return LCP(strs.Select(x => x ?? string.Empty).ToArray());
    }


    // string case
}