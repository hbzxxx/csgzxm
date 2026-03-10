using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectSingleParent : SingleViewBase
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

  
}
