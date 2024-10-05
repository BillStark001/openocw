using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Oocw.Query;
public static class Lexer
{

    public struct Token
    {
        public readonly int Type;
        public readonly string[] Content;

        public Token(int type, IEnumerable<string>? tokens)
        {
            Type = type;
            Content = tokens != null && tokens.Count() > 0 ? tokens.ToArray() : Array.Empty<string>();
        }

        public override string ToString()
        {
            return $"Token({Type})[{string.Join('|', Content)}]";
        }

        public override int GetHashCode()
        {
            var ret = Type;
            foreach (var token in Content)
            {
                ret = HashCode.Combine(ret, token.GetHashCode());
            }
            return ret;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || obj is not Token)
                return false;

            var lex = (Token)obj;
            if (lex.Type != Type || lex.Content.Length != Content.Length)
                return false;

            for (int i = 0; i < Content.Length; ++i)
                if (Content[i] != lex.Content[i])
                    return false;
            return true;
        }
    }

    // ctor etc.
    static Lexer()
    {

    }

    // definitions of literals

    public const int TOKEN_UNKNOWN = 0x7fffffff;

    // basic

    public const int TSPACE_VAL = 0;

    public const int TOKEN_VAL_BOOL = 0;
    public const int TOKEN_VAL_INT = 1;
    public const int TOKEN_VAL_FLOAT = 2;

    public const int TOKEN_VAL_STR = 8;
    public const int TOKEN_VAL_STR2 = 9;

    // variables and var operators

    public const int TSPACE_VAR = 16;

    public const int TOKEN_VAR = 16;
    public const int TOKEN_VAROPR = 17;

    // structure operators

    public const int TSPACE_STRUCT = 32;

    public const int TOKEN_COMMA = 32;
    public const int TOKEN_DOT = 33;
    public const int TOKEN_SPACE = 34;

    public const int TOKEN_BRACKET_SS = 36;
    public const int TOKEN_BRACKET_SE = 37;
    public const int TOKEN_BRACKET_MS = 38;
    public const int TOKEN_BRACKET_ME = 39;
    public const int TOKEN_BRACKET_LS = 40;
    public const int TOKEN_BRACKET_LE = 41;

    // logical operators

    public const int TSPACE_OPR = 48;

    public const int TOKEN_OPR_L_AND = 48;
    public const int TOKEN_OPR_L_OR = 49;
    public const int TOKEN_OPR_L_XOR = 50;
    public const int TOKEN_OPR_L_NOT = 51;

    // algebra(?) operators

    public const int TOKEN_OPR_EQ = 64;
    public const int TOKEN_OPR_NEQ = 65;

    public const int TOKEN_OPR_ADD = 72;
    public const int TOKEN_OPR_MINUS = 73;
    public const int TOKEN_OPR_MULT = 74;
    public const int TOKEN_OPR_DIV = 75;
    public const int TOKEN_OPR_MOD = 76;

    public const int TOKEN_OPR_A_AND = 80;
    public const int TOKEN_OPR_A_OR = 81;
    public const int TOKEN_OPR_A_XOR = 82;
    public const int TOKEN_OPR_A_NOT = 83;

    // set operators
    // do not be larger than 64!

    public const int TOKEN_OPR_S_INCL = 56;
    public const int TOKEN_OPR_S_INCL_BY = 57;
    public const int TOKEN_OPR_S_SUBSET = 58;
    public const int TOKEN_OPR_S_SUBSETEQ = 59;
    public const int TOKEN_OPR_S_SUBSET_INV = 60;
    public const int TOKEN_OPR_S_SUBSETEQ_INV = 61;

    public static IEnumerable<(int, Regex)> Tokens = new List<(int, Regex)>()
    {
        (TOKEN_COMMA, new Regex(@"\G\,")),
        (TOKEN_DOT, new Regex(@"\G\.")),
        (TOKEN_SPACE, new Regex(@"\G +")),

        (TOKEN_VAL_BOOL, new Regex(@"\G(true|false)")),
        (TOKEN_VAL_INT, new Regex(@"\G([0-9]+)")),
        (TOKEN_VAL_FLOAT, new Regex(@"\G((?:(?:[0-9]*\.[0-9]+|[0-9]+\.[0-9]*)(?:[Ee][+-]?[0-9]+)?)|(?:[0-9]+(?:[Ee][+-]?[0-9]+)?))")), 
        (TOKEN_VAL_STR, new Regex(@"\G('(?:\\.|[^'\\])*')")), 
        (TOKEN_VAL_STR2, new Regex(@"\G(""(?:\\.|[^""\\])*"")")), 

        (TOKEN_VAR, new Regex(@"\G([a-zA-Z_$][0-9a-zA-Z_$]*)")), 
        (TOKEN_VAROPR, new Regex(@"\G\\([a-zA-Z_$][0-9a-zA-Z_$]*)")), 

        (TOKEN_BRACKET_SS, new Regex(@"\G\(")),
        (TOKEN_BRACKET_SE, new Regex(@"\G\)")),
        (TOKEN_BRACKET_MS, new Regex(@"\G\[")),
        (TOKEN_BRACKET_ME, new Regex(@"\G\]")),
        (TOKEN_BRACKET_LS, new Regex(@"\G\{")),
        (TOKEN_BRACKET_LE, new Regex(@"\G\}")),

        (TOKEN_OPR_L_AND, new Regex(@"\G&&")),
        (TOKEN_OPR_L_OR, new Regex(@"\G\|\|")),
        (TOKEN_OPR_L_XOR, new Regex(@"\G\^\^")),
        (TOKEN_OPR_L_NOT, new Regex(@"\G!")),

        (TOKEN_OPR_A_AND, new Regex(@"\G&")),
        (TOKEN_OPR_A_OR, new Regex(@"\G\|")),
        (TOKEN_OPR_A_XOR, new Regex(@"\G\^")),
        (TOKEN_OPR_A_NOT, new Regex(@"\G~")),

        (TOKEN_OPR_EQ, new Regex(@"\G==")), 
        (TOKEN_OPR_NEQ, new Regex(@"\G!=")), 
        (TOKEN_OPR_ADD, new Regex(@"\G\+")), 
        (TOKEN_OPR_MINUS, new Regex(@"\G\-")), 
        (TOKEN_OPR_MULT, new Regex(@"\G\*")), 
        (TOKEN_OPR_DIV, new Regex(@"\G/")), 
        (TOKEN_OPR_MOD, new Regex(@"\G%")), 

        (TOKEN_OPR_S_INCL, new Regex(@"\G#>")), 
        (TOKEN_OPR_S_INCL_BY, new Regex(@"\G#<")), 
        (TOKEN_OPR_S_SUBSET, new Regex(@"\G\$<")), 
        (TOKEN_OPR_S_SUBSET_INV, new Regex(@"\G\$>")),
        (TOKEN_OPR_S_SUBSETEQ, new Regex(@"\G\$<=")),
        (TOKEN_OPR_S_SUBSETEQ_INV, new Regex(@"\G\$=>")),
    }.AsReadOnly();

    public static readonly Regex REG_TOKEN_UNKNOWN = new Regex(@"\G([^ ]+)");

    public static IEnumerable<Token> NaiveMatch(string strIn)
    {
        List<Token> result = [];
        int index = 0;
        Match m;
        while (index < strIn.Length)
        {
            var matched = false;
            foreach (var (type, regex) in Tokens)
            {
                m = regex.Match(strIn, index);
                if (m.Success)
                {
                    matched = true;
                    result.Add(new(type, m.Groups.Values.Skip(1).Select(x => x.Value)));
                    index += m.Length;
                    continue;
                }
            }
            if (!matched)
            {
                m = REG_TOKEN_UNKNOWN.Match(strIn, index);
                if (m.Success)
                {
                    matched = true;
                    result.Add(new(TOKEN_UNKNOWN, m.Groups.Values.Skip(1).Select(x => x.Value)));
                    index += m.Length;
                }
                else
                    throw new NotImplementedException("this shouldn't happen");
            }
        }


        return result;
    }

}