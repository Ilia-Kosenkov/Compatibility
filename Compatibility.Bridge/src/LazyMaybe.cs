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

namespace Compatibility.Bridge
{
    public sealed class LazyMaybe<T>
    {
        public struct Awaiter : INotifyCompletion
        {
            private readonly IPromiseAwaiter<T> _awaiter;
            public bool IsCompleted => _awaiter.IsCompleted;

            internal Awaiter(Promise<T> source)
            {
                _awaiter = source?.GetAwaiter() ?? throw new ArgumentNullException(nameof(source));
            }

            public Maybe<T> GetResult() => _awaiter.GetResult();

            public void OnCompleted(Action continuation)
                => _awaiter.OnCompleted(continuation ?? throw new ArgumentNullException(nameof(continuation)));
        }

        private readonly Promise<T> _promise;

        public static LazyMaybe<T> None { get; } = new LazyMaybe<T>();

        public LazyMaybe() => _promise = new ResolvedPromise<T>(Maybe<T>.None);

        public LazyMaybe(T value)
            => _promise = new ResolvedPromise<T>((Maybe<T>) value);

        public LazyMaybe(Maybe<T> value)
            => _promise = new ResolvedPromise<T>(value);

        public LazyMaybe(Task<Maybe<T>> task)
            => _promise = new AsyncPromise<T>(task);

        private LazyMaybe(Promise<T> promise)
            => _promise = promise;

        public T Match(T @default = default)
            => _promise.Evaluate().Match(@default);

        public T Match<TExcept>(TExcept except) where TExcept : Exception
            => except is null
                ? throw new ArgumentNullException(nameof(except))
                : _promise.Evaluate().Match(except);

        public TTarget Match<TTarget>(Func<T, TTarget> selector, TTarget @default = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _promise.Evaluate().Match(selector, @default);
        }

        public LazyMaybe<TTarget> SelectExpression<TTarget>(Expression<Func<T, TTarget>> selector)
            => new LazyMaybe<TTarget>(_promise.CreateTransform(selector));

        public LazyMaybe<TTarget> Select<TTarget>(Func<T, TTarget> selector)
            => new LazyMaybe<TTarget>(_promise.CreateTransform(x => selector(x)));
        

        public LazyMaybe<T> WhereExpression(Expression<Func<T, bool>> predicate)
            => new LazyMaybe<T>(_promise.CreateCondition(predicate));

        public LazyMaybe<T> Where(Func<T, bool> predicate)
            => new LazyMaybe<T>(_promise.CreateCondition(x => predicate(x)));

        public Awaiter GetAwaiter() => new Awaiter(_promise);

        public static explicit operator Maybe<T>(LazyMaybe<T> value)
            => value._promise.Evaluate();

        public static implicit operator LazyMaybe<T>(Maybe<T> value)
            => new LazyMaybe<T>(value);

        public static explicit operator LazyMaybe<T>(T value)
            => new LazyMaybe<T>(value);

        public static implicit operator LazyMaybe<T>(NoneType _)
            => new LazyMaybe<T>();

        internal static LazyMaybe<T> Flatten(LazyMaybe<LazyMaybe<T>> @this)
        {
            return new LazyMaybe<T>(new FlattenPromise<T>(@this._promise));
        }
    }
}
