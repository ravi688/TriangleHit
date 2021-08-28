#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncourageText : MonoBehaviour {

    [SerializeField]
    private StandardAnimation anim;

    private UnityEngine.UI.Text  m_text;
    private string m_string;
    private AlphaAdapter adapter;
    private StandardAnimationPlayer animPlayer;
    private Vector3 initial_pos;
    private Vector3 initial_scale;
    private Quaternion initial_rotation;
      
    public string text
    {
        get
        {
            return m_string; 
        }
        set
        {
            m_string = value;
            m_text.text = m_string; 
            animPlayer.PlayAtPosition(initial_pos, initial_scale, initial_rotation); 
        } 
    }

    private void Awake()
    {

        initial_pos = this.transform.localPosition;
        initial_scale = this.transform.localScale;
        initial_rotation = this.transform.rotation; 

        m_text = GetComponent<UnityEngine.UI.Text>() ; 
        adapter = new UITextAlphaAdapter(m_text) ; 
        adapter.SetAlpha(0) ; 
        animPlayer = new StandardAnimationPlayer() ;
        animPlayer.InitializeWith(adapter,this.transform, anim);
        animPlayer.OnEnd =
            delegate()
            {
                this.gameObject.SetActive(false); 
            }; 
    }
    private void Update()
    {
        GameLoop.Update(animPlayer);

    }

  


}
