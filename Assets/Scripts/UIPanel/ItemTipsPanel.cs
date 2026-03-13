using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class ItemTipsPanel : PanelBase
{
    public ItemData itemData;
    public ItemSetting itemSetting;

    public Transform trans_itemGrid;
    public Text txt_name;
    public Text txt_quality;
    public Text txt_des;

    public Transform trans_equip;
    public Transform trans_equipMainProGrid;
    public Transform trans_equipViceProGrid;
    public Transform trans_equipGemProGrid;

    public Text txt_taoZhuangDes;
    public Text txt_youHuaLv;
    #region 宝石
    public Transform trans_gemShow;
    public Transform trans_gemShowProGrid;
    #endregion

    public Transform trans_comeFromGrid;//来自
    public Text txt_specialDes;//特殊描述
    public Transform grid_downBtns;
    bool cannotHandle = false;
    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        if (args.Length >= 2)
        {
            cannotHandle = (bool)args[1];
        }
        else
        {
            cannotHandle = false;
        }
        
        itemSetting = DataTable.FindItemSetting(itemData.settingId);
        if (itemSetting == null)
        {
            Debug.LogError($"[ItemTipsPanel] ItemSetting not found for settingId={itemData.settingId}, trying to find equipSetting...");
            // 尝试从 equipProtoData 获取
            if (itemData.equipProtoData != null)
            {
                var equipSetting = DataTable.FindEquipSetting(itemData.equipProtoData.settingId);
                if (equipSetting != null)
                {
                    int itemId = equipSetting.ItemId.ToInt32();
                    itemSetting = DataTable.FindItemSetting(itemId);
                    Debug.Log($"[ItemTipsPanel] equipSetting found: {equipSetting.Name}, itemId={itemId}, itemSetting={(itemSetting != null ? itemSetting.Name : "null")}");
                }
                else
                {
                    Debug.LogError($"[ItemTipsPanel] equipSetting not found for equipProtoData.settingId={itemData.equipProtoData.settingId}");
                    // 尝试从已有的 equipProtoData.setting 获取
                    if (itemData.equipProtoData.setting != null)
                    {
                        Debug.Log($"[ItemTipsPanel] using equipProtoData.setting: {itemData.equipProtoData.setting.Name}");
                    }
                    else
                    {
                        Debug.LogError($"[ItemTipsPanel] equipProtoData.setting is also null, using settingId as fallback");
                    }
                }
            }
        }
        RegisterEvent(TheEventType.CloseItemTipsPanel, CloseThePanel);
    }
 
    public override void OnOpenIng()
    { 
        base.OnOpenIng();
        string equipName = "";
        if (itemSetting != null)
        {
            equipName = itemSetting.Name;
        }
        else if (itemData.equipProtoData?.setting != null)
        {
            equipName = itemData.equipProtoData.setting.Name;
        }
        else
        {
            // 使用装备ID作为后备名称
            equipName = $"装备{itemData.settingId}";
        }
        
        if (string.IsNullOrEmpty(equipName))
        {
            Debug.LogError($"[ItemTipsPanel] itemSetting is null, itemData.settingId={itemData?.settingId}");
            txt_name.SetText("未知装备");
        }
        else
        {
            txt_name.SetText(equipName);
        }
        
        if(itemSetting != null && txt_des!=null)
        {
            txt_des.SetText(itemSetting.Des);
        }
        else if (itemData.equipProtoData?.setting != null && txt_des != null)
        {
            txt_des.SetText(itemData.equipProtoData.setting.Des);
        }
        if (trans_itemGrid != null)
            AddSingle<ItemView>(trans_itemGrid, itemData);
        if(txt_quality!=null)
        txt_quality.SetText(CommonUtil.QualityName((Quality)(int)itemData.quality));

        if (itemData.equipProtoData != null)
        {
            ClearCertainParentAllSingle<EquipMainPropertyView>(trans_equipMainProGrid);
            ClearCertainParentAllSingle<EquipVicePropertyView>(trans_equipViceProGrid);

            // 检查propertyList是否有元素，避免数组越界
            if (itemData.equipProtoData.propertyList != null && itemData.equipProtoData.propertyList.Count > 0)
            {
                AddSingle<EquipMainPropertyView>(trans_equipMainProGrid, itemData.equipProtoData.propertyList[0]);
                for(int i=1;i< itemData.equipProtoData.propertyList.Count; i++)
                {
                    AddSingle<EquipVicePropertyView>(trans_equipViceProGrid, itemData.equipProtoData.propertyList[i]);
                }
            }
            trans_equip.gameObject.SetActive(true);

            List<int> gemProIdList = new List<int>();
            List<int> gemProNumList = new List<int>();
            if (itemData.equipProtoData.gemList != null)
            {
                for (int i = 0; i < itemData.equipProtoData.gemList.Count; i++)
                {
                    ItemData gemItem = itemData.equipProtoData.gemList[i];
                    if (gemItem == null || gemItem.onlyId<=0)
                        continue;
                    GemData gem = itemData.equipProtoData.gemList[i].gemData;
                    if (gem != null && gem.propertyList != null)
                    {
                        for (int j = 0; j < gem.propertyList.Count; j++)
                        {
                            int id = gem.propertyList[j].id;
                            if (!gemProIdList.Contains(id))
                            {
                                gemProIdList.Add(id);
                                gemProNumList.Add(0);
                            }
                            int index = gemProIdList.IndexOf(id);
                            gemProNumList[index] += gem.propertyList[j].num;
                        }
                    }
                }
            }
            for (int i = 0; i < gemProIdList.Count; i++)
            {
                AddSingle<GemProTxtView>(trans_equipGemProGrid, gemProIdList[i], gemProNumList[i]);
            }
            //EquipmentSetting equipSetting = DataTable.FindEquipSetting(itemData.equipProtoData.settingId);
            float youHuaLv = itemData.equipProtoData.youHuaLv / 480f;
            float youHuaShow = youHuaLv * 100;
            Rarity rarity = (Rarity)Mathf.CeilToInt(youHuaShow / 20);
            txt_youHuaLv.SetText("总优化率：" + youHuaShow.ToString("0.0") + "%");
            //txt_youHuaLv.color = CommonUtil.RarityColor(rarity);
        }
        else
        {
            trans_equip.gameObject.SetActive(false);
            txt_youHuaLv.SetText("");
        }
        if (trans_gemShow != null)
        {
            if (itemData.gemData != null && itemData.gemData.propertyList != null)
            {
                trans_gemShow.gameObject.SetActive(true);
                //PanelManager.Instance.CloseAllSingle(trans_gemShowItemGrid);
                ClearCertainParentAllSingle<GemPropertyView>(trans_gemShowProGrid);

                // PanelManager.Instance.OpenSingle<ItemView>(trans_gemShowItemGrid,itemView.GetItemData());
                for (int i = 0; i < itemData.gemData.propertyList.Count; i++)
                {
                    AddSingle<GemPropertyView>(trans_gemShowProGrid, true, itemData.gemData.propertyList[i], itemData.gemData);
                }
                int emptyCount = 3 - itemData.gemData.propertyList.Count;
                for (int i = 0; i < emptyCount; i++)
                {
                    AddSingle<GemPropertyView>(trans_gemShowProGrid, false);
                }
            }
            else
            {
                trans_gemShow.gameObject.SetActive(false);
            }
        }
        if(txt_specialDes!=null)
        txt_specialDes.SetText(ItemManager.Instance.ItemSpecialDes(itemData));

        if (trans_comeFromGrid != null)
        {
            //装备不显示来源
            if (itemData.equipProtoData != null)
            {
                trans_comeFromGrid.gameObject.SetActive(false);
            }
            else
            {
                trans_comeFromGrid.gameObject.SetActive(true);
                ClearCertainParentAllSingle<SingleComefromView>(trans_comeFromGrid);
                if (!string.IsNullOrWhiteSpace(itemSetting.ComeFrom))
                {
                    string[] comefromArr = itemSetting.ComeFrom.Split('|');
                    for (int i = 0; i < comefromArr.Length; i++)
                    {
                        AddSingle<SingleComefromView>(trans_comeFromGrid, itemData, itemSetting, i);

                    }
                }
            }
          
        }

        ShowTaoZhuangDes();
        RefreshDownBtnShow();
    }

    public virtual void RefreshDownBtnShow()
    {
        if (cannotHandle)
            return;
        switch ((ItemShowTag)itemData.setting.ShowTag.ToInt32())
        {
            case ItemShowTag.Equip:

                KnapsackDownBtnView view= PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Equip, this);
                KnapsackDownBtnView view2 = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Intense, this);

                if (TaskManager.Instance.guide_equipEquip)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                }
                //PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Intense, this);
                //PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.AddGem, this);
                //PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Sell, this);

                break;
            case ItemShowTag.Item:
                // ShowChoosedItem(curChoosedItemView.GetItemData());

                break;
            case ItemShowTag.Consume:
                if (ItemManager.Instance.CheckIfItemEnough(itemData.settingId, 1))
                {
                    PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Sell, this);
                }
                //技能书I
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.SkillBook)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Study, this);
                    if (TaskManager.Instance.guide_studySkill)
                    {

                        if (itemData.setting.ItemType.ToInt32() == (int)ItemType.SkillBook)
                        {
                            PanelManager.Instance.ShowTaskGuidePanel(btnView.gameObject);
                        }
                    }
                }
                //修为丹
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.Dan)
                {
                    PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //源力结晶
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.YuanLi)
                {
                    PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //功法书
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.GongFaShu)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //宝石箱
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.XingChenBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //清灵草箱子
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.QingLingCaoBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);

                }
                //灵石福袋
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.LingShiFuDai)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);

                }
                //念珠箱
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.NianZhuBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);

                }
                //建筑材料箱
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.BuildingUpgradeBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);

                }
                //接引令
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.JieYinLing)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //原胚箱子
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.OPBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //器火箱子
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.QiHuoBox)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);
                }
                //代金券
                if (itemData.setting.ItemType.ToInt32() == (int)ItemType.FanLiDaiJinQuan)
                {
                    KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use,this);
                }
                break;

            case ItemShowTag.Gem:
                PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Composite, this);
                //PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Use, this);

                PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(grid_downBtns, KnapsackDownBtnType.Sell, this);
                break;
        }


    }

    /// <summary>
    /// 套装描述
    /// </summary>
    void ShowTaoZhuangDes()
    {
        string taoZhuangDes = "";

        string des = "";
        if (itemData.equipProtoData != null && itemData.equipProtoData.setting != null)
        {
            EquipTaoZhuangType taoZhuangType = (EquipTaoZhuangType)itemData.equipProtoData.setting.TaoZhuang.ToInt32();
            if (taoZhuangType != EquipTaoZhuangType.None)
            {
                EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangType);
                if (taoZhuangSetting != null)
                {
                    string des0   = "<color=green><b>" + taoZhuangSetting.Name+ "</b></color>" + "\n";

                    string des1 = EquipmentManager.Instance.TaoZhuangDes(taoZhuangType, false);
                    string des2 = EquipmentManager.Instance.TaoZhuangDes(taoZhuangType, true);
                    //有2件没
                    if (itemData.equipProtoData.isEquipped)
                    {
                        PeopleData p = StudentManager.Instance.FindStudent(itemData.equipProtoData.belongP);
                        if (p != null)
                        {
                            List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(p);
                            if (taoZhuangList != null)
                            {
                                //有2件套
                                if (taoZhuangList.Contains(taoZhuangType))
                                {
                                    des1 = "<color=green><b>" + des1 + "</b></color>";
                                }
                                //有4件套
                                if (taoZhuangList.Count == 2
                                    && taoZhuangList[0] == taoZhuangType
                                    && taoZhuangList[1] == taoZhuangType)
                                {
                                    des2 = "<color=green><b>" + des2 + "</b></color>";
                                }
                            }
                        }
                    }
                    des = des0+ des1 + "\n" + des2;
                }
            }

        }


        txt_taoZhuangDes.SetText(des);
 

    }
    void CloseThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    public override void Clear()
    {
        base.Clear();
        if(trans_itemGrid!=null)
        ClearCertainParentAllSingle<SingleViewBase>(trans_itemGrid);
        if(grid_downBtns!=null)
        ClearCertainParentAllSingle<SingleViewBase>(grid_downBtns);
        if(trans_gemShowProGrid!=null)
        ClearCertainParentAllSingle<GemPropertyView>(trans_gemShowProGrid);


        ClearCertainParentAllSingle<EquipMainPropertyView>(trans_equipMainProGrid);
        ClearCertainParentAllSingle<EquipVicePropertyView>(trans_equipViceProGrid);
        ClearCertainParentAllSingle<SingleViewBase>(trans_equipGemProGrid);
    }
}
