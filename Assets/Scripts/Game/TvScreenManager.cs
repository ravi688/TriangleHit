#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TvScreenManager : MonoBehaviour {

    [SerializeField]
    private EconomyCollectionPage star_collection_page;
    [SerializeField]
    private EconomyCollectionPage coin_collection_page;

    private void Start()
    {
        star_collection_page.Activate();
    }
}
