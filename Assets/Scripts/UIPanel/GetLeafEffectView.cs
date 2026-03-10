using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLeafEffectView : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.position = (Vector3)args[0];
    }
}
