using UnityEngine;

public class GUIScript : MonoBehaviour
{
    private bool m_is_gui_started_called = false;
    protected virtual void OnGUIStart()
    {

    }
    protected virtual void OnGUI()
    {
        if (!m_is_gui_started_called)
        {
            OnGUIStart();
            m_is_gui_started_called = true;
        }
        OnGUIUpdate();
    }
    protected virtual void OnGUIUpdate() { }
}