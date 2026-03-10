using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using System;
using Framework.Data;
using DG.Tweening;

public class SingleMiJingView : SingleViewBase
{
    public Button btn;
    public Image img_icon;//玩法图标
    public Text txt_title;//玩法标题
    public SingleMiJingPaiQianData singleMiJingPaiQianData;


    public MiJingSetting miJingSetting;
    private SingleMiJingData singleMiJingData;
    private CopyPanel copyPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleMiJingData = args[0] as SingleMiJingData;

        //SetupView();
        singleMiJingPaiQianData = MiJingManager.Instance.GetSingleMiJingPaiQianDataById(singleMiJingData.settingId);
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + singleMiJingData.iconName);
        //按钮
        miJingSetting = DataTable.FindMiJingSetting(singleMiJingData.settingId);
        addBtnListener(btn, OnChoose);
        txt_title.SetText(miJingSetting.Name);
    }


    /// <summary>
    /// 选择弟子去秘境
    /// </summary>
    public void OnChoose()
    {
        PanelManager.Instance.OpenPanel<SingleMiJingChoosePanel>(PanelManager.Instance.trans_layer2, singleMiJingPaiQianData);

    }



    ///// <summary>
    ///// 显示可收回
    ///// </summary>
    //public void ShowCanHarvest(object[] param)
    //{
    //    SingleMiJingPaiQianData data = param[0] as SingleMiJingPaiQianData;
    //    if (data.SettingId == settingId)
    //    {
    //        singleMiJingPaiQianData = data;
    //        if (singleMiJingPaiQianData.RemainDay <= 0)
    //        {
    //            btn_harvest.gameObject.SetActive(true);

    //        }
    //        else
    //        {
    //            btn_harvest.gameObject.SetActive(false);

    //        }
    //    }
    //}



    ///// <summary>
    ///// 收到刷新派遣状态的消息
    ///// </summary>
    //public void OnRefreshPaiQianStatus(object[] param)
    //{
    //    SingleMiJingPaiQianData data = param[0] as SingleMiJingPaiQianData;
    //    if (data.SettingId == settingId)
    //    {
    //        this.singleMiJingPaiQianData = data;
    //        //RefreshShow(data);
    //    }

    //}

    ///// <summary>
    ///// 刷新剩余时间和产出
    ///// </summary>
    //public void RefreshShow(SingleMiJingPaiQianData data)
    //{
    //    //MiJingSetting setting = DataTable.FindMiJingSetting(data.SettingId);
    //    int totalDay = miJingSetting.totalDay.ToInt32();
    //    int remainDay = data.RemainDay;
    //    int num = miJingSetting.award.ToInt32();
    //    float rate = (totalDay - remainDay) / (float)totalDay;
    //    img_dayBar.DOFillAmount(rate, 1f);
    //    txt_remainDay.SetText("剩余时间：" + GameTimeManager.Instance.GetWeekByDay(data.RemainDay) + "周");
    //    int lingShiNum = (int)(rate * num);

    //    DOTween.To(() => lingShiNum, (x) =>

    //    {
    //        num = x;
    //        txt_lingShiNum.SetText("X" + x.ToString());
    //    }, lingShiNum, 1);


    //}

    ///// <summary>
    ///// 收获
    ///// </summary>
    //public void OnHarvest(object[] param)
    //{
    //    SingleMiJingPaiQianData data = param[0] as SingleMiJingPaiQianData;
    //    if (data.SettingId == singleMiJingPaiQianData.SettingId)
    //    {
    //        singleMiJingPaiQianData = data;

    //        trans_paiQianIng.gameObject.SetActive(false);
    //        btn_harvest.gameObject.SetActive(false);
    //    }
    //}

    public override void Clear()
    {
        base.Clear();
        RemoveEventRegister();

        singleMiJingData = null;
        copyPanel = null;
    }
}
