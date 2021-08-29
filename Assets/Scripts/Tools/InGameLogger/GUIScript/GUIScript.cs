using UnityEngine;
using System.Collections;

public abstract class GUIScript : MonoBehaviour
{
    private bool is_GUIStart_called = false; 

    private void OnGUI()
    {
        if (!is_GUIStart_called)
        {
            OnGUIStart();
            is_GUIStart_called = true; 
        }
        OnGUIUpdate(); 
    }
    protected abstract void OnGUIStart();
    protected abstract void OnGUIUpdate();
}
