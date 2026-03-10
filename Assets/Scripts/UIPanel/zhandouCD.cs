using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zhandouCD : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        Vector3 pos = (Vector3)args[0];
        transform.position = new Vector3(pos.x, pos.y, transform.parent.position.z);
    }
}
