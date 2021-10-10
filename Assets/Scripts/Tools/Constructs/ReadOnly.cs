using System.Runtime.CompilerServices;

namespace Lila.RpSingh.Constructs
{
    public struct ReadOnly<T>
    {
        private T value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnly<T> Create(T value)
        {
            ReadOnly<T> const_value = new ReadOnly<T>(value);
            return const_value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnly(T value)
        {
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(ReadOnly<T> value)
        {
            return value.value;
        }
    }
}