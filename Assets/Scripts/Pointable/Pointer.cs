using UnityEngine;
using System;


public class Pointer : IManualUpdatable
{
    FadeController alphaController;
    public Transform transform;
    public Action OnHide;
    private Transform target_transform;
    bool IsShown;
    IPointable pointable;
    Vector2 offset;

    public void SetTarget(Transform target)
    {
        target_transform = target;
        pointable = target_transform.GetComponent<IPointable>();
#if DEBUG_MODE
        if (pointable == null)
            GameManager.GetLogManager().LogError("Transform traget is not IPointable, at SetTarget function");
#endif
    }
    public void SetSortingLayer(int sorting_layer_id, int sorting_order = 0)
    {
        SpriteRenderer renderer = transform.GetComponent<SpriteRenderer>();
        renderer.sortingLayerID = sorting_layer_id;
        renderer.sortingOrder = sorting_order;
    }
    public Pointer(Sprite sprite, float scale = 0.5f, bool isHideOnAwake = true)
    {
        GameObject pointerObj = new GameObject("Pointer");
        pointerObj.transform.localScale *= scale;
        SpriteRenderer renderer = pointerObj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        AlphaAdapter targetRenderAdapter = new SpriteRendererAlphaAdapter(renderer);
        IsShown = !isHideOnAwake;
        alphaController = new FadeController(targetRenderAdapter, 0.5f);
        alphaController.OnFadeOut += () =>
        {
            IsShown = false; if (OnHide != null) OnHide();
        };
        transform = pointerObj.transform;
        if (isHideOnAwake)
            alphaController.InstantFadeOut();
        else
            alphaController.InstantFadeIn();
    }

    public bool IsPoints(IPointable pointable)
    {
        return pointable == this.pointable;
    }

    public void Show(Vector2 offset)
    {
        IsShown = true;
        alphaController.FadeIn();
        this.offset = offset;
    }
    public void Hide()
    {
        //Debug.Break();
        //alphaController.OnFadeOut += () =>
        //{
        //    IsShown = false; if (OnHide != null) OnHide();
        //};
        alphaController.FadeOut();
    }

    public bool IsRunning { get { return IsShown; } }
    public void Update()
    {
        transform.position = (Vector2)target_transform.position + offset;
        alphaController.Update();
    }
}
