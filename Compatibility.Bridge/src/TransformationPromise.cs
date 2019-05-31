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
using System.Threading.Tasks;
using Compatibility.Bridge.Internal;

namespace Compatibility.Bridge
{
    internal interface IPromiseAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }
        Maybe<T> GetResult();
    }

    internal abstract class Promise<TTar>
    {
        public abstract Maybe<TTar> Evaluate();

        public abstract Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector);

        public abstract Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate);

        public abstract IPromiseAwaiter<TTar> GetAwaiter();
    }

    internal class ResolvedPromise<TTar> : Promise<TTar>
    {
        internal struct Awaiter : IPromiseAwaiter<TTar>
        {
            private readonly Maybe<TTar> _result;

            public Awaiter(Maybe<TTar> value) => _result = value;

            public bool IsCompleted => true;

            public Maybe<TTar> GetResult() => _result;

            public void OnCompleted(Action continuation)
                => (continuation ?? throw new ArgumentNullException(nameof(continuation)))();
        }

        private readonly Maybe<TTar> _value;

        public ResolvedPromise(Maybe<TTar> value)
        {
            _value = value ?? Maybe<TTar>.None;
        }

        public override Maybe<TTar> Evaluate()
            => _value;

        public override Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector)
            => new TransformationPromise<TTar, TNew>(this, selector);

        public override Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate)
            => new ConditionPromise<TTar>(this, predicate);

        public override IPromiseAwaiter<TTar> GetAwaiter() => new Awaiter(_value);
    }

    internal class TransformationPromise<TSrc, TTar> : Promise<TTar>
    {
        internal struct Awaiter : IPromiseAwaiter<TTar>
        {
            private readonly TransformationPromise<TSrc, TTar> _promise;
            private IPromiseAwaiter<TSrc> _srcAwaiter;

            public bool IsCompleted
            {
                get
                {
                    if (_promise._wasEvaluated)
                        return true;
                    _srcAwaiter = _srcAwaiter ?? _promise._source.GetAwaiter();
                    return _srcAwaiter.IsCompleted;
                }
            }

            public Awaiter(TransformationPromise<TSrc, TTar> source)
            {
                _promise = source ?? throw new ArgumentNullException(nameof(source));
                _srcAwaiter = null;
            }

            public Maybe<TTar> GetResult()
            {
                return _promise.Evaluate();
            }

            public void OnCompleted(Action continuation)
            {
                if(continuation is null)
                    throw new ArgumentNullException(nameof(continuation));

                if (IsCompleted)
                    continuation();
                else
                    _srcAwaiter.OnCompleted(continuation);

            }
        }


        private readonly Promise<TSrc> _source;
        private readonly Expression<Func<TSrc, TTar>> _evaluator;
        private Maybe<TTar> _cached;
        private bool _wasEvaluated;

        public TransformationPromise(Promise<TSrc> source, Expression<Func<TSrc, TTar>> evaluator)
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
                ? (Promise<TNew>) new TransformationPromise<TTar, TNew>(new ResolvedPromise<TTar>(_cached), selector)
                : new TransformationPromise<TSrc, TNew>(_source, LambdaCompositions.Compose(_evaluator, selector));
        }

        public override Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate)
        {
            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return new ConditionPromise<TTar>(
                _wasEvaluated ? (Promise<TTar>) new ResolvedPromise<TTar>(_cached) : this, predicate);
        }

        public override IPromiseAwaiter<TTar> GetAwaiter() => new Awaiter(this);
    }

    internal class ConditionPromise<T> : Promise<T>
    {
        private readonly Promise<T> _source;
        private readonly Expression<Func<T, bool>> _predicate;
        private Maybe<T> _cached;
        private bool _wasEvaluated;

        public ConditionPromise(Promise<T> source, Expression<Func<T, bool>> predicate)
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

            return new TransformationPromise<T, TNew>(_wasEvaluated ? (Promise<T>) new ResolvedPromise<T>(_cached) : this, selector);
        }

        public override Promise<T> CreateConditional(Expression<Func<T, bool>> predicate)
        {
            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return _wasEvaluated
                ? new ConditionPromise<T>(new ResolvedPromise<T>(_cached), predicate)
                : new ConditionPromise<T>(_source, LambdaCompositions.ComposePredicatesAnd(_predicate, predicate));
        }

        public override IPromiseAwaiter<T> GetAwaiter()
        {
            throw new NotImplementedException();
        }
    }

    internal class AsyncPromise<TTar> : Promise<TTar>
    {
        internal struct Awaiter : IPromiseAwaiter<TTar>
        {
            private TaskAwaiter<Maybe<TTar>> _awaiter;
            public bool IsCompleted => _awaiter.IsCompleted;

            public Awaiter(Task<Maybe<TTar>> source) =>
                _awaiter = source?.GetAwaiter() ?? throw new ArgumentNullException(nameof(source));


            public Maybe<TTar> GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action continuation)
                => _awaiter.OnCompleted(continuation ?? throw new ArgumentNullException(nameof(continuation)));
        }

        private readonly Task<Maybe<TTar>> _source;
        private AsyncPromise() { }

        public AsyncPromise(Task<Maybe<TTar>> task)
        {
            _source = task ?? throw new ArgumentNullException(nameof(task));
        }

        public override Promise<TNew> CreateNew<TNew>(Expression<Func<TTar, TNew>> selector)
            => new TransformationPromise<TTar, TNew>(this, selector);

        public override Promise<TTar> CreateConditional(Expression<Func<TTar, bool>> predicate)
            => new ConditionPromise<TTar>(this, predicate);

        public override Maybe<TTar> Evaluate()
        {
            return (Maybe<TTar>)_source.GetAwaiter().GetResult();
        }

        public override IPromiseAwaiter<TTar> GetAwaiter() => new Awaiter(_source);

    }

    
}
