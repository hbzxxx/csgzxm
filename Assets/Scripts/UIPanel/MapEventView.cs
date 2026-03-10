using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class MapEventView : SingleViewBase
{
    public Text txt_name;
    public Text txt_difficulty;
    public Button btn;

    public Transform trans_enemy;//敌人
    public Transform trans_lingQi;//灵气

    public Vector3 localPos;//本地坐标

    public SingleMapEventData singleMapEventData;
    public MapEventSetting mapEventSetting;//地图事件


    public override void Init(params object[] args)
    {
        base.Init(args);
        
        singleMapEventData = args[0] as SingleMapEventData;
        mapEventSetting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);

        addBtnListener(btn, () =>
        {
           
                MapEventManager.Instance.HandleMapEvent(singleMapEventData);

            if (TaskManager.Instance.guide_mapEvent
           && TaskManager.Instance.guide_mapEventOnlyId == singleMapEventData.OnlyId)
            {
                PanelManager.Instance.CloseTaskGuidePanel();
                TaskManager.Instance.guide_mapEvent = false;
            }
        });

       RegisterEvent(TheEventType.RemoveMapEvent, OnRemoveMapEvent);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        if (mapEventSetting.Type.ToInt32() == (int)MapEventType.Enemy)
        {
            trans_enemy.gameObject.SetActive(true);
            trans_lingQi.gameObject.SetActive(false);
            txt_difficulty.ShowDangerouseTxt(mapEventSetting.DangerousLevel.ToInt32());


        }
        else if(mapEventSetting.Type.ToInt32() == (int)MapEventType.XiuLian)
        {
            trans_lingQi.gameObject.SetActive(true);
            trans_enemy.gameObject.SetActive(false);
            txt_difficulty.SetText(MapEventManager.Instance.LingQiTuanXiuWeiAdd(singleMapEventData).ToString());
        }

        Vector2 localPos = Vector2.zero;
        if (singleMapEventData.Pos.Count == 2)
        {
            transform.localPosition =new Vector2(singleMapEventData.Pos[0],singleMapEventData.Pos[1]);
        }
        txt_name.SetText(mapEventSetting.Name);

 
    }

    /// <summary>
    /// 移除地图事件
    /// </summary>
    /// <param name="args"></param>
    void OnRemoveMapEvent(object[] args)
    {
        SingleMapEventData data = args[0] as SingleMapEventData;
        if (data.OnlyId == singleMapEventData.OnlyId)
        {
            PanelManager.Instance.CloseSingle(this);
        }
    }
}

/// <summary>
/// 地图上事件类型
/// </summary>
public enum MapEventType
{
    None=0,
    Enemy=1,//敌人
    XiuLian=2,//修炼
    Kuang=3,//矿
    Rest=4,//休息点
    CheDiSouSuo=5,//选择彻底搜索可能战斗
    Explore_XiuWei=6,//灵气加修为液体
    RecruitStudent=7,//弟子
    Reveal=8,//揭示探索点
    ShenSuanZi=1000,//神算子
    
}

/// <summary>
/// 事件id类型
/// </summary>
public enum MapEventIdType
{
    None=0,
    LingQi=10001,//灵气
    ShenSuanZi=10007,//神算子
}
