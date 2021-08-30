
using System.Collections.Generic;

public class GUIScriptManager : SingletonMonoBehaviour<GUIScriptManager>
{
    private static List<IGUIScript> m_scripts;
    private bool m_start_called = false;
    public static void Register(IGUIScript script)
    {
        if (m_scripts == null)
            m_scripts = new List<IGUIScript>();
        m_scripts.Add(script);
        if (Instance == null)
            ForceAwake();
    }

    private void OnGUI()
    {
        int count = m_scripts.Count;
        if (!m_start_called)
        {
            for (int i = 0; i < count; i++)
                m_scripts[i].OnGUIStart();
            m_start_called = true;
        }
        for (int i = 0; i < count; i++)
            m_scripts[i].OnGUIUpdate();
    }
}