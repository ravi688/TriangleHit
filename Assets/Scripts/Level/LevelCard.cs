using UnityEngine;
using UnityEngine.UI;

public class LevelCard<T> : Clickable<T>
{
    public Sprite Thumbnail
    {
        get { return thumbnail_image.sprite; }
        set { thumbnail_image.sprite = value; }
    }
    public bool IsLocked
    {
        get { return lock_object.activeSelf; }
        set { lock_object.SetActive(value); }
    }
    private GameObject lock_object;
    private Image thumbnail_image;

    public LevelCard(Transform transform) : base(transform, 100)
    {
        lock_object = transform.Find("Lock").gameObject;
        thumbnail_image = transform.Find("Thumbnail").GetComponent<Image>();
    }
}
