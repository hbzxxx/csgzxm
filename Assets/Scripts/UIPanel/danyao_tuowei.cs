using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class danyao_tuowei : SingleViewBase
{

    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.position = (Vector3)args[0];
    }
}
