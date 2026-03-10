using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentAddAnimGroup : SingleViewBase
{
    public SkeletonGraphic ske1;
    public SkeletonGraphic ske2;

    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public void Play(int quality)
    {
        ske1.AnimationState.SetAnimation(0, "animation" + quality, false);
        ske1.AnimationState.AddAnimation(0, "animation" + (quality+5), false,0);

        ske2.AnimationState.SetAnimation(0, "animation" + quality, false);
        ske2.AnimationState.AddAnimation(0, "animation" + (quality + 5), false, 0);
    }
}
