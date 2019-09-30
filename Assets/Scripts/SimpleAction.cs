using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SerializeField]
public class SimpleAction : InteractibleAction
{
    public UnityEvent Tapped;
    public override void PerformAction()
    {
        Tapped?.Invoke();
    }

}
