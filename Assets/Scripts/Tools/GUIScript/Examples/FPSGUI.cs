using UnityEngine;
using Lila.RpSingh;


public class FPSGUI : MonoBehaviour, IGUIScript
{
    [SerializeField]
    private float _updateInterval = 0.01f;
    [SerializeField]
    private int _fontSize = 72;
    int _fps = 0;
    GUIStyle _labelStyle;
    float _timeHelper;
    private void Awake()
    {
        GUIScriptManager.Register(this);
        _timeHelper = Time.realtimeSinceStartup;
    }
    private void Update()
    {
        if(Time.realtimeSinceStartup - _timeHelper > _updateInterval)
        {
            _fps = (int)(1 / Time.deltaTime);
            _timeHelper = Time.realtimeSinceStartup;
        }
    }

    public void OnGUIStart()
    {
        _labelStyle = new GUIStyle(GUI.skin.label);
        _labelStyle.fontStyle = FontStyle.Bold;
        _labelStyle.fontSize = _fontSize;
    }
    public void OnGUIUpdate()
    {
        GUILayout.Label("FPS: " + _fps, _labelStyle);
    }
}