using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineIncomePanel : PanelBase
{
    public Text txt_xiuwei;
    public Transform trans_grid;
    public Button btn_receive;//收获
    public Button btn_adReceive;//广告收取
    public Text txt_totalTime;//时间

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_receive, () =>
        {
            RoleManager.Instance.ReceiveOfflineIncome();
            PanelManager.Instance.ClosePanel(this);
        });
        addBtnListener(btn_adReceive, () =>
        {
            ADManager.Instance.WatchAD(ADType.ReceiveOfflineIncome);
            PanelManager.Instance.ClosePanel(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
       
        //离线收益
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.timeData.OfflineItemList.Count; i++)
        {
            AddSingle<WithCountItemView>(trans_grid,RoleManager.Instance._CurGameInfo.timeData.OfflineItemList[i]);
        }
        int hour = RoleManager.Instance._CurGameInfo.timeData.OffLineTotalMinute / 60;
        int minute = RoleManager.Instance._CurGameInfo.timeData.OffLineTotalMinute - hour * 60;
        string hourStr = "";
        if (hour > 0)
        {
            hourStr = hour.ToString()+"时";
        }
        txt_totalTime.SetText("累计时间："+hourStr + minute + "分");
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.offlineIncomePanel = null;
    }
    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<WithCountItemView>(trans_grid);
    }
}
