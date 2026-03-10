using Framework.Data;
using System.Collections.Generic;
using UnityEngine;
using cfg;

/// <summary>
/// 地图事件
/// </summary>
public class MapEventManager : CommonInstance<MapEventManager>
{
    public Dictionary<SceneType, List<int>> occupiedPosDic = new Dictionary<SceneType, List<int>>();//被占据的

    public Dictionary<int, List<int>> occupiedFixedMapPosDic = new Dictionary<int, List<int>>();//固定地图被占据的 地图id 占据的位置

    public override void Init()
    {
        base.Init();
        RefreshOccupiedPosDic();
    }

    /// <summary>
    /// 刷新被占据的地块
    /// </summary>
    public void RefreshOccupiedPosDic()
    {
        occupiedPosDic.Clear();
        occupiedFixedMapPosDic.Clear();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (data.MapSceneType != (int)SceneType.FixedMap)
            {
                if (!occupiedPosDic.ContainsKey((SceneType)data.MapSceneType))
                {
                    occupiedPosDic.Add((SceneType)data.MapSceneType, new List<int>());
                }
                occupiedPosDic[(SceneType)data.MapSceneType].Add(data.PosIndex);
            }
            else
            {
                if (!occupiedFixedMapPosDic.ContainsKey(data.FixedMapId))
                {
                    occupiedFixedMapPosDic.Add(data.FixedMapId, new List<int>());
                }
                occupiedFixedMapPosDic[data.FixedMapId].Add(data.PosIndex);
            }

        }
    }

    /// <summary>
    /// 在某个index加地图事件
    /// </summary>
    public SingleMapEventData AddMapEventAtIndex(int id, SceneType sceneType, int index)
    {
        if (sceneType != SceneType.FixedMap
            && sceneType != SceneType.MiJingExplore
            && occupiedPosDic.ContainsKey(sceneType)
            && occupiedPosDic[sceneType].Contains(index))
        {
            Debug.Log("被占据");
            return null;
        }
        else
        {
            MapEventSetting mapEventSetting = DataTable.FindMapEventSetting(id);
            SingleMapEventData singleMapEventData = new SingleMapEventData();
            singleMapEventData.SettingId = id;
            singleMapEventData.PosType = (int)MapEventPosType.Index;
            singleMapEventData.PosIndex = index;
            singleMapEventData.MapSceneType = (int)sceneType;

            //singleMapEventData.Pos.Add((int)localPos.x);
            //singleMapEventData.Pos.Add((int)localPos.y);

            //时间随机的
            if (mapEventSetting.Sustain != "-1")
            {
                singleMapEventData.HaveLimitTime = true;
                singleMapEventData.RemainMonth = mapEventSetting.Sustain.ToInt32();
            }

            if (mapEventSetting.Type.ToInt32() == (int)MapEventType.Enemy)
            {
                List<int> enemySettingIdList = CommonUtil.SplitCfgOneDepth(mapEventSetting.EnemyGroup);
                List<int> enemyLevelList= CommonUtil.SplitCfgOneDepth(mapEventSetting.EnemyGroupLevel);
                //开打
                for (int i = 0; i < enemySettingIdList.Count; i++)
                {
                    EnemySetting enemySetting = DataTable.FindEnemySetting(enemySettingIdList[i]);
                    PeopleData p = BattleManager.Instance.GenerateEnemy(enemySetting,1, enemyLevelList[i]);
                    singleMapEventData.PeopleList.Add(p);
                }

            }
            if(mapEventSetting.Type.ToInt32() == (int)MapEventType.XiuLian)
            {
                List<int> numRange = CommonUtil.SplitCfgOneDepth(mapEventSetting.Param);
                 ulong num = (ulong)RandomManager.Next(numRange[0], numRange[1]);// param.ToUInt64();
                singleMapEventData.Param= (int)num;
            }
            if(mapEventSetting.Type.ToInt32() == (int)MapEventType.RecruitStudent)
            {
                singleMapEventData.PeopleList.Add(StudentManager.Instance.GenerateCandidateStudentByZongMenLevel(GenerateCandidateStudentType.AD, (Gender)mapEventSetting.Param.ToInt32()));
            }

            singleMapEventData.OnlyId = ConstantVal.SetId;

            // singleMapEventData.OnlyId = ConstantVal.SetId;
            if (sceneType == SceneType.MiJingExplore)
            {
                singleMapEventData.IsHide = true;
                //80概率问号
                int wenHaoRate = RandomManager.Next(0, 100);
                if (wenHaoRate < 80)
                {
                    singleMapEventData.WenHao = true;
                }
            }

            RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Add(singleMapEventData);
            EventCenter.Broadcast(TheEventType.AddMapEvent, singleMapEventData);
            RefreshOccupiedPosDic();
            return singleMapEventData;
        }

    }

    /// <summary>
    /// 在某个index加固定地图的地图事件
    /// </summary>
    public SingleMapEventData AddFixedMapEventAtIndex(int id, int mapId, int index)
    {
        if (occupiedFixedMapPosDic.ContainsKey(mapId)
            && occupiedFixedMapPosDic[mapId].Contains(index))
        {
            Debug.Log("被占据");
            return null;
        }
        else
        {
            MapEventSetting mapEventSetting = DataTable.FindMapEventSetting(id);
            SingleMapEventData singleMapEventData = new SingleMapEventData();
            singleMapEventData.SettingId = id;
            singleMapEventData.PosType = (int)MapEventPosType.Index;
            singleMapEventData.PosIndex = index;
            singleMapEventData.MapSceneType = (int)SceneType.FixedMap;
            singleMapEventData.FixedMapId = mapId;
            //singleMapEventData.Pos.Add((int)localPos.x);
            //singleMapEventData.Pos.Add((int)localPos.y);

            //时间随机的
            if (mapEventSetting.Sustain != "-1")
            {
                singleMapEventData.HaveLimitTime = true;
                singleMapEventData.RemainMonth = mapEventSetting.Sustain.ToInt32();
            }

            // singleMapEventData.OnlyId = ConstantVal.SetId;

            if (mapEventSetting.Type.ToInt32() == (int)MapEventType.Enemy)
            {
                List<int> enemySettingIdList = CommonUtil.SplitCfgOneDepth(mapEventSetting.EnemyGroup);
                List<int> enemyLevelList = CommonUtil.SplitCfgOneDepth(mapEventSetting.EnemyGroupLevel);

                //开打
                for (int i = 0; i < enemySettingIdList.Count; i++)
                {
                    EnemySetting enemySetting = DataTable.FindEnemySetting(enemySettingIdList[i]);
                    PeopleData p = BattleManager.Instance.GenerateEnemy(enemySetting,1, enemyLevelList[i]);
                    singleMapEventData.PeopleList.Add((p));
                }
            }
            if (mapEventSetting.Type.ToInt32() == (int)MapEventType.XiuLian)
            {
                List<int> numRange = CommonUtil.SplitCfgOneDepth(mapEventSetting.Param);
                ulong num = (ulong)RandomManager.Next(numRange[0], numRange[1]);// param.ToUInt64();
                singleMapEventData.Param = (int)num;
            }
            singleMapEventData.OnlyId = ConstantVal.SetId;


            RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Add(singleMapEventData);
            EventCenter.Broadcast(TheEventType.AddMapEvent, singleMapEventData);
            RefreshOccupiedPosDic();
            return singleMapEventData;
        }

    }

    /// <summary>
    /// 移除掉一个地图事件
    /// </summary>
    /// <param name="singleMapEventData"></param>
    public void RemoveMapEvent(SingleMapEventData singleMapEventData)
    {
        for (int i = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count - 1; i >= 0; i--)
        {
            SingleMapEventData theData = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (theData.OnlyId == singleMapEventData.OnlyId)
            {
                RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Remove(theData);
                EventCenter.Broadcast(TheEventType.RemoveMapEvent, theData);
                break;
            }
        }
        MapEventSetting setting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);
        if (setting.RdmByTime == "1")
        {
            //从该难度梯度里选一个
            if (setting.EventAppearType.ToInt32() == (int)MapEventAppearType.FixXiaoGuai)
            {
                setting = ChooseTheSameDifficultyAppearType1Event(setting);
            }
            else
            {

            }
            List<int> rdmMonth = CommonUtil.SplitCfgOneDepth(setting.RdmMonth);
            MapEventWaitToAppearData next = new MapEventWaitToAppearData();
            next.SettingId = setting.Id.ToInt32();
            next.RemainMonth = RandomManager.Next(rdmMonth[0], rdmMonth[1]);
            RoleManager.Instance._CurGameInfo.allMapEventData.WaitToAppearList.Add(next);
        }
        RefreshOccupiedPosDic();

    }

    /// <summary>
    /// 选择相同难度等级的事件
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public MapEventSetting ChooseTheSameDifficultyAppearType1Event(MapEventSetting setting)
    {
        List<MapEventSetting> candidate = new List<MapEventSetting>();
        for (int i = 0; i < DataTable._mapEventList.Count; i++)
        {
            MapEventSetting theSetting = DataTable._mapEventList[i];
            if (theSetting.EventAppearType.ToInt32() == (int)MapEventAppearType.FixXiaoGuai
                && theSetting.UnlockMapLevel == setting.UnlockMapLevel
                && theSetting.DangerousLevel == setting.DangerousLevel)
            {
                candidate.Add(theSetting);
            }
        }
        return candidate[RandomManager.Next(0, candidate.Count)];
    }
    /// <summary>
    /// 选择相同难度等级的事件
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public MapEventSetting ChooseTheSameDifficultyAppearType1Event(int mapLevel, int dangerousLevel)
    {
        List<MapEventSetting> candidate = new List<MapEventSetting>();
        for (int i = 0; i < DataTable._mapEventList.Count; i++)
        {
            MapEventSetting theSetting = DataTable._mapEventList[i];
            if (theSetting.EventAppearType.ToInt32() == (int)MapEventAppearType.FixXiaoGuai
                && theSetting.UnlockMapLevel.ToInt32() == mapLevel
                && theSetting.DangerousLevel.ToInt32() == dangerousLevel)
            {
                candidate.Add(theSetting);
            }
        }
        return candidate[RandomManager.Next(0, candidate.Count)];
    }
    /// <summary>
    /// 探险对执行地图事件
    /// </summary>
    /// <param name="singleMapEventData"></param>
    public void ExploreHandleMapEvent(SingleMapEventData singleMapEventData, ExploreTeamData exploreTeamData)
    {
        singleMapEventData.ExploreTeamOnlyId = exploreTeamData.OnlyId;

        PanelManager.Instance.OpenPanel<MapEventPanel>(PanelManager.Instance.trans_layer2, singleMapEventData);
    }
    /// <summary>
    /// 操作地图事件
    /// </summary>
    /// <param name="singleMapEventData"></param>
    public void HandleMapEvent(SingleMapEventData singleMapEventData)
    {
        PanelManager.Instance.PingPongBlackMask(() =>
        {
            PanelManager.Instance.OpenPanel<MapEventPanel>(PanelManager.Instance.trans_layer2, singleMapEventData);
        }, null);
    }
    //todo 修炼也改到这里来
    /// <summary>
    /// 执行地图事件
    /// </summary>
    /// <param name="singleMapEventData"></param>
    public void ExecuteMapEvent(SingleMapEventData singleMapEventData,bool chooseLeft=true,int phase=1)
    {
        MapEventSetting setting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);
        string param = setting.Param;
        int consume = setting.Consume.ToInt32();
        ExploreTeamData teamData = null;
        if (singleMapEventData.ExploreTeamOnlyId != 0)
        {
            teamData = MapManager.Instance.FindExploreTeamDataByOnlyId(singleMapEventData.ExploreTeamOnlyId);
            if (teamData.RemainBuJi < consume)
            {
                PanelManager.Instance.OpenFloatWindow("体力不够");
                return;
            }
            else
            {
                MapManager.Instance.DeExploreEnergy(teamData, consume);

                singleMapEventData.ExploreTeamOnlyId = 0;
                teamData.TargetEventOnlyId = 0;
            }
        }
        switch ((MapEventType)setting.Type.ToInt32())
        {
            //灵气修炼 TODO直接加队伍里面
            case MapEventType.XiuLian:
                ulong num =(ulong)LingQiTuanXiuWeiAdd(singleMapEventData) ;// param.ToUInt64();d
                //NeiCunModel.Instance._PlayerPeople.curXiuwei += num;
                //EventCenter.Broadcast(TheEventType.SuccessXiuLian);
                //每名弟子加修为
                List<int> beforeExpList1 = new List<int>();
                List<PeopleData> pList1 = RoleManager.Instance.FindMyBattleTeamList(false,0);
                int teamNum = pList1.Count;


                int xiuWeiNum1 = (int)num / teamNum;
                for (int i = 0; i < pList1.Count; i++)
                {
                    PeopleData beforeP = pList1[i];
                    beforeExpList1.Add((int)(ulong)beforeP.curXiuwei);
                    //List<int> xiuWei = CommonUtil.SplitCfgOneDepth(param);
                    StudentManager.Instance.OnGetStudentExp(beforeP, xiuWeiNum1);
                    //pList1.Add(beforeP);
                }
                PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExpList1, pList1);
                break;
            //矿
            case MapEventType.Kuang:
                List<int> kuangReward = CommonUtil.SplitCfgOneDepth(param);
                int kuangId = kuangReward[0];
                int kuangNum = RandomManager.Next(kuangReward[1], kuangReward[2]);

                List<ItemData> itemList = new List<ItemData>();
                ItemData data = new ItemData();
                data.settingId = kuangId;
                data.count = (ulong)kuangNum;
                itemList.Add(data);
                PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, itemList);
                GetExploreItem(teamData, data.settingId, data.count);
                //ItemManager.Instance.o(new List<int> { kuangId }, new List<ulong> { (ulong)kuangNum });
                EventCenter.Broadcast(TheEventType.SuccessXiuLian);

                break;
            //心智
            case MapEventType.Rest:
                List<int> buji = CommonUtil.SplitCfgOneDepth(param);
                int bujiNum = RandomManager.Next(buji[0], buji[1]);
                MapManager.Instance.AddExploreEnergy(teamData, bujiNum);

                break;
            //彻底搜索
            case MapEventType.CheDiSouSuo:
                List<List<List<int>>> award = CommonUtil.SplitThreeCfg(param);
                string finishStr = "";

                //彻底搜索 
                if (phase == 2)
                {

                    //打怪且 消耗1.5倍补给 根据战斗力发放奖励 若失败则消耗2.5倍补给
                    if (chooseLeft)
                    {
                       int zhanDouLiNeed= setting.Param2.Split('|')[1].ToInt32();
                        //战斗力不足一半 则直接失败
                       int myZhan= MapManager.Instance.CalcExploreTeamZhanDouLi(teamData);
                        //战斗力2倍领取2倍奖励

                        float rate = (myZhan - zhanDouLiNeed / 2) / (float)(zhanDouLiNeed / 2);
                        int theRate = (int)(rate * 100);
                        int index = RandomManager.Next(0, 100);
                        //成功
                        if (index < theRate)
                        {
                            List<ItemData> awardList = new List<ItemData>();
                            //额外获得
                            float ultraGet = (myZhan) / (float)zhanDouLiNeed;
                            if (ultraGet >= 2)
                            {
                                ultraGet = 2;
                                finishStr = "队伍大获全胜，并发现了额外的宝藏。";
                            }
                            else
                            {
                                finishStr = "经过一番鏖战，发现了额外的宝藏。";
                            }
                            List<List<int>> theAward = award[1];
                            for(int i = 0; i < theAward.Count; i++)
                            {
                                List<int> singleAward = theAward[i];
                                int id = singleAward[0];
                                int countMin = (int)(singleAward[1] * ultraGet);
                                int countMax = (int)(singleAward[2] * ultraGet);
                                int count = 0;
                                if (countMax > countMin)
                                {
                                    count = RandomManager.Next(countMin, countMax);
                                    if (count > 0)
                                    {
                                        ItemData item = new ItemData();
                                        item.settingId = id;
                                        item.count = (ulong)count;
                                        GetExploreItem(teamData, item.settingId, item.count);
                                        awardList.Add(item);
                                    }                                 
                                }
                            }
                            //失去体力
                            MapManager.Instance.DeExploreEnergy(teamData,(int)(consume * 1.5f-consume));
                            //拿奖
                            PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, awardList);
                            //显示finish 大获全胜
                        }
                        else
                        {
                            //失败 扣2.5补给
                            MapManager.Instance.DeExploreEnergy(teamData, (int)(consume * 2.5f-consume));

                            finishStr = "失败，不仅空手而归，反而消耗了额外的体力。";

                        }

                    }
                    //逃跑 消耗1.5倍补给
                    else
                    {
                        MapManager.Instance.DeExploreEnergy(teamData, (int)(consume * 1.5f - consume));

                        finishStr = "费劲九牛二虎之力才逃出生天，消耗了额外的体力。";

                    }
                }
               //简单搜索
                else if (phase == 1)
                {
                    finishStr = setting.FinishShow;
                    List<ItemData> awardList = new List<ItemData>();
           
                    List<List<int>> theAward = award[0];
                    for (int i = 0; i < theAward.Count; i++)
                    {
                        List<int> singleAward = theAward[i];
                        int id = singleAward[0];
                        int countMin = (int)(singleAward[1] );
                        int countMax = (int)(singleAward[2] );
                        int count = 0;
                        if (countMax > countMin)
                        {
                            count = RandomManager.Next(countMin, countMax);
                            if (count > 0)
                            {
                                ItemData item = new ItemData();
                                item.settingId = id;
                                item.count = (ulong)count;
                                GetExploreItem(teamData, item.settingId, item.count);
                                awardList.Add(item);
                            }
                        }
                    }
                    //拿奖面板
                    PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, awardList);
                }
                EventCenter.Broadcast(TheEventType.ShowMapEventFinish, finishStr);

                break;
            case MapEventType.Explore_XiuWei:
                //每名弟子加修为
                List<int> beforeExpList = new List<int>();
                List<PeopleData> pList = new List<PeopleData>();
                for(int i=0;i< teamData.StudentOnlyIdList.Count; i++)
                {
                    PeopleData beforeP = StudentManager.Instance.FindStudent(teamData.StudentOnlyIdList[i]);
                    beforeExpList.Add((int)(ulong)beforeP.curXiuwei);
                    List<int> xiuWei = CommonUtil.SplitCfgOneDepth(param);
                    int xiuWeiNum = RandomManager.Next(xiuWei[0], xiuWei[1]);
                    StudentManager.Instance.OnGetStudentExp(beforeP, xiuWeiNum);
                    pList.Add(beforeP);
                }
                PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExpList, pList);
                //PanelManager.Instance.OpenPanel<GetAwardWithStudentUpgradePanel>(PanelManager.Instance.trans_layer2,
                // new List<ItemNeiCunData>(), beforeExpList, pList);
                break;
                //潜在弟子
            case MapEventType.RecruitStudent:
                //招募弟子 在CandidateStudentView中已经执行完毕了

                break;
                //揭示探索点
            case MapEventType.Reveal:
                //SingleExploreData exploreData = MapManager.Instance.FindSingleExploreDataById(teamData.ExploreId);
                List<SingleMapEventData> mapEventList = MapManager.Instance.FindAllExploreMapEventById(teamData.ExploreId);
                for(int i=0;i< mapEventList.Count; i++)
                {
                    SingleMapEventData theData = mapEventList[i];
                    if(Mathf.Abs(theData.LogicPos[0]-teamData.LogicPos[0])<=setting.Param.ToInt32()
                        && Mathf.Abs(theData.LogicPos[1] - teamData.LogicPos[1]) <= setting.Param.ToInt32())
                    {
                        EventCenter.Broadcast(TheEventType.RevealWenHao, mapEventList[i].OnlyId);

                    }
                }
                break;
        }
        if (teamData == null)
        {
            TaskManager.Instance.GetAchievement(AchievementType.AccomplishedMapEvent, setting.Type);
            TaskManager.Instance.GetDailyAchievement(TaskType.AccomplishMapEvent, setting.Type);
            TaskManager.Instance.TryAccomplishGuideBook(TaskType.AccomplishMapEvent);
        }
        RemoveMapEvent(singleMapEventData);
        //自动保存
#if !UNITY_EDITOR
                ArchiveManager.Instance.SaveArchive();
#endif
    }



    /// <summary>
    /// 通过唯一id找地图事件
    /// </summary>
    /// <returns></returns>
    public SingleMapEventData FindMapEventDataByOnlyId(ulong onlyId)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (data.OnlyId == onlyId)
                return data;
        }
        return null;
    }


    /// <summary>
    /// 找是否应该生成算命的
    /// </summary>
    public void CheckIfGenerateShenSuanZiMapEvent()
    {
        //TODO找个空位
        //for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapEventData.SingleMapEventList.Count; i++)
        //{

        //}
        MapEventManager.Instance.AddMapEventAtIndex((int)MapEventIdType.ShenSuanZi, SceneType.OutsideMap, 10);

    }
    /// <summary>
    /// 秘境探索得到物品
    /// </summary>
    /// <param name="singleExploreData"></param>
    public void GetExploreItem(ExploreTeamData teamData,int itemId,ulong num)
    {
        bool haveSameItem = false;
        for(int i=0;i < teamData.ItemList.Count; i++)
        {
            ItemData data = teamData.ItemList[i];
            if (data.settingId == itemId)
            {
                data.count += num;
                haveSameItem = true;
                break;
            }
        }
        if (!haveSameItem)
        {
            ItemData data = new ItemData();
            data.settingId = itemId;
            data.count = (ulong)num;
            teamData.ItemList.Add(data);
        }
        EventCenter.Broadcast(TheEventType.GetExploreItem);
    }

    /// <summary>
    /// 激活随机事件开关
    /// </summary>
    public void TriggerRdmMapEvent()
    {
        //激活的时候 该刷新的世界就要刷新
        //UnlockOutsideMapEvent();
        //UnlockWorldAllEventBrush();
    }

    /// <summary>
    /// 解锁外出地图的事件
    /// </summary>
    public void UnlockOutsideMapEvent()
    {
        for (int i = 0; i < DataTable._mapEventList.Count; i++)
        {
            MapEventSetting setting = DataTable._mapEventList[i];
            if (setting.Pos != null)
            {
                List<int> pos = CommonUtil.SplitCfgOneDepth(setting.Pos);
                SceneType sceneType = (SceneType)pos[0];
                if (sceneType == SceneType.OutsideMap)
                    AddByTimeMapEvent(setting);
            }

        }
    }

    /// <summary>
    /// 解锁根据时间来的单个地图事件
    /// </summary>
    /// <param name="setting"></param>
    bool AddByTimeMapEvent(MapEventSetting setting)
    {
        bool success = false;
        if (setting.Pos != null)
        {
            List<int> pos = CommonUtil.SplitCfgOneDepth(setting.Pos);

            SceneType sceneType = (SceneType)pos[0];
            if (sceneType == SceneType.OutsideMap)
            {
                //生成该事件 找个没人的
                int posIndex = RandomManager.Next(pos[1], pos[2]);
                if (occupiedPosDic.ContainsKey(sceneType)
      && occupiedPosDic[sceneType].Contains(posIndex))
                {
                    Debug.Log("被占据，不生成");
                    success = false;
                }
                else
                {
                    
                   // PanelManager.Instance.AddTongZhi(TongZhiType.Common, setting.Name + "在" + CommonUtil.SceneName(sceneType) + "出现了，修士可前往探索一番");
                    AddMapEventAtIndex(setting.Id.ToInt32(), sceneType, posIndex);
                    success = true;
                }
            }
            else if (sceneType == SceneType.FixedMap)
            {
                //生成该事件 找个没人的
                int mapId = pos[1];
                int posIndex = 0;
                if (setting.EventAppearType.ToInt32() == (int)MapEventAppearType.FixXiaoGuai)
                {
                    posIndex = FindUnOccupiedPosIndex(mapId, ConstantVal.Appear1EventPos(SceneType.FixedMap, mapId));
                }
                else
                {
                    posIndex = pos[2];

                }


                if (occupiedPosDic.ContainsKey(sceneType)
&& occupiedPosDic[sceneType].Contains(posIndex))
                {
                    Debug.Log("被占据，不生成");
                    success = false;

                }
                else
                {
                  
                    AddFixedMapEventAtIndex(setting.Id.ToInt32(), mapId, posIndex);
                    success = true;
                }
            }

        }
        return success;
    }
    /// <summary>
    /// 找未占据的
    /// </summary>
    /// <param name="candidate"></param>
    /// <returns></returns>
    public int FindUnOccupiedPosIndex(int mapId, List<int> candidate)
    {
        List<int> realCandidate = new List<int>();
        for (int i = 0; i < candidate.Count; i++)
        {
            int posIndex = candidate[i];
            if(occupiedFixedMapPosDic.ContainsKey(mapId)
                &&occupiedFixedMapPosDic[mapId].Contains(posIndex))
       
            {

            }
            else
            {
                realCandidate.Add(posIndex);
            }
        }

        if (realCandidate.Count > 0)
            return realCandidate[RandomManager.Next(0, realCandidate.Count)];
        return 0;
    }
    /// <summary>
    /// 刚解锁世界时直接刷新该地图事件
    /// </summary>
    public void UnlockWorldAllEventBrush()
    {
        for (int i = 0; i < DataTable._mapEventList.Count; i++)
        {
            MapEventSetting setting = DataTable._mapEventList[i];
            if (setting.Pos != null)
            {
                List<int> pos = CommonUtil.SplitCfgOneDepth(setting.Pos);
                SceneType sceneType = (SceneType)pos[0];
                if (sceneType == SceneType.FixedMap)
                {
                    //生成该事件 找个没人的
                    int mapId = pos[1];
                    if (mapId == 10000)
                    {
                        AddByTimeMapEvent(setting);
                    }
                }
            }
        }
    }
 
    /// <summary>
    /// 一个月过去了
    /// </summary>
    public void OnMonthPassed()
    {
        //if (!RoleManager.Instance._CurGameInfo.AllMapEventData.TriggerRdmMapEvent)
        //    return;
        for (int i = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count - 1; i >= 0; i--)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (data.HaveLimitTime)
            {
                data.RemainMonth--;
                if (data.RemainMonth <= 0)
                {
                    MapEventSetting setting = DataTable.FindMapEventSetting(data.SettingId);

                    RemoveMapEvent(data);

                }
            }

        }
        for (int i = RoleManager.Instance._CurGameInfo.allMapEventData.WaitToAppearList.Count - 1; i >= 0; i--)
        {
            MapEventWaitToAppearData next = RoleManager.Instance._CurGameInfo.allMapEventData.WaitToAppearList[i];
            next.RemainMonth--;
            if (next.RemainMonth <= 0)
            {
                //RoleManager.Instance._CurGameInfo.mapd
                MapEventSetting setting = DataTable.FindMapEventSetting(next.SettingId);
                if (AddByTimeMapEvent(setting))
                {
                    RoleManager.Instance._CurGameInfo.allMapEventData.WaitToAppearList.Remove(next);
                }
            }
        }
 
    }

    /// <summary>
    /// 灵气团修为量
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int LingQiTuanXiuWeiAdd(SingleMapEventData data)
    {
        int juLingZhenAdd = 0;
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.JuLingZhen) > 0)
        {
            juLingZhenAdd = ConstantVal.juLingZhenAdd;
        }
        return (int)( data.Param * (1 + juLingZhenAdd * 0.01f));
    }
}

/// <summary>
/// 地图事件位置类型 
/// </summary>
public enum MapEventPosType
{
    None = 0,
    Pos = 1,//根据坐标  
    Index = 2,//根据下标确定
}

/// <summary>
/// 事件出现类型
/// </summary>
public enum MapEventAppearType
{
    None = 0,
    FixXiaoGuai = 1,//固定位置的小怪副本
}