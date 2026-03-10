using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMai : FinishKillEffect
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        Transform trans = args[0] as Transform;
        transform.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, -547);
    }
}
