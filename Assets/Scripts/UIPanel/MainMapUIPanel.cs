using Framework.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMapUIPanel : PanelBase
{
    public Transform trans_award;//奖励

    public Button btn_leave;//离开

    public Button btn_saoDang;//扫荡

    public override void Init(params object[] args)
    {
        base.Init(args);
        RegisterEvent(TheEventType.LevelResult, RefreshShow);
        RegisterEvent(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.OnUnEquip, OnEquip);
        RegisterEvent(TheEventType.OnSaoDang, OnSaoDang);


        addBtnListener(btn_leave, () =>
        {
            
                SingleMapData singleMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                List<ItemData> itemList = new List<ItemData>();
                for (int i = 0; i < singleMapData.CurAwardList.Count; i++)
                {
                    itemList.Add(singleMapData.CurAwardList[i]);
                 }
                Action callBack = MapManager.Instance.LeaveMap;
                PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2, itemList, callBack);

 
       
        });

        //扫荡裂隙
        addBtnListener(btn_saoDang, () =>
         {
             SingleLevelData level = MapManager.Instance.FindCanSaoDangLieXiLevel();
             if (level != null)
             {
                 

                 BattleManager.Instance.SaoDang(BattleType.LevelBattle, level.LevelId.ToString());

             }
             else
             {
                 PanelManager.Instance.OpenFloatWindow("只有通关后的关卡可以扫荡");
             }
         });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        RefreshShow();


    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="param"></param>
    void OnEquip(object[] param)
    {
        RefreshShow();
    }

    void RefreshShow()
    {
        ShowRole();
        ShowStudent();
        ShowAward();
        ShowSaoDang();

    }

    /// <summary>
    /// 扫荡
    /// </summary>
    /// <param name="args"></param>
    void OnSaoDang(object[] args)
    {
        BattleType battleType = (BattleType)args[0];
        if (battleType == BattleType.LevelBattle)
        {
            RefreshShow();
        }
    }

    /// <summary>
    /// 显示扫荡
    /// </summary>
    void ShowSaoDang()
    {
        if (MapManager.Instance.FindCanSaoDangLieXiLevel() != null)
        {
            btn_saoDang.gameObject.SetActive(true);
        }
        else
        {
            btn_saoDang.gameObject.SetActive(false);
        }
    }
    void ShowRole()
    {
     
    }

    void ShowStudent()
    {    //显示弟子


    }

    

    /// <summary>
    /// 显示所有奖励
    /// </summary>
    void ShowAward()
    {
        PanelManager.Instance.CloseAllSingle(trans_award);

        List<ItemData> awardList = new List<ItemData>();
        SingleMapData mapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);

        for(int i = 0; i < mapData.CurAwardList.Count; i++)
        {
            awardList.Add(mapData.CurAwardList[i]);
        }

        awardList= ItemManager.Instance.CombineItemList(awardList);

        for(int i = 0; i < awardList.Count; i++)
        {
            PanelManager.Instance.OpenSingle<WithCountItemView>(trans_award, awardList[i]);
        }
    }

    public override void Clear()
    {
        base.Clear();

    }
}
