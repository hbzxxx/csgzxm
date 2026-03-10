using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HuoDongPanel : PanelBase
{
    public Button btn_shouChong;//首充
    public Button btn_leiChong;//累充
    public Button btn_qianDao;//签到


    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_shouChong, () =>
        {
            PanelManager.Instance.OpenPanel<ShouChongPanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_leiChong, () =>
        {
            PanelManager.Instance.OpenPanel<LeiChongPanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_qianDao, () =>
        {
            PanelManager.Instance.OpenPanel<SevenDayQianDaoPanel>(PanelManager.Instance.trans_layer2);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        if (RoleManager.Instance._CurGameInfo.allShopData.shouChongAwardGet)
            btn_shouChong.gameObject.SetActive(false);
        else
            btn_shouChong.gameObject.SetActive(true);
    }
}
