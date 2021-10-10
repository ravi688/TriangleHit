using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystemTest : MonoBehaviour
{
    IContiguousPool<int> m_integers;
    int cache;
    private void Start()
    {
        m_integers = new StaticContiguousPool<int>(10);
        for (int i = 0; i < 10; i++)
            m_integers.Add(i);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            cache = m_integers.GetFromPool();
            Debug.Log(cache);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            m_integers.ReturnToPool(cache);
        }
    }
}
