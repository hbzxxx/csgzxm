using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qiehuanrenwutexiao : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = (Vector3)args[0];
    }
}
