
using System.Runtime.CompilerServices;

public class Reference<T>
{
    private T m_data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Reference(T data)
    {
        m_data = data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(Reference<T> reference)
    {
        return reference.m_data;
    }

    public static implicit operator Reference<T>(T data)
    {
        return new Reference<T>(data);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get() { return m_data; }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(T data) { m_data = data; }
}