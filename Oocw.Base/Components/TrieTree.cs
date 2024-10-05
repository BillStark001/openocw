using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;

public class TrieNode<T> where T: notnull
{

    public readonly Dictionary<T, TrieNode<T>> Children;

    public readonly T? Character;

    public int CountEnd;

    public int CountPref;

    public TrieNode(T? word = default(T), int cend = 0, int cpref = 0)
    {
        Character = word;
        CountEnd = cend;
        CountPref = cpref;
        Children = [];
    }
}


public class TrieTree<T> where T: notnull
{

    public TrieNode<T> Root { get; private set; }

    
    public TrieTree()
    {
        Root = new(default(T));
    }

    public virtual TrieTree<T> AddWord(IEnumerable<T> word)
    {
        var curNode = Root;
        int i = 0;
        foreach (var chrc in word)
        {
            var cadd = (i == word.Count() - 1) ? 1 : 0;
            if (!curNode.Children.ContainsKey(chrc))
            {
                var new_node = new TrieNode<T>(chrc, cadd, 1);
                curNode.Children[chrc] = new_node;
                curNode = new_node;
            }
            else
            {
                curNode = curNode.Children[chrc];
                curNode.CountEnd += cadd;
                curNode.CountPref += 1;
            }
            ++i;
        }
        return this;
    }

    public virtual TrieTree<T> AddWords(params IEnumerable<T>[] words)
    {
        foreach (var w in words)
        {
            AddWord(w);
        }
        return this;
    }

    public virtual IList<T> ScanCommonWord(bool ignoreConflict = false)
    {
        var ret = new List<T>();
        var curNode = Root;
        while (true)
        {
            var child = curNode.Children.Values.OrderBy(x => (-x.CountPref, x.Character)).ToList();
            if (child.Count == 0 || child.Count > 1 && child[0].CountPref == child[1].CountPref && !ignoreConflict)
            {
                break;
            }
            //print([x.chrc for x in chld])
            if (child[0].Character != null)
                ret.Add(child[0].Character!);
            curNode = child[0];
        }
        return ret.AsReadOnly();
    }
}

public class StringTrieTree : TrieTree<char>
{
    public new StringTrieTree AddWord(IEnumerable<char> w) { base.AddWord(w); return this; }
    public new StringTrieTree AddWords(params IEnumerable<char>[] w) { base.AddWords(w); return this; }

    public new string ScanCommonWord(bool ignoreConflict = false)
    {
        return new(base.ScanCommonWord(ignoreConflict).ToArray());
    }

    public static string ScanCommonWord(bool ignoreConflict = false, params string[] words)
    {
        if (words.Length == 0)
            return "";
        if (words.Length == 1)
            return words[0];
        return new StringTrieTree().AddWords(words).ScanCommonWord(ignoreConflict);
    }
}
