using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoChanShowPanel : PanelBase
{
    public float animTime = 1.5f;
    public float animTimer = 0;

    bool startAnim = false;
    public SkeletonGraphic ske;
    public override void Init(params object[] args)
    {
        base.Init(args);
        animTimer = 0;
        startAnim = true;
    }
   

    private void Update()
    {

        if (startAnim)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animTime)
            {
                
                startAnim = false;
                PanelManager.Instance.ClosePanel(this);
            }
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ske.AnimationState.SetAnimation(0, "lingshidonghua", false);
    }

    public override void Clear()
    {
        base.Clear();
        startAnim = false;
    }
}
