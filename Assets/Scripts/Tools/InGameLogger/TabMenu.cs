using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To use this you have to create an object of It
/// Call OnGUIStart when the GUIStarts
/// Call OnGUIUpdate per GUI Update
/// </summary>


public class TabMenu
{
    public enum TabMenuAlignment
    {
        HorizontalLeft,
        HorizontalRight,
        VerticalUp,
        VerticalDown
    }

    public float spacing = 10f;                                         //how much the spacing between the tabs should be , should be changed before the Call of SetAlignment function
    public float tab_width = 150;                                       //how much the tab width should be, should be changed before the Call of SetAlignment function
    public float tab_height = 50;                                       //how much the tab height should be, should be changed before the Call of SetAlignment function

                                                    

    //position of the window , from top left corner of the screen
    //change this before calling OnGUIStart
    public Vector2 position = new Vector2(10, Screen.height - 10);      
    public GUIStyle tab_style;                                          //Can be changed any time
    public bool draw_background = false;                                //Can be changed any time, whether background should be drawn or not
    public int font_size = 30;                                          //Should be changed before the call of OnGUIStart function and SetAlignment function
    public float background_horizontal_offset = 10;                     //Should be changed before the call of OnGUIStart function and SetAlignment function
    public float background_vertical_offset = 10;                       //Should be changed before the call of OnGUIStart function and SetAlignment function

    private int tab_count;
    private Rect[] tab_rects;
    private Action<int , bool> call_back;
    private Rect background_rect;
    private string[] offstate_tab_labels;
    private string[] onstate_tab_labels;
    private string[] tab_labels;

    private bool is_alignment_set = false;

    //off_state_map , when the Tabs are not pressed
    //on_state_map , when the Tabs are pressed
    //Call back function when a perticular Tab is pressed
    public TabMenu(string[] off_state_map, string[] on_state_map, Action<int, bool> call_back)
    {
        this.call_back = call_back;
        tab_count = off_state_map.Length;
        offstate_tab_labels = off_state_map;
        onstate_tab_labels = on_state_map;
        tab_labels = new string[tab_count];
        for (int i = 0; i < tab_count; i++)
            tab_labels[i] = (string)offstate_tab_labels[i].Clone(); 

        tab_rects = new Rect[tab_count];
    }
    //This function Can be called any time
    public void SetAlignment(TabMenuAlignment alignment)
    {
        switch (alignment)
        {
            case TabMenuAlignment.VerticalDown:
                for (int i = 0; i < tab_count; i++)
                {
                    tab_rects[i] = new Rect(position.x, position.y + (tab_height + spacing) * i, tab_width, tab_height);
                }
                background_rect = new Rect(
                    position.x - background_horizontal_offset,
                    position.y - background_vertical_offset,
                    tab_width + background_horizontal_offset * 2,
                    tab_height * tab_count + (tab_count - 1) * spacing + background_vertical_offset * 2
                    );
                break;
            case TabMenuAlignment.VerticalUp:
                for (int i = 0; i < tab_count; i++)
                {
                    tab_rects[i] = new Rect(position.x, position.y - tab_height * (i + 1) - spacing * i, tab_width, tab_height);
                }

                background_rect = new Rect(
                    position.x - background_horizontal_offset,
                    position.y - tab_height * tab_count - (tab_count - 1) * spacing - background_vertical_offset,
                    tab_width + background_horizontal_offset * 2,
                    tab_height * tab_count + (tab_count - 1) * spacing + background_vertical_offset * 2
                    );
                break;
            case TabMenuAlignment.HorizontalLeft:
                for (int i = 0; i < tab_count; i++)
                {
                    tab_rects[i] = new Rect(position.x - tab_width * (i + 1) - spacing * i, position.y - tab_height, tab_width, tab_height);
                }
                background_rect = new Rect(
                   position.x - tab_count * tab_width - (tab_count - 1) * spacing - background_horizontal_offset,
                   position.y - tab_height - background_vertical_offset,
                   tab_count * tab_width + (tab_count - 1) * spacing + background_horizontal_offset * 2,
                   tab_height + background_vertical_offset * 2
                   );
                break;
            case TabMenuAlignment.HorizontalRight:
                for (int i = 0; i < tab_count; i++)
                {
                    tab_rects[i] = new Rect(position.x + (tab_width + spacing) * i, position.y - tab_height, tab_width, tab_height);
                }
                background_rect = new Rect(
                    position.x - background_horizontal_offset,
                    position.y - tab_height - background_vertical_offset,
                    tab_width * tab_count + spacing * (tab_count - 1) + background_horizontal_offset * 2,
                    tab_height + background_vertical_offset * 2
                    );
                break;
        }
        is_alignment_set = true;
    }
    //Must be called when the GUIStarts
    public void OnGUIStart()
    {
        if (!is_alignment_set)
            SetAlignment(TabMenuAlignment.VerticalUp);
        tab_style = new GUIStyle(GUI.skin.button);
        tab_style.fontSize = font_size;
        tab_style.fontStyle = FontStyle.Bold;
    }
    //Must be called per GUI update
    public void OnGUIUpdate()
    {
        if (draw_background)
        {
            GUI.Box(background_rect, "");
        }
        for (int i = 0; i < tab_count; i++)
        {
            if (GUI.Button(tab_rects[i], tab_labels[i], tab_style))
            {

                if (tab_labels[i].Equals(offstate_tab_labels[i]))
                {
                    tab_labels[i] = onstate_tab_labels[i];
                    call_back(i, false); 
                } 
                else
                    if (tab_labels[i].Equals(onstate_tab_labels[i]))
                    {
                        tab_labels[i] = offstate_tab_labels[i];
                        call_back(i, true); 
                    } 
            }
        }
    }
}