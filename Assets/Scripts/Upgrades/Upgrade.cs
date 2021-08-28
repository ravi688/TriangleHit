
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class Upgrade : IPurchasable, IStreamSavable, IStreamLoadable
{
    //Serialized Fields
    [SerializeField]
    private EconomyType economy_type = EconomyType.Star;
    public string name;
    public int max_upgrade_steps = 5;
    public int base_cost = 1;

    //Non Serialized Fields
    public EconomyType EconomyType { get { return economy_type; } }
    public int Cost { get { return base_cost + cost_function(upgraded_steps, max_upgrade_steps); } }
    public string PurchaseID
    {
        get
        {
#if DEBUG_MODE
            if(cost_function == null)
            {
                GameManager.GetLogManager().LogError("Cost Function is null");
            }
#endif
            return string.Format("purchase-{0}-upgrade-{1}-{2}", base_cost + cost_function(upgraded_steps, max_upgrade_steps), name, upgraded_steps);
        }
    }
    public delegate int CostFunction(int upgraded_steps, int max_upgrade_steps);
    [NonSerialized]
    public CostFunction cost_function;
    [NonSerialized]
    public int upgraded_steps = 0;

    public void LoadFromStream(Stream file_stream)
    {
        byte[] bytes = new byte[sizeof(int)];
        file_stream.Read(bytes, 0, bytes.Length);
        upgraded_steps = BitConverter.ToInt32(bytes, 0);
    }
    public void SaveToStream(Stream file_stream)
    {
        byte[] bytes = BitConverter.GetBytes(upgraded_steps);
        file_stream.Write(bytes, 0, bytes.Length);
    }
}