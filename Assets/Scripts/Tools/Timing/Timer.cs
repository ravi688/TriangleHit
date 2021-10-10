using UnityEngine;
using System.Collections;

using System;

[Flags]
public enum OnTimer
{
    End = 1,
    Start = 2,
    Update = 4
}

public class Timer : IManualUpdatable
{
    public bool IsRunning { get; set; }
    public bool IsLoop
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public bool IsUnscaledTime
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public float time { get { return m_current_time; } }
    private float m_From;
    private float m_To;
    private float m_current_time;
    private float m_InitialTimer;
    private float m_Step;
    private Action m_OnTimerEnd;
    private Action m_OnTimerStart;
    private Action m_OnTimerUpdate;

    public Timer(float from, float to, float step)
    {
        m_From = from;
        m_To = to;
        IsRunning = false;
        m_Step = step;
        IsLoop = false;
        IsUnscaledTime = false;
    }
    public void AddListner(Action action, OnTimer onTimer)
    {
        if ((onTimer & OnTimer.End) == OnTimer.End)
            m_OnTimerEnd += action;
        if ((onTimer & OnTimer.Start) == OnTimer.Start)
            m_OnTimerStart += action;
        if ((onTimer & OnTimer.Update) == OnTimer.Update)
            m_OnTimerUpdate += action;

    }
    public void Start()
    {
        m_current_time = m_From;
        IsRunning = true;
        m_InitialTimer = IsUnscaledTime ? Time.unscaledTime : Time.time;
        if (m_OnTimerStart != null)
            m_OnTimerStart();
    }
    public void Pause()
    {
        IsRunning = false;
    }
    public void Resume()
    {
        IsRunning = true;
    }
    public void Update()
    {
        if ((IsUnscaledTime ? Time.unscaledTime : Time.time) - m_InitialTimer >= m_Step)
        {
            m_InitialTimer = IsUnscaledTime ? Time.unscaledTime : Time.time;
            m_current_time += m_Step;
            if (m_OnTimerUpdate != null)
                m_OnTimerUpdate();
        }
        if (m_current_time >= m_To)
        {
            if (!IsLoop)
                IsRunning = false;
            else
            {
                m_current_time = m_From;
            }
            if (m_OnTimerEnd != null)
                m_OnTimerEnd();
        }
    }

}