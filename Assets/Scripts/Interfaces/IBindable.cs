
using System.Collections.Generic;
using UnityEngine;

public interface IBindable
{
    bool IsBinded { get; }
    List<IBindable> BindedObjects { get; set; }
}