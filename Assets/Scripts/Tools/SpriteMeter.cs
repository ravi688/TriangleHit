using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;

public class SpriteMeter
{
    BarMeter meter; 
    AlphaAdapter[] adapters;
    float inactiveOpacity; 
    public int Value
    {
        get
        {
            return (int)meter.Value;
        }
        set
        {
            meter.Value = (int)value;
            SetSpritesWithValue((int)meter.Value);  
        }
    }

    void SetSpritesWithValue(int value)
    {
        for (int i = 0; i < adapters.Length; i++)
        {
            if (i < value)
                  adapters[i].SetAlpha(1);
            else
                adapters[i].SetAlpha(inactiveOpacity);
        } 
    }
    public SpriteMeter(Rect meterPixelRect,int spriteSize, Sprite sprite,SpriteAlignment alignment  = SpriteAlignment.LeftCenter, Transform parent = null, int maxValue = 3, float offset = 10, float inactiveOpacity = 0.2f)
    {
       
        adapters = new ImageAlphaAdapter[maxValue];
        meter = new BarMeter(0, maxValue, maxValue);
        this.inactiveOpacity = inactiveOpacity; 
        float eachImageXSize =  spriteSize; 

        if (parent == null)
        {
            GameObject meterObj = new GameObject("SpriteMeter");
            parent = meterObj.transform; 
            parent.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
            parent.transform.position = Vector2.zero; 
            parent.transform.localPosition = new Vector2(-Screen.width * 0.5f + eachImageXSize * 0.5f + meterPixelRect.position.x
                , Screen.height * 0.5f - meterPixelRect.size.y * 0.5f - meterPixelRect.position.y);
        } 
        for (int i = 0; i < maxValue; i++)
        {
            GameObject obj = new GameObject(i.ToString());
            obj.transform.SetParent(parent.transform);
            obj.transform.position = Vector2.zero; 
            Image image = obj.AddComponent<Image>();
            image.sprite = sprite; 
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, eachImageXSize);
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, meterPixelRect.size.y);
            adapters[i] = new ImageAlphaAdapter(image);
            adapters[i].SetAlpha(1);
            switch (alignment)
            {
                case SpriteAlignment.Center :
                    image.rectTransform.localPosition = Vector2.right *( (eachImageXSize * 1.5f + offset ) * i - meterPixelRect.size.x * 0.5f);
                    break;
                case SpriteAlignment.LeftCenter :
                    image.rectTransform.localPosition = Vector2.right * (eachImageXSize * 1.5f + offset) * i;
                    break;
                case SpriteAlignment.RightCenter :
                    image.rectTransform.localPosition = Vector2.left * ((eachImageXSize * 1.5f + offset) * i + meterPixelRect.size.x);
                    break;
                default :
                    Debug.LogError("Invalid SpriteAlignment in SpriteMeter");
                    break;
            }

            //image.rectTransform.localPosition = Vector2.right * (eachImageXSize + offset) * i; 
        }
    }

}
