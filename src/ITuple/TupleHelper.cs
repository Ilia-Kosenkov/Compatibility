//     MIT License
//     
//     Copyright(c) 2019 Ilia Kosenkov
//     
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
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
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Compatibility.ITuple
{
    public static class TupleHelper
    {
        private readonly struct Tuple<T1> : ITuple
        {
            private readonly ValueTuple<T1> _field;

            public Tuple(ValueTuple<T1> arg) => _field = arg;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 1;
        }
        private readonly struct Tuple<T1, T2> : ITuple
        {
            private readonly (T1, T2) _field;

            public Tuple((T1, T2) arg) => _field = arg;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 2;
        }

        private readonly struct Tuple<T1, T2, T3> : ITuple
        {
            private readonly (T1, T2, T3) _field;

            public Tuple((T1, T2, T3) field) => _field = field;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 3;
        }

        private readonly struct Tuple<T1, T2, T3, T4> : ITuple
        {
            private readonly (T1, T2, T3, T4) _field;

            public Tuple((T1, T2, T3, T4) field) => _field = field;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        case 3: return _field.Item4;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 4;
        }

        private readonly struct Tuple<T1, T2, T3, T4, T5> : ITuple
        {
            private readonly (T1, T2, T3, T4, T5) _field;

            public Tuple((T1, T2, T3, T4, T5) field) => _field = field;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        case 3: return _field.Item4;
                        case 4: return _field.Item5;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 5;
        }

        private readonly struct Tuple<T1, T2, T3, T4, T5, T6> : ITuple
        {
            private readonly (T1, T2, T3, T4, T5, T6) _field;

            public Tuple((T1, T2, T3, T4, T5, T6) field) => _field = field;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        case 3: return _field.Item4;
                        case 4: return _field.Item5;
                        case 5: return _field.Item6;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 6;
        }

        private readonly struct Tuple<T1, T2, T3, T4, T5, T6, T7> : ITuple
        {
            private readonly (T1, T2, T3, T4, T5, T6, T7) _field;

            public Tuple((T1, T2, T3, T4, T5, T6, T7) field) => _field = field;

            object ITuple.this[int position]
            {
                get
                {
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        case 3: return _field.Item4;
                        case 4: return _field.Item5;
                        case 5: return _field.Item6;
                        case 6: return _field.Item7;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 7;
        }

        private readonly struct Tuple<T1, T2, T3, T4, T5, T6, T7, TOther> : ITuple where TOther : struct
        {
            private readonly ValueTuple<T1, T2, T3, T4, T5, T6, T7, TOther> _field;
            private readonly ITuple _others;
            public Tuple(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TOther> field)
            {
                _field = field;
                _others = _field.Rest.IsValueTuple() ?? throw new InvalidCastException();
            }

            object ITuple.this[int position]
            {
                get
                {
                    if (position >= 7 && position < (7 + _others?.Length ?? 8))
                        return _others?[position - 7];
                    switch (position)
                    {
                        case 0: return _field.Item1;
                        case 1: return _field.Item2;
                        case 2: return _field.Item3;
                        case 3: return _field.Item4;
                        case 4: return _field.Item5;
                        case 5: return _field.Item6;
                        case 6: return _field.Item7;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            int ITuple.Length => 7 + _others?.Length ?? 8;
        }

        public interface ITuple
        {
            int Length { get; }
            object this[int position] { get; }
        }

        public static ITuple IsValueTuple(this object @this)
        {
            if (@this is null)
                return null;

            var tp = @this.GetType();
            if (tp.IsValueType && tp.IsGenericType)
            {
                var def = tp.GetGenericTypeDefinition();
                Type newType = null;
                if (def == typeof(ValueTuple<>))
                    newType = typeof(Tuple<>);
                if (def == typeof(ValueTuple<,>))
                    newType = typeof(Tuple<,>);
                else if (def == typeof(ValueTuple<,,>))
                    newType = typeof(Tuple<,,>);
                else if (def == typeof(ValueTuple<,,,>))
                    newType = typeof(Tuple<,,,>);
                else if (def == typeof(ValueTuple<,,,,>))
                    newType = typeof(Tuple<,,,,>);
                else if (def == typeof(ValueTuple<,,,,,>))
                    newType = typeof(Tuple<,,,,,>);
                else if (def == typeof(ValueTuple<,,,,,,>))
                    newType = typeof(Tuple<,,,,,,>);
                else if (def == typeof(ValueTuple<,,,,,,,>))
                    newType = typeof(Tuple<,,,,,,,>);


                return newType is null
                    ? null
                    : Activator.CreateInstance(newType.MakeGenericType(tp.GenericTypeArguments),
                        @this) as ITuple;
            }

            return null;
        }
    }
}
