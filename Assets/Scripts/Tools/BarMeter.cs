using UnityEngine;
using UnityEngine.UI;

public class BarMeter
{
    private float m_value;
    public float Value
    {
        get { return m_value; }
        set
        {
            m_value = Mathf.Clamp(value, m_MinValue, m_MaxValue);
        }
    }
    public float Value01
    {
        get { return m_value * m_inverse_zero_based_value; }
    }
    public float MaxValue { get { return m_MaxValue; } }
    public float MinValue { get { return m_MinValue; } }

    private float m_MaxValue;
    private float m_MinValue;
    private float m_InitialValue;
    private float m_inverse_zero_based_value;

    public BarMeter(float MinValue = 0, float MaxValue = 100, float InitialValue = 100)
    {
        m_MaxValue = MaxValue;
        m_MinValue = MinValue;
        m_inverse_zero_based_value = 1 / (m_MaxValue - m_MinValue);
        m_InitialValue = InitialValue;
        m_value = Mathf.Clamp(InitialValue, MinValue, MaxValue);
    }
    public void IncreaseValue(float amount) { Value += amount; if (Value > m_MaxValue) Value = m_MaxValue; }
    public void DecreaseValue(float amount) { Value -= amount; if (Value < 0) Value = 0; }
    public void Reset()
    {
        m_value = Mathf.Clamp(m_InitialValue, MinValue, MaxValue);
    }
}


public class SliderAutoHideController : IManualUpdatable
{
    public bool IsRunning { get { return true; } }
    private float m_cache_value;
    private Slider m_slider;
    private FadeController m_controller;
    private float m_timing;
    private float m_AutoHideTime;
    private bool m_isValueUpdated;

    public SliderAutoHideController(Slider slider, float AutoHideTime = 0.3f, bool IsHideOnAwake = true)
    {
        m_slider = slider;
        m_AutoHideTime = AutoHideTime;
        CanvasGroup renderer;
        CanvasGroupAlphaAdapter adapter;
        if (!(renderer = slider.GetComponent<CanvasGroup>()))
            renderer = slider.gameObject.AddComponent<CanvasGroup>();
        adapter = new CanvasGroupAlphaAdapter(renderer);
        if (IsHideOnAwake)
            adapter.SetAlpha(0);
        else
            adapter.SetAlpha(1);
        m_controller = new FadeController(adapter);
        m_isValueUpdated = false;
        m_cache_value = slider.value;
    }

    public void Update()
    {
        GameLoop.Update(m_controller);
        if (m_cache_value != m_slider.value)
        {
            m_controller.FadeIn();
            m_timing = Time.time;
            m_cache_value = m_slider.value;
            m_isValueUpdated = true;
        }
        if (m_isValueUpdated && Time.time - m_timing > m_AutoHideTime)
        {

            m_isValueUpdated = false;
            m_controller.FadeOut();
        }
    }
}

