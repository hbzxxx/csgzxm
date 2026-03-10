using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dujie_xishou : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        Vector3 localPos =(Vector3)args[0];
        transform.localPosition = new Vector3(localPos.x, localPos.y, -300);
    }
}
