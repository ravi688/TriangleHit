using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

[Flags]
public enum CheatType
{
    KillAllEnemies = 0x1,
    FullPlayerHealth = 0x1 << 1,
    FullPlayerLives = 0x1 << 2,
    ZeroPlayerHeatlh = 0x1 << 3,
    ZeroPlayerLives = 0x1 << 4,
    None = 0x1 << 5
}
public class CheatManager : GUIScript
{
    public  event Action<CheatType> OnCheatHandler;
    private Dictionary<string, CheatType> cheat_codes;
    private string cheat_code;
    private StringBuilder cheat_code_string;
    private CheatType current_cheat_type;
    private string[] cheats = 
    {
        "KillAllEnemies", 
        "FullPlayerHealth",
        "FullPlayerLives",
        "ZeroPlayerHealth",
        "ZeroPlayerLives"
    };

    private bool IsShowVisualAlert = false;
    private GUIStyle VisualAlertStyle;
    private Timer VisualAlertTimer;
    private string current_cheat_string;
    private int cheat_count;
    private bool isCheatButtonPressed = false;
    private Rect cheat_button_rect;
    private GUIStyle cheat_button_style;
    private TabMenu tab_menu;
    private bool isTabMenuActive = false;
    private void Awake()
    {
        cheat_button_rect = new Rect(30, Screen.height - 50, 150, 40);
    }
    private void Start()
    {
        tab_menu = new TabMenu(cheats, cheats, CheatButtonCallBack);
        tab_menu.position = new Vector2(30, Screen.height - 90);
        tab_menu.tab_width = 300;
        tab_menu.tab_height = 50;
        tab_menu.font_size = 30;
        tab_menu.spacing = 10;
        tab_menu.SetAlignment(TabMenu.TabMenuAlignment.VerticalUp);

        cheat_code_string = new StringBuilder();
        cheat_codes = new Dictionary<string, CheatType>();
        VisualAlertTimer = new Timer(0, 1.8f, 1.0f);

        cheat_codes.Add(cheats[0], CheatType.KillAllEnemies);
        cheat_codes.Add(cheats[1], CheatType.FullPlayerHealth);
        cheat_codes.Add(cheats[2], CheatType.FullPlayerLives);
        cheat_codes.Add(cheats[3], CheatType.ZeroPlayerHeatlh);
        cheat_codes.Add(cheats[4], CheatType.ZeroPlayerLives);

        VisualAlertTimer.AddListner(
            delegate
            {
                IsShowVisualAlert = false;
            }
            , OnTimer.End);
        VisualAlertTimer.AddListner(
            delegate
            {
                IsShowVisualAlert = true;
            }
            , OnTimer.Start);
        cheat_count = cheats.Length;
    }
    private void CheatButtonCallBack(int id, bool isSecondTime)
    {
        isCheatButtonPressed = true;
        switch (id)
        {
            case 0:
                current_cheat_type = CheatType.KillAllEnemies;
                break;
            case 1:
                current_cheat_type = CheatType.FullPlayerHealth;
                break;
            case 2:
                current_cheat_type = CheatType.FullPlayerLives;
                break;
            case 3:
                current_cheat_type = CheatType.ZeroPlayerHeatlh;
                break;
            case 4:
                current_cheat_type = CheatType.ZeroPlayerLives;
                break;
        }
    }

    private void LateUpdate()
    {
        if (!isCheatButtonPressed)
        {
            current_cheat_type = CheatType.None;
        }

        if (VisualAlertTimer.IsRunning)
            VisualAlertTimer.Update();

#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.anyKeyDown)
        {
            cheat_code_string.Append(GetPressedChar());
            if (ValidateCurrentCheatType() == true)
            {
                cheat_code_string.Length = 0;
                VisualAlertTimer.Start();
                if (OnCheatHandler != null)
                    OnCheatHandler(current_cheat_type);
            }
        }
#endif
        if (isCheatButtonPressed)
        {
            current_cheat_string = current_cheat_type.ToString();
            VisualAlertTimer.Start();
            if (OnCheatHandler != null)
                OnCheatHandler(current_cheat_type);
        }
        isCheatButtonPressed = false;
    }
    protected override void OnGUIStart()
    {
        cheat_button_style = new GUIStyle(GUI.skin.button);
        cheat_button_style.fontSize = 30;
        cheat_button_style.fontStyle = FontStyle.Bold;
        cheat_button_style.alignment = TextAnchor.MiddleCenter;
        tab_menu.OnGUIStart();

        VisualAlertStyle = new GUIStyle(GUI.skin.box);
        VisualAlertStyle.fontStyle = FontStyle.Bold;
        VisualAlertStyle.fontSize = 32;
        VisualAlertStyle.alignment = TextAnchor.MiddleCenter;
    }

    protected override void OnGUIUpdate()
    {
        if (IsShowVisualAlert)
        {
            GUI.Box(new Rect(10, 20, 300, 50), new GUIContent(current_cheat_string), VisualAlertStyle);
        }
        if (GUI.Button(cheat_button_rect, "Cheats", cheat_button_style))
        {
            isTabMenuActive = !isTabMenuActive;
        }
        if (isTabMenuActive)
            tab_menu.OnGUIUpdate();
    }
    private bool ValidateCurrentCheatType()
    {
        if (IsAnyValideCheat(out current_cheat_string, cheat_code_string.ToString()))
            if (cheat_codes.TryGetValue(current_cheat_string, out current_cheat_type))
                return true;

        current_cheat_type = CheatType.None;
        return false;
    }
    private bool IsAnyValideCheat(out string out_cheat_string, string in_str)
    {

        for (int i = 0; i < cheat_count; i++)
        {
            if (in_str.Contains(cheats[i].ToUpper()))
            {
                out_cheat_string = cheats[i];
                return true;
            }
        }
        out_cheat_string = "";
        return false;
    }

    private string GetPressedChar()
    {
        for (int i = 97; i < 123; i++)
        {
            if (GetChar(i).Equals(i))
            {
                return ((char)i).ToString().ToUpper();
            }
        }
        return "";
    }
    private int GetChar(int _char)
    {
        if (Input.GetKeyDown((KeyCode)_char))
            return _char;
        else
            return " "[0];
    }
    public CheatType GetCheat()
    {
        return current_cheat_type;
    }
    private void OnDestroy()
    {
        OnCheatHandler = null;
    }

}