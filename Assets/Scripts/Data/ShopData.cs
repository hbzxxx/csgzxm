using System;
using System.Collections.Generic;

/// <summary>
/// 所有商店数据
/// </summary>
[Serializable]
public class AllShopData
{
    /// <summary>商店列表</summary>
    public List<SingleShopData> ShopList = new List<SingleShopData>();
    
    /// <summary>人民币购买记录列表</summary>
    public List<int> rmbBuyRecordList = new List<int>();
    
    /// <summary>总充值金额</summary>
    public int totalChargeNum;
    
    /// <summary>完成状态列表</summary>
    public List<int> accomplishStatusList = new List<int>();
    
    /// <summary>首充奖励是否已领取</summary>
    public bool shouChongAwardGet;
}  /// 

/// <summary>
/// 单个商店数据
/// </summary>
[Serializable]
public class SingleShopData
{
    /// <summary>上次刷新时间</summary>
    public long LastBrushTime;
    
    /// <summary>商店类型</summary>
    public int ShopType;
    
    /// <summary>商品列表</summary>
    public List<ShopItemData> ShopItemList = new List<ShopItemData>();
    
    /// <summary>今日刷新次数</summary>
    public int TodayBrushNum;
    
    /// <summary>上次购买时间</summary>
    public long LastBuyTime;
    
    /// <summary>已首充的商品ID列表</summary>
    public List<int> ShouChonged = new List<int>();
}

/// <summary>
/// 商品数据
/// </summary>
[Serializable]
public class ShopItemData
{
    /// <summary>ID</summary>
    public int Id;
    
    /// <summary>剩余数量</summary>
    public int RemainCount;
    
    /// <summary>物品ID</summary>
    public int ItemId;
    
    /// <summary>月卡到期时间</summary>
    public long moonCardReachTime;
}

/// <summary>
/// 签到数据
/// </summary>
[Serializable]
public class QianDaoData
{
    /// <summary>七日签到索引</summary>
    public int SevenDayQianDaoIndex;
    
    /// <summary>可七日签到索引</summary>
    public int CanSevenDayQianDaoIndex;
    
    /// <summary>三十日签到索引</summary>
    public int ThirtyDayQianDaoIndex;
    
    /// <summary>可三十日签到索引</summary>
    public int CanThirtyDayQianDaoIndex;
    
    /// <summary>上次签到时间</summary>
    public long LastQianDaoTime;
}

/// <summary>
/// /// 所有邮件数据
/// </summary>
[Serializable]
public class AllMailData
{
    /// <summary>邮件列表</summary>
    public List<SingleMailData> mailList = new List<SingleMailData>();
}

/// <summary>
/// 单个邮件数据
/// </summary>
[Serializable]
public class SingleMailData
{
    /// <summary>邮件ID</summary>
    public string mailId = "";
    
    /// <summary>标题</summary>
    public string title = "";
    
    /// <summary>内容</summary>
    public string content = "";
    
    /// <summary>发送时间</summary>
    public long sendTime;
    
    /// <summary>是否已读</summary>
    public bool isRead;
    
    /// <summary>是否已领取</summary>
    public bool isReceived;
    
    /// <summary>附件物品列表</summary>
    public List<ItemData> attachmentList = new List<ItemData>();
}
