using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPView : SingleViewBase
{
    public Spine.Unity.SkeletonGraphic ske;
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = (Vector2)args[0];
        ske.AnimationState.SetAnimation(0, "daiji", true);
    }
}
