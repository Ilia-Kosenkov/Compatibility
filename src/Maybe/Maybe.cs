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

namespace Maybe
{

    public abstract class Maybe
    {
        public abstract Maybe<T> Select<T>(Func<object, T> selector);
        public abstract object Match(object @default = default);
        public abstract T Match<T>(Func<object, T> selector, T @default = default);
        public abstract Maybe<T> OfType<T>();

    }
    public sealed class Maybe<T> : Maybe
    {
        public static Maybe<T> None { get; } = new Maybe<T>();

        private readonly T _value;
        private readonly bool _hasValue;
        
        public Maybe()
        {
            _hasValue = false;
        }

        public Maybe(T value)
        {
            _value = value;
            _hasValue = value != null;
        }

        public Maybe<TTarget> Select<TTarget>(Func<T, TTarget> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _hasValue
                ? new Maybe<TTarget>(selector(_value))
                : new Maybe<TTarget>();
        }

        public T Match(T @default = default)
            => _hasValue ? _value : @default;

        public override TTarget Match<TTarget>(Func<object, TTarget> selector, TTarget @default = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _hasValue
                ? selector(_value)
                : @default;
        }

        public override Maybe<TTarget> OfType<TTarget>()
        {
            if (_hasValue && _value is TTarget targetVal)
                return targetVal;
            return Maybe<TTarget>.None;
        }

        public T Match<TExcept>(TExcept except) where TExcept : Exception
            => _hasValue ? _value : throw ((Exception) except ?? new ArgumentNullException(nameof(except)));

        public TTarget Match<TTarget>(Func<T, TTarget> selector, TTarget @default = default)
        {
            if(selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _hasValue
                ? selector(_value)
                : @default;
        }

        public Maybe<T> Where(Func<T, bool> predicate)
        {
            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return
                _hasValue
                    ? predicate(_value)
                        ? new Maybe<T>(_value)
                        : new Maybe<T>()
                    : new Maybe<T>();
        }

        public override object Match(object @default = default)
            => _hasValue ? _value : @default;

        public override Maybe<TTarget> Select<TTarget>(Func<object, TTarget> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return _hasValue
                ? new Maybe<TTarget>(selector(_value))
                : new Maybe<TTarget>();
        }

        public static implicit operator Maybe<T> (T value)
            => new Maybe<T>(value);

        public static implicit operator Maybe<T>(None _)
            => new Maybe<T>();

    }
}
