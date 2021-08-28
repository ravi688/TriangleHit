using UnityEngine;
using System;

public class Rope : IManualUpdatable
{
    public bool IsRunning { get; set; }
    public bool IsSomethingBinded { get; private set; }
    public float BindOffset = 0.7f;
    public Action OnHide;
    private SpringJoint2D joint;

    public Transform RopeBodyTransform;
    public Transform Pin1Transform;
    public Transform Pin2Transform;

    private Transform BindedFromBodyTransform;
    private Transform BindedToBodyTransform;

    private RopeSettings RopeSettings;

    private FadeController RopeFadeController;
    private FadeController Pin1FadeController;
    private FadeController Pin2FadeController;

    private SpriteRenderer Pin1Renderer;
    private SpriteRenderer Pin2Renderer;
    private SpriteRenderer RopeRenderer;

    private IBindable bindable1;
    private IBindable bindable2;

    private float bind_offset;

    public Rope(RopeSettings Settings)
    {
        RopeSettings = Settings;
        IsSomethingBinded = false;
        CreateRopeObjects();
    }
    public void Reset()
    {
        IsSomethingBinded = false;
    }
    private void CreateRopeObjects()
    {
        GameObject RopeContainer = new GameObject("Rope");
        //Rop Body
        GameObject RopeRendererObj = new GameObject("RopeRenderer");
        Texture2D texture = new Texture2D(4, 4);
        for (int i = 0; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, Color.white);
        texture.Apply();
        Sprite RopeBodySprite = Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
        RopeRenderer = RopeRendererObj.AddComponent<SpriteRenderer>();
        RopeRenderer.sprite = RopeBodySprite;
        RopeRenderer.color = RopeSettings.Color;
        AlphaAdapter RopeAdapter = new SpriteRendererAlphaAdapter(RopeRenderer);
        RopeAdapter.SetAlpha(0);
        RopeFadeController = new FadeController(RopeAdapter, RopeSettings.FadeDuration);
        RopeFadeController.OnFadeOut = () => IsRunning = false;
        RopeBodyTransform = RopeRendererObj.transform;


        //Pin1 and Pin2
        GameObject Pin1RendererObj = new GameObject("Pin1");
        GameObject Pin2RendererObj = new GameObject("Pin2");
        Pin1Transform = Pin1RendererObj.transform;
        Pin2Transform = Pin2RendererObj.transform;
        Pin1Renderer = Pin1RendererObj.AddComponent<SpriteRenderer>();
        Pin2Renderer = Pin2RendererObj.AddComponent<SpriteRenderer>();
        Pin1Renderer.sprite = RopeSettings.HookSprite;
        Pin2Renderer.sprite = RopeSettings.HookSprite;

        AlphaAdapter Pin1Adapter = new SpriteRendererAlphaAdapter(Pin1Renderer);
        AlphaAdapter Pin2Adapter = new SpriteRendererAlphaAdapter(Pin2Renderer);
        Pin1Adapter.SetAlpha(0);
        Pin2Adapter.SetAlpha(0);
        Pin1FadeController = new FadeController(Pin1Adapter, RopeSettings.FadeDuration);
        Pin2FadeController = new FadeController(Pin2Adapter, RopeSettings.FadeDuration);

        Pin1Transform.SetParent(RopeContainer.transform);
        Pin2Transform.SetParent(RopeContainer.transform);
        RopeBodyTransform.SetParent(RopeContainer.transform);

        Pin1Transform.localScale *= RopeSettings.HookScale;
        Pin2Transform.localScale *= RopeSettings.HookScale;

    }
    public void OnDestroy()
    {
        if (Pin1Transform != null)
            MonoBehaviour.Destroy(Pin1Transform.gameObject);
        if (Pin2Transform != null)
            MonoBehaviour.Destroy(Pin2Transform.gameObject);
        if (RopeBodyTransform != null)
            MonoBehaviour.Destroy(RopeBodyTransform.gameObject);
    }

    public bool IsBinds(IBindable bindable1, IBindable bindable2)
    {
        return ((this.bindable1 == bindable1) && (this.bindable2 == bindable2)) || ((this.bindable1 == bindable2) && (this.bindable2 == bindable1));
    }
    public void Bind(Rigidbody2D From, Rigidbody2D To)
    {
        if (IsSomethingBinded)
        {
            Debug.Log("<color=yellow>Something is Already binded , First UnBind It</color>");
            return;
        }
        bindable1 = From.GetComponent<IBindable>();
        bindable2 = To.GetComponent<IBindable>();

        BindedFromBodyTransform = From.transform;                               //attached to pin1 
        BindedToBodyTransform = To.transform;                                   //attached to pin2

        if ((joint = From.GetComponent<SpringJoint2D>()) == null)
            joint = From.gameObject.AddComponent<SpringJoint2D>();
        if (!joint.enabled)
            joint.enabled = true;
        joint.autoConfigureDistance = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = To;
        joint.anchor = Vector2.up * (bind_offset = (From.GetComponent<Collider2D>().bounds.extents.y - BindOffset));
        joint.frequency = RopeSettings.Frequency;
        joint.dampingRatio = RopeSettings.DampingRatio;
        joint.distance = VectorUtility.GetDistance(From.position + (Vector2)From.transform.up * bind_offset, To.position);
        IsSomethingBinded = true;
        RopeFadeController.OnFadeOut = OnHide;
        RopeFadeController.FadeIn();
        Pin1FadeController.FadeIn();
        Pin2FadeController.FadeIn();
        RopeBodyTransform.localScale = Vector3.one;

        SpriteRenderer fromBodyRenderer = From.GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer toBodyRenderer = To.GetComponentInChildren<SpriteRenderer>();

        int LayerId;
        int Order;

        if (SortingLayer.GetLayerValueFromID(fromBodyRenderer.sortingLayerID) > SortingLayer.GetLayerValueFromID(toBodyRenderer.sortingLayerID))
        {
            LayerId = fromBodyRenderer.sortingLayerID;
            Order = fromBodyRenderer.sortingOrder;
        }
        else if (SortingLayer.GetLayerValueFromID(fromBodyRenderer.sortingLayerID) < SortingLayer.GetLayerValueFromID(toBodyRenderer.sortingLayerID))
        {
            LayerId = toBodyRenderer.sortingLayerID;
            Order = toBodyRenderer.sortingOrder;
        }
        else
        {
            Order = fromBodyRenderer.sortingOrder > toBodyRenderer.sortingOrder ? fromBodyRenderer.sortingOrder : toBodyRenderer.sortingOrder;
            LayerId = fromBodyRenderer.sortingLayerID;
        }

        Order++;
        RopeRenderer.sortingLayerID = LayerId;
        RopeRenderer.sortingOrder = Order;
        Order++;
        Pin1Renderer.sortingLayerID = LayerId;
        Pin1Renderer.sortingOrder = Order;
        Pin2Renderer.sortingLayerID = LayerId;
        Pin2Renderer.sortingOrder = Order;

        IsRunning = true;
    }
    public void UnBind(bool IsDestroySpringJoint = false)
    {
        if (!IsSomethingBinded) return;
        if (joint && IsDestroySpringJoint)
            Component.Destroy(joint);
        else
        {
            joint.connectedBody = null;
            joint.enabled = false;
        }
        IsSomethingBinded = false;
        HideRope();
    }

    private void HideRope()
    {
        RopeFadeController.FadeOut();
        Pin1FadeController.FadeOut();
        Pin2FadeController.FadeOut();
    }
    public void Update()
    {
        GameLoop.Update(Pin1FadeController);
        GameLoop.Update(Pin2FadeController);
        GameLoop.Update(RopeFadeController);

        if (BindedToBodyTransform == null || BindedFromBodyTransform == null)
            UnBind(true);
        else
            RefreshRopeRenderer();
    }
    private Vector2 binded_ToBodyPosition;
    private Vector2 binded_FromBodyPosition;
    void RefreshRopeRenderer()
    {
        binded_ToBodyPosition = BindedToBodyTransform.position;
        binded_FromBodyPosition = BindedFromBodyTransform.position + BindedFromBodyTransform.up * bind_offset;

        RopeBodyTransform.position = (binded_ToBodyPosition + binded_FromBodyPosition) * 0.5f;
        RopeBodyTransform.localScale = new Vector3(RopeSettings.RopeWidth, VectorUtility.GetDistance(binded_FromBodyPosition, binded_ToBodyPosition), 1);
        RopeBodyTransform.Rotate(Vector3.forward * VectorUtility.GetSignedAngle(RopeBodyTransform.up, binded_ToBodyPosition - binded_FromBodyPosition));
        Pin1Transform.position = binded_FromBodyPosition;
        Pin2Transform.position = binded_ToBodyPosition;
    }

}
