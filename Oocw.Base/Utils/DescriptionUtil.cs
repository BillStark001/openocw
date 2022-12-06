using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;

public static class DescriptionUtil
{
    private static readonly MethodInfo DictDescribe = null!;
    private static readonly MethodInfo ListDescribe = null!;
    private static readonly Type GenericDict;
    private static readonly Type GenericList;

    static DescriptionUtil()
    {

        foreach (var method in typeof(DescriptionUtil).GetMethods())
        {
            if (method.Name == "Describe")
            {
                var args = method.GetGenericArguments();
                if (args.Length == 2)
                    DictDescribe = method;
                else if (args.Length == 1)
                    ListDescribe = method;
            }
        }

        GenericDict = typeof(IDictionary<object, object>).GetGenericTypeDefinition()!;
        GenericList = typeof(IEnumerable<object>).GetGenericTypeDefinition()!;

    }

    public static string Describe<TKey, TValue>(this IDictionary<TKey, TValue> dict)
    {
        StringBuilder sb = new();
        sb.Append("Dict { ");
        var first = true;
        foreach (var kv in dict)
        {
            if (first)
                first = false;
            else
                sb.Append(", ");

            sb.Append('[');
            sb.Append(kv.Key.Describe());
            sb.Append("]=");
            sb.Append(
                kv.Value is IDictionary<TKey, TValue> d ?
                d.Describe() :
                kv.Value.Describe()
                );
        }
        sb.Append(" }");
        return sb.ToString();
    }

    public static string Describe<T>(this IEnumerable<T> list)
    {
        StringBuilder sb = new();
        sb.Append("List { ");
        var first = true;
        foreach (var kv in list)
        {
            if (first)
                first = false;
            else
                sb.Append(", ");

            sb.Append(
                kv is IEnumerable<T> kvl ?
                kvl.Describe() :
                kv.Describe()
                );
        }
        sb.Append(" }");
        return sb.ToString();
    }

    public static string Describe(this byte[] bytes, int sep = 0, bool fullHex = false)
    {
        StringBuilder sb = new();
        sb.Append("B[");
        sb.Append(bytes.Length);
        sb.Append("]:");
        for (int i = 0; i < bytes.Length; ++i)
        {
            if (sep > 0 && i % sep == 0)
                sb.Append(' ');

            var b = bytes[i];
            if (!fullHex && b > 0x20 && b < 0x7f)
                sb.Append((char)b);
            else if (!fullHex && b == 0x20 && sep <= 0)
                sb.Append((char)b);
            else
            {
                sb.Append("\\x");
                sb.Append(b.ToString("x2"));
            }
        }
        if (sep <= 0 && bytes.Last() == 0x20)
            sb.Append("<END>");
        return sb.ToString();
    }


    public static string Describe(this string? str)
    {
        if (str == null)
            str = "[null str]";
        else if (string.IsNullOrWhiteSpace(str))
            str = $"[space({str.Length})]";
        return str;
    }

    public static string Describe(this object? obj)
    {
        if (obj == null)
            return "[null]";
        else if (obj is byte[] bytes)
            return Describe(bytes);
        else if (obj is string str1)
            return Describe(str1);

        var enumerable = obj as System.Collections.IEnumerable;
        if (enumerable != null)
        {
            var enumType = enumerable.GetType();
            if (enumType.GenericTypeArguments.Length == 2 &&
                typeof(System.Collections.IDictionary).IsAssignableFrom(enumType))
            {
                return (string)DictDescribe.MakeGenericMethod(enumType.GenericTypeArguments).Invoke(null, new object[] { enumerable })!;
            }
            else if (enumType.GenericTypeArguments.Length == 1)
            {
                return (string)ListDescribe.MakeGenericMethod(enumType.GenericTypeArguments).Invoke(null, new object[] { enumerable })!;
            }
        }

        return Describe(obj.ToString());
    }
}