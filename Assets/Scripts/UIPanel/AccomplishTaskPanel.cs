using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccomplishTaskPanel : PanelBase
{
    public float animTime = 1.5f;
    public float animTimer = 0;
    public bool startAnim = false;


    public override void Init(params object[] args)
    {
        base.Init(args);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        startAnim = true;
        animTimer = 0;
    }

    private void Update()
    {
        if (startAnim)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= animTime)
            {
                PanelManager.Instance.ClosePanel(this);
                startAnim = false;
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        startAnim = false;
    }
}
