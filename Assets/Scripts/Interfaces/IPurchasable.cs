
using System;

public interface IPurchasable
{
    EconomyType EconomyType { get; }
    int Cost { get; }

    /*ID format: purchase-<cost>-<category>-<name>-<meta_data>*/
    string PurchaseID { get; }
}