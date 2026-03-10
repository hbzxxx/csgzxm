using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : CommonInstance<GameSceneManager>
{
    public SceneType toGoSceneType;//即将去的场景
    //public SceneType curSceneType;//当前场景

    // 是否跳过打开TopPanel和MainPanel（新存档时使用）
    public bool skipOpenTopMainPanel = false;

    public void GoToScene(SceneType sceneType,bool refresh=true)
    {
        toGoSceneType = sceneType;
        switch (sceneType)
        {
            case SceneType.MatchPrepare:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
 
                break;

            case SceneType.Battle:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                PanelManager.Instance.OpenPanel<BattlePanel>(PanelManager.Instance.trans_layer2,BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
                    BattleManager.Instance.p2List[BattleManager.Instance.p2Index]);
                PanelManager.Instance.OpenPanel<BattleScenePanel>(PanelManager.Instance.trans_sceneLayer, BattleManager.Instance.p1List[BattleManager.Instance.p1Index],
                    BattleManager.Instance.p2List[BattleManager.Instance.p2Index]);
                
                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_Battle);
                break;
            case SceneType.Mountain:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                // 新存档时跳过打开TopPanel和MainPanel，等SetNamePanel完成后再打开
                if (!skipOpenTopMainPanel)
                {
                    PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                    PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);
                }

                PanelManager.Instance.OpenPanel<MountainPanel>(PanelManager.Instance.trans_sceneLayer);

                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_Mountain);
                if (RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex >= 7)
                {
                    if (GameTimeManager.Instance.connectedToFuWuQiTime
                && RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex)
                        PanelManager.Instance.OpenThirtyDayQianDaoPanel();
                }
                break;
                //单个仙门
            case SceneType.SingleMap:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                //PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                //PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                if (refresh)   
                    MapManager.Instance.InitAllLieXiLevel(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);

                GameTimeManager.Instance.StopMove();

                switch (RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId)
                {
                    case 10000:
                        PanelManager.Instance.OpenPanel<SingleMapScenePanel_0>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.XianMen,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10001:
                      PanelManager.Instance.OpenPanel<SingleMapScenePanel_1>(PanelManager.Instance.trans_sceneLayer,
                          MapSceneType.XianMen,
                          RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10002:
                        PanelManager.Instance.OpenPanel<SingleMapScenePanel_2>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.XianMen,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10003:
                        SingleMapScenePanel_3 map3= PanelManager.Instance.OpenPanel<SingleMapScenePanel_3>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.XianMen,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        map3.EnterLayer(MapManager.Instance.curMapIndex);
                        break;
                    case 10004:
                        PanelManager.Instance.OpenPanel<SingleMapScenePanel_4>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.XianMen,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10005:
                        PanelManager.Instance.OpenPanel<SingleMapScenePanel_5>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.XianMen,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                }

                PanelManager.Instance.OpenPanel<DuJiePanel>(PanelManager.Instance.trans_layer2);

                PanelManager.Instance.OpenPanel<MainMapUIPanel>(PanelManager.Instance.trans_layer2);
                //如果所有人人血没了 说明彻底失败，进入结算界面
                List<PeopleData> teamList = RoleManager.Instance.FindMyBattleTeamList(false,0);
                int haveHpPNum = teamList.Count;
                for(int i = 0; i < teamList.Count; i++)
                {
                    PeopleData p = teamList[i];
                    if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num <= 0)
                    {
                        haveHpPNum--;
                    }
                }
                if (haveHpPNum <= 0)
                {
                    MapManager.Instance.MapResult();
                }
                

                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_WorldMap);


                break;

            case SceneType.WorldMap:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<WorldMapPanel>(PanelManager.Instance.trans_sceneLayer);
                // GameTimeManager.Instance.StopMove();
                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_YunHaiZong);

                break;
                //神树
 
            //派遣
            case SceneType.MiJing:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);


                PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                PanelManager.Instance.OpenPanel<MiJingPanel>(PanelManager.Instance.trans_layer2);
                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_MiJing);

                break;
            //外部地图
            case SceneType.OutsideMap:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);


                PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                PanelManager.Instance.OpenPanel<OutsidePanel>(PanelManager.Instance.trans_sceneLayer);
                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_YunHaiZong);

                break;
            //单个主线
            case SceneType.FixedMap:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                //if (refresh)
                MapManager.Instance.InitAllFixedLevel(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);

                //GameTimeManager.Instance.StopMove();

                switch (RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId)
                {
                    case 10000:
                        PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_0>(PanelManager.Instance.trans_sceneLayer,//FixedSingleMapScenePanel_0
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10001:
                        PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_1>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10002:
                        PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_2>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10003:
                        FixedSingleMapScenePanel_3 map3= PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_3>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        map3.EnterLayer(MapManager.Instance.curMapIndex);
                        break;
                    case 10004:
                        PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_4>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                    case 10005:
                        PanelManager.Instance.OpenPanel<FixedSingleMapScenePanel_5>(PanelManager.Instance.trans_sceneLayer,
                                MapSceneType.Fixed,
                            RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                        break;
                }

                //SingleMapData mapData= MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
                //mapData.CurAwardList
                PanelManager.Instance.OpenPanel<FixedMainMapUIPanel>(PanelManager.Instance.trans_layer2);
                AuditionManager.Instance.PlayMapBGM(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);


                break;
            //弟子探索
            case SceneType.MiJingExplore:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                if (refresh)
                {
                    //MapManager.Instance.EnterNewExplore(MapManager.Instance.curChoosedExploreId)
                }
                switch (MapManager.Instance.curChoosedExploreId)
                {
                    case 10000:
                        PanelManager.Instance.OpenPanel<ExplorePanel_0>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;
                    case 10001:
                        PanelManager.Instance.OpenPanel<ExplorePanel_1>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;
                    case 10002:
                        PanelManager.Instance.OpenPanel<ExplorePanel_2>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;
                    case 10003:
                        PanelManager.Instance.OpenPanel<ExplorePanel_3>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;
                    case 10004:
                        PanelManager.Instance.OpenPanel<ExplorePanel_4>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;
                    case 10005:
                        PanelManager.Instance.OpenPanel<ExplorePanel_5>(PanelManager.Instance.trans_sceneLayer,
                                MapManager.Instance.curChoosedExploreId);
                        break;

                }
                PanelManager.Instance.OpenPanel<ExploreUIPanel>(PanelManager.Instance.trans_sceneLayer,
                                           MapManager.Instance.curChoosedExploreId);

                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_MiJing);

                break;

            case SceneType.TaoFa:
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_sceneLayer);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.CloseAllPanel(PanelManager.Instance.trans_layer3);

                PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);
                PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);

                PanelManager.Instance.OpenPanel<MiJingPanel>(PanelManager.Instance.trans_sceneLayer);

                AuditionManager.Instance.PlayBGM(AudioClipType.BGM_WorldMap);

                break;
        }
        Resources.UnloadUnusedAssets();
        RoleManager.Instance._CurGameInfo.SceneData.LastSceneType = RoleManager.Instance._CurGameInfo.SceneData.CurSceneType;

        RoleManager.Instance._CurGameInfo.SceneData.CurSceneType = (int)sceneType;
        //Game.Instance.testImg.SetActive(false);
        EventCenter.Broadcast(TheEventType.RefreshMainPanelBtnShow);
    }



    public void BackLastScene()
    {
       // if()
        GoToScene((SceneType)RoleManager.Instance._CurGameInfo.SceneData.LastSceneType,false);
    }
}

/// <summary>
/// 场景类型
/// </summary>
public enum SceneType
{
    None=0,
    //House=1,//房间
    [EnumAttirbute("山门")]
    Mountain=2,//山上
    Battle=3,//战斗
    MatchPrepare=4,//战斗准备
    SingleMap=5,//单个地图
    WorldMap=6,//世界地图
    Tree=7,//神树
    MiJing=8,//秘境派遣
    OutsideMap=9,//外部地图
    FixedMap=10,//固定地图
    MiJingExplore=11,//秘境探险
    TaoFa=12,//讨伐地图

}
