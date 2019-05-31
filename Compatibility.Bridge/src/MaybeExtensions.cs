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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Compatibility.Bridge
{
    public static class MaybeExtensions
    {
        public static Maybe<TTarget> Select<TSource, TTarget>(this TSource? @this, Func<TSource, TTarget> selector)
            where TSource : struct
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this is null
                    ? new Maybe<TTarget>()
                    : selector(@this.Value);

        public static Maybe<TResult> SelectMany<TSource, TSelection, TResult>(
            this Maybe<TSource> @this,
            Func<TSource, Maybe<TSelection>> mapper,
            Func<TSource, TSelection, TResult> selector)
        {
            if(mapper is null)
                throw new ArgumentNullException(nameof(mapper));
            if(selector is null)
                throw new ArgumentNullException(nameof(selector));

            if(@this is null)
                return new Maybe<TResult>();

            return @this.Match(x => mapper(x).Select(y => selector(x, y)), None);
        }

        public static LazyMaybe<TResult> SelectMany<TSource, TSelection, TResult>(
            this LazyMaybe<TSource> @this,
            Func<TSource, LazyMaybe<TSelection>> mapper,
            Func<TSource, TSelection, TResult> selector)
        {
            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (@this is null)
                return new LazyMaybe<TResult>();

            return @this.Match(x => mapper(x).Select(y => selector(x, y)), None);
        }

        public static Maybe<T> Some<T>(T @this)
            => @this == null
                ? throw new ArgumentNullException(nameof(@this))
                : @this;

        public static NoneType None => NoneType.None;

        public static IEnumerable<Maybe<T>> WhereMaybe<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(item => predicate(item)
                    ? item
                    : Maybe<T>.None);

        public static IEnumerable<LazyMaybe<T>> WhereMaybeLazy<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(item => predicate(item)
                    ? item
                    : LazyMaybe<T>.None);

        public static IEnumerable<Maybe<TTar>> Select<TSrc, TTar>(this IEnumerable<Maybe<TSrc>> @this,
            Func<TSrc, TTar> selector)
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this?.Select(x => x.Select(selector));

        public static IEnumerable<LazyMaybe<TTar>> Select<TSrc, TTar>(this IEnumerable<LazyMaybe<TSrc>> @this,
            Expression<Func<TSrc, TTar>> selector)
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this?.Select(x => x.Select(selector));

        public static IEnumerable<LazyMaybe<TTar>> SelectLambda<TSrc, TTar>(this IEnumerable<LazyMaybe<TSrc>> @this,
            Func<TSrc, TTar> selector)
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this?.Select(x => x.SelectLambda(selector));

        public static IEnumerable<Maybe<T>> Where<T>(this IEnumerable<Maybe<T>> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(x => x.Where(predicate));

        public static IEnumerable<LazyMaybe<T>> Where<T>(this IEnumerable<LazyMaybe<T>> @this, Expression<Func<T, bool>> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(x => x.Where(predicate));

        public static IEnumerable<LazyMaybe<T>> WhereLambda<T>(this IEnumerable<LazyMaybe<T>> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(x => x.WhereLambda(predicate));


        public static IEnumerable<T> Match<T>(this IEnumerable<Maybe<T>> @this, T @default = default)
            => @this?.Select(x => x.Match(@default));

        public static IEnumerable<T> Match<T>(this IEnumerable<LazyMaybe<T>> @this, T @default = default)
            => @this?.Select(x => x.Match(@default));

    }
}
