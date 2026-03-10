using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleXueMaiView : SingleViewBase
{
    public Image icon;
    public Button btn;
    public XueMaiUpgradeSetting upgradeSetting;
    public Text txt_curLevel;
    PeopleData p;
    public XueMaiPanel parentPanel;
    XueMaiType xueMaiType;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        xueMaiType = (XueMaiType)args[1];
        parentPanel = args[2] as XueMaiPanel;
        upgradeSetting = DataTable.FindXueMaiUpgradeSettingByType(xueMaiType);


        addBtnListener(btn, () =>
         {
             parentPanel.OnChoosedXueMai(this);
         });
    }

    public void RefreshShow()
    {
        icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + upgradeSetting.Icon);
        int index = p.xueMai.xueMaiTypeList.IndexOf(xueMaiType);
        int curLevel = p.xueMai.xueMaiLevelList[index];
        txt_curLevel.SetText(curLevel.ToString());
    }
}
