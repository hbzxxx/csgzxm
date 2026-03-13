 using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包
/// </summary>
public class KnapsackPanel : PanelBase
{


    public ItemModel itemModel;

    public Transform trans_grid;//格子

    public List<int> curSettingIdList = new List<int>();
    public List<UInt64> curOnlyIdList = new List<ulong>();
    public List<ItemView> curItemViewList = new List<ItemView>();
    public List<ItemData> curItemDataList = new List<ItemData>();


    public ItemShowTag curTag;


    //public Button btn_sell;
    public Button btn_allTag;//全部tag
    public Image img_allTag;
    public Button btn_equipTag;//装备tag
    public Image img_equipTag;
    public Button btn_itemTag;//物品tag
    public Image img_itemTag;

    public Button btn_gemTag;//宝石tag
    public Image img_gemTag;

    public Button btn_consumeTag;//消耗tag
    public Image img_consumeTag;

    public Transform trans_downBtnGrid;//下面操作物品的按钮父物体

    public ItemView curChoosedItemView;//当前选了哪个物品


    //public Transform trans_curEquipGrid;//当前装备父物体
    //public Transform trans_curSkillGrid;//当前技能父物体
    // 按钮文字颜色
    private Color selectedTextColor = new Color(0.408f, 0.310f, 0.212f, 1f); // #684F36
    private Color unselectedTextColor = new Color(0.757f, 0.741f, 0.702f, 1f); // #C1BDB3


    #region tips 相关

    public Transform trans_tips;
    public Transform trans_showItemParent;//用于展示的物品
    public Transform trans_nameGrid;
    public Text txt_showItemDesTxt;
    public Text txt_specialDes;//特殊描述
    public Transform trans_itemShow;//显示物品页面
    public Transform trans_equipShow;//显示装备页面
    public Transform trans_gemShow;//显示宝石页面
    public Transform trans_comefromGrid;//来源
    #region 装备详情
    public Transform trans_equipMainProGrid;//装备主属性
    public Transform trans_equipViceProGrid;//装备副属性
    public Transform trans_equipGemProGrid;//装备上的宝石属性
    public Button btn_wenHao;
    public Text txt_youHuaLv;//优化率
    #endregion

    public Transform trans_gemShowProGrid;//宝石的属性
    //public Text txt_gemName;//宝石名

    public Text tag_txt;
    #endregion
    public override void Init(object[] args)
    {
        base.Init(args);
        itemModel = RoleManager.Instance._CurGameInfo.ItemModel;
        EventCenter.Register(TheEventType.GetItem, OnGetItem);
        EventCenter.Register(TheEventType.LoseItem, OnLoseItem);
        EventCenter.Register(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.OnUnEquip, OnUnEquip);
        RegisterEvent(TheEventType.EquipIntense, OnIntense);
        RegisterBtnClick();//初始化点击事件

    }
    /// <summary>
    /// 初始化点击事件
    /// </summary>
    void RegisterBtnClick()
    {
        //addBtnListener(btn_emptyClose, () =>
        //{
        //    PanelManager.Instance.ClosePanel(this);
        //});
        //addBtnListener(btn_commonTag, OnCommonTagClick,false);

        //addBtnListener(btn_sell, () =>
        //{

        //    List<UInt64> idList = new List<ulong>();
        //    List<ulong> numList = new List<ulong>();
        //    for (int i = 0; i < curItemViewList.Count; i++)
        //    {
        //        idList.Add(curItemViewList[i].GetItemData().OnlyId);
        //        numList.Add(curItemViewList[i].GetItemData().Count);
        //    }
        //    if (idList.Count > 0)
        //    {
        //        PanelManager.Instance.OpenCommonHint("确定出售这些物品吗？", () =>
        //        {
        //            ItemManager.Instance.SellItem(idList, numList);

        //        }, null);
        //    }
        //});
        addBtnListener(btn_allTag, OnAllTagClick);
        addBtnListener(btn_equipTag, OnEquipTagClick);
        addBtnListener(btn_itemTag, OnItemTagClick);
        addBtnListener(btn_gemTag, OnGemTagClick);
        addBtnListener(btn_consumeTag, OnConsumeTagClick);

        addBtnListener(btn_wenHao, () =>
         {
             SkillIdType skillIdType = SkillIdType.GuangDan;
             SkillUpgradeSetting upgradeSetting= DataTable.FindSkillUpgradeListBySkillId((int)skillIdType)[curChoosedItemView.GetItemData().equipProtoData.curLevel-1];
             int rate =(int)MathF.Round( upgradeSetting.Damage.ToFloat() + 100);
             PanelManager.Instance.OpenPanel<NoTransparentTipsPanel>(PanelManager.Instance.trans_layer2, "当前普攻加成"+ rate+"%",(Vector2)btn_wenHao.transform.position);
         });
        //OnAllTagClick();
    }

    public override void OnOpenIng()
    {
        if (itemModel == null)
        {
            Debug.LogError("没有初始化itemModel");
            return;
        }
        //InitRedPointShow();//初始化红点
        base.OnOpenIng();
        //ShowItems();
        curTag = ItemShowTag.Equip;
        OnAllTagClick();
        //btn_allTag.Select();
        // 确保UI状态正确
        img_allTag.gameObject.SetActive(true);
        img_equipTag.gameObject.SetActive(false);
        img_itemTag.gameObject.SetActive(false);
        img_gemTag.gameObject.SetActive(false);
        img_consumeTag.gameObject.SetActive(false);

        if (curItemViewList.Count > 0)
        {   //点击第一个
            int clickIndex = 0;
            if (TaskManager.Instance.curGuideItemOnlyId != 0)
            {
                for (int i = 0; i < curItemViewList.Count; i++)
                {
                    if (curItemViewList[i].GetItemData().onlyId == TaskManager.Instance.curGuideItemOnlyId)
                    {
                        clickIndex = i;
                        PanelManager.Instance.CloseTaskGuidePanel();
                        TaskManager.Instance.ShowGuide();
                        break;
                    }
                }
            }
            //curItemViewList[clickIndex].btn.onClick.Invoke();

        }

        //if (RoleManager.Instance._CurGameInfo.playerPeople.CurEquip != null)
        //{
        //    for(int i = 0; i < EquipmentManager.Instance.myEquipList.Count; i++)
        //    {
        //        ItemData item = EquipmentManager.Instance.myEquipList[i];
        //        if(item.OnlyId== RoleManager.Instance._CurGameInfo.playerPeople.CurEquip.OnlyId)
        //        {
        //            ShowCurEquip();
        //            break;
        //        }
        //    }
        //}
        ShowGuide();
    }
 
    void ShowGuide()
    {
        if (TaskManager.Instance.guide_studySkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_consumeTag.gameObject);
        }
        else if (TaskManager.Instance.guide_equipEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_equipTag.gameObject);
        }
    }
    /// <summary>
    /// 点击了全部标签页
    /// </summary>
    void OnAllTagClick()
    {

        if (curTag != ItemShowTag.None)
        {
            PanelManager.Instance.CloseTaskGuidePanel();

            ClearExistedItem();
            ClearCertainParentAllSingle<ItemView>(trans_showItemParent);
            curTag = ItemShowTag.None;
            ShowItems();

            img_allTag.gameObject.SetActive(true);
            img_equipTag.gameObject.SetActive(false);
            img_itemTag.gameObject.SetActive(false);
            img_gemTag.gameObject.SetActive(false);
            img_consumeTag.gameObject.SetActive(false);

            UpdateTagButtonTextColors(ItemShowTag.None);

            trans_equipShow.gameObject.SetActive(false);
            trans_gemShow.gameObject.SetActive(false);
            //点第一个
            if (curItemViewList.Count > 0)
            {
                //curItemViewList[0].btn.onClick.Invoke();
            }
            //否则啥也不显示
            else
            {
                PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);
                //showchoo();
            }
            tag_txt.SetText(btn_allTag.GetComponentInChildren<Text>().text);
            //btn_allTag.GetComponentInChildren<Text>().color = Color.white;
        }
    }
    /// <summary>
    /// 点击了物品标签页
    /// </summary>
    void OnItemTagClick()
    {

        if (curTag != ItemShowTag.Item)
        {
            PanelManager.Instance.CloseTaskGuidePanel();

            ClearExistedItem();
            ClearCertainParentAllSingle<ItemView>(trans_showItemParent);
            curTag = ItemShowTag.Item;
            ShowItems();



            img_allTag.gameObject.SetActive(false);
            img_equipTag.gameObject.SetActive(false);
            img_itemTag.gameObject.SetActive(true);
            img_gemTag.gameObject.SetActive(false);
            img_consumeTag.gameObject.SetActive(false);

            UpdateTagButtonTextColors(ItemShowTag.Item);

            trans_equipShow.gameObject.SetActive(false);
            trans_gemShow.gameObject.SetActive(false);
            //点第一个
            if (curItemViewList.Count > 0)
            {
                //curItemViewList[0].btn.onClick.Invoke();
            }
            //否则啥也不显示
            else
            {
                PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);
                //showchoo();
            }
            tag_txt.SetText(btn_itemTag.GetComponentInChildren<Text>().text);
        }
    }
    /// <summary>
    /// 点击装备标签
    /// </summary>
    void OnEquipTagClick()
    {
        if (curTag != ItemShowTag.Equip)
        {
            PanelManager.Instance.CloseTaskGuidePanel();

            ClearExistedItem();
            ClearCertainParentAllSingle<ItemView>(trans_showItemParent);

            curTag = ItemShowTag.Equip;
            ShowItems();


            img_allTag.gameObject.SetActive(false);
            img_equipTag.gameObject.SetActive(true);
            img_itemTag.gameObject.SetActive(false);
            img_gemTag.gameObject.SetActive(false);
            img_consumeTag.gameObject.SetActive(false);

            UpdateTagButtonTextColors(ItemShowTag.Equip);

            trans_itemShow.gameObject.SetActive(false);
            trans_gemShow.gameObject.SetActive(false);
            //点第一个
            if (curItemViewList.Count > 0)
            {
                //curItemViewList[0].btn.onClick.Invoke();
            }
            else
            {
                PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);
            }

            //显示装备引导
            if (TaskManager.Instance.guide_equipEquip)
            {
                for(int i=0;i< curItemViewList.Count; i++)
                {
                    ItemView itemView = curItemViewList[i];
                    if (itemView.setting.ItemType.ToInt32() == (int)ItemType.Equip)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(itemView.gameObject);

                        break;
                    }
                }

            }
            tag_txt.SetText(btn_equipTag.GetComponentInChildren<Text>().text);
        }
    }
    /// <summary>
    /// 点击宝石标签
    /// </summary>
    void OnGemTagClick()
    {
        if (curTag != ItemShowTag.Gem)
        {
            PanelManager.Instance.CloseTaskGuidePanel();

            ClearExistedItem();
            ClearCertainParentAllSingle<ItemView>(trans_showItemParent);

            curTag = ItemShowTag.Gem;
            ShowItems();



            img_allTag.gameObject.SetActive(false);
            img_equipTag.gameObject.SetActive(false);
            img_itemTag.gameObject.SetActive(false);
            img_gemTag.gameObject.SetActive(true);
            img_consumeTag.gameObject.SetActive(false);

            UpdateTagButtonTextColors(ItemShowTag.Gem);

            trans_equipShow.gameObject.SetActive(false);
            trans_itemShow.gameObject.SetActive(false);
            //点第一个
            if (curItemViewList.Count > 0)
            {
                //curItemViewList[0].btn.onClick.Invoke();
            }
            else
            {
                PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);
                //ShowCurEquip();

            }
            tag_txt.SetText(btn_gemTag.GetComponentInChildren<Text>().text);
        }
    }
    /// <summary>
    /// 点击消耗标签
    /// </summary>
    void OnConsumeTagClick()
    {
        if (curTag != ItemShowTag.Consume)
        {
            PanelManager.Instance.CloseTaskGuidePanel();

            ClearExistedItem();
            ClearCertainParentAllSingle<ItemView>(trans_showItemParent);

            curTag = ItemShowTag.Consume;
            ShowItems();




            img_allTag.gameObject.SetActive(false);
            img_equipTag.gameObject.SetActive(false);
            img_itemTag.gameObject.SetActive(false);
            img_gemTag.gameObject.SetActive(false);
            img_consumeTag.gameObject.SetActive(true);

            UpdateTagButtonTextColors(ItemShowTag.Consume);

            trans_equipShow.gameObject.SetActive(false);
            trans_itemShow.gameObject.SetActive(false);
            //点第一个
            if (curItemViewList.Count > 0)
            {
                //curItemViewList[0].btn.onClick.Invoke();
            }
            else
            {
                PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);
                //ShowCurEquip();

            }
            tag_txt.SetText(btn_consumeTag.GetComponentInChildren<Text>().text);
        }
    }
    /// <summary>
    /// 更新标签按钮文字颜色
    /// </summary>
    void UpdateTagButtonTextColors(ItemShowTag selectedTag)
    {
        btn_allTag.GetComponentInChildren<Text>().color = (selectedTag == ItemShowTag.None) ? selectedTextColor : unselectedTextColor;
        btn_equipTag.GetComponentInChildren<Text>().color = (selectedTag == ItemShowTag.Equip) ? selectedTextColor : unselectedTextColor;
        btn_itemTag.GetComponentInChildren<Text>().color = (selectedTag == ItemShowTag.Item) ? selectedTextColor : unselectedTextColor;
        btn_gemTag.GetComponentInChildren<Text>().color = (selectedTag == ItemShowTag.Gem) ? selectedTextColor : unselectedTextColor;
        btn_consumeTag.GetComponentInChildren<Text>().color = (selectedTag == ItemShowTag.Consume) ? selectedTextColor : unselectedTextColor;
    }


    /// <summary>
    /// 得到物品 若得到的超过了限度，则重新刷新
    /// </summary>
    void OnGetItem(object[] param)
    {
        int settingId = (int)param[0];
        ulong count = (ulong)param[1];
        if (curOnlyIdList.Count == 0)
        {
            ShowItems();
            return;
        }
        ItemSetting setting = DataTable.FindItemSetting(settingId);
        //不显示在背包
        if (setting.ShowInKnapsack.ToInt32() != 1)
            return;
        if ((ItemShowTag)setting.ShowTag.ToInt32() != curTag
            && curTag != ItemShowTag.None)
            return;
        //如果不可叠加 且得到当前页面的物品
        if (setting.OverLay.ToInt32() == 0)
        {
            ShowItems();
        }
        //如果可叠加
        else
        {
            //如果有同类物品 找未满堆叠上限的物品
            if (itemModel.itemIdList.Contains(settingId))
            {
                ItemData notFullItem =ItemManager.Instance.FindOveryLayNotFullItemBySettingId(settingId);
                //如果规定了该物品的叠加上限且增加值超过了上限
                if (setting.MaxOverLay.ToInt32() > 0
                    && count + notFullItem.count > setting.MaxOverLay.ToUInt64())
                {
                    ShowItems();
                }
                //如果没有规定叠加上限 则只变它
                else
                {
                    int index = curSettingIdList.IndexOf(notFullItem.settingId);
                    if (index < 0)
                    {
                        ShowItems();
                    }
                    else
                    {
                        curItemDataList[index] = notFullItem;
                        curItemViewList[index].SetItemData(notFullItem);
                        curItemViewList[index].RefreshShow();
                    }
    
                }
            }
            //如果没有同类物品
            else
            {
                ShowItems();
            }
        }
    }


    /// <summary>
    /// 失去物品 若失去的超过了限度，则重新刷新
    /// </summary>
    void OnLoseItem(object[] param)
    {

        int settingId = (int)param[0];
        ulong count = (ulong)param[1];

        ItemSetting setting = DataTable.FindItemSetting(settingId);
        if (setting.ShowInKnapsack.ToInt32() != 1)
            return;
        if ((ItemShowTag)setting.ShowTag.ToInt32() != curTag
            && curTag != ItemShowTag.None)
            return;
        //如果不可叠加 且失去了当前页面的物品
        if (setting.OverLay.ToInt32() == 0)
        {
            ShowItems();
        }
        //如果可叠加 且失去了当前页面的物品
        else 
        {
            //如果有同类物品 找未满堆叠上限的物品
            if (itemModel.itemIdList.Contains(settingId))
            {
                ItemData notFullItem = ItemManager.Instance.FindOveryLayNotFullItemBySettingId(settingId);
                //如果丢完了或者规定了该物品的叠加上限且减少值大于等于上限
                if (notFullItem==null||(
                    setting.MaxOverLay.ToInt32() > 0
                    && count >= notFullItem.count))
                {
                    ShowItems();
                }
                //如果没有规定叠加上限 则只变它
                else
                {
                    int index = curSettingIdList.IndexOf(notFullItem.settingId);
                    if (index < 0)
                    {
                        ShowItems();
                    }
                    else
                    {
                        curItemDataList[index] = notFullItem;
                        curItemViewList[index].SetItemData(notFullItem);
                        curItemViewList[index].RefreshShow();
                    }

                }
            }
            //如果没有同类物品
            else
            {
                ShowItems();
            }
        }
    }


    /// <summary>
    /// 显示物品 完全刷新，性能消耗最大
    /// </summary>
    void ShowItems()
    {
        ClearExistedItem();
        //已持有物品或题材
        for(int i=0;i< itemModel.itemIdList.Count; i++)
        {
            ItemData showData = itemModel.itemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(showData.settingId);
            if (setting == null) continue;
            if ((setting.ShowTag.ToInt32() != (int)curTag
                && curTag!=ItemShowTag.None)
                || setting.ShowInKnapsack!="1")
            {
                continue;
            }
            //如果数量过多则分割
        
            curOnlyIdList.Add(showData.onlyId);
            curSettingIdList.Add(showData.settingId);
            curItemDataList.Add(showData);
            
        }
        //排序
        //消耗品排前面
        for (int i = 0; i < curItemDataList.Count - 1; i++)
        {
            for (int j = 0; j < curItemDataList.Count - 1 - i; j++)
            {
                //跳过空物品
                if (curItemDataList[j] == null || curItemDataList[j].setting == null
                    || curItemDataList[j + 1] == null || curItemDataList[j + 1].setting == null)
                    continue;
                    
                //前面的大于后面的，则二者交换
                if (curItemDataList[j].setting.ItemType.ToInt32()!=(int)ItemType.Dan
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.GongFaShu
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.YuanLi
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.XingChenBox
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.NianZhuBox
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.BuildingUpgradeBox
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.QingLingCaoBox
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.LingShiFuDai
                    && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.JieYinLing
                     && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.OPBox
                     && curItemDataList[j].setting.ItemType.ToInt32() != (int)ItemType.QiHuoBox)
                {
                    var temp = curItemDataList[j];
                    curItemDataList[j] = curItemDataList[j + 1];
                    curItemDataList[j + 1] = temp;

                    var temp2 = curOnlyIdList[j];
                    curOnlyIdList[j] = curOnlyIdList[j + 1];
                    curOnlyIdList[j + 1] = temp2;


                    var temp3 = curSettingIdList[j];
                    curSettingIdList[j] = curSettingIdList[j + 1];
                    curSettingIdList[j + 1] = temp3;
                }
            }
        }
        for(int i = 0; i < curItemDataList.Count; i++)
        {
            ItemView single =AddSingle<KnapsackItemView>(trans_grid, curItemDataList[i], this);// UIUtil.CreateSingleItemView(trans_grid, showData);
            curItemViewList.Add(single);

        }
        ////点第一个
        //if (curItemViewList.Count > 0)
        //{
        //    curItemViewList[0].btn.onClick.Invoke();
        //}
        //else
        //{
        //    HideItemTips();
        //    trans_itemShow.gameObject.SetActive(false);
        //    trans_equipShow.gameObject.SetActive(false);
        //    trans_gemShow.gameObject.SetActive(false);
        //}

    }
    /// <summary>
    /// 隐藏tips
    /// </summary>
    public void HideItemTips()
    {
        trans_tips.gameObject.SetActive(false);
    }
    ///// <summary>
    ///// 显示物品tips
    ///// </summary>
    ///// <param name="itemData"></param>
    //public void ShowItemTips(ItemData itemData)
    //{  
    //    trans_tips.gameObject.SetActive(true);

    //    trans_itemShow.gameObject.SetActive(false);
    //    trans_equipShow.gameObject.SetActive(false);
    //    trans_gemShow.gameObject.SetActive(false);
    //    ItemSetting setting = DataTable.FindItemSetting(itemData.settingId);
    //    ClearCertainParentAllSingle<NameWordView>(trans_nameGrid);
    //    for (int i = 0; i < setting.name.Length; i++)
    //    {
    //        char word = setting.name[i];
    //        AddSingle<NameWordView>(trans_nameGrid, word);
    //    }
    //    txt_showItemDesTxt.SetText(setting.des);
    //    txt_specialDes.SetText(ItemManager.Instance.ItemSpecialDes(itemData));
    //    //显示当前页面
    //    switch (setting.itemType.ToInt32())
    //    {
    //        case (int)ItemType.Gem:
    //            trans_gemShow.gameObject.SetActive(true);
    //            //PanelManager.Instance.CloseAllSingle(trans_gemShowItemGrid);
    //            ClearCertainParentAllSingle<GemPropertyView>(trans_gemShowProGrid);

    //            // PanelManager.Instance.OpenSingle<ItemView>(trans_gemShowItemGrid,itemView.GetItemData());
    //            for (int i = 0; i < itemData.gemData.propertyList.Count; i++)
    //            {
    //                AddSingle<GemPropertyView>(trans_gemShowProGrid, true, itemData.gemData.propertyList[i],itemData.gemData);
    //            }
    //            int emptyCount = 3 - itemData.gemData.propertyList.Count;
    //            for (int i = 0; i < emptyCount; i++)
    //            {
    //                AddSingle<GemPropertyView>(trans_gemShowProGrid, false);
    //            }
    //            break;
    //        case (int)ItemType.Equip:
    //            ShowChoosedEquip(itemData);
    //            if (TaskManager.Instance.triggerGuide_Equip)
    //            {
    //                for (int i = 0; i < trans_downBtnGrid.childCount; i++)
    //                {
    //                    KnapsackDownBtnView downBtnView = trans_downBtnGrid.GetChild(i).GetComponent<KnapsackDownBtnView>();
    //                    if (downBtnView.btnType == KnapsackDownBtnType.Equip)
    //                    {
    //                        PanelManager.Instance.ShowTaskGuidePanel(downBtnView.gameObject);
    //                    }
    //                }
    //            }

    //            break;

    //    }
    //    ClearCertainParentAllSingle<SingleComefromView>(trans_comefromGrid);
    //    if (!string.IsNullOrWhiteSpace(setting.comeFrom))
    //    {
    //        string[] comefromArr = setting.comeFrom.Split('|');
    //        for (int i = 0; i < comefromArr.Length; i++)
    //        {
    //            AddSingle<SingleComefromView>(trans_comefromGrid, itemData, setting, i);

    //        }
    //    }
     
    //}

    ///// <summary>
    ///// 点击了物体
    ///// </summary>
    //public void OnClickedItem(KnapsackItemView itemView)
    //{
    //    ShowItemTips(itemView.GetItemData());

    //    for (int i = 0; i < curItemViewList.Count; i++)
    //    {
    //        KnapsackItemView theItemView = (KnapsackItemView)curItemViewList[i];
    //        if (itemView.GetItemData().onlyId == theItemView.GetItemData().onlyId)
    //        {
    //            theItemView.OnChoosed(true);
    //        }
    //        else
    //        {
    //            theItemView.OnChoosed(false);
    //        }
    //    }

    //    ClearCertainParentAllSingle<ItemView>(trans_showItemParent);
    //    AddSingle<ItemView>(trans_showItemParent, itemView.GetItemData());

    //    curChoosedItemView = itemView;
    //    //显示下部按钮
    //    PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);

    //    //switch ((ItemShowTag)curChoosedItemView.setting.showTag.ToInt32())
    //    //{
    //    //    case ItemShowTag.Equip:
      
    //    //        //已装备，显示卸下
    //    //        if (curChoosedItemView.GetItemData().equipProtoData.isEquipped)
    //    //        {
    //    //            PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.UnEquip, this);
    //    //        }
    //    //        //未装备，替换
    //    //        else
    //    //        {
    //    //            KnapsackDownBtnView view = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Equip, this);

    //    //            //装备

    //    //            if (TaskManager.Instance.guide_equipEquip)
    //    //            {
    //    //                PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);


    //    //            }

    //    //        }
    //    //        PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Intense, this);
    //    //        PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.AddGem, this);
    //    //        //PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Sell, this);

    //    //        break;
    //    //    case ItemShowTag.Item:
    //    //       // ShowChoosedItem(curChoosedItemView.GetItemData());
            
    //    //        break;
    //    //    case ItemShowTag.Consume:
    //    //        PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Sell, this);
    //    //        //技能书
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.SkillBook)
    //    //        {
    //    //           KnapsackDownBtnView btnView=  PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Study, this);
    //    //            if (TaskManager.Instance.guide_studySkill)
    //    //            {

    //    //                if (itemView.setting.itemType.ToInt32() == (int)ItemType.SkillBook)
    //    //                {
    //    //                    PanelManager.Instance.ShowTaskGuidePanel(btnView.gameObject);
    //    //                }

    //    //            }
    //    //        }
    //    //        //修为丹
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.Dan)
    //    //        {
    //    //            PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);
    //    //        }   
    //    //        //源力结晶
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.YuanLi)
    //    //        {
    //    //            PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);
    //    //        }
    //    //        //功法书
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.GongFaShu)
    //    //        {
    //    //            KnapsackDownBtnView btnView= PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);
               
    //    //        }
    //    //        //宝石箱
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.XingChenBox)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);
    //    //        }
    //    //        //清灵草箱子
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.QingLingCaoBox)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);

    //    //        }
    //    //        //灵石福袋
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.LingShiFuDai)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);

    //    //        }
    //    //        //念珠箱
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.NianZhuBox)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);

    //    //        }
    //    //        //建筑材料箱
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.BuildingUpgradeBox)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);

    //    //        }
    //    //        //接引令
    //    //        if (itemView.setting.itemType.ToInt32() == (int)ItemType.JieYinLing)
    //    //        {
    //    //            KnapsackDownBtnView btnView = PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Use, this);

    //    //        }
    //    //        break;

    //    //    case ItemShowTag.Gem:
    //    //        PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Composite, this);

    //    //        PanelManager.Instance.OpenSingle<KnapsackDownBtnView>(trans_downBtnGrid, KnapsackDownBtnType.Sell, this);
    //    //        break;
    //    //}
   

    //}

    ///// <summary>
    ///// 显示选择的物品
    ///// </summary>
    //void ShowChoosedItem(ItemData itemData)
    //{
    //    trans_equipShow.gameObject.SetActive(false);
    //    trans_gemShow.gameObject.SetActive(false);
    //    trans_itemShow.gameObject.SetActive(true);
    //    ItemSetting itemSetting = DataTable.FindItemSetting(itemData.SettingId);
    //    txt_showItemDesTxt.SetText(itemSetting.des);

    //}
    /// <summary>
    /// 显示装备
    /// </summary>
    void ShowChoosedEquip(ItemData itemData)
    {
        
        trans_itemShow.gameObject.SetActive(false);
        trans_gemShow.gameObject.SetActive(false);
        trans_equipShow.gameObject.SetActive(true);
        //ClearCertainParentAllSingle<ItemView>(trans_curEquipGrid);
        //ClearCertainParentAllSingle<SingleSkillView>(trans_curSkillGrid);
        //ItemData itemData = EquipmentManager.Instance.GetCurEquip();

        ClearCertainParentAllSingle<EquipMainPropertyView>(trans_equipMainProGrid);
        ClearCertainParentAllSingle<EquipVicePropertyView>(trans_equipViceProGrid);
        ClearCertainParentAllSingle<GemProTxtView>(trans_equipGemProGrid);

        //AddSingle<ItemView>(trans_curEquipGrid, itemData);
        AddSingle<EquipMainPropertyView>(trans_equipMainProGrid, itemData.equipProtoData.propertyList[0]);

        for (int i = 1; i < itemData.equipProtoData.propertyList.Count; i++)
        {
            AddSingle<EquipVicePropertyView>(trans_equipViceProGrid, itemData.equipProtoData.propertyList[i]);

        }
        List<int> gemProIdList = new List<int>();
        List<int> gemProNumList = new List<int>();
        for(int i = 0; i < itemData.equipProtoData.gemList.Count; i++)
        {
            ItemData gemItem = itemData.equipProtoData.gemList[i];
            if (gemItem == null||gemItem.onlyId<=0)
                continue;
            GemData gem = itemData.equipProtoData.gemList[i].gemData;
            for(int j = 0; j < gem.propertyList.Count; j++)
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
        for(int i = 0; i < gemProIdList.Count; i++)
        {
            AddSingle<GemProTxtView>(trans_equipGemProGrid, gemProIdList[i], gemProNumList[i]);
        }
        EquipmentSetting equipSetting = DataTable.FindEquipSetting(itemData.equipProtoData.settingId);

        float youHuaLv = itemData.equipProtoData.youHuaLv / 480f;
        float youHuaShow = youHuaLv * 100;
        Rarity rarity =(Rarity)Mathf.CeilToInt(youHuaShow / 20);
        txt_youHuaLv.SetText("总优化率：" + youHuaShow.ToString("0.0")+"%");
        txt_youHuaLv.color = CommonUtil.RarityColor(rarity);
            ////显示技能
            //List<int> skillIdList = CommonUtil.SplitCfgOneDepth(equipSetting.skillId);
            //for (int j = 0; j < skillIdList.Count; j++)
            //{
            //    int skillId = skillIdList[j];
            //   AddSingle<SingleSkillView>(trans_curSkillGrid, skillId);
            //}
        
      
    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="param"></param>
    void OnEquip(object[] param)
    {
        ItemData itemData = param[0] as ItemData;
 
        for(int i = 0; i < curItemViewList.Count; i++)
        {
            ItemView itemView = curItemViewList[i];

            if (itemView.setting.ItemType.ToInt32()==(int)ItemType.Equip)
            {
                ItemData viewItemData = itemView.GetItemData();
                if (viewItemData == null || viewItemData.equipProtoData == null) continue;
                itemView.RefreshShow();
                if (viewItemData.equipProtoData.onlyId == itemData.onlyId)
                {
                    //ShowCurEquip();
                    itemView.btn.onClick.Invoke();
                }
            }
        }
    }
  
    /// <summary>
    /// 卸下
    /// </summary>
    void OnUnEquip()
    {
    
        //curChoosedItemView.btn.onClick.Invoke();
        if(curChoosedItemView!=null)
            curChoosedItemView.RefreshShow();
    }

    /// <summary>
    /// 强化装备
    /// </summary>
    void OnIntense()
    {
        if (curChoosedItemView != null)
        {
            curChoosedItemView.RefreshShow();
        }
    }

    /// <summary>
    /// 清理已有的物品
    /// </summary>
    void ClearExistedItem()
    {
        curSettingIdList = new List<int>();
        curOnlyIdList = new List<ulong>();
        curItemViewList = new List<ItemView>();
        curItemDataList = new List<ItemData>();
        //UIUtil.ClearChildren(trans_grid, ObjectPoolSingle.ItemView);
        PanelManager.Instance.CloseAllSingle(trans_grid);
    }


    public override void Clear()
    {
        EventCenter.Remove(TheEventType.GetItem, OnGetItem);
        EventCenter.Remove(TheEventType.LoseItem, OnLoseItem);
        EventCenter.Remove(TheEventType.OnEquip, OnEquip);
        EventCenter.Remove(TheEventType.OnUnEquip, OnUnEquip);

        ClearExistedItem();
        PanelManager.Instance.CloseAllSingle(trans_downBtnGrid);


        curTag = ItemShowTag.None;
        curChoosedItemView = null;

    }
    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.guide_studySkill = false;
        TaskManager.Instance.guide_equipEquip = false;

    }


}


/// <summary>
/// 物品标签页
/// </summary>
public enum ItemShowTag
{
    None=0,
    Item=1,//道具
    Equip=2,//法器
    Gem=3,//宝石
    Consume=4,//消耗
}