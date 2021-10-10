using Lila.RpSingh;
using Lila.RpSingh.Constructs;
using Lila.RpSingh.Pooling;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DebugView : SingletonMonoBehaviour<DebugView>, IGUIScript
{
	private Dictionary<int, string> _strings;
	private IDPool _iDPool;
	private void Awake()
	{
		GUIScriptManager.Register(this);
		_strings = new Dictionary<int, string>();
		_iDPool = new IDPool();
	}

	public static void SetString(int id, string str)
	{
		if (!Instance._iDPool.IsValid(id))
			throw new InvalidOperationException("Invalid string id");
		Instance._strings[id] = str;
	}
	public static ReadOnly<int> AddString(string str)
	{
		int id = Instance._iDPool.Get();
		Instance._strings.Add(id, str);
		return ReadOnly<int>.Create(id);
	}

	public static void RemoveString(int handle)
	{
		if (handle == -1) return;
		Instance._strings.Remove(handle);
	}

	private GUIStyle _labelStyle;
	public void OnGUIStart()
	{
		_labelStyle = new GUIStyle(GUI.skin.label);
		_labelStyle.fontSize = 20;
		_labelStyle.fontStyle = FontStyle.Bold;
	}

	public void OnGUIUpdate()
	{
		GUI.backgroundColor = Color.black;
		GUI.Box(new Rect(0, 0, 500, 400), "Debug");
		foreach(KeyValuePair<int, string> str in _strings)
		{
			GUILayout.Label(str.Value, _labelStyle);
		}
	}
}