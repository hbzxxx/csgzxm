using COSXML.Network;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;

public class ShopManager : CommonInstance<ShopManager>
{

    public GameInfo gameInfo;
    void OnBuyItem()
    {

    }

    ShopItemData curBuyShopItem;

    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
        //if (gameInfo.allShopData.AccomplishStatusList.Count <= 0)
        //{
        //    for(int i = 0; i < DataTable._leiChongList.Count; i++)
        //    {
        //        gameInfo.allShopData.AccomplishStatusList.Add((int)AccomplishStatus.UnAccomplished);
        //    }
        //}
        int addNum = 0;
        //可能有变动 只能增不能删
        if (DataTable._leiChongList.Count > gameInfo.allShopData.accomplishStatusList.Count)
        {
            addNum = DataTable._leiChongList.Count - gameInfo.allShopData.accomplishStatusList.Count;
        }
        for (int i = 0; i < addNum; i++)
        {
            gameInfo.allShopData.accomplishStatusList.Add((int)AccomplishStatus.UnAccomplished);
        }

    }
    /// <summary>
    /// 刷新商店
    /// </summary>
    /// <param name="shopType"></param>
    public List<ShopSetting> BrushShop(ShopType shopType)
    {
        List<ShopSetting> shopSettingList = DataTable.FindShopSettingListByType((int)shopType);
        List<ShopSetting> candidateList = new List<ShopSetting>();
        //广告令商店(已改成万界商城
        if (shopType == ShopType.GuangGaoLing)
        {
            List<ShopSetting> type1CandidateList = GetGuangGaoTypeList(shopSettingList, GuangGaoShopBrushRuleType.Type1);
            for (int i = 0; i < type1CandidateList.Count; i++)
            {
                candidateList.Add(type1CandidateList[i]);
            }
            //type2
            List<ShopSetting> type2CandidateList = GetGuangGaoTypeList(shopSettingList, GuangGaoShopBrushRuleType.Type2);
            type2CandidateList = CommonUtil.Shuffle(type2CandidateList);
            for (int i = 0; i < 15; i++)
            {
                candidateList.Add(type2CandidateList[i]);
            }

            //List<int> weight2 = new List<int>();
            //for(int i = 0; i < type2CandidateList.Count; i++)
            //{
            //    weight2.Add(type2CandidateList[i].param.ToInt32());
            //}
            //for(int i = 0; i < 3; i++)
            //{
            //    int index = CommonUtil.GetIndexByWeight(weight2);
            //    candidateList.Add(type2CandidateList[index]);
            //    type2CandidateList.RemoveAt(index);
            //    weight2.RemoveAt(index);
            //}

            ////type3
            //List<ShopSetting> type3CandidateList = GetGuangGaoTypeList(shopSettingList, GuangGaoShopBrushRuleType.Type3);
            //List<int> weight3 = new List<int>();
            //for (int i = 0; i < type3CandidateList.Count; i++)
            //{
            //    ShopSetting theSetting3 = type3CandidateList[i];
            //    int weight = 0;
            //    //跟等级有关
            //    if (theSetting3.param.Contains("|"))
            //    {
            //        List<int> paramList = CommonUtil.SplitCfgOneDepth(theSetting3.param);
            //        int minLevel = paramList[0];
            //        int maxLevel = paramList[1];
            //        if(RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel>=minLevel
            //            && RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel<= maxLevel)
            //        {
            //            weight = paramList[2];
            //        }
            //        else
            //        {
            //            weight = paramList[3];
            //        }
            //    }
            //    else
            //    {
            //        weight = theSetting3.param.ToInt32();
            //    }
            //    weight3.Add(weight);

            //}
            //for (int i = 0; i < 7; i++)
            //{
            //    int index = CommonUtil.GetIndexByWeight(weight3);
            //    candidateList.Add(type3CandidateList[index]);
            //    type3CandidateList.RemoveAt(index);
            //    weight3.RemoveAt(index);
            //}

        }
        else if (shopType == ShopType.Match)
        {
            for (int i = 0; i < shopSettingList.Count; i++)
            {
                ShopSetting setting = shopSettingList[i];
                candidateList.Add(setting);
            }
        }
        //每日广告福利
        else if (shopType == ShopType.DailyGuangGaoFuLi)
        {
            for (int i = 0; i < shopSettingList.Count; i++)
            {
                ShopSetting setting = shopSettingList[i];
                candidateList.Add(setting);
            }
        }
        //活跃币
        else if (shopType == ShopType.HuoYueBi)
        {
            for (int i = 0; i < shopSettingList.Count; i++)
            {
                ShopSetting setting = shopSettingList[i];
                candidateList.Add(setting);
            }
        }
        //充值
        else if (shopType == ShopType.ChongZhi)
        {
            for (int i = 0; i < shopSettingList.Count; i++)
            {
                ShopSetting setting = shopSettingList[i];
                candidateList.Add(setting);
            }
        } //每日礼包
        else if (shopType == ShopType.DailyLiBao)
        {
            for (int i = 0; i < shopSettingList.Count; i++)
            {
                ShopSetting setting = shopSettingList[i];
                candidateList.Add(setting);
            }
        }

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allShopData.ShopList.Count; i++)
        {
            SingleShopData singleShopData = RoleManager.Instance._CurGameInfo.allShopData.ShopList[i];
            if (singleShopData.ShopType == (int)shopType)
            {
                singleShopData.ShopItemList.Clear();
                for (int j = 0; j < candidateList.Count; j++)
                {
                    ShopSetting setting = candidateList[j];
                    ShopItemData shopItemData = new ShopItemData();
                    shopItemData.Id = setting.Id.ToInt32();
                    shopItemData.RemainCount = setting.MaxCount.ToInt32();
                    List<int> theItem = CommonUtil.SplitCfgOneDepth(setting.Item);
                    shopItemData.ItemId = theItem[0];
                    singleShopData.ShopItemList.Add(shopItemData);
                }
                singleShopData.TodayBrushNum++;
                break;
            }
        }

        EventCenter.Broadcast(TheEventType.BrushShop);
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
        return candidateList;
    }

    #region 广告令商店
    /// <summary>
    /// 广告令商店按刷新类型找到候选
    /// </summary>
    /// <param name="candidateList"></param>
    public List<ShopSetting> GetGuangGaoTypeList(List<ShopSetting> candidateList, GuangGaoShopBrushRuleType theType)
    {
        List<ShopSetting> res = new List<ShopSetting>();
        for (int i = 0; i < candidateList.Count; i++)
        {
            ShopSetting setting = candidateList[i];
            if (setting.Param2.ToInt32() == (int)theType)
            {
                res.Add(setting);
            }
        }
        return res;
    }
    #endregion

    /// <summary>
    /// 通过商店类型找到商店
    /// </summary>
    /// <param name="shopType"></param>
    /// <returns></returns>
    public SingleShopData FindSingleShopDataByType(ShopType shopType)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allShopData.ShopList.Count; i++)
        {
            SingleShopData data = RoleManager.Instance._CurGameInfo.allShopData.ShopList[i];
            if (data.ShopType == (int)shopType)
                return data;
        }
        return null;
    }

    public void ShopingChoose(List<Button> btns,Button btn,Sprite choose,Sprite unchoose) {
        for (int i = 0; i < btns.Count; i++)
        {
            if (btns[i] == btn)
            {
                btns[i].transform.parent.GetComponent<Image>().sprite = choose;
            }
            else {
                btns[i].transform.parent.GetComponent<Image>().sprite = unchoose;
            }
        }
    }

    /// <summary>
    /// 购买物品
    /// </summary>
    /// <param name="shopItemData"></param>
    /// <param name="num"></param>
    public void OnBuyItem(ShopItemData shopItemData, int num)
    {

        ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);

        if (!string.IsNullOrWhiteSpace(setting.RecordTime))
        {
            GameTimeManager.Instance.GetServiceTime((x) =>
            {
                if (x > 0)
                {
                    OnReallyBuy(shopItemData, num, x);
                }
            });
        }
        else
        {
            OnReallyBuy(shopItemData, num, 0);

        }
    }

    /// <summary>
    /// 真正购买
    /// </summary>
    public void OnReallyBuy(ShopItemData shopItemData, int num, long time)
    {

        ShopSetting shopSetting = DataTable.FindShopSetting(shopItemData.Id);
        List<int> theItem = CommonUtil.SplitCfgOneDepth(shopSetting.Item);
        int getItemNum = theItem[1] * num;

        List<int> consume = CommonUtil.SplitCfgOneDepth(shopSetting.Price);
        if (shopSetting.Rmb == "1")
        {
            if (shopSetting.Price.ToInt32() == 0)
            {
                OnRMBBuy(shopItemData, time);

            }
            else
            {
                //有没有返利券
                ItemData fanLiQuan = ItemManager.Instance.FindFanLiQuan(shopItemData.Id);
                if (fanLiQuan != null)
                {

                    ItemManager.Instance.LoseItem(fanLiQuan.settingId, 1);
                    OnRMBBuy(shopItemData, time);

                }
                else
                {
                    //OnRMBBuy(shopItemData, time);
// #if UNITY_EDITOR

 
//                     OnRMBBuy(shopItemData, time);
// #else

                    if (Game.Instance.banHaoMode)
                    {
                        HandleRechargeAsync(shopItemData, time);
                    }
                    else
                    {
                        if (!Game.Instance.isLogin)
                        {
                            PanelManager.Instance.OpenFloatWindow("非登录状态，请等待重连或重启游戏");
                            return;
                        }
                        if (Game.Instance.isRightEdition
                            && Game.Instance.useFuWuQi
                             && Game.Instance.onSuccessSDKLogin
                            && GameTimeManager.Instance.connectedToFuWuQiTime)
                        {
                            int moneyParam = consume[0] * 100;
                            AddQQManager.Instance.CallAndroidMethod("OnCharge", RoleManager.Instance._CurGameInfo.TheGuid,
                                RoleManager.Instance._CurGameInfo.NickName, moneyParam, shopItemData.Id.ToString(), shopSetting.ItemName, RoleManager.Instance._CurGameInfo.quIndex.ToString());
                        }

                    }

//#endif
                }

            }



        }
        else
        {
            if (consume.Count == 2)
            {
                int singleConsumeNum = consume[1];
                cfg.ItemSetting consumeSetting = DataTable.FindItemSetting(consume[0]);

                ulong totalConsume = (ulong)(num * singleConsumeNum);


                if (!ItemManager.Instance.CheckIfItemEnough(consumeSetting.Id.ToInt32(), totalConsume))
                {
                    PanelManager.Instance.OpenFloatWindow(consumeSetting.Name + "不够");
                    return;
                }
                if (num > shopItemData.RemainCount)
                {
                    return;
                }
                //购买成功
                shopItemData.RemainCount -= num;
                ItemManager.Instance.LoseItem(consumeSetting.Id.ToInt32(), totalConsume);
                if (shopSetting.Type.ToInt32() == (int)ShopType.DailyLiBao)
                {
                    List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
                    List<int> awardIdList = new List<int>();
                    List<ulong> awardNumList = new List<ulong>();
                    for (int i = 0; i < award.Count; i++)
                    {
                        List<int> single = award[i];
                        awardIdList.Add(single[0]);
                        awardNumList.Add((ulong)(single[1] * num));
                        //PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid);
                    }
                    //购买成功
                    //shopItemData.RemainCount -= num;
                    ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardNumList);


                }
                else
                {
                    ItemManager.Instance.GetItemWithTongZhiPanel(shopItemData.ItemId, (ulong)getItemNum);

                }
                EventCenter.Broadcast(TheEventType.OnBuyItem);
            }
            //广告购买
            else
            {
                curBuyShopItem = shopItemData;
                ADManager.Instance.WatchAD(ADType.DailyFuli);
            }
        }


    }


    /// <summary>
    /// 处理充值购买
    /// </summary>
    private   void HandleRechargeAsync(ShopItemData data, long time)
    {
        try
        {
            ShopSetting shopSetting = DataTable.FindShopSetting(data.Id);

            List<int> consume = CommonUtil.SplitCfgOneDepth(shopSetting.Price);
            // 获取当前登录账号
            string currentAccount = MyHttpServer.Instance.curAccount;
            if (string.IsNullOrEmpty(currentAccount))
            {
                PanelManager.Instance.OpenOnlyOkHint("请先登录账号！", null);
                return;
            }

            // 获取充值金额（使用商品价格作为充值金额）
            int rechargeAmount = consume[0];

            Debug.Log($"开始充值：账号={currentAccount}, 金额={rechargeAmount}");
            // 调用充值接口
            var rechargeResult = Game.Instance.StartCoroutine(MyHttpServer.Instance.Recharge(MyHttpServer.Instance.curSessionId, currentAccount, rechargeAmount,
            response =>
            {
                         Debug.Log($"充值成功，开始购买商品");
                    PanelManager.Instance.OpenOnlyOkHint($"充值成功！{ response.message}", () =>
                    {
                        // 充值成功后执行购买
                        ShopManager.Instance.OnRMBBuy(data, time);
                    });
            },
            errorMessage=>
            {
                                   // 充值失败
                    Debug.LogError($"充值失败: {errorMessage}");
                    PanelManager.Instance.OpenOnlyOkHint($"充值失败：{errorMessage}", null);

            }
            
            ));

            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"充值异常: {ex.Message}");
            PanelManager.Instance.OpenOnlyOkHint($"充值异常: {ex.Message}", null);
        }
    }





    public void OnRMBBuy(ShopItemData shopItemData, long time)
    {
        int num = 1;
        ShopSetting shopSetting = DataTable.FindShopSetting(shopItemData.Id);
        List<int> theItem = CommonUtil.SplitCfgOneDepth(shopSetting.Item);
        int getItemNum = theItem[1] * 1;
        List<int> consume = CommonUtil.SplitCfgOneDepth(shopSetting.Price);
        //充值
        if (shopSetting.Type.ToInt32() == (int)ShopType.ChongZhi)
        {
            //是否首充还在
            SingleShopData shop = FindSingleShopDataByType(ShopType.ChongZhi);
            int multiple = 1;
            if (!shop.ShouChonged.Contains(shopSetting.Id.ToInt32()))
            {
                shop.ShouChonged.Add(shopSetting.Id.ToInt32());
                multiple = 2;
            }
            getItemNum *= multiple;
            ItemManager.Instance.GetItemWithTongZhiPanel(shopItemData.ItemId, (ulong)getItemNum);
            EventCenter.Broadcast(TheEventType.OnBuyItem);
        }
        //月卡
        else if (shopSetting.Type.ToInt32() == (int)ShopType.MoonCard)
        {
            bool newAdd = false;
            //时间早到期了 或者从未买过
            if (shopItemData.moonCardReachTime <= time)
            {
                long reachTime = time + 60 * 60 * 24 * 30;
                shopItemData.moonCardReachTime = reachTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(reachTime);
                newAdd = true;
            }
            //时间没到期
            else
            {
                long reachTime = shopItemData.moonCardReachTime + 60 * 60 * 24 * 30;
                shopItemData.moonCardReachTime = reachTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(reachTime);
            }

            if (shopSetting.Id.ToInt32() == (int)ShopIDType.MoonCard1)
            {
                ItemManager.Instance.GetItemWithAwardPanel(new List<int> { (int)ItemIdType.LingJing, (int)ItemIdType.LingJing }, new List<ulong> { ConstantVal.moonCard1OnceSend, ConstantVal.moonCard1PerDaySend });
            }
            else if (shopSetting.Id.ToInt32() == (int)ShopIDType.MoonCard2)
            {
                ItemManager.Instance.GetItemWithAwardPanel(new List<int> { (int)ItemIdType.LingJing }, new List<ulong> { ConstantVal.moonCard2OnceSend });

                LiLianManager.Instance.OnMoonCardAddLiLianLimit();
                MiJingManager.Instance.OnMoonCardAddTaoFaLimit(newAdd);
            }
            //免广告令
            else if (shopSetting.Id.ToInt32() == (int)ShopIDType.MoonCard3)
            {
                PanelManager.Instance.OpenFloatWindow("已获得免广告权限");
            }

            EventCenter.Broadcast(TheEventType.OnBuyItem);
        }
        //日礼包
        else if (shopSetting.Type.ToInt32() == (int)ShopType.DailyLiBao)
        {
            List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
            List<int> awardIdList = new List<int>();
            List<ulong> awardNumList = new List<ulong>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> single = award[i];
                awardIdList.Add(single[0]);
                awardNumList.Add((ulong)single[1]);
                //PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid);
            }
            //购买成功
            shopItemData.RemainCount -= num;
            ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardNumList);
            EventCenter.Broadcast(TheEventType.OnBuyItem);

        }
        //新手礼包
        else if (shopSetting.Type.ToInt32() == (int)ShopType.XinShouLiBao)
        {
            List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
            List<int> awardIdList = new List<int>();
            List<ulong> awardNumList = new List<ulong>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> single = award[i];
                awardIdList.Add(single[0]);
                awardNumList.Add((ulong)single[1]);
                //PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid);
            }
            //购买成功
            shopItemData.RemainCount -= num;
            ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardNumList);
            EventCenter.Broadcast(TheEventType.OnBuyItem);

        }
        else
        {

        }

        ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
        RoleManager.Instance._CurGameInfo.allShopData.totalChargeNum += setting.Price.ToInt32();

        for (int i = 0; i < DataTable._leiChongList.Count; i++)
        {
            int thePrice = DataTable._leiChongList[i].Price.ToInt32();
            if (RoleManager.Instance._CurGameInfo.allShopData.totalChargeNum >= thePrice)
            {
                if (RoleManager.Instance._CurGameInfo.allShopData.accomplishStatusList[i] == (int)AccomplishStatus.UnAccomplished)
                {
                    RoleManager.Instance._CurGameInfo.allShopData.accomplishStatusList[i] = (int)AccomplishStatus.Accomplished;
                }
            }
        }
    }
    /// <summary>
    /// 有没有月卡2
    /// </summary>
    public ShopItemData CheckIfHaveMoonCard2(long curTime)
    {
        SingleShopData shop = FindSingleShopDataByType(ShopType.MoonCard);
        for (int i = 0; i < shop.ShopItemList.Count; i++)
        {
            ShopItemData data = shop.ShopItemList[i];
            if (data.Id == (int)ShopIDType.MoonCard2)
            {
                if (data.moonCardReachTime > curTime)
                    return data;
            }
        }
        return null;
    }

    /// <summary>
    /// 免广告令
    /// </summary>
    /// <param name="curTime"></param>
    /// <returns></returns>
    public ShopItemData CheckIfHaveMoonCard3(long curTime)
    {
        SingleShopData shop = FindSingleShopDataByType(ShopType.MoonCard);
        for (int i = 0; i < shop.ShopItemList.Count; i++)
        {
            ShopItemData data = shop.ShopItemList[i];
            if (data.Id == (int)ShopIDType.MoonCard3)
            {
                if (data.moonCardReachTime > 0
                    && data.moonCardReachTime > curTime)
                    return data;
            }
        }
        return null;
    }

    public void OnDailyADBuy()
    {
        ShopSetting shopSetting = DataTable.FindShopSetting(curBuyShopItem.Id);

        List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
        List<int> idList = new List<int>();
        List<ulong> numList = new List<ulong>();
        for (int i = 0; i < award.Count; i++)
        {
            List<int> single = award[i];
            idList.Add(single[0]);
            numList.Add((ulong)single[1]);
        }
        //List<int> theItem = CommonUtil.SplitCfgOneDepth(shopSetting.item);
        //int getItemNum = theItem[1];

        curBuyShopItem.RemainCount -= 1;
        // ItemManager.Instance.GetItemWithTongZhiPanel(curBuyShopItem.ItemId, (ulong)getItemNum);
        ItemManager.Instance.GetItemWithAwardPanel(idList, numList);

        EventCenter.Broadcast(TheEventType.OnBuyItem);
    }

    /// <summary>
    /// 天晶买体力
    /// </summary>
    public void OnTianJingBuyTiLi()
    {
        if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum >= ConstantVal.dayliWatchTiliLimit)
        {
            PanelManager.Instance.OpenFloatWindow("今天购买体力次数已达上限，请明天再来哦");
            return;
        }
        int num = ConstantVal.ReviveTiLiNeedTianJing;

        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingJing, (ulong)num))
        {
            ItemSetting itemSetting=DataTable.table.TbItem.Get(((int)ItemIdType.LingJing).ToString());
            PanelManager.Instance.OpenCommonHint(itemSetting.Name+"不够，是否前往获取？", () =>
             {
                 PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2, ShopTag.ChongZhi);
             }, null);
        }
        else
        {
            ItemManager.Instance.LoseItem((int)ItemIdType.LingJing, (ulong)num);
            RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum++;
            RecordManager.Instance.AddTongZhi("恭喜获得", ConsumeType.Property, (int)PropertyIdType.Tili, ConstantVal.tianJingReviveTiliNum);
            RoleManager.Instance.AddProperty(PropertyIdType.Tili, ConstantVal.tianJingReviveTiliNum);
            //ItemManager.Instance.GetItem
        }

    }

    public ShopItemData FindShopItemDataByShopId(int id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allShopData.ShopList.Count; i++)
        {
            SingleShopData singleShopData = RoleManager.Instance._CurGameInfo.allShopData.ShopList[i];
            for (int j = 0; j < singleShopData.ShopItemList.Count; j++)
            {
                ShopItemData data = singleShopData.ShopItemList[j];
                if (data.Id == id)
                    return data;
            }
        }
        return null;
    }
    /// <summary>
    /// 领累充奖励
    /// </summary>
    public void GetLeiChongAward(int index)
    {
        LeiChongSetting setting = DataTable._leiChongList[index];
        int thePrice = setting.Price.ToInt32();
        if (RoleManager.Instance._CurGameInfo.allShopData.totalChargeNum >= thePrice)
        {
            if (RoleManager.Instance._CurGameInfo.allShopData.accomplishStatusList[index] == (int)AccomplishStatus.Accomplished)

            {
                List<List<int>> award = CommonUtil.SplitCfg(setting.Award);
                List<ItemData> awardList = new List<ItemData>();
                for (int i = 0; i < award.Count; i++)
                {
                    List<int> singleAward = award[i];
                    ItemData singleData = new ItemData();
                    singleData.settingId = singleAward[0];
                    singleData.count = (ulong)singleAward[1];
                    awardList.Add(singleData);
                }
                ItemManager.Instance.GetItemWithAwardPanel(awardList);
                RoleManager.Instance._CurGameInfo.allShopData.accomplishStatusList[index] = (int)AccomplishStatus.GetAward;
                EventCenter.Broadcast(TheEventType.GetLeiChongAward);
            }
        }
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif

    }

    /// <summary>
    /// 首充奖励
    /// </summary>
    public void GetShouChongAward()
    {
        if (!gameInfo.allShopData.shouChongAwardGet
            && gameInfo.allShopData.totalChargeNum > 0)
        {
            List<ItemData> awardList = new List<ItemData>();
            List<List<int>> award = CommonUtil.SplitCfg(ConstantVal.shouChongAward);
            for (int i = 0; i < award.Count; i++)
            {
                List<int> singleAward = award[i];
                ItemData data = new ItemData();
                data.settingId = singleAward[0];
                data.count = (ulong)singleAward[1];
                awardList.Add(data);
            }
            ItemManager.Instance.GetItemWithAwardPanel(awardList);
            gameInfo.allShopData.shouChongAwardGet = true;

            EventCenter.Broadcast(TheEventType.GetShouChongAward);
        }
        else
        {
            PanelManager.Instance.OpenFloatWindow("未达到领取条件");
        }

#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
    }

}

public enum ShopType
{
    Match = 1,//大比
    GuangGaoLing = 2,//广告令
    DailyGuangGaoFuLi = 3,//每日广告福利
    HuoYueBi = 4,//活跃币坊市
    ChongZhi = 5,//充值
    MoonCard = 6,//月卡
    DailyLiBao = 7,//日礼包
    XinShouLiBao = 8,//新手礼包

}

public enum ShopIDType
{
    None = 0,
    MoonCard1 = 60001,//月卡1
    MoonCard2 = 60002,//月卡2
    MoonCard3 = 60003,//免广告令
}
/// <summary>
/// 大标签
/// </summary>
public enum ShopTag
{
    None = 0,
    FangShi = 1,
    ChongZhi = 2,
    MoonCard = 3,//月卡
    LiBao = 4,//礼包
}

public enum GuangGaoShopBrushRuleType
{
    None = 0,
    Type1 = 1,//固定刷新5个
    Type2 = 2,//每次出现3个
    Type3 = 3,//每次出现7个
}

/// <summary>
/// 签到类型
/// </summary>
public enum QianDaoType
{
    None = 0,
    SevenDay = 1,//7日签到
    ThirtyDay = 2,//30日签到
}