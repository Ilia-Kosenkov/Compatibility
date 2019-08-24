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
using System.Collections.Generic;
using System.Linq;

namespace Maybe
{
    public static class MaybeExtensions
    {
        public static Maybe<TResult> SelectMany<TSource, TSelection, TResult>(
            this Maybe<TSource> @this,
            Func<TSource, Maybe<TSelection>> mapper,
            Func<TSource, TSelection, TResult> selector)
        {
            if(mapper is null)
                throw new ArgumentNullException(nameof(mapper));
            if(selector is null)
                throw new ArgumentNullException(nameof(selector));

            return @this is null ? new Maybe<TResult>() : @this.Match(x => mapper(x).Select(y => selector(x, y)), None.Get);
        }

  
        public static Maybe<T> Some<T>(this T @this)
            => new Maybe<T>(@this);

        public static IEnumerable<Maybe<TTarget>> SelectSome<TSource, TTarget>(this IEnumerable<TSource> @this,
            Func<TSource, TTarget> selector)
            where TSource : struct
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this?.Select(x => new Maybe<TSource>(x).Select(selector));

        public static IEnumerable<Maybe<T>> WhereSome<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(item => new Maybe<T>(item).Where(predicate));

       public static IEnumerable<Maybe<TTar>> Select<TSrc, TTar>(this IEnumerable<Maybe<TSrc>> @this,
            Func<TSrc, TTar> selector)
            => selector is null
                ? throw new ArgumentNullException(nameof(selector))
                : @this?.Select(x => x.Select(selector));

        public static IEnumerable<Maybe<T>> Where<T>(this IEnumerable<Maybe<T>> @this, Func<T, bool> predicate)
            => predicate is null
                ? throw new ArgumentNullException(nameof(predicate))
                : @this?.Select(x => x.Where(predicate));

        public static IEnumerable<T> Match<T>(this IEnumerable<Maybe<T>> @this, T @default = default)
            => @this?.Select(x => x.Match(@default));

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in @this)
                if (predicate(item))
                    return item;

            return Maybe<T>.None;
        }

    }
}
