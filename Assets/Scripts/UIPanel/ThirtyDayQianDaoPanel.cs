using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirtyDayQianDaoPanel : PanelBase
{
    public Transform grid;//格子
    public ScrollViewNevigation nevigation;
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        for(int i = 0; i < 30; i++)
        {
            SingleThirtyDayQianDaoView view=AddSingle<SingleThirtyDayQianDaoView>(grid, i + 1,this);
            if (RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex == i)
            {
                StartCoroutine(YieldLocate(view));
            }
        }
    }

    IEnumerator YieldLocate(SingleThirtyDayQianDaoView view)
    {
        yield return null;
        //指向当前签到的部分
        nevigation.NevigateImmediately(view.GetComponent<RectTransform>());
        if(RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex!= RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex)
        {
            btn_emptyClose.gameObject.SetActive(false);
            view.btn_qian.onClick.Invoke();
        }
        else
        {
            btn_emptyClose.gameObject.SetActive(true);
        }
    }

    public void SuccessQian()
    {
        btn_emptyClose.gameObject.SetActive(true);
    }
    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.thirtyDayQianDaoPanel = null;
    }
}
