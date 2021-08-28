using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {
    public static Notification instance; 
    [TextArea]
    public string[] Notifications;

    Text text; 
    int num_notifications;
    int curr_notification_index =0; 
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        text = GetComponentInChildren<Text>(true);
        curr_notification_index =PlayerPrefs.GetInt("NotificationIndex");
    }
	// Use this for initialization
	void Start () {
        num_notifications = Notifications.Length;
        text.text = Notifications[curr_notification_index];
        PlayerPrefs.SetInt("NotificationIndex", (curr_notification_index + 1) % num_notifications);
	}
	
}
