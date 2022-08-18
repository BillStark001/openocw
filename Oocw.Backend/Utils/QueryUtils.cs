using System;
using System.Text.RegularExpressions;

using MongoDB.Bson;
using MongoDB.Driver;

using Oocw.Backend.Database;

namespace Oocw.Backend.Utils
{
    public static class QueryUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inStr"></param>
        public static string FormSearchKeyWords(string inStr)
        {
            // TODO add support of quoted string
            string tokens = string.Join(" ", TokenUtils.TokenizeJapanese(inStr));
            return tokens;
        }

        public static string? TryGetTranslation(this object dict, string lang = "ja")
        {
            if (dict is BsonDocument)
                dict = ((BsonDocument)dict).ToDictionary();
            var d = (Dictionary<string, object>)dict;
            return (string?) d.GetValueOrDefault(lang);
        }
    }
}
