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

namespace Compatibility.Bridge
{
    public sealed class LazyMaybe<T>
    {
        private readonly Promise<T> _promise;

        public static LazyMaybe<T> None { get; } = new LazyMaybe<T>();

        public LazyMaybe() => _promise = new ResolvedPromise<T>(Maybe<T>.None);

        public LazyMaybe(Maybe<T> value)
            => _promise = new ResolvedPromise<T>(value);

        private LazyMaybe(Promise<T> promise)
            => _promise = promise;


        public T Match(T @default = default)
            => _promise.Evaluate().Match(@default);
        public TTarget Match<TTarget>(Func<T, TTarget> selector, TTarget @default = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _promise.Evaluate().Match(selector, @default);
        }


        public LazyMaybe<TTarget> Select<TTarget>(Expression<Func<T, TTarget>> selector)
            => new LazyMaybe<TTarget>(_promise.CreateNew(selector));

        public LazyMaybe<TTarget> SelectLambda<TTarget>(Func<T, TTarget> selector)
            => new LazyMaybe<TTarget>(_promise.CreateNew(x => selector(x)));



        public LazyMaybe<T> Where(Expression<Func<T, bool>> predicate)
            => new LazyMaybe<T>(_promise.CreateConditional(predicate));

        public LazyMaybe<T> WhereLambda(Func<T, bool> predicate)
            => new LazyMaybe<T>(_promise.CreateConditional(x => predicate(x)));



        public static explicit operator Maybe<T>(LazyMaybe<T> value)
            => value._promise.Evaluate();

        public static explicit operator LazyMaybe<T>(Maybe<T> value)
            => new LazyMaybe<T>(value);

        public static implicit operator LazyMaybe<T>(T value)
            => new LazyMaybe<T>(value);

        public static implicit operator LazyMaybe<T>(NoneType _)
            => new LazyMaybe<T>();
    }
}
