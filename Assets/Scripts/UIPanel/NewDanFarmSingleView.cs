using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;


public class NewDanFarmSingleView : SingleViewBase
{
    public Image img_bg;

    public Button btn;

    NewDanFarmBuildPanel parentPanel;

    public Image img;

    public Text txt_name;
    public Text txt_des;

    public Text txt_consume;//消耗是多少

    public Text txt_farmLimit;//田的数量限制

    public DanFarmSetting danFarmSetting;
    public GameObject obj_choose;

    public override void Init(params object[] args)
    {
        base.Init(args);
        danFarmSetting = args[0] as DanFarmSetting;
        parentPanel = args[1] as NewDanFarmBuildPanel;

        //danFarmSetting = DataTable.FindDanFarmSetting(id);
        addBtnListener(btn, () =>
         {
             parentPanel.OnChoosedSingleNewDanFarm(this);
         });


    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (danFarmSetting == null) return;
        txt_name.SetText(danFarmSetting.Name);
        img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.UiName);
        if (danFarmSetting.WorkType.ToInt32() == (int)DanFarmWorkType.Common)
        {
            txt_des.gameObject.SetActive(true);
            List<int> priceList = CommonUtil.SplitCfgOneDepth(danFarmSetting.DanPrice);
            // 使用字符串参数查找配置，保留原始格式
            string productName = DataTable.FindItemSetting(danFarmSetting.Param).Name;
            txt_des.SetText("收益：" + (priceList[0] * 10) + productName+"/月");
        }
        else
        {
            txt_des.gameObject.SetActive(false);
        }
        txt_consume.SetText("X" + danFarmSetting.BuildCost);

        txt_farmLimit.SetText(LianDanManager.Instance.FindMyFarmNum(danFarmSetting.Id.ToInt32()) + 
            "/" + ZongMenManager.Instance.GetFarmNumLimit(danFarmSetting.Id.ToInt32()));
    }

    public void OnChoosed(bool choose)
    {
        obj_choose.SetActive(choose);
    }

    public override void Clear()
    {
        base.Clear();
        OnChoosed(false);
    }
}
