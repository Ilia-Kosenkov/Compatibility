//     MIT License
//     
//     Copyright(c) 2019 Ilia Kosenkov
//     
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of bytes software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and bytes permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.


using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo(@"Tests")]
namespace Compatibility.Bridge.Internal
{
    internal static class LambdaCompositions
    {
        private static readonly Random R = new Random();
        private class PredicateLambdaVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _targetParameter;
            private readonly ParameterExpression _replaceParameter;

            private PredicateLambdaVisitor()
            {
            }

            public PredicateLambdaVisitor(ParameterExpression target, ParameterExpression replaceWith)
            {
                _targetParameter = target ?? throw new ArgumentNullException(nameof(target));
                _replaceParameter = replaceWith ?? throw new ArgumentNullException(nameof(replaceWith));
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Equals(_targetParameter)
                    ? _replaceParameter
                    : node;
            }
            
        }

        private class ComposingLambdaVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _targetParameter;
            private readonly Expression _replaceExpression;

            private ComposingLambdaVisitor() { }

            public ComposingLambdaVisitor(ParameterExpression target, Expression replaceWith)
            {
                _targetParameter = target ?? throw new ArgumentNullException(nameof(target));
                _replaceExpression = replaceWith ?? throw new ArgumentNullException(nameof(replaceWith));
            }

            public override Expression Visit(Expression node)
            {
                return base.Visit(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Equals(_targetParameter)
                    ? _replaceExpression
                    : node;
            }
        }

        public static Expression<Func<T, bool>> ComposePredicatesAnd<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if(first is null)
                throw new ArgumentNullException(nameof(first));
            if(second is null)
                throw new ArgumentNullException(nameof(second));
            
            var param = Expression.Parameter(typeof(T), $"_{R.Next():x}_");

            first = new PredicateLambdaVisitor(first.Parameters[0], param).Visit(first) as Expression<Func<T, bool>> ?? throw new InvalidOperationException(@"Failed to convert lambda.");
            second = new PredicateLambdaVisitor(second.Parameters[0], param).Visit(second) as Expression<Func<T, bool>> ?? throw new InvalidOperationException(@"Failed to convert lambda.");

            var andExpr = Expression.AndAlso(first.Body, second.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(andExpr, param);

            return lambda;
        }

        public static Expression<Func<T1, T3>> Compose<T1, T2, T3>(Expression<Func<T1, T2>> first, Expression<Func<T2, T3>> second)
        {
            if (first is null)
                throw new ArgumentNullException(nameof(first));
            if (second is null)
                throw new ArgumentNullException(nameof(second));

            var body = new ComposingLambdaVisitor(second.Parameters[0], first.Body).Visit(second.Body) ?? throw new InvalidOperationException(@"Failed to convert lambda.");

            var lambda = Expression.Lambda<Func<T1, T3>>(body, first.Parameters[0]);

            return lambda;
        }
    }
}
