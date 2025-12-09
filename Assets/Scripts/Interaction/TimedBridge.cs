using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBridge : TimedMechanism
{
    public BridgeMover bridgeMover;

    protected override void OnActivated()
    {
        if (bridgeMover != null)
            bridgeMover.isActive = true;
    }

    protected override void OnDeactivated()
    {
        if (bridgeMover != null)
            bridgeMover.isActive = false;
    }
}
