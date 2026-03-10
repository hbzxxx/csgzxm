using Framework.Data;
using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipIntensePanel : PanelBase
{
    public Text txt_nameInChooseEquipParent;//选装备部分的名
    public Text txt_lvInCHooseEquipParent;//选装备部分的等级

    public Transform trans_itemGrid;//物品图标
    public Transform trans_maxLevel;//最大等级

    //public RectTransform rectTrans_layoutGrid;//总管理的grid
    public Text txt_needPassedLevel;//需要通过关卡
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
    public Button btn_intense;//强化

    public Transform trans_choosedEquipParent;//选装备页面
    public Transform trans_choosedItemGridInChooseParent;//选装备页面的装备显示
    //public Text txt_choosedItemNameInChooseParent;//选装备页面的名字显示
    //public Text txt_choosedItemLevelInChooseParent;//选装备页面的等级显示
    public Transform trans_mainProGridInChooseParent;//选装备页面的主要属性父物体
    public Transform trans_viceProGridInChooseParent;//选装备页面的次要属性父物体
    public Transform trans_itemGridInChooseParent;//选装备页面的物品格子

    //public EquipIntenseKnapsackItemView curChoosedItemView;//当前选择的
    ItemData choosedItem;
    public List<EquipIntenseKnapsackItemView> allEquipList;//所有装备

    //public Transform trans_chooseToIntense;//强化页面

    public EquipProtoData curChoosedEquipData;//当前选的装备
    public EquipmentSetting curChoosedEquipSetting;//当前选择的装备配置
    public SingleDanFarmData singleDanFarmData;

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        choosedItem = args[1] as ItemData;
        curChoosedEquipData = choosedItem.equipProtoData;
        RegisterEvent(TheEventType.EquipIntense, OnReceivedIntense);
        //equipProtoData = args[0] as EquipProtoData;
        //equipmentSetting = DataTable.FindEquipSetting(equipProtoData.SettingId);

        //addBtnListener(btn_chooseToIntense, () =>
        // {
        //     ShowIntense();
        // });

        addBtnListener(btn_intense, () =>
         {
             EquipmentSetting equipSetting = DataTable.FindEquipSetting(curChoosedEquipData.settingId);
             List<List<List<int>>> allCostList = CommonUtil.SplitThreeCfg(equipSetting.UpgradeCost);
             List<List<int>> curCost = allCostList[curChoosedEquipData.curLevel - 1];
             for (int j = 0; j < curCost.Count; j++)
             {
                 List<int> cost = curCost[j];
                 int costId = cost[0];
                 int costNum = cost[1];
                 if (!ItemManager.Instance.CheckIfItemEnough(costId, (ulong)costNum))
                 {
                     ItemSetting costSetting = DataTable.FindItemSetting(costId);
                     PanelManager.Instance.OpenFloatWindow(costSetting.Name + "不够");
                     return;
                 }
             }


             //是玩家的

             string levelNeed =EquipmentManager.Instance.JudgeIfEquipIntenseSatisfyLevelCondition(curChoosedEquipData.curLevel);
             if (!string.IsNullOrWhiteSpace(levelNeed))
             {
                 PanelManager.Instance.OpenFloatWindow("通关" + levelNeed + "后可继续强化");
                 return;
             }

             if (EquipmentManager.Instance.IfEquipMaxLevel(curChoosedEquipData))
             {
                 PanelManager.Instance.OpenFloatWindow("该装备已达最高等级");
                 return;
             }
             for (int j = 0; j < curCost.Count; j++)
             {
                 List<int> cost = curCost[j];
                 int costId = cost[0];
                 int costNum = cost[1];
                 ItemManager.Instance.LoseItem(costId, (ulong)costNum);

             }
             EquipmentManager.Instance.OnIntenseEquip(choosedItem, singleDanFarmData);
             TaskManager.Instance.guide_intenseEquip = false;
             PanelManager.Instance.CloseTaskGuidePanel();
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
            PanelManager.Instance.ShowTaskGuidePanel(btn_intense.gameObject);
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
        if (TaskManager.Instance.guide_intenseEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_intense.gameObject);
        }
    }

    /// <summary>
    /// 显示选择装备
    /// </summary>
    void ShowChooseEquipParent()
    {
        OnChoosedEquip();
        return;
        //trans_chooseToIntense.gameObject.SetActive(false);
        trans_choosedEquipParent.gameObject.SetActive(true);

        PanelManager.Instance.CloseAllSingle(trans_itemGridInChooseParent);
        allEquipList.Clear();
        List<ItemData> dataList= ItemManager.Instance.FindItemListByType(ItemType.Equip);
        for (int i = 0; i < dataList.Count; i++)
        {
            ItemData data = dataList[i];
            EquipIntenseKnapsackItemView view = PanelManager.Instance.OpenSingle<EquipIntenseKnapsackItemView>(trans_itemGridInChooseParent, data,this);
            allEquipList.Add(view);
        }

        allEquipList[0].btn.onClick.Invoke();//默认选第一个
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
        for (int i = 0; i < allEquipList.Count; i++)
        {
            EquipIntenseKnapsackItemView theView = allEquipList[i];

            theView.RefreshShow();
        }
    }
    /// <summary>
    /// 选择了装备
    /// </summary>
    void OnChoosedEquip()
    {
        //curChoosedEquipData = curChoosedItemView.GetItemData().equipProtoData;
        curChoosedEquipSetting = DataTable.FindEquipSetting(curChoosedEquipData.settingId);

        txt_nameInChooseEquipParent.SetText(curChoosedEquipSetting.Name);
        txt_lvInCHooseEquipParent.SetText("Lv." + curChoosedEquipData.curLevel);
        if (EquipmentManager.Instance.IfEquipMaxLevel(curChoosedEquipData))
        {
            trans_maxLevel.gameObject.SetActive(true);
        }
        else
        {
            trans_maxLevel.gameObject.SetActive(false);

        }
        //装备icon
        PanelManager.Instance.CloseAllSingle(trans_choosedItemGridInChooseParent);
        PanelManager.Instance.OpenSingle<ItemViewE>(trans_choosedItemGridInChooseParent, choosedItem);


        //主要属性
        PanelManager.Instance.CloseAllSingle(trans_mainProGridInChooseParent);
        SinglePropertyData proMain = curChoosedEquipData.propertyList[0];

        PanelManager.Instance.OpenSingle<EquipMainPropertyView1>(trans_mainProGridInChooseParent, proMain);
        //次要属性
        PanelManager.Instance.CloseAllSingle(trans_viceProGridInChooseParent);
        for(int i=1;i< curChoosedEquipData.propertyList.Count; i++)
        {
            SinglePropertyData proVice = curChoosedEquipData.propertyList[i];
            PanelManager.Instance.OpenSingle<EquipVicePropertyView1>(trans_viceProGridInChooseParent, proVice);

        }
       
        OnShowEquipIntense();
    }

    /// <summary>
    /// 装备强化
    /// </summary>
    void OnShowEquipIntense()
    {

        string levelNeed= EquipmentManager.Instance.JudgeIfEquipIntenseSatisfyLevelCondition(curChoosedEquipData.curLevel);
        if (!string.IsNullOrWhiteSpace(levelNeed))
        {
            txt_needPassedLevel.SetText("需要通过主线" + levelNeed);
            txt_needPassedLevel.gameObject.SetActive(true);
        }
        else
        {
            txt_needPassedLevel.gameObject.SetActive(false);
        }

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

        if ((curChoosedEquipData.curLevel + 1) % 5 == 0)
        {
            int proNum = 0;
            List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
            for (int i = 0; i < studentList.Count; i++)
            {
                PeopleData p = studentList[i];
                if (p.talent == (int)StudentTalent.DuanZhao)
                {
                    for (int j = 0; j < p.propertyIdList.Count; j++)
                    {
                        int theId = p.propertyIdList[j];
                        if (theId == (int)PropertyIdType.ShiWu)
                        {
                            proNum += p.propertyList[j].num;
                        }
                    }
                }

            }
      
        }
        

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
        if (!EquipmentManager.Instance.IfEquipMaxLevel(curChoosedEquipData))
        {
            List<List<List<int>>> allCostList = CommonUtil.SplitThreeCfg(curChoosedEquipSetting.UpgradeCost);
            List<List<int>> curCost = allCostList[curChoosedEquipData.curLevel - 1];
            for (int j = 0; j < curCost.Count; j++)
            {
                List<int> cost = curCost[j];
                int costId = cost[0];
                int costNum = cost[1];
                AddSingle<SingleConsumeView1>(trans_mat, costId, costNum, ConsumeType.Item);
                //if (!ItemManager.Instance.CheckIfItemEnough(costId, (ulong)costNum))
                //{
                //    ItemSetting costSetting = DataTable.FindItemSetting(costId);
                //    PanelManager.Instance.OpenFloatWindow(costSetting.name + "不够");
                //    return;
                //}
            }
            //int needNumCount = DataTable._equipUpgradeList[curChoosedEquipProtoData.CurLevel].needExp.ToInt32();
            //int id = (int)ItemIdType.LingShi;
            //ItemData data = new ItemData();
            //data.SettingId = id;
            //data.Count =(ulong) needNumCount;
            //PanelManager.Instance.OpenSingle<WithCountItemView>(trans_mat, data);
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

