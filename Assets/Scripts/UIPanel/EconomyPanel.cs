using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EconomyPanel : PanelBase
{
    public Text txt_lingShiProductSpeed;//灵石产出速度
    public Text txt_lingShiConsumeSpeed;//灵石消耗速度
    public Transform trans_grid;
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_lingShiProductSpeed.SetText("+" + 12*RentManager.Instance.CalcLingShiProductPerMonth() + "/年");
        txt_lingShiConsumeSpeed.SetText("-" + 12*RentManager.Instance.CalcRentConsume() + "/年");

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (!data.IsEmpty)
            {
                SingleEconomyView singleEconomyView = AddSingle<SingleEconomyView>(trans_grid, data);
                if (singleEconomyView.txt_workStatus.text != "空闲")
                {
                    singleEconomyView.kuohao.SetActive(true);
                    singleEconomyView.txt_workStatus.color = new Color32(93, 187, 37, 255);
                }
                else
                {
                    singleEconomyView.kuohao.SetActive(false);
                    singleEconomyView.txt_workStatus.color = new Color32(199, 186, 145, 255);
                }
            }
        }
    }
}
