using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 路径点
/// </summary>
public class RoutePointView : SingleViewBase
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = (Vector2)args[0];
    }
}
