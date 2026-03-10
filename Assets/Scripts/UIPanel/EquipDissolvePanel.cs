 using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipDissolvePanel : PanelBase
{
    public Transform trans_equip;//当前装备
    public Transform trans_choosedEquipParent;//选装备页面

    public Transform trans_itemGridInChooseParent;//格子
    public List<EquipDissolveKnapsackItemView> allEquipList;//所有装备

    #region 选装备页面
    public ItemData curChoosedItem;//当前选择的
    public EquipProtoData curChoosedEquipData;//当前选的装备
    public EquipmentSetting curChoosedEquipSetting;//当前选的装备
    public Text txt_nameInChooseEquipParent;//名字
    public Text txt_lvInCHooseEquipParent;//lv
    public Transform trans_choosedItemGridInChooseParent;//物品格子
    public Transform trans_mainProGridInChooseParent;//主属性
    public Transform trans_viceProGridInChooseParent;//副属性
    #endregion
    

    public Button btn_dissolve;//选择去分解
    public Transform trans_dissolveRes;//分解结果
    public Transform trans_dissolveResItemGrid;//分解得物品
    public Transform trans_dissolveResJiaChengTxtShowGrid;//分解弟子加成
    public Button btn_emptyCloseDissolveRes;//点空白关闭结果页面

    public StudentAddAnimGroup studentAddAnimGroup;

    public override void Init(params object[] args)
    {
        base.Init(args);
        SingleDanFarmData farmData = args[0] as SingleDanFarmData;
        curChoosedItem = args[1] as ItemData;
        curChoosedEquipData = curChoosedItem.equipProtoData;
        addBtnListener(btn_dissolve, () =>
        {
            if (curChoosedEquipData.isEquipped)
            {
                PanelManager.Instance.OpenFloatWindow("已被装备，请先卸下");
                return;
            }
            for(int i=0;i< curChoosedEquipData.gemList.Count; i++)
            {
                ItemData gem = curChoosedEquipData.gemList[i];
                if (gem!=null
                &&gem.onlyId != 0)
                {
                    PanelManager.Instance.OpenFloatWindow("已镶嵌宝石，请先卸下");
                    return;
                }
            }


            PanelManager.Instance.OpenCommonHint("确定分解吗？", () =>
            {
                if(curChoosedItem != null)
                EquipmentManager.Instance.OnEquipDissolve(curChoosedItem, farmData);
            }, null);
        });
        addBtnListener(btn_emptyCloseDissolveRes, () =>
        {
            trans_dissolveRes.gameObject.SetActive(false);
            PanelManager.Instance.ClosePanel(this);
        });
        RegisterEvent(TheEventType.DissolvedEquip, OnEquipDissolve);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowChooseEquipParent();
        trans_dissolveRes.gameObject.SetActive(false);
    }


    /// <summary>
    /// 显示选择装备
    /// </summary>
    void ShowChooseEquipParent()
    {
        OnClickedItem();
        //PanelManager.Instance.CloseAllSingle(trans_itemGridInChooseParent);
        //allEquipList.Clear();
        //List<ItemData> equipItemList = ItemManager.Instance.FindItemListByType(ItemType.Equip);
        //for (int i = 0; i < equipItemList.Count; i++)
        //{
        //    ItemData data = equipItemList[i];
        //    EquipDissolveKnapsackItemView view = PanelManager.Instance.OpenSingle<EquipDissolveKnapsackItemView>(trans_itemGridInChooseParent, data, this);
        //    allEquipList.Add(view);
        //}
        ////if(allEquipList.Count>0)
        ////    allEquipList[0].btn.onClick.Invoke();//默认选第一个
        ////else
        ////{
        ////    curChoosedItemView = null;
        ////    btn_dissolve.gameObject.SetActive(false);
        ////    trans_equip.gameObject.SetActive(false);
        ////}

        //curChoosedItemView = null;
        //btn_dissolve.gameObject.SetActive(false);
        //trans_equip.gameObject.SetActive(false);
    }

    

    public void OnClickedItem( )
    {
        //curChoosedItemView = view;

        //for (int i = 0; i < allEquipList.Count; i++)
        //{
        //    EquipDissolveKnapsackItemView theView = allEquipList[i];

        //    if (theView.GetItemData().onlyId == view.GetItemData().onlyId)
        //    {
        //        theView.OnChoosed(true);
        //    }
        //    else
        //    {
        //        theView.OnChoosed(false);
        //    }
        //}
        OnChoosedEquip();
    }

    /// <summary>
    /// 选择了装备
    /// </summary>
    void OnChoosedEquip()
    {
        trans_equip.gameObject.SetActive(true);
 
        curChoosedEquipSetting = DataTable.FindEquipSetting(curChoosedEquipData.settingId);

        txt_nameInChooseEquipParent.SetText(curChoosedEquipSetting.Name);
        txt_lvInCHooseEquipParent.SetText("Lv." + curChoosedEquipData.curLevel);
        //装备icon
        PanelManager.Instance.CloseAllSingle(trans_choosedItemGridInChooseParent);
        PanelManager.Instance.OpenSingle<ItemView>(trans_choosedItemGridInChooseParent, curChoosedItem);


        //主要属性
        PanelManager.Instance.CloseAllSingle(trans_mainProGridInChooseParent);
        SinglePropertyData proMain = curChoosedEquipData.propertyList[0];

        PanelManager.Instance.OpenSingle<EquipMainPropertyView>(trans_mainProGridInChooseParent, proMain);
        //次要属性
        PanelManager.Instance.CloseAllSingle(trans_viceProGridInChooseParent);
        for (int i = 1; i < curChoosedEquipData.propertyList.Count; i++)
        {
            SinglePropertyData proVice = curChoosedEquipData.propertyList[i];
            PanelManager.Instance.OpenSingle<EquipVicePropertyView>(trans_viceProGridInChooseParent, proVice);

        }
        btn_dissolve.gameObject.SetActive(true);
        //OnShowEquipIntense();
    }


    /// <summary>
    /// 装备分解
    /// </summary>
    public void OnEquipDissolve(object[] args)
    {
        
        List<int> matIdList = args[0] as List<int>;
        List<ulong> matNumList = args[1] as List<ulong>;
        List<ulong> initMatNumList = args[2] as List<ulong>;
        int quality = (int)args[3];
        ClearCertainParentAllSingle<ItemView>(trans_dissolveResItemGrid);
        ClearCertainParentAllSingle<StudentProAddShowView>(trans_dissolveResJiaChengTxtShowGrid);

        trans_dissolveRes.gameObject.SetActive(true);
        for (int i = 0; i < matIdList.Count; i++)
        {
            int theId = matIdList[i];
            ulong num = matNumList[i];
            ulong initNum = initMatNumList[i];
            ItemData data = new ItemData();
            data.settingId = theId;
            data.count = num;
            AddSingle<ItemView>(trans_dissolveResItemGrid, data);

            AddSingle<StudentProAddShowView>(trans_dissolveResJiaChengTxtShowGrid, (int)(int)initMatNumList[i], (int)(int)matNumList[i]);
        }
        ShowChooseEquipParent();
        studentAddAnimGroup.Play(quality);
    }

}
