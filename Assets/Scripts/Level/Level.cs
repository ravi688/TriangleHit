#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "Custom/Level")]
public class Level : ScriptableObject, IPurchasable
{
    public EconomyType EconomyType { get { return EconomyType.Coin; } }
    public int Cost { get { return cost; } }
    public string PurchaseID { get { return string.Format("purchase-{0}-level-{1}-{2}", cost, name, build_index); } }

    public string Name { get { return name; } }
    public int BuildIndex { get { return build_index; } }
    public Sprite ThumbnailSprite { get { return thumbnail_sprite; } }
    public Sprite SplashSprite { get { return splash_sprite; } }

    [SerializeField]
    private new string name;
    [SerializeField]
    private int build_index;
    [SerializeField]
    private int cost;
    [SerializeField]
    private Sprite thumbnail_sprite;
    [SerializeField]
    private Sprite splash_sprite;
}
