using System.Collections.Generic;
using UnityEngine;

public class GUIScriptManager : SingletonMonoBehaviour<GUIScriptManager>
{
    private static List<IGUIScript> _scripts;
    private bool _startCalled = false;
    public static void Register(IGUIScript script)
    {
        if (_scripts == null)
            _scripts = new List<IGUIScript>();
        _scripts.Add(script);
        ForceAwake();
    }

    private void Start()
    {
        gameObject.hideFlags = HideFlags.NotEditable;
        DontDestroyOnLoad(this);
    }

    private void OnGUI()
    {
        int count = _scripts.Count;
        if (!_startCalled)
        {
            for (int i = 0; i < count; i++)
                _scripts[i].OnGUIStart();
            _startCalled = true;
        }
        for (int i = 0; i < count; i++)
            _scripts[i].OnGUIUpdate();
    }
}