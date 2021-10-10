public class Reference<T>
{
    private T m_data;
    public Reference(T data)
    {
        m_data = data;
    }
    public T Get() { return m_data; }
    public void Set(T data) { m_data = data; }
}