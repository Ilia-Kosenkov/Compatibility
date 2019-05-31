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
using Compatibility.Bridge.Internal;

namespace Compatibility.Bridge
{
    internal abstract class Promise<TTar>
    {
        public abstract Maybe<TTar> Evaluate();

        public abstract Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector);

        public abstract Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate);
    }

    internal class ResolvedPromise<TTar> : Promise<TTar>
    {
        private readonly Maybe<TTar> _value;

        public ResolvedPromise(Maybe<TTar> value)
        {
            _value = value ?? Maybe<TTar>.None;
        }

        public override Maybe<TTar> Evaluate()
            => _value;

        public override Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector)
            => new Promise<TTar, TNew>(this, selector);

        public override Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate)
            => new ConditionalPromise<TTar>(this, predicate);
    }

    internal class Promise<TSrc, TTar> : Promise<TTar>
    {
        private readonly Promise<TSrc> _source;
        private readonly Expression<Func<TSrc, TTar>> _evaluator;
        private Maybe<TTar> _cached;
        private bool _wasEvaluated;

        public Promise(Promise<TSrc> source, Expression<Func<TSrc, TTar>> evaluator)
        {
            _source = source;
            _evaluator = evaluator;
        }

        public override Maybe<TTar> Evaluate()
        {
            if (_wasEvaluated)
                return _cached;

            _cached = _source.Evaluate().Select(_evaluator.Compile());
            _wasEvaluated = true;

            return _cached;
        }

        public override Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _wasEvaluated
                ? (Promise<TNew>) new Promise<TTar, TNew>(new ResolvedPromise<TTar>(_cached), selector)
                : new Promise<TSrc, TNew>(_source, LambdaCompositions.Compose(_evaluator, selector));
        }

        public override Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate)
        {
            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return new ConditionalPromise<TTar>(
                _wasEvaluated ? (Promise<TTar>) new ResolvedPromise<TTar>(_cached) : this, predicate);
        }
    }

    internal class ConditionalPromise<T> : Promise<T>
    {
        private readonly Promise<T> _source;
        private readonly Expression<Func<T, bool>> _predicate;
        private Maybe<T> _cached;
        private bool _wasEvaluated;

        public ConditionalPromise(Promise<T> source, Expression<Func<T, bool>> predicate)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public override Maybe<T> Evaluate()
        {
            if (_wasEvaluated)
                return _cached;

            _cached = _source.Evaluate().Where(_predicate.Compile());
            _wasEvaluated = true;

            return _cached;
        }

        public override Promise<TNew> CreateNew<TNew>(Expression<Func<T, TNew>> selector)
        {
            if(selector is null)
                throw new ArgumentNullException(nameof(selector));

            return new Promise<T, TNew>(_wasEvaluated ? (Promise<T>) new ResolvedPromise<T>(_cached) : this, selector);
        }

        public override Promise<T> CreateConditional(Expression<Func<T, bool>> predicate)
        {
            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return _wasEvaluated
                ? new ConditionalPromise<T>(new ResolvedPromise<T>(_cached), predicate)
                : new ConditionalPromise<T>(_source, LambdaCompositions.ComposePredicatesAnd(_predicate, predicate));
        }
    }

}
