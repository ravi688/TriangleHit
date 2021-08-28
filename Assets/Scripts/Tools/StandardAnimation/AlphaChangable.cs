using System;
using UnityEngine;
using UnityEngine.UI;


public abstract class AlphaAdapter
{
    public abstract void SetAlpha(float value);
    public abstract float GetAlpha();
}
public abstract class AlphaAdapter<T> : AlphaAdapter where T : UnityEngine.Object
{
    public abstract void SetTarget(T target);
}


public class TextMeshAlphaAdapter : AlphaAdapter<TextMesh>
{
    public TextMesh textRenderer { get; set; }

    public override void SetTarget(TextMesh target)
    {
        textRenderer = target;
    }
    public TextMeshAlphaAdapter(TextMesh renderer)
    {
        textRenderer = renderer; 
    }
    public override void SetAlpha(float value)
    {
        textRenderer.color = new Color(textRenderer.color.r, textRenderer.color.g, textRenderer.color.b, value); 
    }
    public override float GetAlpha()
    {
        return textRenderer == null ? 0: textRenderer.color.a; 
    }

}

public class UITextAlphaAdapter : AlphaAdapter<Text>
{
    Text textRenderer;

    public override void SetTarget(Text target)
    {
        textRenderer = target;
    }
    public UITextAlphaAdapter(Text renderer)
    {
        textRenderer = renderer; 
    }
    public override void SetAlpha(float value)
    {
        textRenderer.color = new Color(textRenderer.color.r, textRenderer.color.g, textRenderer.color.b, value); 
    }
    public override float GetAlpha()
    {
        return textRenderer == null ? 0 : textRenderer.color.a; 
    }
}
public class ImageAlphaAdapter : AlphaAdapter<Image>
{
    Image imageRenderer;

    public override void SetTarget(Image target)
    {
        imageRenderer = target;
    }
    public ImageAlphaAdapter() { }
    public ImageAlphaAdapter(Image renderer)
    {
        imageRenderer = renderer; 
    }
    public override void SetAlpha(float value)
    {
        if (!imageRenderer) return;
        imageRenderer.color = new Color(imageRenderer.color.r, imageRenderer.color.g, imageRenderer.color.b, value); 
    }
    public override float GetAlpha()
    {
        return imageRenderer == null ? 0 : imageRenderer.color.a; 
    }
}
public class SpriteRendererAlphaAdapter : AlphaAdapter<SpriteRenderer>
{
    SpriteRenderer spriteRenderer;
    public override void SetTarget(SpriteRenderer target)
    {
        spriteRenderer = target;
    }
    public SpriteRendererAlphaAdapter(SpriteRenderer renderer)
    {
        spriteRenderer = renderer; 
    }

    public override void SetAlpha(float value)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value);
    }
    public override float GetAlpha()
    {
        return spriteRenderer == null ? 0 : spriteRenderer.color.a; 
    }
} 
public class CanvasGroupAlphaAdapter : AlphaAdapter<CanvasGroup>
{
     CanvasGroup canvasGroup;
    public override void SetTarget(CanvasGroup target)
    {
        canvasGroup = target;
    }
    public CanvasGroupAlphaAdapter(CanvasGroup renderer)
    {
        canvasGroup = renderer; 
    }

    public override void SetAlpha(float value)
    {
        canvasGroup.alpha = value; 
    }
    public override float GetAlpha()
    {
        return canvasGroup == null ? 0 : canvasGroup.alpha; 
    }
}