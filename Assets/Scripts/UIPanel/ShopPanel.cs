using cfg;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : PanelBase
{
    public Button bigtag_chouJiang;
    public Button bigtag_fangShi;
    public Button bigtag_chongZhi;
    public Button bigtag_moonCard;
    public Button bigtag_liBao;//礼包

    public Color color_txtBlack = new Color32(196, 175, 154, 255);


    #region 礼包
    public Transform trans_liBao;//礼包
    public Transform trans_liBaoGrid;//礼包格子

    public Button btn_dailyLiBao;//日礼包
    public Button btn_xinShouLiBao;//新手礼包

    #endregion

    #region 月卡
    public Transform trans_moonCard;//月卡
    public Transform trans_moonCardGrid;//月卡格子
    #endregion


    #region 充值
    public Transform trans_chongZhi;
    public Transform grid_chongZhi;
    #endregion


    #region 坊市
    public Transform trans_fangShi;

    public RectTransform rectTrans_scrollBg;//scroll背景
    public int smallScrollHeight = 1000;//小高度
    public int bigScrollHeight = 1286;//大高度
    public Transform trans_grid;

    public Sprite sprt_btnOn;//亮按钮
    public Sprite sprt_btnOff;//暗按钮

    public Button btn_GuangGaoLingShopTag;//广告令商城
    public Button btn_MatchShopTag;//大比商城
    public Button btn_DayliGuangGaoFuLiTag;//每日广告福利商城
    public Button btn_huoYueBiTag;//活跃币广告商城

    public ShopType curShowShopType;//当前显示的商店类型

    public ShopTag curBigShopTag;//当前商城大标签

    public Text txt_brushCD;//刷新cd
    public Image img_moneyIcon;
    public Text txt_moneyNum;
    public Transform trans_money;//我的钱
    #region 广告币商城
    public Transform trans_guangGaoLingShop;
    public Button btn_GuangGaoLingShopBrush_GuangGaoLing;//广告令商店刷新按钮用广告令
    public Button btn_GuangGaoLingShopBrush_LingShi;//广告令商店刷新按钮用灵石
    public Text txt_todayBrushRemain;//今日剩余刷新
    public Transform trans_guangGaoLingBrushConsume;//广告令商店刷新消耗
    public Transform trans_lingShiBrushConsume;//灵石消耗
    #endregion

    #region 每日广告
    public Transform trans_dayLiGuangGaoFuLi;//总面板
    public Transform trans_totalADAward;//广告总次数奖励
    public Text txt_allADProcess;
    public Transform trans_allADProcess;//广告奖励进度
    public Image img_allADProcessBar;
    public Text txt_allADDes;//剩余几次可领取
    public Transform trans_allADAwardGrid;
    public Button btn_lingQuTotalADAward;//可领取
    public Transform trans_allADAwardGridLeftPos;//左边的位置
    public Transform trans_allADAwardGridRightPos;//右边的位置

    #endregion

    #region 活跃币广告
    #endregion
    #endregion

    ShopTag initShopTag;
    public int graid_vtop=75;
    public float spacingjg;

    public Image xzimage1;
    public Image xzimage2;

    public GameObject nextBrushTime;

    public Image buttom_btn;

    public Sprite img_fangshi;
    public Sprite img_yueka;
    public Sprite img_chongzhi;
    public Sprite img_libao;
    public override void Init(params object[] args)
    {
        base.Init(args);
        initShopTag = (ShopTag)args[0];
        
        //graid_vtop = 70;
        spacingjg = trans_grid.GetComponent<VerticalLayoutGroup>().spacing;
        nextBrushTime.SetActive(true);
        xzimage1.gameObject.SetActive(false);
        xzimage2.gameObject.SetActive(false);

            

        addBtnListener(btn_GuangGaoLingShopTag, () =>
         {
             ShowShopItem(ShopType.GuangGaoLing);
         });
        addBtnListener(btn_MatchShopTag, () =>
        {
            ShowShopItem(ShopType.Match);
        });
        addBtnListener(btn_DayliGuangGaoFuLiTag, () =>
        {
            ShowShopItem(ShopType.DailyGuangGaoFuLi);
        });
        addBtnListener(btn_huoYueBiTag, () =>
        {
            ShowShopItem(ShopType.HuoYueBi);
        });

        addBtnListener(btn_GuangGaoLingShopBrush_GuangGaoLing, () =>
        {
            xzimage1.gameObject.SetActive(false);
            xzimage2.gameObject.SetActive(true);
            List<List<List<int>>> brushCost = CommonUtil.SplitThreeCfg(ConstantVal.guangGaoLingShopBrushCost);
            SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(ShopType.GuangGaoLing);
            if (data.TodayBrushNum >= brushCost.Count)
            {
                PanelManager.Instance.OpenFloatWindow("今日刷新次数已达上限");
                return;
            }
            List<List<int>> needItem = brushCost[data.TodayBrushNum];
            ulong consumeNum = 0;
            for(int i = 0; i < needItem.Count; i++)
            {
                List<int> single = needItem[i];
                if (single[0] == (int)ItemIdType.LingJing)
                {
                    consumeNum = (ulong)single[1];
                }
            }
            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingJing, consumeNum))
            {
                PanelManager.Instance.OpenFloatWindow("天晶不够");
                return;
            }
            if(ItemManager.Instance.LoseItem((int)ItemIdType.LingJing, consumeNum))
            {
                ShopManager.Instance.BrushShop(ShopType.GuangGaoLing);
            }
        });
        addBtnListener(btn_GuangGaoLingShopBrush_LingShi, () =>
        {
            xzimage1.gameObject.SetActive(true);
            xzimage2.gameObject.SetActive(false);
            List<List<List<int>>> brushCost = CommonUtil.SplitThreeCfg(ConstantVal.guangGaoLingShopBrushCost);
            SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(ShopType.GuangGaoLing);
            if (data.TodayBrushNum >= brushCost.Count)
            {
                PanelManager.Instance.OpenFloatWindow("今日刷新次数已达上限");
                return;
            }
            if (data.TodayBrushNum >= 3)
            {
                return;
            }
            List<List<int>> needItem = brushCost[data.TodayBrushNum];
            ulong consumeNum = 0;
            for (int i = 0; i < needItem.Count; i++)
            {
                List<int> single = needItem[i];
                if (single[0] == (int)ItemIdType.LingShi)
                {
                    consumeNum = (ulong)single[1];
                }
            }
            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, consumeNum))
            {
                         ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
                return;
            }
            if (ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, consumeNum))
            {
                ShopManager.Instance.BrushShop(ShopType.GuangGaoLing);
            }
        });
        addBtnListener(btn_lingQuTotalADAward, () =>
        {
            ADManager.Instance.OnLingQuTotalADAward();
        });
        addBtnListener(bigtag_chouJiang, () =>
        {
            PanelManager.Instance.OpenPanel<ChouJiangPanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(bigtag_chongZhi, () =>
        {
            ShowTagShop(ShopTag.ChongZhi);
        });

        addBtnListener(bigtag_fangShi, () =>
        {
            ShowTagShop(ShopTag.FangShi);
        });
        addBtnListener(bigtag_moonCard, () =>
        {
            ShowTagShop(ShopTag.MoonCard);
        });
        addBtnListener(bigtag_liBao, () =>
        {
            ShowTagShop(ShopTag.LiBao);
        });

        addBtnListener(btn_dailyLiBao, () =>
        {
            ShowLiBaoShopItem(ShopType.DailyLiBao);
        });
        addBtnListener(btn_xinShouLiBao, () =>
        {
            ShowLiBaoShopItem(ShopType.XinShouLiBao);
        });

        RegisterEvent(TheEventType.RealityOneMinProcess, BrushCDShow);

        RegisterEvent(TheEventType.BrushShop, OnBrushShop);
        RegisterEvent(TheEventType.OnSuccessWatchGuangGao, OnSuccessWatchGuangGao);
        RegisterEvent(TheEventType.RefreshAllGuangGaoAwardShow, OnRefreshAllGuangGaoAwardShow);
        RegisterEvent(TheEventType.OnBuyItem, OnBuyItem);


        sprt_btnOn = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.sprt_shopBtnOn);
        sprt_btnOff = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.sprt_shopBtnOff);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowTagShop(initShopTag);
    }

    void OnBrushShop()
    {
        ShowShopItem(curShowShopType);
    }

    void BrushCDShow()
    {
        long guardNextTimeDistance = CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.allShopData.ShopList[0].LastBrushTime);
        long theNextTimeStamp = guardNextTimeDistance + RoleManager.Instance._CurGameInfo.allShopData.ShopList[0].LastBrushTime;
        long nowToNextTimeDistance = theNextTimeStamp - CGameTime.Instance.GetTimeStamp();
        long hour = nowToNextTimeDistance / 3600;
        long min = (nowToNextTimeDistance - hour * 3600) / 60;
        long sec = nowToNextTimeDistance - hour * 3600 - min * 60;
        txt_brushCD.SetText(hour + "时" + min + "分");
    }
    #region 每日广告福利
    void OnSuccessWatchGuangGao()
    {
        if (curShowShopType == ShopType.DailyGuangGaoFuLi)
        {
            ShowShopItem(curShowShopType);
        }
    }
    void OnRefreshAllGuangGaoAwardShow()
    {
        if (curShowShopType == ShopType.DailyGuangGaoFuLi)
        {
            ShowShopItem(curShowShopType);
        }
    }
    #endregion

    public List<Button> btns = new List<Button>();
    #region 坊市
    /// <summary>
    /// 显示商品
    /// </summary>
    /// <param name="shopType"></param>
    void ShowShopItem(ShopType shopType)
    {
        curShowShopType = shopType;
        ClearCertainParentAllSingle<SingleViewBase>(trans_grid);
        trans_grid.GetComponent<VerticalLayoutGroup>().padding.top = 28;
        SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(shopType);
        if (data == null)
            return;
        ItemIdType moneyIdType = ItemIdType.None;

        nextBrushTime.SetActive(true);
        if (shopType == ShopType.GuangGaoLing)
        {
            SetScroll(true);

            trans_grid.GetComponent<VerticalLayoutGroup>().padding.top = graid_vtop;
            trans_grid.GetComponent<VerticalLayoutGroup>().spacing = spacingjg;

            //btn_GuangGaoLingShopTag.GetComponentInParent<Image>().sprite = sprt_btnOn;
            //btn_MatchShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_huoYueBiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_DayliGuangGaoFuLiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;


            ShopManager.Instance.ShopingChoose(btns, btn_GuangGaoLingShopTag, sprt_btnOn, sprt_btnOff);

            trans_guangGaoLingShop.gameObject.SetActive(true);
            trans_dayLiGuangGaoFuLi.gameObject.SetActive(false);

            for (int i = 0; i < 5; i++)
            {
                ShopItemData shopItemData= data.ShopItemList[i];

                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                AddSingle<ShopItemView>(trans_grid, itemData, shopItemData, setting);
            }
            AddSingle<FenGeView>(trans_grid);
            for (int i = 5; i < 8; i++)
            {
                ShopItemData shopItemData = data.ShopItemList[i];

                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                AddSingle<ShopItemView>(trans_grid, itemData, shopItemData, setting);
            }
            AddSingle<FenGeView>(trans_grid);
            for (int i = 8; i < 12; i++)
            {
                ShopItemData shopItemData = data.ShopItemList[i];

                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                AddSingle<ShopItemView>(trans_grid, itemData, shopItemData, setting);

                if (moneyIdType == ItemIdType.None)
                {
                    List<int> price = CommonUtil.SplitCfgOneDepth(setting.Price);
                    if (price.Count == 2)
                    {
                        moneyIdType = (ItemIdType)price[0];
                    }
                }
               
            }

            //广告令刷新价格
            List<List<List<int>>> brushCost = CommonUtil.SplitThreeCfg(ConstantVal.guangGaoLingShopBrushCost);
            if (data.TodayBrushNum >= brushCost.Count)
            {
                btn_GuangGaoLingShopBrush_LingShi.gameObject.SetActive(false);
                btn_GuangGaoLingShopBrush_GuangGaoLing.gameObject.SetActive(false);
            }
            else
            {
       
                txt_todayBrushRemain.SetText(data.TodayBrushNum + "/" + brushCost.Count);
                List<List<int>> needItem = brushCost[data.TodayBrushNum];
                ulong consumeNum = 0;

                for (int i = 0; i < needItem.Count; i++)
                {
                    List<int> single = needItem[i];
                    if (single[0] == (int)ItemIdType.LingJing)
                    {
                        consumeNum = (ulong)single[1];
                    }
                }
                ClearCertainParentAllSingle<SingleViewBase>(trans_guangGaoLingBrushConsume);
                //隐藏文本
                SingleConsumeView guangGaoLingBrushConsume_singleConsumeview = AddSingle<SingleConsumeView>(trans_guangGaoLingBrushConsume, (int)ItemIdType.LingJing, (int)consumeNum, ConsumeType.Item);
                guangGaoLingBrushConsume_singleConsumeview.img_bg.gameObject.SetActive(false);
                guangGaoLingBrushConsume_singleConsumeview.txt.gameObject.SetActive(false);
                if (data.TodayBrushNum >= 3)
                {
                    btn_GuangGaoLingShopBrush_LingShi.gameObject.SetActive(false);
                }
                else
                {
                    for (int i = 0; i < needItem.Count; i++)
                    {
                        List<int> single = needItem[i];
                        if (single[0] == (int)ItemIdType.LingShi)
                        {
                            consumeNum = (ulong)single[1];
                        }
                    }
                    ClearCertainParentAllSingle<SingleViewBase>(trans_lingShiBrushConsume);
                    //隐藏文本
                    SingleConsumeView lingShiBrushConsume_singleConsumeview = AddSingle<SingleConsumeView>(trans_lingShiBrushConsume, (int)ItemIdType.LingShi, (int)consumeNum, ConsumeType.Item);
                    lingShiBrushConsume_singleConsumeview.img_bg.gameObject.SetActive(false);
                    lingShiBrushConsume_singleConsumeview.txt.gameObject.SetActive(false);

                    btn_GuangGaoLingShopBrush_LingShi.gameObject.SetActive(true);

                }
            }
         
        }
        //大比商城
        else if (shopType == ShopType.Match)
        {
            SetScroll(false);

            trans_grid.GetComponent<VerticalLayoutGroup>().padding.top = -10;
            trans_grid.GetComponent<VerticalLayoutGroup>().spacing = spacingjg;

            //btn_GuangGaoLingShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_MatchShopTag.GetComponentInParent<Image>().sprite = sprt_btnOn;
            //btn_huoYueBiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_DayliGuangGaoFuLiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;

            ShopManager.Instance.ShopingChoose(btns, btn_MatchShopTag, sprt_btnOn, sprt_btnOff);

            trans_guangGaoLingShop.gameObject.SetActive(false);
            trans_dayLiGuangGaoFuLi.gameObject.SetActive(false);

            //按照段位分
            List<int> duanWeiList = new List<int>();
            List<List<ShopItemData>> duanWeiShopItemDataList = new List<List<ShopItemData>>();
 
            for (int i = 0; i < data.ShopItemList.Count; i++)
            {
                ShopItemData shopItemData = data.ShopItemList[i];
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                int duanWei = setting.Param.ToInt32();
                if (!duanWeiList.Contains(duanWei))
                {
                    duanWeiList.Add(duanWei);
                    duanWeiShopItemDataList.Add(new List<ShopItemData>());
                }
                int index = duanWeiList.IndexOf(duanWei);
                duanWeiShopItemDataList[index].Add(shopItemData);

                if (moneyIdType == ItemIdType.None)
                {
                    List<int> price = CommonUtil.SplitCfgOneDepth(setting.Price);
                    if (price.Count == 2)
                    {
                        moneyIdType = (ItemIdType)price[0];
                    }
                }
            }

            for (int i = 0; i < duanWeiShopItemDataList.Count; i++)
            {
                List<ShopItemData> theList = duanWeiShopItemDataList[i];
                int duanWei = duanWeiList[i];
                AddSingle<SingleLayerMatchShopGroupView>(trans_grid, theList, duanWei, i + 1);
            }
        }
        //活跃币商城
        else if (shopType == ShopType.HuoYueBi)
        {
            SetScroll(false);

            trans_grid.GetComponent<VerticalLayoutGroup>().padding.top = graid_vtop;
            trans_grid.GetComponent<VerticalLayoutGroup>().spacing = spacingjg;

            //btn_GuangGaoLingShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_MatchShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_huoYueBiTag.GetComponentInParent<Image>().sprite = sprt_btnOn;
            //btn_DayliGuangGaoFuLiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            ShopManager.Instance.ShopingChoose(btns, btn_huoYueBiTag, sprt_btnOn, sprt_btnOff);

            trans_guangGaoLingShop.gameObject.SetActive(false);
            trans_dayLiGuangGaoFuLi.gameObject.SetActive(false);

            for (int i = 0; i < data.ShopItemList.Count; i++)
            {
                ShopItemData shopItemData = data.ShopItemList[i];
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                List<int> zongmenRange = CommonUtil.SplitCfgOneDepth(setting.Param);
                if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel <= zongmenRange[1]
                    && RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel >= zongmenRange[0])
                {

                    ItemData itemData = new ItemData();
                    itemData.settingId = shopItemData.ItemId;
                    itemData.count = (ulong)shopItemData.RemainCount;
                    AddSingle<ShopItemView>(trans_grid, itemData, shopItemData, setting);
                }
                if (moneyIdType == ItemIdType.None)
                {
                    List<int> price = CommonUtil.SplitCfgOneDepth(setting.Price);
                    if (price.Count == 2)
                    {
                        moneyIdType = (ItemIdType)price[0];
                    }
                }
            }

        }
        //每日广告次数福利
        else if (shopType == ShopType.DailyGuangGaoFuLi)
        {
            SetScroll(true);

            trans_grid.GetComponent<VerticalLayoutGroup>().padding.top = 25;
            trans_grid.GetComponent<VerticalLayoutGroup>().spacing = 40;
            nextBrushTime.SetActive(false);

            //btn_GuangGaoLingShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_MatchShopTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_huoYueBiTag.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_DayliGuangGaoFuLiTag.GetComponentInParent<Image>().sprite = sprt_btnOn;
            ShopManager.Instance.ShopingChoose(btns, btn_DayliGuangGaoFuLiTag, sprt_btnOn, sprt_btnOff);

            trans_guangGaoLingShop.gameObject.SetActive(false);
            trans_dayLiGuangGaoFuLi.gameObject.SetActive(true);
            for (int i = 0; i < data.ShopItemList.Count; i++)
            {
                ShopItemData shopItemData = data.ShopItemList[i];

                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                GuangGaoShopItemView view= AddSingle<GuangGaoShopItemView>(trans_grid, itemData, shopItemData, setting);
                //int needZongMenLevel = setting.param.ToInt32();
                //if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel < needZongMenLevel)
                //{
                //    view.Lock("宗门Lv." + needZongMenLevel + "解锁");
                //}
            }

            TotalADAwardSetting nextAwardSetting = null;
            TotalADAwardSetting foreAwardSetting = null;
            //总次数
            for (int i = 0; i < DataTable._totalADAwardList.Count; i++)
            {
                if (RoleManager.Instance._CurGameInfo.AllADData.getAwardTotalADWatchNumIndex + 1 == i)
                {
                    nextAwardSetting= DataTable._totalADAwardList[i];
                    if (i > 0)
                        foreAwardSetting = DataTable._totalADAwardList[i - 1];
                    break;
                }
            }

            if (nextAwardSetting != null)
            {
                ClearCertainParentAllSingle<WithCountItemView>(trans_allADAwardGrid);
                List<List<int>> award = CommonUtil.SplitCfg(nextAwardSetting.Award);
                for(int i = 0; i < award.Count; i++)
                {
                    List<int> singleAward = award[i];
                    ItemData item = new ItemData();
                    item.settingId = singleAward[0];
                    item.count = (ulong)singleAward[1];

                    AddSingle<WithCountItemView>(trans_allADAwardGrid, item);
                }
        

                trans_totalADAward.gameObject.SetActive(true);
                //可领取

                if (RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum >= nextAwardSetting.Id.ToInt32())
                {
                    txt_allADDes.gameObject.SetActive(false);
                    trans_allADProcess.gameObject.SetActive(false);
                    txt_allADProcess.gameObject.SetActive(false);
                    btn_lingQuTotalADAward.gameObject.SetActive(true);
                    trans_allADAwardGrid.localPosition = trans_allADAwardGridLeftPos.localPosition;
                }
                else
                {
                    trans_allADProcess.gameObject.SetActive(true);
                    int numMax = nextAwardSetting.Id.ToInt32();
                    int numMin = 0;
                    if (foreAwardSetting != null)
                        numMin = foreAwardSetting.Id.ToInt32();

                    txt_allADProcess.SetText(RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum + "/" + numMax);
                    img_allADProcessBar.fillAmount = ((RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum - numMin) / (float)numMax);
                    txt_allADDes.SetText("再观看" + (nextAwardSetting.Id.ToInt32() - RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum) + "次广告可领取");
                   
                    txt_allADDes.gameObject.SetActive(true);
                    btn_lingQuTotalADAward.gameObject.SetActive(false);
                    trans_allADAwardGrid.localPosition = trans_allADAwardGridRightPos.localPosition;

                }
            }
            //领完了
            else
            {
                trans_totalADAward.gameObject.SetActive(false);

            }
           
        }
     
        if (moneyIdType == ItemIdType.None)
        {
            trans_money.gameObject.SetActive(false);
        }
        else
        {
            trans_money.gameObject.SetActive(true);
            img_moneyIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + DataTable.FindItemSetting((int)moneyIdType).UiName);
            txt_moneyNum.SetText(ItemManager.Instance.FindItemCount((int)moneyIdType).ToString());
        }

        BrushCDShow();

    }

    #endregion
    void OnBuyItem()
    {
        if (curBigShopTag ==ShopTag.FangShi)
            ShowShopItem(curShowShopType);
    }
    #region 充值
    void ShowChongZhiShopItem()
    {
        curShowShopType = ShopType.ChongZhi;
        ClearCertainParentAllSingle<SingleViewBase>(grid_chongZhi);
        SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(curShowShopType);
        if (data == null)
            return;

        for (int i = 0; i < data.ShopItemList.Count; i++)
        {
            ShopItemData shopItemData = data.ShopItemList[i];
            ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);


            ItemData itemData = new ItemData();
            itemData.settingId = shopItemData.ItemId;
            itemData.count = (ulong)shopItemData.RemainCount;
            AddSingle<ChongZhiShopItemView>(grid_chongZhi, itemData, shopItemData, setting);


        }

    }

    #endregion


    #region 月卡
    void ShowMoonCardShopItem()
    {
        curShowShopType = ShopType.MoonCard;
        ClearCertainParentAllSingle<SingleViewBase>(trans_moonCardGrid);
        SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(curShowShopType);
        if (data == null)
            return;

        for (int i = 0; i < data.ShopItemList.Count; i++)
        {
            ShopItemData shopItemData = data.ShopItemList[i];
            ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);

            ItemData itemData = new ItemData();
            itemData.settingId = shopItemData.ItemId;
            itemData.count = (ulong)shopItemData.RemainCount;
            AddSingle<MoonCardShopItemView>(trans_moonCardGrid, itemData, shopItemData, setting);
        }

    }

    #endregion

    public List<Button> btns1;
    #region 礼包
    void ShowLiBaoShopItem(ShopType type)
    {
        curShowShopType = type;


        if (type == ShopType.DailyLiBao)
        {
            //btn_dailyLiBao.GetComponentInParent<Image>().sprite = sprt_btnOn;
            //btn_xinShouLiBao.GetComponentInParent<Image>().sprite = sprt_btnOff;
            ShopManager.Instance.ShopingChoose(btns1, btn_dailyLiBao, sprt_btnOn, sprt_btnOff);
        }
        else
        {
            //btn_dailyLiBao.GetComponentInParent<Image>().sprite = sprt_btnOff;
            //btn_xinShouLiBao.GetComponentInParent<Image>().sprite = sprt_btnOn;
            ShopManager.Instance.ShopingChoose(btns1, btn_xinShouLiBao, sprt_btnOn, sprt_btnOff);
        }

        ClearCertainParentAllSingle<SingleViewBase>(trans_liBaoGrid);
        SingleShopData data = ShopManager.Instance.FindSingleShopDataByType(curShowShopType);
        if (data == null)
            return;

        for (int i = 0; i < data.ShopItemList.Count; i++)
        {
            ShopItemData shopItemData = data.ShopItemList[i];
            if (type == ShopType.XinShouLiBao&&shopItemData.RemainCount<=0)
            {
                continue;
            }
            ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);        
            ItemData itemData = new ItemData();
            itemData.settingId = shopItemData.ItemId;
            itemData.count = (ulong)shopItemData.RemainCount;
            AddSingle<LiBaoShopItemView>(trans_liBaoGrid, itemData, shopItemData, setting);
        }

    }

    #endregion


    void ShowTagShop(ShopTag shopTag)
    {
        curBigShopTag = shopTag;
        switch (shopTag)
        {
            case ShopTag.FangShi:
                buttom_btn.sprite = img_fangshi;
                trans_chongZhi.gameObject.SetActive(false);
                trans_liBao.gameObject.SetActive(false);
                trans_moonCard.gameObject.SetActive(false);
                trans_fangShi.gameObject.SetActive(true);

                bigtag_fangShi.GetComponentInChildren<Text>().color = Color.white;
                bigtag_chongZhi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_liBao.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_moonCard.GetComponentInChildren<Text>().color = color_txtBlack;

                btn_GuangGaoLingShopTag.onClick.Invoke();

     

                break;
            case ShopTag.ChongZhi:
                buttom_btn.sprite = img_chongzhi;

                trans_fangShi.gameObject.SetActive(false);
                trans_liBao.gameObject.SetActive(false);
                trans_moonCard.gameObject.SetActive(false);
                trans_chongZhi.gameObject.SetActive(true);

                bigtag_fangShi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_chongZhi.GetComponentInChildren<Text>().color = Color.white;
                bigtag_liBao.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_moonCard.GetComponentInChildren<Text>().color = color_txtBlack;

                ShowChongZhiShopItem();
                break;
            case ShopTag.MoonCard:
                buttom_btn.sprite = img_yueka;

                trans_fangShi.gameObject.SetActive(false);
                trans_liBao.gameObject.SetActive(false);
                trans_chongZhi.gameObject.SetActive(false);
                trans_moonCard.gameObject.SetActive(true);

                bigtag_fangShi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_chongZhi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_liBao.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_moonCard.GetComponentInChildren<Text>().color = Color.white;

                ShowMoonCardShopItem();
                break;
            case ShopTag.LiBao:
                buttom_btn.sprite = img_libao;

                trans_fangShi.gameObject.SetActive(false);
                trans_chongZhi.gameObject.SetActive(false);
                trans_moonCard.gameObject.SetActive(false);
                trans_liBao.gameObject.SetActive(true);


                bigtag_fangShi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_chongZhi.GetComponentInChildren<Text>().color = color_txtBlack;
                bigtag_liBao.GetComponentInChildren<Text>().color = Color.white;
                bigtag_moonCard.GetComponentInChildren<Text>().color = color_txtBlack;

                btn_dailyLiBao.onClick.Invoke();
                 break;
        }
    }


    void SetScroll(bool small)
    {
        //if (small)
            rectTrans_scrollBg.sizeDelta = new Vector2(rectTrans_scrollBg.sizeDelta.x, smallScrollHeight);
        //else
        //    rectTrans_scrollBg.sizeDelta = new Vector2(rectTrans_scrollBg.sizeDelta.x, bigScrollHeight);

    }
}
