using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleMapView : SingleViewBase
{
    public Button btn_enterWorld;
    public Button btn_enterMiJing;
    public Button btn_enterXianMen;
   

    public Text txt_name;
    public Transform trans_lock;
    public int settingId;
    public int unlockExploreId;
    public MapSetting mapSetting;
    public Transform trans_unlockedContent;//三个按钮
    public Button btn;
    public WorldMapPanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[0] as WorldMapPanel;
        mapSetting = DataTable.FindMapSetting(settingId);
        addBtnListener(btn_enterWorld, () =>
        {
            
            //判断是否解锁
            if (!MapManager.Instance.CheckIfUnlockMap(settingId))
            {
                
                //通过上一关后解锁
                MapSetting foreMap = DataTable.FindMapSetting(mapSetting.ForeMap.ToInt32());
                PanelManager.Instance.OpenFloatWindow("通关" + foreMap.Name + LanguageUtil.GetLanguageText((int)LanguageIdType.界域裂隙) + "后解锁");
            }
            else
            {
                RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId = settingId;
                GameSceneManager.Instance.GoToScene(SceneType.FixedMap, false);
                if (TaskManager.Instance.guide_passFixLevel)
                {

                    if (mapSetting.MapLevel.ToInt32() == TaskManager.Instance.guide_passFixLevelMapId)
                    {
                        TaskManager.Instance.guide_passFixLevel = false;

                        TaskManager.Instance.guide_passFixLevelMapId = 0;
                    }

                }
            }

        });
        //进入秘境
        addBtnListener(btn_enterMiJing, () =>
        {
            int exploreId = mapSetting.ExploreMapId.ToInt32();
 
            //解锁了
            if (MapManager.Instance.FindSingleExploreDataById(exploreId).Unlocked)
            {
                SingleExploreData curExploreData = null;
                for (int i=0;i< RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
                {
                    SingleExploreData data = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
                    if (data.SettingId == exploreId)
                    {
                        curExploreData = data;
                        break;
                    }
              
                }
           
                MapManager.Instance.curChoosedExploreId = curExploreData.SettingId;

                //秘境是不是有弟子
                if (curExploreData.ExploreTeamData != null)
                {
                    GameSceneManager.Instance.GoToScene(SceneType.MiJingExplore, false);
                }
                else
                {
                    PanelManager.Instance.OpenPanel<GotoExplorePreparePanel>(PanelManager.Instance.trans_layer2, settingId);
                }
            }
            else
            {
                MapSetting foreMap = DataTable.FindMapSetting(mapSetting.ForeMap.ToInt32());
                if (foreMap != null)
                    PanelManager.Instance.OpenFloatWindow("通关" + foreMap.Name + LanguageUtil.GetLanguageText((int)LanguageIdType.界域裂隙) + "后解锁");
                else
                    PanelManager.Instance.OpenFloatWindow("通关上个地图后解锁");
            }

        });
        //进入仙门
        addBtnListener(btn_enterXianMen, () =>
        {
       
        });

        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedMap(this);
        });
    }

    public void OnChoose(bool choose)
    {
        if (choose)
        {
            if (parentPanel.isWorld)
            {
                trans_unlockedContent.gameObject.SetActive(true);
                ShowGuide();
            }
            else
            {
                OnEnterLieXiClick();
            }
     
        }
        else
        {
            trans_unlockedContent.gameObject.SetActive(false);

        }
    }

    /// <summary>
    /// 进入裂隙
    /// </summary>
    void OnEnterLieXiClick()
    {
      

        if (MapManager.Instance.FindMapById(settingId).MapStatus != (int)AccomplishStatus.Accomplished)
        {
            PanelManager.Instance.OpenFloatWindow("通关" + mapSetting.Name + "后解锁");

        }
        else if (!RoleManager.Instance._CurGameInfo.XianMenOpen)
        {
            PanelManager.Instance.OpenFloatWindow( LanguageUtil.GetLanguageText((int)LanguageIdType.界域裂隙) + "每年12月开启");
        }
        else
        {
            //RoleManager.Instance.DeProperty(PropertyIdType.Tili, -ConstantVal.lieXiNeedYuanLi);
           
                RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId = settingId;
                TDGAMission.OnBegin(mapSetting.Name + LanguageUtil.GetLanguageText((int)LanguageIdType.界域裂隙));
                MapManager.Instance.curMapIndex = 1;
                List<PeopleData> team = RoleManager.Instance.FindMyBattleTeamList(false, 0);
                for (int i = 0; i < team.Count; i++)
                {
                    RoleManager.Instance.FullMP(team[i]);
                }
                GameSceneManager.Instance.GoToScene(SceneType.SingleMap, true);
            
        }
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_passFixLevel)
        {
            
                if (mapSetting.MapLevel.ToInt32() == TaskManager.Instance.guide_passFixLevelMapId)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(btn_enterWorld.gameObject);
                }
            
        }
 
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public void InitShow()
    {
        MapSetting setting = DataTable.FindMapSetting(settingId);
        txt_name.SetText(setting.Name);

        if (MapManager.Instance.CheckIfUnlockMap(settingId))
        {
            trans_lock.gameObject.SetActive(false);
        }
        else
        {
            trans_lock.gameObject.SetActive(true);

        }
        trans_unlockedContent.gameObject.SetActive(false);
    }
}
