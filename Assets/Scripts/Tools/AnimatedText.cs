
using UnityEngine;
using UnityEngine.UI;

public class AnimatedText : IManualUpdatable
{
    StandardAnimationPlayer m_engine;
    public Text m_UIText
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public TextMesh m_TextMesh
#if !RELEASE_MODE
    { get; set; } 
#else
        ;
#endif

    public bool IsRunning { get { return m_engine.IsRunning; } }

    public static AnimatedText UIText(StandardAnimation animation, Vector2 size , Color color , int fontSize = 100, TextAnchor anchor = TextAnchor.MiddleCenter,
        FontStyle style = FontStyle.Bold)
    {
        GameObject textObj = new GameObject("PopUpText");
        textObj.transform.localPosition = Vector2.zero;
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
#if DEBUG_MODE
        if (!canvas)
            GameManager.GetLogManager().LogError("There is no canvas");
#endif
        textObj.transform.SetParent(canvas.transform);
        Text uiText = textObj.AddComponent<Text>();
        AlphaAdapter adapter = new UITextAlphaAdapter(uiText);
        uiText.color = color;
        adapter.SetAlpha(0);
        StandardAnimationPlayer engine = new StandardAnimationPlayer();
        engine.InitializeWith(adapter, textObj.transform, animation);
        uiText.alignment = anchor;
        uiText.font = Font.CreateDynamicFontFromOSFont("Arial",1); 
        uiText.fontSize = fontSize;
        uiText.fontStyle = style;
        uiText.raycastTarget = false; 
        uiText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        uiText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        return new AnimatedText(engine, uiText);
    }
    public static AnimatedText TextMesh(StandardAnimation animation, Color color ,  int fontSize = 100, float charSize = 0.1f,  TextAnchor anchor = TextAnchor.MiddleCenter,
        TextAlignment alignment = TextAlignment.Center, FontStyle style = FontStyle.Bold)
    {
        GameObject textObj = new GameObject("PopUpText");
        textObj.transform.localPosition = Vector2.zero;
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        AlphaAdapter adapter = new TextMeshAlphaAdapter(textMesh);
        textMesh.color = color; 
        adapter.SetAlpha(0);
        StandardAnimationPlayer engine = new StandardAnimationPlayer();
        engine.InitializeWith(adapter, textObj.transform, animation);
        textMesh.fontSize = fontSize; 
        textMesh.fontStyle = style;
        textMesh.alignment = alignment;
        textMesh.anchor = anchor;
        textMesh.characterSize = charSize; 
        return new AnimatedText(engine, textMesh);
    }

    private AnimatedText(StandardAnimationPlayer engine, Text uiText)
    {
        m_UIText = uiText;
        m_engine = engine;
    }
    private AnimatedText(StandardAnimationPlayer engine, TextMesh textMesh)
    {
        m_engine = engine;
        m_TextMesh = textMesh;

    }
    public void Update() { m_engine.Update();  }
    public void PopUp(string str, Vector2 position, Vector2 scale, Quaternion rotation)
    {
        if (m_UIText)
            m_UIText.text = str;
        else
            m_TextMesh.text = str;
        m_engine.PlayAtPosition(position, scale, rotation);
    }

}
