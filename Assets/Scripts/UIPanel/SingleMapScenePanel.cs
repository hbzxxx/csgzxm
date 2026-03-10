using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;

public class SingleMapScenePanel : PanelBase
{
    public int lieNum;//列数
    public Transform trans_mapEventParent;//地图事件父物体
    public Transform trans_eventStartPos;//事件开始坐标位
    public List<SingleLevelView> levelViewList;
    public List<MapEventView> mapEventViewList;
    public ScrollViewNevigation nevigation;
    public MapSceneType mapSceneType;
    public int mapId;
    public override void Init(params object[] args)
    {
        base.Init(args);
        mapSceneType = (MapSceneType)args[0];
        mapId =(int) args[1];
        
        SingleMapData singleMapData = MapManager.Instance.FindMapById(mapId);

        for(int i = 0; i < levelViewList.Count; i++)
        {
            if(mapSceneType==MapSceneType.XianMen)
                levelViewList[i].settingId = singleMapData.LevelList[i].LevelId;
            else
                levelViewList[i].settingId = singleMapData.FixedLevelList[i].LevelId;

            levelViewList[i].Init(mapSceneType);
            levelViewList[i].InitShow();
        }
        GenerateFootstepsBetweenUnlockedLevels();

        //进入战斗
        RegisterEvent(TheEventType.RefreshLevelShow, () =>
        {
            for (int i = 0; i < levelViewList.Count; i++)
            {
                if (mapSceneType == MapSceneType.XianMen)
                    levelViewList[i].singleLevelData = MapManager.Instance.FindLevelById(levelViewList[i].singleLevelData.LevelId);
                else
                    levelViewList[i].singleLevelData = MapManager.Instance.FindFixedLevelById(levelViewList[i].singleLevelData.LevelId);

                levelViewList[i].InitShow();
            }
            GenerateFootstepsBetweenUnlockedLevels();

        });
        RefreshMapEvent();
    
        RegisterEvent(TheEventType.AddMapEvent, GenerateMapEvent);

        RegisterEvent(TheEventType.NevigateToLevel, NevigateToLevel);

        RegisterEvent(TheEventType.ShowAccomplishLevelAnim, AccomplishLevelAnimShow);
        ShowGuide();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_passFixLevel&&mapId==TaskManager.Instance.guide_passFixLevelMapId)
        {
            TaskManager.Instance.guide_passFixLevel = false;
        }
    }

    /// <summary>
    /// 完成关卡拔剑动画
    /// </summary>
    void AccomplishLevelAnimShow(object[] args)
    {
        int theMapId = (int)args[0];
        string theLevelId = (string)args[1];
        MapSceneType theMapSceneType = (MapSceneType)args[2];
        if (theMapId == mapId && mapSceneType == theMapSceneType)
        {
            for (int i = 0; i < levelViewList.Count; i++)
            {
                SingleLevelView view = levelViewList[i];
                if (view.settingId == theLevelId)
                {
                    view.ShowAccomplishLevelAnim();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 导航到某一关
    /// </summary>
    void NevigateToLevel(object[] args)
    {
        int theMapId = (int)args[0];
        string theLevelId = (string)args[1];
        MapSceneType theMapSceneType= (MapSceneType)args[2];
        if (theMapId == mapId&&mapSceneType== theMapSceneType)
        {
            for (int i = 0; i < levelViewList.Count; i++)
            {
                SingleLevelView view = levelViewList[i];
                if (view.settingId == theLevelId)
                {
                    if (i != 0)
                    {
                        RefreshFootsteps(view, levelViewList[i - 1]);
                    }
                    nevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 核心逻辑：筛选已解锁/已通关关卡，并在相邻关卡间生成脚步
    /// </summary>
    private void GenerateFootstepsBetweenUnlockedLevels()
    {
        // 筛选已解锁关卡
        List<SingleLevelView> validLevelViews = new List<SingleLevelView>();
        foreach (var levelView in levelViewList)
        {
            bool isUnlocked = false;
            LevelSetting setting = DataTable.FindLevelSetting(levelView.settingId);
            if (setting.IsFixed == "1")
                isUnlocked = MapManager.Instance.CheckIfUnlockFixedLevel(levelView.settingId);
            else
                isUnlocked = MapManager.Instance.CheckIfUnlockLevel(levelView.settingId);

            if (isUnlocked)
                validLevelViews.Add(levelView);
        }
        Debug.Log($"已解锁关卡={validLevelViews.Count}");

        // 相邻已解锁关卡间生成脚步
        for (int i = 1; i < validLevelViews.Count; i++)
        {
            SingleLevelView prevLevel = validLevelViews[i - 1];
            SingleLevelView currLevel = validLevelViews[i];
            currLevel.CompletedLevelStepDisplay(prevLevel);
        }
    }

    /// <summary>
    /// 原有刷新脚步方法（补充实现）
    /// </summary>
    private void RefreshFootsteps(SingleLevelView current, SingleLevelView previous)
    {
        // 导航到指定关卡时，单独刷新该关卡与上一关的脚步
        current.ClearFootstepPrefabs();
        if (previous != null)
        {
            current.CompletedLevelStepDisplay(previous);
        }
    }
    /// <summary>
    /// 刷新地图事件
    /// </summary>
    void RefreshMapEvent()
    {
        mapEventViewList.Clear();
        ClearCertainParentAllSingle<MapEventView>(trans_mapEventParent);
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];

            if (mapSceneType == MapSceneType.Fixed
                && data.FixedMapId == mapId)
            {
                Vector2Int logicPos = CommonUtil.GetLogicPosByLogicIndex(data.PosIndex, lieNum);
                data.Pos.Clear();
                if (data.Pos.Count == 0)
                {
                    data.Pos.Add((int)trans_eventStartPos.localPosition.x + logicPos[0] * ConstantVal.eventOffset);
                    data.Pos.Add((int)trans_eventStartPos.localPosition.y + logicPos[1] * ConstantVal.eventOffset);
                }

                mapEventViewList.Add(AddSingle<MapEventView>(trans_mapEventParent, data));
            }
        }

     
    }
    /// <summary>
    /// 生成地图事件
    /// </summary>
    /// <param name="args"></param>
    void GenerateMapEvent(object[] args)
    {
        SingleMapEventData data = args[0] as SingleMapEventData;
        MapEventSetting setting = DataTable.FindMapEventSetting(data.SettingId);
        if (data.MapSceneType == (int)SceneType.FixedMap
            &&data.FixedMapId==mapId)
        {
            bool haveSame = false;
            for (int i = 0; i < mapEventViewList.Count; i++)
            {
                MapEventView existView = mapEventViewList[i];
                if (existView.singleMapEventData.OnlyId == data.OnlyId)
                {
                    haveSame = true;
                    break;
                }
            }
            if (!haveSame)
            {
                int index = data.PosIndex;
                Vector2Int logicPos = CommonUtil.GetLogicPosByLogicIndex(data.PosIndex, lieNum);
                if (data.Pos.Count == 0)
                {
                    data.Pos.Add((int)trans_eventStartPos.localPosition.x + logicPos[0] * ConstantVal.eventOffset);
                    data.Pos.Add((int)trans_eventStartPos.localPosition.y + logicPos[1] * ConstantVal.eventOffset);
                }
                MapEventView view = PanelManager.Instance.OpenSingle<MapEventView>(trans_mapEventParent, data);
                mapEventViewList.Add(view);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();

    }
}
/// <summary>
/// 地图场景类型
/// </summary>
public enum MapSceneType
{
    None=0,
    XianMen=1,//仙门
    Fixed=2,//固定关卡
}