
using UnityEditor;
using UnityEngine;
using System;


public abstract class DuplicateUtility : EditorWindow
{
    protected static T Show<T>(string utility_name = "Duplicate Utility", float width = 300, float height = 400) where T : DuplicateUtility
    {
        DuplicateUtility window = (DuplicateUtility)EditorWindow.GetWindow(typeof(T));
        window.name = utility_name;
        window.maxSize = new Vector2(width, height);
        window.minSize = new Vector2(width, height);
        window.ShowUtility();
        return window as T;
    }
    protected static void ForeachSelectedGameObject<T>(Action<GameObject, T> callback, T args)
    {
        GameObject[] selected_objects = Selection.gameObjects;
        int count = selected_objects.Length;
        for (int i = 0; i < count; i++)
            callback(selected_objects[i], args);
    }


    protected abstract void OnDisplay();
    protected abstract void OnDuplicate();
    protected virtual void OnClose() { }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label(this.name, EditorStyles.boldLabel);
        EditorGUI.indentLevel += 1;
        OnDisplay();
        EditorGUI.indentLevel -= 1;
        if (GUILayout.Button("Duplicate Selected"))
        {
            OnDuplicate();
        }
        if(GUILayout.Button("Close"))
        {
            OnClose();
            this.Close();
        }
    }
}