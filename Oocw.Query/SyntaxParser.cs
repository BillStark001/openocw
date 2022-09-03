using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using L = Oocw.Query.Lexer;

namespace Oocw.Query;

public static class SyntaxParser
{
    public class SyntaxNode
    {
        public int Type { get; set; }
        public virtual IEnumerable<SyntaxNode> Nodes { get; set; } = Enumerable.Empty<SyntaxNode>();
    }

    public class TokenNode: SyntaxNode
    {
        public override IEnumerable<SyntaxNode> Nodes => Enumerable.Empty<SyntaxNode>();
        public int TokenType { get; set; }
    }

    public static int I = 65536;

    // symbols

    public static int SMBL_EXPR = I + 2;

    public static int SMBL_EXPR_SET2 = I + 4;
    public static int SMBL_EXPR_OOP = I + 5;
    public static int SMBL_EXPR_MONO = I + 6;
    public static int SMBL_EXPR_BIT = I + 7;
    public static int SMBL_EXPR_ALG1 = I + 8;
    public static int SMBL_EXPR_ALG2 = I + 8;
    public static int SMBL_EXPR_ALG3 = I + 8;
    public static int SMBL_EXPR_SET = I + 9;
    public static int SMBL_EXPR_LOGIC1 = I + 10;
    public static int SMBL_EXPR_LOGIC2 = I + 10;
    public static int SMBL_EXPR_MERGED = I + 11;


    // end symbols

    public static int SMBL_VAR = SMBL_EXPR + 32;
    public static int SMBL_OPR = SMBL_EXPR + 33;


    // syntax definition
    public static IEnumerable<(int, int[])> SyntaxDefinitions;

    static SyntaxParser()
    {
        var def = new List<(int, int[])>();

        // M -> M,E | E,E
        def.Add(D(SMBL_EXPR_MERGED, SMBL_EXPR_MERGED, L.TOKEN_COMMA, SMBL_EXPR));
        def.Add(D(SMBL_EXPR_MERGED, SMBL_EXPR, L.TOKEN_COMMA, SMBL_EXPR));

        // S2 -> {M} | {M,} | {E} | {E,} | {} | E
        def.Add(D(SMBL_EXPR_SET2, L.TOKEN_BRACKET_LS, SMBL_EXPR_MERGED, L.TOKEN_BRACKET_LE));
        def.Add(D(SMBL_EXPR_SET2, L.TOKEN_BRACKET_LS, SMBL_EXPR_MERGED, L.TOKEN_COMMA, L.TOKEN_BRACKET_LE));
        def.Add(D(SMBL_EXPR_SET2, L.TOKEN_BRACKET_LS, SMBL_EXPR, L.TOKEN_BRACKET_LE));
        def.Add(D(SMBL_EXPR_SET2, L.TOKEN_BRACKET_LS, SMBL_EXPR, L.TOKEN_COMMA, L.TOKEN_BRACKET_LE));
        def.Add(D(SMBL_EXPR_SET2, L.TOKEN_BRACKET_LS, L.TOKEN_BRACKET_LE));
        def.Add(D(SMBL_EXPR_SET2, SMBL_EXPR));

        // O -> O.S2 | O(S2) | O(M) | O[S2] | O[M] | S2
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_OOP, L.TOKEN_DOT, SMBL_EXPR_SET2));
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_OOP, L.TOKEN_BRACKET_SS, SMBL_EXPR_SET2, L.TOKEN_BRACKET_SE));
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_OOP, L.TOKEN_BRACKET_SS, SMBL_EXPR_MERGED, L.TOKEN_BRACKET_SE));
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_OOP, L.TOKEN_BRACKET_MS, SMBL_EXPR_SET2, L.TOKEN_BRACKET_ME));
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_OOP, L.TOKEN_BRACKET_MS, SMBL_EXPR_MERGED, L.TOKEN_BRACKET_ME));
        def.Add(D(SMBL_EXPR_OOP, SMBL_EXPR_SET2));

        // MO -> !MO | ~MO | O
        // TODO add unary negative sign?
        def.Add(D(SMBL_EXPR_MONO, L.TOKEN_OPR_L_NOT, SMBL_EXPR_MONO));
        def.Add(D(SMBL_EXPR_MONO, L.TOKEN_OPR_A_NOT, SMBL_EXPR_MONO));
        def.Add(D(SMBL_EXPR_MONO, SMBL_EXPR_OOP));

        // B -> B&MO | B|MO | B^MO | MO
        foreach (var T in new int[] { L.TOKEN_OPR_A_AND, L.TOKEN_OPR_A_OR, L.TOKEN_OPR_A_XOR, })
            def.Add(D(SMBL_EXPR_BIT, SMBL_EXPR_BIT, T, SMBL_EXPR_MONO));
        def.Add(D(SMBL_EXPR_BIT, SMBL_EXPR_MONO));

        // A1 -> A1%B | B
        def.Add(D(SMBL_EXPR_ALG1, SMBL_EXPR_ALG1, L.TOKEN_OPR_MOD, SMBL_EXPR_BIT));
        def.Add(D(SMBL_EXPR_ALG1, SMBL_EXPR_BIT));

        // A2 -> A2*A1 | A2/A1 | A1
        def.Add(D(SMBL_EXPR_ALG2, SMBL_EXPR_ALG2, L.TOKEN_OPR_MULT, SMBL_EXPR_ALG1));
        def.Add(D(SMBL_EXPR_ALG2, SMBL_EXPR_ALG2, L.TOKEN_OPR_DIV, SMBL_EXPR_ALG1));
        def.Add(D(SMBL_EXPR_ALG2, SMBL_EXPR_ALG1));

        // A3 -> A3+A2 | A3-A2 | A2
        def.Add(D(SMBL_EXPR_ALG3, SMBL_EXPR_ALG3, L.TOKEN_OPR_ADD, SMBL_EXPR_ALG2));
        def.Add(D(SMBL_EXPR_ALG3, SMBL_EXPR_ALG3, L.TOKEN_OPR_MINUS, SMBL_EXPR_ALG2));
        def.Add(D(SMBL_EXPR_ALG3, SMBL_EXPR_ALG2));

        // S -> S#>A3 | S#<A3 | S$<A3 | S$>A3 | S$<=A3 | S$=>A3 | A3
        foreach (var T in new int[] {
            L.TOKEN_OPR_S_INCL, L.TOKEN_OPR_S_INCL_BY,
            L.TOKEN_OPR_S_SUBSET, L.TOKEN_OPR_S_SUBSET_INV,
            L.TOKEN_OPR_S_SUBSETEQ, L.TOKEN_OPR_S_SUBSETEQ_INV,
        })
            def.Add(D(SMBL_EXPR_SET, SMBL_EXPR_SET, T, SMBL_EXPR_ALG3));
        def.Add(D(SMBL_EXPR_SET, SMBL_EXPR_ALG3));

        // L1 -> L1==S | L1!=S | S
        foreach (var T in new int[] { L.TOKEN_OPR_EQ, L.TOKEN_OPR_NEQ, })
            def.Add(D(SMBL_EXPR_LOGIC1, SMBL_EXPR_LOGIC1, T, SMBL_EXPR_SET));
        def.Add(D(SMBL_EXPR_LOGIC1, SMBL_EXPR_SET));

        // L2 -> L2&&L1 | L2||L1 | L2^^L1 | L1
        foreach (var T in new int[] { L.TOKEN_OPR_L_AND, L.TOKEN_OPR_L_OR, L.TOKEN_OPR_L_XOR, })
            def.Add(D(SMBL_EXPR_LOGIC2, SMBL_EXPR_LOGIC2, T, SMBL_EXPR_LOGIC1));
        def.Add(D(SMBL_EXPR_LOGIC2, SMBL_EXPR_LOGIC1));

        // E -> (L2) | v | v'
        def.Add(D(SMBL_EXPR, L.TOKEN_BRACKET_SS, SMBL_EXPR_LOGIC2, L.TOKEN_BRACKET_SE));
        foreach (var T in new int[]
        {
            L.TOKEN_VAR, L.TOKEN_VAL_BOOL, L.TOKEN_VAL_INT, L.TOKEN_VAL_FLOAT, L.TOKEN_VAL_STR, L.TOKEN_VAL_STR2,
        })
            def.Add(D(SMBL_EXPR, T));


        SyntaxDefinitions = def.AsReadOnly();
    }

    private static (int, int[]) D(int target, params int[] src)
    {
        return (target, src);
    }
}
