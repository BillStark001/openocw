using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;

public class NestedDictionary<K, V> : Dictionary<K, NestedDictionary<K, V>> where K : notnull
{

    private V _value = default(V)!;
    public V Value { get { return _value; } set { _value = value; ValueSet = true; } }
    public bool ValueSet { get; private set; } = false;
    public bool SubValueSet { get; private set; } = false;

    public bool DeleteValue()
    {
        var ret = ValueSet;
        ValueSet = false;
        _value = default(V)!;
        return ret;
    }

    public new void Clear()
    {
        base.Clear();
        SubValueSet = false;
    }


    public new NestedDictionary<K, V> this[K key]
    {
        set
        {
            base[key] = value;
            SubValueSet = true;
        }

        get
        {
            if (!Keys.Contains(key))
            {
                base[key] = [];
            }
            return base[key];
        }
    }

}

public class NestedDictionaryConverter<K, V> : JsonConverter<NestedDictionary<K, V>> where K : notnull
{
    public override NestedDictionary<K, V>? ReadJson(JsonReader reader, Type objectType, NestedDictionary<K, V>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {

        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, NestedDictionary<K, V>? value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
        {
            if (value.SubValueSet)
            {
                JObject obj = [];
                foreach (var (k, v) in value)
                    obj.Add(new JProperty(k.ToString() ?? "", v));
                if (value.ValueSet)
                    obj.Add(new JProperty("!VALUE", value.Value));
                writer.WriteValue(obj);
            }
            else if (value.ValueSet)
                writer.WriteValue(value.Value);
            else
                writer.WriteNull();
        }
        throw new NotImplementedException();
    }
}
