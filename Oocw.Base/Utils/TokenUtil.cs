using MeCab;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Immutable;
using Oocw.Base.Properties;
using System.Text.RegularExpressions;

namespace Oocw.Base;

public static class TokenUtil
{
    private static readonly MeCabTagger Tagger;
    private static readonly ImmutableHashSet<string> Stopwords;

    private static readonly Regex LineReturn = new(@"\r?\n");

    static TokenUtil()
    {
        var param = new MeCabParam();
        Tagger = MeCabTagger.Create(param);
        
        var stopwords = new HashSet<string>();
        foreach (var file in new string[] {
            Resources.stopwords_py,
            Resources.stopwords_ja,
            Resources.stopwords_en,
            Resources.stopwords_zh,
        })
        {
            foreach (var word in LineReturn.Split(file))
                if (!string.IsNullOrWhiteSpace(word))
                    stopwords.Add(word.Trim());
        }
        Stopwords = stopwords.ToImmutableHashSet();
    }

    public static IEnumerable<string> TokenizeJapanese(string inStr)
    {
        Dictionary<string, bool> tokens = new();
       
        foreach (var node in Tagger.ParseToNodes(inStr))
        {
            if (node.Feature == null)
                continue;
            var word = node.Surface;
            var features = node.Feature.Split(",");
            var wtype = features.First();
            var orig = features.SkipLast(2).Last();
            if (wtype.Contains("記号"))
                continue;
            if (Stopwords.Contains(word) || Stopwords.Contains(orig))
                continue;
            tokens[word] = true;
            tokens[orig] = true;
        }
        return tokens.Keys;
    }

}
