using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Base;


/// <summary>
/// Reference: https://social.msdn.microsoft.com/Forums/en-US/473ade84-66aa-45e0-822f-e83744188bc8/combine-2-expressions-using-expression-api?forum=csharpgeneral
/// </summary>
public static class ExpressionUtils
{

    public class ReplaceVisitor : ExpressionVisitor
    {
        Expression _left;
        Expression _right;
        public ReplaceVisitor(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }
        public override Expression? Visit(Expression? node)
        {
            if (node != null && node.Equals(_left))
            {
                return _right;
            }

            return base.Visit(node);
        }
    }


    public static Expression Replace(Expression main, Expression current, Expression replacement)
    {
        return new ReplaceVisitor(current, replacement).Visit(main)!;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="f">Expression like x => f(x)</param>
    /// <param name="g">Expression like x => g(x)</param>
    /// <returns>Expression line x => f(g(x))</returns>

    public static Expression<Func<T, R>> Combine<T, U, R>(this Expression<Func<T, U>> f, Expression<Func<U, R>> g)
    {
        var expressionBody = Replace(g.Body, g.Parameters[0], f.Body);
        var resultExpression = Expression.Lambda<Func<T, R>>(expressionBody, f.Parameters[0]);
        return resultExpression;
    }


    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="f">Expression like x => f(x)</param>
    /// <param name="g">Expression like x => g(x)</param>
    /// <returns>Expression line x => f(g(x))</returns>
    public static Expression<Func<T, R>> SlowCombine<T, U, R>(this Expression<Func<T, U>> f, Expression<Func<U, R>> g)
    {
        var input = Expression.Variable(typeof(T), "input");
        var invoke1 = Expression.Invoke(f, input);
        Expression invoke2 = Expression.Invoke(g, invoke1);
        if (invoke2.CanReduce)
            invoke2 = invoke2.Reduce();

        return Expression.Lambda<Func<T, R>>(invoke2, input);
    }
}
