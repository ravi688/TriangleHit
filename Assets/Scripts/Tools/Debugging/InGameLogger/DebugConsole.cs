#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
#define HAS_POINTING_DEVICE
#endif

#if (UNITY_ANDROID || UNIT_IOS || UNITY_IPHONE) && !UNITY_EDITOR
#define HAS_TOUCH_DEVICE
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DebugConsole
{
    public static int MaxLogCount = 200;
    private LinkedList<string> logs;                                //Stores all the logs

    private static int id = 0;
    private int m_id;                                               //Console id , used to identify the console window
    public int Id { get { return m_id; } }
    public bool IsActive; 
    private Rect label_rect;
    private Rect window_rect;
    private Rect window_content_rect; 
    private Rect menu_button_rect;
    private Rect close_button_rect;
    private TabMenu tab_menu;

    private static string error_color_string = ColorUtility.ToHtmlStringRGBA(new Color(1, 0.3f, 0.3f, 1.0f));
    private static string warning_color_string = ColorUtility.ToHtmlStringRGBA(Color.yellow);
    //The file path will be  set automatically to 
    //Application.persistentDataPath + consolename.logdata
    private string save_directory;
    private string console_name;
    private Vector2 scroll_view;
    private float log_board_size;
    public static int character_width = 8;
    private float previous_label_height;
    private float current_label_height;

    public static GUIStyle LabelStyle;
    public static GUIStyle WindowStyle;
    private static GUIStyle ButtonStyle;
    public Action OnClose;

    public static float label_height = 30;                          //The height of the labels
    public static float label_spacing = 2;                          //The spacing between the labels
    public static float label_horizontal_margin = 30;               //Margin  of Labels from the Sides of the console window
    public static float upper_border_margin = 30;                   //Title bar height of the console window
    public static float drag_sensitivity = 4f;                      //when the user slide throught the console window how fast the drag should be

    private int save_file_index_counter;

    public bool isTabMenuActive;
    public Vector2 position = new Vector2(10, 10);
    public static Vector2 Size = new Vector2(500, 400);

    private float previous_pointer_pos;
    private float default_alpha_value;
    private float current_alpha_value;
    private bool isWindowGrabbed;
    public DebugConsole(string console_name)
    {
        save_file_index_counter = 0;
        isTabMenuActive = false;
        this.console_name = console_name;
        m_id = ++id;
        IsActive = false; 

        logs = new LinkedList<string>();
        scroll_view = Vector2.zero;
        log_board_size = 0;
        current_alpha_value = GUI.color.a;
        isWindowGrabbed = false;
        CreateSavingDirectories(console_name);
        tab_menu = new TabMenu(new string[4] { "Clear", "SaveTxt", "SaveBin", "LoadBin" }, new string[4] { "Clear", "SaveTxt", "SaveBin", "LoadBin" }, TabMenuCallback);
    }
    private void CreateSavingDirectories(string console_name)
    {
        save_directory = string.Format("{0}/Logs", Application.persistentDataPath);

        if (!Directory.Exists(save_directory))
            Directory.CreateDirectory(save_directory);
    }
    public void SaveTxt()
    {
        string file_name = string.Format("{0}_{1}.txt", console_name, save_file_index_counter);
        ++save_file_index_counter;
        string save_file_path = string.Format("{0}/{1}", save_directory, file_name);
        FileStream save_file_stream;
        if (!File.Exists(save_file_path))
        {
            save_file_stream = File.Open(save_file_path, FileMode.Create);
        }
        else
            save_file_stream = File.Open(save_file_path, FileMode.Truncate);
        StreamWriter writer = new StreamWriter(save_file_stream);

        LinkedListNode<string> node = logs.Last;
        int err_decorator_string_length = string.Format("<color=#{0}", error_color_string).Length;
        int wrn_decorator_string_length = string.Format("<color=#{0}", warning_color_string).Length;

        while (node != null)
        {
            string str = node.Value;
            if (str.StartsWith("<"))
            {
                int index = str.IndexOf("#");
                if (str.Substring(index + 1, 8) == error_color_string)
                {
                    int last_index = str.LastIndexOf("<");
                    string log_str = str.Substring(err_decorator_string_length + 1, last_index - err_decorator_string_length - 1);
                    writer.WriteLine(string.Format("[ERROR]{0}", log_str));
                }
                else if (str.Substring(index + 1, 8) == warning_color_string)
                {
                    int last_index = str.LastIndexOf("<");
                    string log_str = str.Substring(wrn_decorator_string_length + 1, last_index - wrn_decorator_string_length - 1);
                    writer.WriteLine(string.Format("[WARNING]{0}", log_str));
                }

            }
            else
            {
                writer.WriteLine(string.Format("[LOG]{0}", str));
            }
            node = node.Previous;
        }
        writer.Close();
        writer.Dispose();
        save_file_stream.Close();
        save_file_stream.Dispose();
    }
    public void TabMenuCallback(int id, bool isSecondTime)
    {
        switch (id)
        {
            case 0:
                ClearLogs();
                Log("ClearLogs command accepted");
                break;
            case 1:
                SaveTxt();
                Log("SaveTxt command accepted");
                break;
            case 2:
                SaveBin();
                Log("SaveBin command accepted");
                break;
            case 3:
                LoadBin();
                Log("LoadBin command accepted");
                break;
        }
        isTabMenuActive = false;
    }


    //Must be called in OnGUI callback function
    public void OnGUIUpdate()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, current_alpha_value);
        GUI.Window(m_id, window_rect, WindowFunction, new GUIContent(console_name), WindowStyle);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, default_alpha_value);
    }
    public void OnNormalUpdate()
    {
        if (!IsActive) return; 
        Vector2 pointer_position = new Vector2(InputManager.GetInputPosition().x, -InputManager.GetInputPosition().y + Screen.height);

        if (InputManager.IsInputDown() && IsInRect(pointer_position))
        {
            isWindowGrabbed = true;
            GameManager.GetTouchManager().IsRunning = false ;
        }
        if (isWindowGrabbed)
        {
#if HAS_TOUCH_DEVICE 
            scroll_view += new Vector2(0, Input.GetTouch(0).deltaPosition.y * drag_sensitivity);
#endif
#if HAS_POINTING_DEVICE
            scroll_view += new Vector2(0, InputManager.GetInputPosition().y - previous_pointer_pos); 
#endif
        }
        if (isWindowGrabbed && InputManager.IsInputUp())
        {
            GameManager.GetTouchManager().IsRunning = true; 
            isWindowGrabbed = false;
        }
#if HAS_POINTING_DEVICE
        previous_pointer_pos = InputManager.GetInputPosition().y; 
#endif
    }
    private bool IsInRect(Vector2 pointer_pos)
    {
        return pointer_pos.x > window_content_rect.xMin && pointer_pos.x < window_content_rect.xMax
            && pointer_pos.y < window_content_rect.yMax && pointer_pos.y > window_content_rect.yMin;

    }

    private void WindowFunction(int _id)
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, current_alpha_value);
        if (GUI.Button(menu_button_rect, "Menu", ButtonStyle))
            isTabMenuActive = !isTabMenuActive;
        if (GUI.Button(close_button_rect, "Close", ButtonStyle))
            OnClose();

        scroll_view = GUI.BeginScrollView(new Rect(5, upper_border_margin, window_rect.width - 10, window_rect.height - upper_border_margin), scroll_view,
             new Rect(0, upper_border_margin, window_rect.width - 50, log_board_size));
        int log_count = logs.Count;

        LinkedListNode<string> node = logs.Last;

        ResetLabelRects();
        while (node != null)
        {
            Label(node.Value);
            node = node.Previous;
        }
        log_board_size = label_rect.y - upper_border_margin + current_label_height;


        GUI.EndScrollView();
        if (isTabMenuActive)
        {
            tab_menu.OnGUIUpdate();
        }
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, default_alpha_value);
    }

    private void ResetLabelRects()
    {
        previous_label_height = 0;
        label_rect.height = 0;
        label_rect.y = upper_border_margin;
    }
    //if any of the position , Size changes , you must have to call this immediately
    public void ReCalculateRect()
    {
        window_rect = new Rect(position, Size);
        window_content_rect = new Rect(new Vector2(position.x , position.y + upper_border_margin), new Vector2(Size.x , Size.y - upper_border_margin)); 
        label_rect = new Rect(5, 0, window_rect.width - label_horizontal_margin, label_height);
        menu_button_rect = new Rect(10, 2, 70, upper_border_margin - 4);
        close_button_rect = new Rect(window_rect.width - menu_button_rect.width - 10, 2, menu_button_rect.width, menu_button_rect.height);
    }
    //must be called OnGUIStart callback function
    public void OnGUIStart()
    {
        tab_menu.position = new Vector2(10, upper_border_margin + 10);
        tab_menu.tab_height = 40;
        tab_menu.tab_width = 150;
        tab_menu.spacing = 2;
        tab_menu.font_size = 25;
        tab_menu.draw_background = true;
        tab_menu.SetAlignment(TabMenu.TabMenuAlignment.VerticalDown);
        tab_menu.OnGUIStart();
        ReCalculateRect();
        LabelStyle = new GUIStyle(GUI.skin.label);
        LabelStyle.fontSize = 20;
        LabelStyle.fontStyle = FontStyle.Bold;
        LabelStyle.alignment = TextAnchor.MiddleLeft;
        LabelStyle.richText = true;

        WindowStyle = new GUIStyle(GUI.skin.window);
        WindowStyle.fontSize = 20;
        WindowStyle.fontStyle = FontStyle.BoldAndItalic;
        WindowStyle.alignment = TextAnchor.UpperCenter;
        WindowStyle.richText = true;

        ButtonStyle = new GUIStyle(GUI.skin.button);
        ButtonStyle.fontSize = 20;
        ButtonStyle.fontStyle = FontStyle.Bold;
        ButtonStyle.alignment = TextAnchor.MiddleLeft;
        default_alpha_value = GUI.color.a;
    }
    //If you have saved some logs of this console window and you can call this function to load the saved logs
    public void LoadBin()
    {
        FileStream file;
        string file_name = string.Format("{0}.bin", console_name);
        string load_file_path = string.Format("{0}/{1}", save_directory, file_name);
        if (!File.Exists(load_file_path))
        {
            return;
        }
        file = File.Open(load_file_path, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        logs = (LinkedList<string>)formatter.Deserialize(file);
        file.Close();
        file.Dispose();
        log_board_size = logs.Count * (label_height + upper_border_margin);
    }
    //Call this function if you want to save the logs
    public void SaveBin()
    {
        FileStream file;
        string file_name = string.Format("{0}.bin", console_name);
        string load_file_path = string.Format("{0}/{1}", save_directory, file_name);
        if (!File.Exists(load_file_path))
        {
            file = File.Open(load_file_path, FileMode.CreateNew);
        }
        else
        {
            file = File.Open(load_file_path, FileMode.Truncate);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(file, logs);

        file.Close();
        file.Dispose();
    }
    //Call this function to clear up the logs
    public void ClearLogs()
    {
        logs.Clear();
    }
    //Call this function , whenever you feel to change to opacity of the console window
    public void SetAlpha(float alpha_value)
    {
        default_alpha_value = GUI.color.a;
        current_alpha_value = alpha_value;
    }

    private void Label(string str)
    {
        if (logs.Count > MaxLogCount)
            ClearLogs();
        current_label_height = get_line_count(str, label_rect.width, character_width) * label_height;
        label_rect.y += previous_label_height + label_spacing;
        label_rect.height = current_label_height;
        previous_label_height = current_label_height;
        GUI.Box(label_rect, "");
        GUI.Label(label_rect, str, LabelStyle);
    }
    //Call this to add warning in the console window
    public void Warning(string wrn_string)
    {
        if (logs.Count > MaxLogCount)
            ClearLogs();
        logs.AddFirst(string.Format("<color=#{0}>{1}</color>", warning_color_string, wrn_string));
        float height = get_line_count(wrn_string, label_rect.width, character_width) * label_height;
        if ((log_board_size + height) > (Size.y - upper_border_margin))
        {
            scroll_view += Vector2.up * (log_board_size + height - Size.y + upper_border_margin + label_spacing);
        }
    }
    private int get_line_count(string str, float width, float character_width)
    {
        int count = str.Length;
        int new_line_count = 0;
        for (int i = 0; i < count; i++)
            if (str[i] == '\n')
                new_line_count++;
        return (int)((count * character_width) / width) + 2 + new_line_count;
    }
    //Call this to add Error in the console window
    public void Error(string err_string)
    {
        if (logs.Count > MaxLogCount)
            ClearLogs();
        logs.AddFirst(string.Format("<color=#{0}>{1}</color>", error_color_string, err_string));
       
        float height = get_line_count(err_string, label_rect.width, character_width) * label_height;
        if ((log_board_size + height) > (Size.y - upper_border_margin))
        {
            scroll_view += Vector2.up * (log_board_size + height - Size.y + upper_border_margin + label_spacing);
        }
    }
    //call this to add Log in the console window
    public void Log(string log_string)
    {
        if (logs.Count > MaxLogCount)
            ClearLogs();
        logs.AddFirst(log_string);
        float height = get_line_count(log_string, label_rect.width, character_width) * label_height;
        if ((log_board_size + height) > (Size.y - upper_border_margin))
        {
            scroll_view += Vector2.up * (log_board_size + height - Size.y + upper_border_margin + label_spacing);
        }
    }
}

