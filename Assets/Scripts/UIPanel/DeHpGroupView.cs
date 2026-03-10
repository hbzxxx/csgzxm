using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一组掉血特效
public class DeHpGroupView : FinishKillEffect
{

    public bool cannotContainNewTxt;//不能包含更多掉血txt了
    public float validTime = 0.6f;//0.6秒内可以有更多txt
    public float validTimer = 0;//计时器

    public override void Init(params object[] args)
    {
        base.Init(args);
        validTimer = 0;
        cannotContainNewTxt = false;
    }

    public override void Update()
    {
        base.Update();
        if (!cannotContainNewTxt)
        {
            validTimer += Time.deltaTime;
            if (validTimer >= validTime)
            {
                cannotContainNewTxt = true;
            }
        }
    }

    public void AddDeHpTxt(AttackResData attackResData)
    {
        int existedDeHpCount = transform.childCount;
        int baseYPos = 0;
        if (existedDeHpCount > 0)
        {
            baseYPos = (int)transform.GetChild(existedDeHpCount - 1).transform.localPosition.y;
        }
        DeHpTxtView deHpTxtView = PanelManager.Instance.OpenSingle<DeHpTxtView>(transform, baseYPos, attackResData);

    }



    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(transform);
    }

}
