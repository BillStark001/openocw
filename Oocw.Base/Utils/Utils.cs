using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oocw.Base;


public static class Utils
{



    // reference: https://www.cnblogs.com/xu-yi/p/10525090.html
    public static string ToFullWidth(this string input)
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

    public static string ToHalfWidth(this string input)
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

    public static readonly Regex ReturnRegex = new(@"\r?\n");
    public static string NormalizeReturns(this string strIn, bool crlf = false)
    {
        return ReturnRegex.Replace(strIn, crlf ? "\r\n" : "\n");
    }

    public static readonly Regex SpaceRegex = new(@"[ \xa0\u3000]");
    public static string NormalizeSpaces(this string strIn)
    {
        return SpaceRegex.Replace(strIn, " ");
    }

    public static readonly Regex SpacesBeforeReturnRegex = new(@"([ \xa0\u3000]*)$");
    public static readonly Regex EmptyLineRegex = new(@"^[ \xa0\u3000]*\r?\n", RegexOptions.Multiline);
    public static readonly Regex SpacesAtEndOfStringRegex = new(@"[ \xa0\u3000\r\n]*$");
    public static readonly Regex SpacesAtBeginningOfStringRegex = new(@"^[ \xa0\u3000\r\n]*");
    public static string RemoveUnnecessarySpaces(this string strIn)
    {
        var ans = strIn;
        ans = EmptyLineRegex.Replace(strIn, "");
        ans = SpacesBeforeReturnRegex.Replace(strIn, "");
        ans = SpacesAtEndOfStringRegex.Replace(strIn, "");
        ans = SpacesAtBeginningOfStringRegex.Replace(strIn, "");
        return ans.TrimEnd();
    }

    public static string NormalizeWebString(this string strIn, bool replaceReturns = true)
    {
        var strOut = strIn;
        strOut = strOut.Replace("\t", "    ").RemoveUnnecessarySpaces();
        strOut = SpaceRegex.Replace(strOut, " ");
        if (replaceReturns)
            strOut = ReturnRegex.Replace(strOut, " ");
        return strOut;
    }

    public static string RemoveReturnsAndTables(this string str)
    {
        var ans = str;
        ans = ReturnRegex.Replace(ans, "");
        ans = ans.Replace("\t", "").Trim();
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


    public static string NormalizeBrackets(this string strIn)
    {
        return strIn
            .Replace("（", "(").Replace("）", ")")
            .Replace("｛", "{").Replace("｝", "}")
            .Replace("【", "[").Replace("】", "]")
            .Replace("〔", "[").Replace("〕", "]")
            .Replace("［", "[").Replace("］", "]")
            ;
    }


    public static (int, int) FindBrackets(
        this string passage,
        int start = 0,
        string brl = "(",
        string brr = ")",
        bool strict = false,
        (string, string)? excludeIn = null)
    {

        var hasExclude = excludeIn.HasValue;
        var exclude = excludeIn ?? ("\"", "\\\"");

        var pointer = start;
        var endpoint = passage.Length;
        var lbrl = brl.Length;
        var lbrr = brr.Length;
        var lex0 = hasExclude ? exclude.Item1.Length : -1;
        var lex1 = hasExclude ? exclude.Item2.Length : -1;
        var layer = 0;
        var outer_brl = -1;
        while (pointer < endpoint)
        {
            var next_brl = passage.IndexOf(brl, pointer);
            var next_brr = passage.IndexOf(brr, pointer);
            var next_exclude = hasExclude ? passage.IndexOf(exclude.Item1, pointer) : -1;
            // print(pointer, next_brl, next_brr, next_exclude, layer)
            if (next_exclude >= 0 && (next_brl < 0 || next_exclude < next_brl) && (next_brr < 0 || next_exclude < next_brr))
            {
                pointer = next_exclude + lex0;
                while (true)
                {
                    var next_exclude_2 = passage.IndexOf(exclude.Item1, pointer);
                    var next_ignore_exclude_2 = passage.IndexOf(exclude.Item2, pointer);
                    if (next_ignore_exclude_2 >= 0 && next_exclude_2 > next_ignore_exclude_2 && next_exclude_2 <= next_ignore_exclude_2 + lex1)
                    {
                        // this exclude mark is ignored
                        pointer = next_ignore_exclude_2 + lex1;
                    }
                    else if (next_exclude_2 >= 0)
                    {
                        // this mark is real
                        pointer = next_exclude_2 + lex0;
                        break;
                    }
                    else if (strict)
                    {
                        throw new InvalidDataException("Excluding section matching failed");
                    }
                    else
                    {
                        return (outer_brl, -1);
                    }
                }
            }
            else if (next_brl >= 0 && (next_brl < next_brr || next_brr < 0))
            {
                pointer = next_brl + lbrl;
                if (layer == 0)
                {
                    outer_brl = next_brl;
                }
                layer += 1;
            }
            else if (next_brr >= 0 && (next_brr < next_brl || next_brl < 0))
            {
                if (layer == 0)
                {
                    if (strict)
                    {
                        throw new InvalidDataException("Brackets matching failed (no left bracket)");
                    }
                    else
                    {
                        pointer = next_brr + lbrr;
                        continue;
                    }
                }
                else
                {
                    pointer = next_brr + lbrr;
                    layer -= 1;
                    if (layer == 0)
                    {
                        return (outer_brl, next_brr);
                    }
                }
            }
            else if (next_brl < 0 && next_brr < 0)
            {
                return (-1, -1);
            }
            else
            {
                throw new InvalidDataException("what the hell?");
            }
        }
        if (strict)
        {
            throw new InvalidDataException("Brackets matching failed (no right bracket)");
        }
        return (outer_brl, -1);
    }

    public static ImmutableHashSet<T> DefineSet<T>(params T[] p)
    {
        return p.ToImmutableHashSet();
    }

    public static bool HasKeyword(this string strIn, IEnumerable<string> strs)
    {
        foreach (var str in strs)
            if (strIn.Contains(str))
                return true;
        return false;
    }
    public static bool HasKeyword(this string strIn, params string[] strs) => strIn.HasKeyword((IEnumerable<string>)strs);

    public static IEnumerable<(int, T)> GetIndexedEnumerator<T>(this IEnumerable<T> arr)
    {
        return arr.Select((x, i) => (i, x));
    }


    public static int BoolToBinary(IEnumerable<bool> arr)
    {
        var ans = 0;
        foreach (var (i, b) in arr.GetIndexedEnumerator())
            if (b)
                ans |= (1 << i);
        return ans;
    }
    public static int BoolToBinary(params bool[] arr) => BoolToBinary((IEnumerable<bool>)arr);

    public static List<bool> BinaryToBool(int bin, int digit)
    {
        var ans = new List<bool>();
        foreach (var i in Enumerable.Range(0, digit))
        {
            ans.Add((bin & 1 << i) > 0);
        }
        return ans;
    }

    public static string LCP(string? a, string? b)
    {
        a = a ?? string.Empty;
        b = b ?? string.Empty;
        for (int i = 0; i < a.Length; ++i)
        {
            if (b.Length <= i)
                return b;
            if (a[i] != b[i])
                return a.Substring(0, i);
        }
        return a;
    }

    private static string _lcp(in string?[] strs, int l, int r)
    {
        if (r - l < 2)
            return strs[l] ?? string.Empty;
        else if (r - l == 2)
            return LCP(strs[l], strs[l + 1]);
        int m = (r + l) / 2;
        return LCP(_lcp(strs, l, m), _lcp(strs, m, r));
    }

    public static string LCP(params string?[] strs)
    {
        if (strs.Length == 0)
            return string.Empty;
        if (strs.Length == 1)
            return strs[0] ?? string.Empty;
        return _lcp(strs, 0, strs.Length);
    }

    public static string LCP<T>(this IEnumerable<T> strs, Func<T, string?> selector)
    {
        return LCP(strs.Select(selector).ToArray());
    }

    public static string LCP(this IEnumerable<string?> strs)
    {
        return LCP(strs.Select(x => x ?? string.Empty).ToArray());
    }


}