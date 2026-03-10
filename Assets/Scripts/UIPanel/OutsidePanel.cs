using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutsidePanel : PanelBase
{

    public ScrollViewNevigation scrollViewNevigation;

    public Transform trans_mapEventParent;//地图事件父物体
    public Transform trans_mapEventStartPos;//地图事件起始点
    public int mapEventXCount = 3;//x轴最多几个事件
    public float mapEventOffsetX = 205;//地图事件x距离
    public float mapEventOffsetY = 205;//地图事件y距离
    public List<MapEventView> mapEventViewList = new List<MapEventView>();//地图事件
    public Transform trans_npcParent;//npc所在位置的父物体

    public Button btn_worldEnter;//世界入口

    #region 历练相关
    public List<SingleLiLianView> liLianViewList;//历练

    #endregion

    #region 讨伐相关
    public List<SingleMiJingView> taoFaViewList;//讨伐
    #endregion

    public override void Init(params object[] args)
    {
        base.Init(args);
 

        for(int i = 0; i < liLianViewList.Count; i++)
        {
            liLianViewList[i].Clear();

            liLianViewList[i].Init();
        }

        for (int i = 0; i < taoFaViewList.Count; i++)
        {
            taoFaViewList[i].Clear();

            taoFaViewList[i].Init();
            taoFaViewList[i].OnOpenIng();
        }


        RegisterEvent(TheEventType.AddMapEvent, GenerateMapEvent);
        RegisterEvent(TheEventType.ShowNPC, GenerateNPC);
        RegisterEvent(TheEventType.RefreshLiLianShow, OnAccomplishedLiLian);

        addBtnListener(btn_worldEnter, () =>
        {
            GameSceneManager.Instance.GoToScene(SceneType.WorldMap);
        });

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshMapEvent();
        RefreshNPCShow();
        ShowGuide();
    }

    public void ShowGuide()
    {
        if (TaskManager.Instance.guide_mapEvent)
        {
            for (int j = 0; j < mapEventViewList.Count; j++)
            {
                MapEventView view = mapEventViewList[j];
                if (view.singleMapEventData.OnlyId == TaskManager.Instance.guide_mapEventOnlyId)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                    break;
                }
            }
        }else if (TaskManager.Instance.guide_passFixLevel)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_worldEnter.gameObject);

        }
    }

    void OnAccomplishedLiLian()
    {

        for (int i = 0; i < liLianViewList.Count; i++)
        {
            liLianViewList[i].Clear();
            liLianViewList[i].Init();
        }

    }
    /// <summary>
    /// 刷新npc显示
    /// </summary>
    void RefreshNPCShow()
    {
        ClearCertainParentAllSingle<NPCView>(trans_npcParent);
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            ulong npcOnlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
            SingleNPCData data = TaskManager.Instance.FindNPCByOnlyId(npcOnlyId);
            if (data.CurShowScene == (int)SceneType.OutsideMap)
            {
                if (data.LocalPos.Count == 0)
                {
                    data.LocalPos.Add(0);
                    data.LocalPos.Add(0);
                }
                NPCView npcView = AddSingle<NPCView>(trans_npcParent, data, new Vector2(data.LocalPos[0],data.LocalPos[1]));

            }
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
            SingleMapEventData singleMapEventData = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (singleMapEventData.MapSceneType == (int)SceneType.OutsideMap)
            {
                int index = singleMapEventData.PosIndex;
                Vector2 localPos = GetMapEventPosByIndex(index);
                if (singleMapEventData.Pos.Count == 0)
                {
                    singleMapEventData.Pos.Add(Mathf.RoundToInt(localPos.x));
                    singleMapEventData.Pos.Add(Mathf.RoundToInt(localPos.y));

                }
                else
                {
                    singleMapEventData.Pos[0] = Mathf.RoundToInt(localPos.x);
                    singleMapEventData.Pos[1] = Mathf.RoundToInt(localPos.y);

                }
                mapEventViewList.Add( AddSingle<MapEventView>(trans_mapEventParent, singleMapEventData));
            }
        }
    }

    /// <summary>
    /// 通过index得到坐标位
    /// </summary>
    public Vector2 GetMapEventPosByIndex(int index)
    {
        Vector2 startPos = trans_mapEventStartPos.transform.localPosition;
        //第几行 从0数
        int hangIndex = index / 5;
        //第几列 从0数
        int lieIndex = index % mapEventXCount;
        Vector2 localPos = new Vector2(startPos.x + mapEventOffsetX * lieIndex, startPos.y - mapEventOffsetY * hangIndex);
        return localPos;
    }

    /// <summary>
    /// 生成地图事件
    /// </summary>
    /// <param name="args"></param>
    void GenerateMapEvent(object[] args)
    {
        SingleMapEventData data = args[0] as SingleMapEventData;
        MapEventSetting setting = DataTable.FindMapEventSetting(data.SettingId);
        if (data.MapSceneType == (int)SceneType.OutsideMap)
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
                Vector2 localPos = GetMapEventPosByIndex(index);
                if (data.Pos.Count == 0)
                {
                    data.Pos.Add(Mathf.RoundToInt(localPos.x));
                    data.Pos.Add(Mathf.RoundToInt(localPos.y));

                }
                else
                {
                    data.Pos[0] = Mathf.RoundToInt(localPos.x);
                    data.Pos[1] = Mathf.RoundToInt(localPos.y);

                }
                MapEventView view = PanelManager.Instance.OpenSingle<MapEventView>(trans_mapEventParent, data);
                mapEventViewList.Add(view);
            }
        }
    }

    /// <summary>
    /// 生成npc
    /// </summary>
    /// <param name="args"></param>
    void GenerateNPC(object[] args)
    {
        SingleNPCData singleNPCData = args[0] as SingleNPCData;
        if (singleNPCData.CurShowScene == (int)SceneType.OutsideMap)
        {
            NPCView npcView = PanelManager.Instance.OpenSingle<NPCView>(trans_npcParent, singleNPCData, (Vector2)Vector3.zero);

        }
    }


 
 
 

    public override void Clear()
    {
        base.Clear();
 
        for(int i = 0; i < liLianViewList.Count; i++)
        {
            liLianViewList[i].Clear();
        }
        for (int i = 0; i < taoFaViewList.Count; i++)
        {
            taoFaViewList[i].Clear();
        }
        for (int i = 0; i < mapEventViewList.Count; i++)
        {
            mapEventViewList[i].Clear();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        for (int i = 0; i < taoFaViewList.Count; i++)
        {
            taoFaViewList[i].OnClose();
        }
        for (int i = 0; i < liLianViewList.Count; i++)
        {
            liLianViewList[i].OnClose();
        }
        for (int i = 0; i < mapEventViewList.Count; i++)
        {
            mapEventViewList[i].OnClose();
        }
        TaskManager.Instance.guide_mapEvent = false;
    }
}
