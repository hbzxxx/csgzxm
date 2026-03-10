using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        Vector3 pos = (Vector3)args[0];
        transform.position = pos;
    }

}
