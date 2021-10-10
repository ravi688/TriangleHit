using System.Collections.Generic;

namespace Lila.RpSingh.Pooling
{
    public class IDPool
    {
        public static int invalidID { get { return -1; } }
        private Queue<int> m_returned_integers;
        private int m_counter;

        public IDPool()
        {
            m_counter = 0;
            m_returned_integers = new Queue<int>();
        }
        public bool IsValid(int id)
        {
            return !m_returned_integers.Contains(id) && m_counter > id && id >= 0;
        }
        public int Get()
        {
            if (m_returned_integers.Count > 0)
                return m_returned_integers.Dequeue();
            int id = m_counter;
            m_counter++;
            return id;
        }
        public void Return(int id)
        {
            if (!m_returned_integers.Contains(id))
                m_returned_integers.Enqueue(id);
        }

        public void Clear()
        {
            m_counter = 0;
            m_returned_integers.Clear();
        }
    }
}
