using UnityEngine;
using System.Collections;
using System;

public class Economy : MonoBehaviour
{
    [NonSerialized]
    public EconomyCollectionPage economyCollectionPage;

    void OnDestroy()
    {
        economyCollectionPage.IncrementEconomy();
    }
}
