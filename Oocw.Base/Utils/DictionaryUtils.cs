using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MatchKeyAttribute : Attribute
{
    public string Key { get; set; }

    public MatchKeyAttribute(string key)
    {
        Key = key;
    }
}

public static class DictionaryUtils
{
    public static void AssignFrom(this object obj, Dictionary<string, string> dict)
    {
        foreach (var prop in obj.GetType().GetProperties())
        {
            foreach (var attr in prop.GetCustomAttributes(typeof(MatchKeyAttribute), true)
                .Where(x => x != null)
                .Select(x => (MatchKeyAttribute) x)
                .Where(x => x.Key != null) // intentional
                )
            {
                if (dict.TryGetValue(attr.Key, out var value))
                {
                    prop.SetValue(obj, value);
                }
            }
        }
    }
}
