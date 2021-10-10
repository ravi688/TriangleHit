using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ; 

public class CLog : MonoBehaviour {


    public static CLog instance;
    private static Text _text;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        _text = GetComponent<Text>(); 
    }
    public static void msg(string str)
    {
        _text.text = str; 
    }

}
