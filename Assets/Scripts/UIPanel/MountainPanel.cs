using DG.Tweening;
using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountainPanel : PanelBase
{
    public Button btn_gate;//大门

    public Button btn_offLine;//离线
    public GameObject ganTanHao;//感叹号
   
 


    public Button btn_zhaoCha;//找茬者


    //public RectTransform rectTrans_singleTypeStudentScroll;//学生scroll
    //public Transform trans_singleTypeStudentGrid;//学生grid
    //public Button btn_lianDanStudentTag;//炼丹师tag
    //public Button btn_equipMakeStudentTag;//炼器师tag
    //public Button btn_battleStudentTag;//内门弟子tag

    //public Transform studentTagScrollParent;//学生

    public Button btn_miJing;//秘境
    public Button btn_produce;//生产

     public Transform trans_effectParent;//一些特效小东西的父物体

    #region 研究塔
    public Transform trans_researchBar;//研究塔进度
    public Image img_researchBar;//研究塔
    public Button btn_research;//研究塔按钮
    #endregion

    public List<MapEventView> mapEventViewList;//地图事件view
    public List<ulong> curNPCViewOnlyIdList;//当前显示的npc唯一id
    public List<NPCView> curNPCViewList;//当前显示的npc

    public Transform trans_cloudParent;//云层父物体

    // public Button btn_economy;//经济
    
    public Transform trans_farmParent;

    public List<SingleFarmView> farmList;//所有丹田
    public Button btn_empty;//空白

    public Button btn_zongMen;//宗门
    public Text txt_zongMenLevel;//宗门等级
    public ScrollViewNevigation scrollViewNevigation;//scroll

    public Transform trans_buildingSingleParent;//建造界面父物体

    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn_gate, () =>
        {
            if (BuildingManager.Instance.curMode == MountainMode.Building)
                return;
            PanelManager.Instance.OpenPanel<ShanMenPanel>(PanelManager.Instance.trans_layer2);
        });


 
        addBtnListener(btn_zhaoCha, () =>
        {
            ZhaoChaPeopleManager.Instance.ZhaoCha();
        });
        addBtnListener(btn_miJing, () =>
         {
             GameSceneManager.Instance.GoToScene(SceneType.MiJing);
         });


 

     

        addBtnListener(btn_empty, () =>
        {

            OnClickedFarm(null);
        });

        addBtnListener(btn_zongMen, () =>
        {
        
            if (BuildingManager.Instance.curMode == MountainMode.Building)
                return;
            PanelManager.Instance.OpenPanel<ZongMenUpgradePanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_offLine, () =>
        {
            PanelManager.Instance.OpenOfflinePanel();
        });
        //建造模式跟着动
        scrollViewNevigation.scrollRect.onValueChanged.RemoveAllListeners();
        scrollViewNevigation.scrollRect.onValueChanged.AddListener((x) =>
        {
            EventCenter.Broadcast(TheEventType.OnMountainScrollMove, trans_content);
        });
         RegisterEvent(TheEventType.GetItemFlyAnim, OnGetItemFlyAnim);
        RegisterEvent(TheEventType.ResearchProcess, OnResearchProcessing);
        RegisterEvent(TheEventType.AccomplishResearchProcess, OnAccomplishResearch);
        RegisterEvent(TheEventType.AddMapEvent, GenerateMapEvent);
        RegisterEvent(TheEventType.ZongMenLevelUpgrade, ShowZongMenLevel);
        RegisterEvent(TheEventType.RefreshOfflineShow, RefreshOfflineShow);

        RegisterEvent(TheEventType.NevigateToMountainPos, OnNevigateToPos);
        RegisterEvent(TheEventType.Building_overlapSearch, CheckIfBuildingOverlap);
        RegisterEvent(TheEventType.Moving_overlapSearch, CheckIfMovingOverlap);

        RegisterEvent(TheEventType.StartFarmBuild, AddADanFarm);
        RegisterEvent(TheEventType.EnterBuildingMode, EnterBuildingMode);
        RegisterEvent(TheEventType.QuitBuildingMode, ReArrangeFarmLayer);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //RefreshStudentNum();
  
         //显示学生
        ShowStudentScroll();
        //显示npc
        //if (RoleManager.Instance._CurGameInfo.CurAppearNPCId !=0) 
        //{
        //    SingleNPCData singleNPCData = TaskManager.Instance.FindNPCById(RoleManager.Instance._CurGameInfo.CurAppearNPCId);
        //    NPCView npcView = PanelManager.Instance.OpenSingle<NPCView>(trans_npc, singleNPCData,(Vector2)Vector3.zero);

        //}
        trans_researchBar.gameObject.SetActive(false);

        InitMapEvent();
 


 
        ////叛宗弟子
        //if (PanelManager.Instance.curYieldShowInMainPanelType == YieldShowInMainPanelType.WinPanZongBattle)
        //{
      
        //    PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.None;
        //    //PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting(10002));
        //}
        //丹田
        ShowDanFarm();

        ShowZongMenLevel();
        RefreshOfflineShow();
        trans_content.localPosition = new Vector3(502, 1209);
    }
    /// <summary>
    /// 为了适配延迟一会冻结移动
    /// </summary>
    /// <returns></returns>
    IEnumerator YieldFrozenScroll()
    {
        yield return null;
        scrollViewNevigation.scrollRect.horizontal = false;
        scrollViewNevigation.scrollRect.vertical = false;
    }
    /// <summary>
    /// 重排建筑层级
    /// </summary>
    void ReArrangeFarmLayer()
    {
        //排序
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count - 1; i++)
        {
            for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count - 1 - i; j++)
            {
                //后面的大于前面 则二者交换
                if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j + 1].LocalPos[1] >= RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j].LocalPos[1])
                {
                    var temp = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j];
                    RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j] = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j + 1];
                    RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j + 1] = temp;

                    var temp2 = farmList[j];
                    farmList[j] = farmList[j + 1];
                    farmList[j + 1] = temp2;

                    farmList[j].transform.SetSiblingIndex(j);
                    farmList[j + 1].transform.SetSiblingIndex(j+1);
                }
            }
        }
    }
    /// <summary>
    /// 进入建造模式
    /// </summary>
    void EnterBuildingMode(object[] args)
    {
        int theId = (int)args[0];
        AddSingle<BuildingSingleView>(trans_buildingSingleParent, theId, this);
    }

    private void Update()
    {
   
    }
    /// <summary>
    /// 增加一个建筑
    /// </summary>
    /// <param name="args"></param>
    void AddADanFarm(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        //Vector2 buildingModePos = (Vector2)args[1];
        ////给该位置赋值
        //Vector2 pos= buildingModePos - (Vector2)trans_content.localPosition;
        //data.LocalPos.SetPos(pos);
        SingleFarmView view=AddSingle<SingleFarmView>(trans_farmParent, data, this);
        view.transform.SetAsLastSibling();
        farmList.Add(view);
    }
    /// <summary>
    /// 建筑是否重叠
    /// </summary>
    void CheckIfBuildingOverlap(object[] args)
    {
        bool overLap = false;
        RectTransform rt = args[0] as RectTransform;
        for(int i = 0; i < farmList.Count; i++)
        {
            SingleFarmView theView = farmList[i];
            RectTransform TargetRt = theView.img_range.GetComponent<RectTransform>();
            if (TargetRt.gameObject.activeInHierarchy)
            {
                overLap = BuildingSingleView.RectTransToScreenPos(rt, Camera.main).Overlaps(BuildingSingleView.RectTransToScreenPos(TargetRt, Camera.main));
            }
            if (overLap)
            {
                break;
            }
        }
        //是否和obstacle重叠
        if (!overLap)
        {
            BuildObstacle[] obstacles = GetComponentsInChildren<BuildObstacle>();
            for (int i = 0; i < obstacles.Length; i++)
            {
               
                RectTransform TargetRt = obstacles[i].GetComponent<RectTransform>();
                if (TargetRt.gameObject.activeInHierarchy)
                {
                    overLap = BuildingSingleView.RectTransToScreenPos(rt, Camera.main).Overlaps(BuildingSingleView.RectTransToScreenPos(TargetRt, Camera.main));

                }
                if (overLap)
                {
                    break;
                }
            }
        }

        EventCenter.Broadcast(TheEventType.Building_overlapRes, overLap);
    }
    /// <summary>
    /// 建筑是否重叠
    /// </summary>
    void CheckIfMovingOverlap(object[] args)
    {
        bool overLap = false;
        RectTransform rt = args[0] as RectTransform;
        SingleDanFarmData data = args[1] as SingleDanFarmData;
        for (int i = 0; i < farmList.Count; i++)
        {
            SingleFarmView theView = farmList[i];
            
            RectTransform TargetRt = theView.img_range.GetComponent<RectTransform>();
            if (data == theView.singleDanFarmData)
                continue;
            if (TargetRt.gameObject.activeInHierarchy)
                overLap = BuildingSingleView.RectTransToScreenPos(rt, Camera.main).Overlaps(BuildingSingleView.RectTransToScreenPos(TargetRt, Camera.main));
            if (overLap)
            {
                break;
            }
        }
        if (!overLap)
        {
            RectTransform gateRect = btn_gate.GetComponent<RectTransform>();

            overLap = BuildingSingleView.RectTransToScreenPos(rt, Camera.main).Overlaps(BuildingSingleView.RectTransToScreenPos(gateRect, Camera.main));

        }
        if (!overLap)
        {
            RectTransform zongMen = btn_zongMen.GetComponent<RectTransform>();

            overLap = BuildingSingleView.RectTransToScreenPos(rt, Camera.main).Overlaps(BuildingSingleView.RectTransToScreenPos(zongMen, Camera.main));

        }
        EventCenter.Broadcast(TheEventType.Moving_overlapRes, overLap, data);
    }
    void OnNevigateToPos(object[] args)
    {
        RectTransform rect = args[0] as RectTransform;
        scrollViewNevigation.NevigateImmediately(rect);
    }

    void RefreshOfflineShow()
    {
        int hour = RoleManager.Instance._CurGameInfo.timeData.OffLineTotalMinute / 60;
        if (hour >= 3)
            ganTanHao.gameObject.SetActive(true);
        else
            ganTanHao.gameObject.SetActive(false);

    }

 

    /// <summary>
    /// 初始化地图事件
    /// </summary>
    void InitMapEvent()
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            //MapEventSetting setting = DataTable.FindMapEventSetting(data.SettingId);
            if (data.MapSceneType ==(int) SceneType.Mountain)
            {
                bool haveSame = false;
                for (int j = 0; j < mapEventViewList.Count; j++)
                {
                    MapEventView existView = mapEventViewList[j];
                    if (existView.singleMapEventData.OnlyId == data.OnlyId)
                    {
                        haveSame = true;
                        break;
                    }
                }
                if (!haveSame)
                {
                    MapEventView view = PanelManager.Instance.OpenSingle<MapEventView>(trans_effectParent, data);
                    mapEventViewList.Add(view);
                }
            }
        }
    }

  

    /// <summary>
    /// 研究塔进行中
    /// </summary>
    /// <param name="param"></param>
    void OnResearchProcessing(object[] param)
    {
        float process = (float)param[0];
        if (!trans_researchBar.gameObject.activeInHierarchy)
        {
            trans_researchBar.gameObject.SetActive(true);
            img_researchBar.DOKill();
            img_researchBar.fillAmount = process;
        }
        else
        {
            img_researchBar.DOFillAmount(process,1f);

        }
    }
    /// <summary>
    /// 研究结束
    /// </summary>
    void OnAccomplishResearch()
    {
        trans_researchBar.gameObject.SetActive(false);

    }
 

    /// <summary>
    /// 生成地图事件
    /// </summary>
    /// <param name="args"></param>
    void GenerateMapEvent(object[] args)
    {
        SingleMapEventData data = args[0] as SingleMapEventData;
        MapEventSetting setting = DataTable.table.TbMapEvent.Get(data.SettingId.ToString());
        if (data.MapSceneType == (int)SceneType.Mountain)
        {
            //生成 特殊处理
            //if (setting.id == (int)MapEventIdType.Guide_HunHun)
            //{
            //    //if (mapEventViewList.Count > 0)
            //    //{
            //    //    Vector2 pos
            //    //}
            //    //setting.m

            //}
            bool haveSame = false;
            for(int i = 0; i < mapEventViewList.Count; i++)
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
                MapEventView view = PanelManager.Instance.OpenSingle<MapEventView>(trans_effectParent, data);
                mapEventViewList.Add(view);
            }
        }
    }

    void ShowStudentScroll()
    {
        
       // StudentTagScrollView studentTagScrollView = PanelManager.Instance.OpenSingle<StudentTagScrollView>(studentTagScrollParent,false);
    }



 
    ///// <summary>
    ///// 刷新学生数量
    ///// </summary>
    //public void RefreshStudentNum()
    //{
    //    txt_studentNum.SetText(RoleManager.Instance._CurGameInfo.StudentData.CurStudentNum + "/" + RoleManager.Instance._CurGameInfo.StudentData.MaxStudentNum);
    //    txt_freeStudentNum.SetText(RoleManager.Instance._CurGameInfo.StudentData.CurFreeStudentNum.ToString());
    //}



    /// <summary>
    /// 获得物品
    /// </summary>
    /// <param name="param"></param>
    public void OnGetItemFlyAnim(object[] param)
    {
        Vector3 pos = (Vector3)param[0];
        ItemData item = param[1] as ItemData;
        AddSingle<GetItemFlyUpAnimView>(trans_effectParent, pos, item);
    }

    /// <summary>
    /// 显示丹田
    /// </summary>
    void ShowDanFarm()
    {
        for(int i=0;i< RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (singleDanFarmData.SettingId == 70001) return;
            SingleFarmView farmView= AddSingle<SingleFarmView>(trans_farmParent,singleDanFarmData,this);
            farmList.Add(farmView);
        }
    }

    /// <summary>
    /// 
    /// 点击了单个farm
    /// </summary>
    public void OnClickedFarm(SingleFarmView singleFarmView)
    {
        //for(int i = 0; i < farmList.Count; i++)
        //{
        //    SingleFarmView theFarm = farmList[i];
        //    if (theFarm == singleFarmView)
        //    {
        //        theFarm.OnChoose(true);
        //    }
        //    else
        //    {
        //        theFarm.OnChoose(false);
        //    }
        //}
    }
    /// <summary>
    /// 显示宗门等级
    /// </summary>
    void ShowZongMenLevel()
    {
        txt_zongMenLevel.SetText("Lv" + RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel);

    }

    public SingleFarmView FindFarmViewById(int id)
    {
        for(int i = 0; i < farmList.Count; i++)
        {
            SingleFarmView farmView = farmList[i];
            if (farmView.singleDanFarmData.SettingId == id)
                return farmView;
        }
        return null;
    }
    //通过index找田
    public SingleFarmView FindFarmViewByOnlyId(ulong onlyId)
    {
        for (int i = 0; i < farmList.Count; i++)
        {
            SingleFarmView farmView = farmList[i];
            if (farmView.singleDanFarmData.OnlyId == onlyId)
                return farmView;
        }
        return null;
    }
    public override void Clear()
    {
        base.Clear();
   

      

        //PanelManager.Instance.CloseAllSingle(studentTagScrollParent);

 
        mapEventViewList.Clear();
        curNPCViewOnlyIdList.Clear();
        PanelManager.Instance.CloseAllSingle(trans_effectParent);
        PanelManager.Instance.CloseAllPanel(trans_cloudParent);
       
        ClearCertainParentAllSingle<SingleViewBase>(trans_buildingSingleParent);
        ClearCertainParentAllSingle<SingleViewBase>(trans_farmParent);
        farmList.Clear();
     
    }
}
