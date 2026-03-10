using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 
public class MiJingExploreResultPanel : GetAwardPanel
{
    public Button btn_adGet;//广告
    public Button btn_get;//获取
    SingleExploreData exploreData;

    public override void Init(params object[] args)
    {
        base.Init(args);
        exploreData = args[1] as SingleExploreData;
        addBtnListener(btn_get, () =>
        {
            Leave(false);


        });

        addBtnListener(btn_adGet, () =>
         {
             ADManager.Instance.WatchAD(ADType.MiJingDouble);
         });
        RegisterEvent(TheEventType.OnADMiJingDouble, ReceiveADGet);
    }


    public void Leave(bool ad)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemData item = itemList[i];
            if(ad)
            item.count *= 2;
            if (item.count > 0)
                ItemManager.Instance.GetItem(item.settingId, item.count);

        }
        MapManager.Instance.OnLeaveExplore(exploreData, exploreData.ExploreTeamData);

        for(int i = 0; i < itemList.Count; i++)
        {
            PanelManager.Instance.AddTongZhi(TongZhiType.Consume, "", ConsumeType.Item, itemList[i].settingId, (int)(int)(ulong)itemList[i].count);

        }
    }
    /// <summary>
    /// 广告双倍领取
    /// </summary>
    public void ReceiveADGet()
    {
        Leave(true);

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
