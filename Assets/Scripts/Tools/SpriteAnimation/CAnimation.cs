using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CAnimation" , menuName = "CAnimation")]
public class CAnimation : ScriptableObject {

    public new string name; 
    public Sprite[] frames;
    public bool isLoop = true;
    public int fps = 15;
    public int currentFrameIndex = 0;
}
