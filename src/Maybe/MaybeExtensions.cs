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

            return @this.Match(x => mapper(x).Select(y => selector(x, y)), None.Get);
        }

  
        public static Maybe<T> Some<T>(this T @this)
            => new Maybe<T>(@this);

        public static Maybe<T> SomeNullable<T>(this T? @this) where T : struct
            => @this?.Some() ?? Maybe<T>.None;

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

        
        public static Maybe<T> AggregateSome<T>(this IEnumerable<Maybe<T>> @this,
            Func<T, T, T> aggregator)
        {
            if(aggregator is null)
                throw new ArgumentNullException(nameof(aggregator));
            if(@this is null)
                throw new ArgumentNullException(nameof(@this));

            return @this.Aggregate((old, @new) =>
                from o in old
                from n in @new
                select aggregator(o, n));
        }

        public static Maybe<TSeed> AggregateSome<T, TSeed>(
            this IEnumerable<Maybe<T>> @this,
            Maybe<TSeed> seed,
            Func<TSeed, T, TSeed> aggregator)
        {
            if (aggregator is null)
                throw new ArgumentNullException(nameof(aggregator));
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));

            return @this.Aggregate(seed,
                (old, @new) =>
                    from o in old
                    from n in @new
                    select aggregator(o, n));
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> @this, Func<T, bool> predicate)
        {
            if (@this is null)
                throw new ArgumentNullException(nameof(@this));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in @this)
                if (predicate(item))
                    return item.Some();

            return Maybe<T>.None;
        }

        public static Maybe<T> ElementAtOrNone<T>(this IEnumerable<T> @this, int index)
        {
            if(@this is null)
                throw new ArgumentNullException(nameof(@this));

            if(index < 0 )
                throw new ArgumentException(nameof(index));

            using (var enumerator = @this.GetEnumerator())
            {
                for (var i = 0; enumerator.MoveNext(); i++)
                {
                    if (i == index)
                        return enumerator.Current.Some();

                }

                return Maybe<T>.None;
            } 
        }
    }
}
