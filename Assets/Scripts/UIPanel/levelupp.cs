using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelupp : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = new Vector3(0, 0, -200);
    }
}
