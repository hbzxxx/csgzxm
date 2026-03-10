using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class EquipJingLianPanel : PanelBase
{
    public Text txt_nameInChooseEquipParent;//选装备部分的名
    public Text txt_lvInCHooseEquipParent;//选装备部分的等级

    public Transform trans_itemGrid;//物品图标
    public Transform trans_maxJingLian;//最大精炼

    //public RectTransform rectTrans_layoutGrid;//总管理的grid
    //public Text txt_needPassedLevel;//需要通过关卡
    //public Transform trans_mainPropertyInIntenseParent;//强化页面的主要属性父物体
    //public Transform trans_vicePropertyInIntenseParent;//强化页面的副属性父物体
    //public Transform trans_skillCompareTrans;
    public Transform trans_mat;//材料

    //public Transform trans_levelLeftPos;//等级位置左边
    //public Transform trans_levelCenterPos;//等级位置中间

    //public Text txt_curLevel;//当前等级

    //public EquipProtoData equipProtoData;//装备数据
    //public EquipmentSetting equipmentSetting;//装备配置

    //public Button btn_chooseToIntense;//选择该装备进行强化
    public Button btn_jingLian;//精炼

    public Transform trans_choosedEquipParent;//选装备页面
    public Transform trans_choosedItemGridInChooseParent;//选装备页面的装备显示
    //public Text txt_choosedItemNameInChooseParent;//选装备页面的名字显示
    //public Text txt_choosedItemLevelInChooseParent;//选装备页面的等级显示
    public Transform trans_mainProGridInChooseParent;//选装备页面的主要属性父物体
    public Transform trans_viceProGridInChooseParent;//选装备页面的次要属性父物体
 
    //public EquipIntenseKnapsackItemView curChoosedItemView;//当前选择的
    ItemData choosedItem;
    public List<EquipIntenseKnapsackItemView> allEquipList;//所有装备

    //public Transform trans_chooseToIntense;//强化页面

    public EquipProtoData curChoosedEquipData;//当前选的装备
    public EquipmentSetting curChoosedEquipSetting;//当前选择的装备配置
    //public SingleDanFarmData singleDanFarmData;

    public override void Init(params object[] args)
    {
        base.Init(args);
        //singleDanFarmData = args[0] as SingleDanFarmData;
        choosedItem = args[0] as ItemData;
        curChoosedEquipData = choosedItem.equipProtoData;
        RegisterEvent(TheEventType.JingLianEquip, OnReceivedIntense);
        //equipProtoData = args[0] as EquipProtoData;
        //equipmentSetting = DataTable.FindEquipSetting(equipProtoData.SettingId);

        //addBtnListener(btn_chooseToIntense, () =>
        // {
        //     ShowIntense();
        // });

        addBtnListener(btn_jingLian, () =>
        {
      
            EquipmentManager.Instance.OnJingLianEquip(choosedItem );
    
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowChooseEquipParent();
        ShowGuide();

        transform.localPosition = Vector3.zero;
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_intenseEquip)
        {
            //PanelManager.Instance.ShowTaskGuidePanel(btn_chooseToIntense.gameObject);
        }
    }


    /// <summary>
    /// 显示强化面板
    /// </summary>
    void ShowIntense()
    {
        trans_choosedEquipParent.gameObject.SetActive(false);
        //trans_chooseToIntense.gameObject.SetActive(true);


        PanelManager.Instance.CloseAllSingle(trans_itemGrid);
        PanelManager.Instance.OpenSingle<ItemViewE>(trans_itemGrid, choosedItem);
        //txt_name.SetText(curChoosedEquipSetting.name);
        //txt_des.SetText(curChoosedEquipSetting.des);

        OnShowEquipIntense();

        //ShowMat();
        //ShowLevelCompare();
        //ShowPropertyInIntensePanel();
        //ShowSkillCompare();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans_layoutGrid);
        //if (TaskManager.Instance.guide_intenseEquip)
        //{
        //    PanelManager.Instance.ShowTaskGuidePanel(btn_intense.gameObject);
        //}
    }

    /// <summary>
    /// 显示选择装备
    /// </summary>
    void ShowChooseEquipParent()
    {
        OnChoosedEquip();
        return;

    }

    //public void OnClickedItem(EquipIntenseKnapsackItemView view)
    //{
    //    curChoosedItemView = view;

    //    for (int i = 0; i < allEquipList.Count; i++)
    //    {
    //        EquipIntenseKnapsackItemView theView = allEquipList[i];

    //        if (theView.GetItemData().onlyId == view.GetItemData().onlyId)
    //        {
    //            theView.OnChoosed(true);
    //        }
    //        else
    //        {
    //            theView.OnChoosed(false);
    //        }
    //    }
    //    OnChoosedEquip(view.GetItemData().equipProtoData);
    //}
    void OnReceivedIntense()
    {
        OnChoosedEquip();
     
    }
    /// <summary>
    /// 选择了装备
    /// </summary>
    void OnChoosedEquip()
    {
        //curChoosedEquipData = curChoosedItemView.GetItemData().equipProtoData;
        curChoosedEquipSetting = DataTable.FindEquipSetting(curChoosedEquipData.settingId);
        string jingLianNum = "";
        if (curChoosedEquipData.jingLianLv > 0)
            jingLianNum = "+" + curChoosedEquipData.jingLianLv;
        txt_nameInChooseEquipParent.SetText(curChoosedEquipSetting.Name);
        txt_lvInCHooseEquipParent.SetText("Lv." + curChoosedEquipData.curLevel);
        if ( curChoosedEquipData.jingLianLv>=5)
        {
            trans_maxJingLian.gameObject.SetActive(true);
        }
        else
        {
            trans_maxJingLian.gameObject.SetActive(false);

        }
        //装备icon
        PanelManager.Instance.CloseAllSingle(trans_choosedItemGridInChooseParent);
        PanelManager.Instance.OpenSingle<ItemViewE>(trans_choosedItemGridInChooseParent, choosedItem);


        //主要属性
        PanelManager.Instance.CloseAllSingle(trans_mainProGridInChooseParent);
        SinglePropertyData proMain = curChoosedEquipData.propertyList[0];

        PanelManager.Instance.OpenSingle<EquipMainPropertyView2>(trans_mainProGridInChooseParent, proMain);
        //次要属性
        PanelManager.Instance.CloseAllSingle(trans_viceProGridInChooseParent);
        for (int i = 1; i < curChoosedEquipData.propertyList.Count; i++)
        {
            SinglePropertyData proVice = curChoosedEquipData.propertyList[i];
            PanelManager.Instance.OpenSingle<EquipVicePropertyView>(trans_viceProGridInChooseParent, proVice);

        }

        OnShowEquipIntense();
    }

    /// <summary>
    /// 装备强化
    /// </summary>
    void OnShowEquipIntense()
    {

        string levelNeed = EquipmentManager.Instance.JudgeIfEquipIntenseSatisfyLevelCondition(curChoosedEquipData.curLevel);
        //if (!string.IsNullOrWhiteSpace(levelNeed))
        //{
        //    txt_needPassedLevel.SetText("需要通过主线" + levelNeed);
        //    txt_needPassedLevel.gameObject.SetActive(true);
        //}
        //else
        //{
        //    txt_needPassedLevel.gameObject.SetActive(false);
        //}

        //curChoosedEquipData = curChoosedItemView.GetItemData().equipProtoData;
        curChoosedEquipSetting = DataTable.FindEquipSetting(curChoosedEquipData.settingId);
        //if (EquipmentManager.Instance.IfEquipMaxLevel(curChoosedEquipData))
        //{
        //    txt_levelAdd.gameObject.SetActive(false);
        //}
        //else
        //{
        //    txt_levelAdd.gameObject.SetActive(true);
        //}

        ShowMat();
        ShowLevelCompare();
        ShowPropertyInIntensePanel();
        //ShowSkillCompare();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans_layoutGrid);

 

    }

    /// <summary>
    /// 显示等级
    /// </summary>
    void ShowLevelCompare()
    {
        //txt_curLevel.SetText("Lv."+ curChoosedEquipData.curLevel.ToString());
        //如果满级了
        //if (EquipmentManager.Instance.IfEquipMaxLevel(curChoosedEquipProtoData))
        //{
        //   // txt_afterLevel.gameObject.SetActive(false);
        //    txt_curLevel.transform.position = trans_levelCenterPos.position;
        //}
        //else
        //{
        //  //  txt_afterLevel.gameObject.SetActive(true);
        //   // txt_afterLevel.SetText("Lv."+(curChoosedEquipProtoData.CurLevel + 1).ToString());
        //    txt_curLevel.transform.position = trans_levelLeftPos.position;
        //}


    }

    /// <summary>
    /// 显示属性比较
    /// </summary>
    public void ShowPropertyInIntensePanel()
    {
        //PanelManager.Instance.CloseAllSingle(trans_mainPropertyInIntenseParent);


        ////主要属性
        //PanelManager.Instance.CloseAllSingle(trans_mainPropertyInIntenseParent);
        //SinglePropertyData proMain = curChoosedEquipData.propertyList[0];

        //PanelManager.Instance.OpenSingle<EquipMainPropertyView>(trans_mainPropertyInIntenseParent, proMain);
        ////次要属性
        //PanelManager.Instance.CloseAllSingle(trans_vicePropertyInIntenseParent);
        //for (int i = 1; i < curChoosedEquipData.propertyList.Count; i++)
        //{
        //    SinglePropertyData proVice = curChoosedEquipData.propertyList[i];
        //    PanelManager.Instance.OpenSingle<EquipVicePropertyView>(trans_vicePropertyInIntenseParent, proVice);

        //}
        // equipProtoData.PropertyIdList.Count;
    }



    /// <summary>
    /// 显示材料
    /// </summary>
    void ShowMat()
    {
        ClearCertainParentAllSingle<SingleConsumeView1>(trans_mat);





        //没满级
        if ( curChoosedEquipData.jingLianLv<5)
        {
            List<ItemData> matList = EquipmentManager.Instance.EquipJingLianCostList(curChoosedEquipData.setting, curChoosedEquipData.jingLianLv);
            for(int i=0;i< matList.Count; i++)
            {
                ItemData data = matList[i];
                AddSingle<SingleConsumeView1>(trans_mat, (int)data.settingId, (int)(ulong)data.count, ConsumeType.Item);

            }
            //List<List<List<int>>> allCostList = CommonUtil.SplitThreeCfg(curChoosedEquipSetting.upgradeCost);
            //List<List<int>> curCost = allCostList[curChoosedEquipData.curLevel - 1];
            //for (int j = 0; j < curCost.Count; j++)
            //{
            //    List<int> cost = curCost[j];
            //    int costId = cost[0];
            //    int costNum = cost[1];
            //    AddSingle<SingleConsumeView>(trans_mat, costId, costNum, ConsumeType.Item);
            //    //if (!ItemManager.Instance.CheckIfItemEnough(costId, (ulong)costNum))
            //    //{
            //    //    ItemSetting costSetting = DataTable.FindItemSetting(costId);
            //    //    PanelManager.Instance.OpenFloatWindow(costSetting.name + "不够");
            //    //    return;
            //    //}
            //}
 
        }
    }

    public override void Clear()
    {
        base.Clear();

        PanelManager.Instance.CloseAllSingle(trans_itemGrid);
    }

    public override void OnClose()
    {
        base.OnClose();
        TaskManager.Instance.guide_intenseEquip = false;
        PanelManager.Instance.CloseTaskGuidePanel();
    }


}
