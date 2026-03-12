using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;

public class GotoExplorePreparePanel : PanelBase
{

    #region 弟子上阵
    public List<ulong> curStudentPosList = new List<ulong>();//学生位

    public List<Transform> trans_UpStudentParent;//上阵学生的位置
    public List<Button> posBtnList;//位置按钮

    public List<ExplorePrepareStudentView> candidateList = new List<ExplorePrepareStudentView>();

    public Button btn_openChooseStudent;//打开学生选择界面

    public Transform trans_chooseStudent;//选择学生
    public Transform trans_chooseStudentGrid;//选择学生格子
    public Button btn_closeChooseStudent;//关闭选择学生界面

    public Transform trans_tiliGrid;

    public Transform grid_possibleAward;//可能产出
    #endregion


    #region 选择地图
    //public SingleChooseExploreView curChoosedMapView;
    //public Transform trans_mapGrid;//所有地图
    //public List<SingleChooseExploreView> allMapViewList = new List<SingleChooseExploreView>();//所有地图
    public int curExploreId;//探索地图id
    public Button btn_go;
    public Button btn_close;

    #endregion

    public override void Init(params object[] args)
    {
        base.Init(args);
        curExploreId = (int)args[0];
 

        addBtnListener(btn_openChooseStudent, () =>
        {
            //PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);

            //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
            //{
            //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
            //    if(p.Talent==(int)StudentTalent.LianGong)
            //    PanelManager.Instance.OpenSingle<ExplorePrepareStudentView>(trans_chooseStudentGrid, p, this);
            //}
            ShowAllCandidateStudent();
            trans_chooseStudent.gameObject.SetActive(true);
        });

        addBtnListener(btn_closeChooseStudent, () =>
        {
            trans_chooseStudent.gameObject.SetActive(false);

        });



        addBtnListener(btn_go, () =>
        {
          if (!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, ConstantVal.miJingExploreNeedTiLi))
                {
                    PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
                }
                else
                {
                    ReallyEnterMap();

                }
        });

        addBtnListener(btn_close, () =>
        {
            //所有弟子变为未上阵
            for(int i=0;i< curStudentPosList.Count; i++)
            {
                ulong onlyId = curStudentPosList[i];
                if (onlyId > 0)
                {
                   StudentManager.Instance.FindStudent(onlyId).studentStatusType = (int)StudentStatusType.None;
                }
            }
            PanelManager.Instance.ClosePanel(this);
        });

        for (int i = 0; i < posBtnList.Count; i++)
        {
            int index = i;
            Button btn = posBtnList[i];
            addBtnListener(btn, () =>
            {
                ulong onlyId = curStudentPosList[index];

                if (onlyId > 0)
                {
                    PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                    PrepareExplore(p, false);
                    
                }

            });
        }

 

    }

    public void PrepareExplore(PeopleData p,bool up)
    {

         if (up)
        {
            if (p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                PanelManager.Instance.OpenFloatWindow(p.name + "已上阵，请先下阵");
                return;
            }
            if (p.studentStatusType == (int)StudentStatusType.DanFarmWork
                || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
            {
                SingleDanFarmData studentFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];

                DanFarmSetting studentFarmSetting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);


                if (studentFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
               && p.studentStatusType == (int)DanFarmStatusType.Working)
                {
                    DanFarmSetting setting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);
                    PanelManager.Instance.OpenFloatWindow(p.name + "正在" + setting.Name + "工作，请等待工作完成");
                    return;

                }
                else
                {
                    PanelManager.Instance.OpenCommonHint("正在" + studentFarmSetting.Name + "驻守，是否取消其驻守?", () =>
                    {
                        LianDanManager.Instance.StopZuoZhen(p.onlyId);

                    }, null);
                    return;
                }
            }
            if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {
                PanelManager.Instance.OpenFloatWindow(p.name + "正在" + LanguageUtil.GetLanguageText((int)LanguageIdType.秘境) + "中，无法出战");
     
                return;
            }
            if (p.seriousInjury)
            {
                 return;
            }

             EventCenter.Broadcast(TheEventType.StudentPrepareExplore, p);
        }
        else
        {
             EventCenter.Broadcast(TheEventType.StudentPrepareExplore, p);
        }
        OnSuccessUpOrDownStudent(p);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_chooseStudent.gameObject.SetActive(false);

        AddSingle<WithoutKuangConsumeView>(trans_tiliGrid, (int)PropertyIdType.Tili, ConstantVal.miJingExploreNeedTiLi, ConsumeType.Property);
         
        ClearCertainParentAllSingle<ShowTipsItemView>(grid_possibleAward);
        ExploreMapSetting exploreMapSetting = DataTable.FindExploreMapSetting(curExploreId);
        List<int> itemIdList = CommonUtil.SplitCfgOneDepth(exploreMapSetting.PossibleAward);
        for(int i = 0; i < itemIdList.Count; i++)
        {
            int settingId = itemIdList[i];
            ItemSetting setting = DataTable.FindItemSetting(settingId);
            if (setting == null)
            {
                Debug.LogWarning($"[GotoExplorePreparePanel] 物品配置不存在，settingId={settingId}，已替换为灵石");
                settingId = 10001;
            }
            ItemData data = new ItemData();
            data.count = 1;
            data.settingId = settingId;
            AddSingle<ShowTipsItemView>(grid_possibleAward, data);
        }
    }

    /// <summary>
    /// 真正进入地图
    /// </summary>
    void ReallyEnterMap()
    {
        bool noStudent = true;
        for(int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong theId = curStudentPosList[i];
            if (theId > 0)
            {
                noStudent = false;
                break;
            }
        }
        if (noStudent)
        {
            PanelManager.Instance.OpenFloatWindow("您至少选择一名"+ LanguageUtil.GetLanguageText((int)LanguageIdType.弟子) +"进行探险");
            return;
        }
        else
        {
            List<ulong> studentList = new List<ulong>();
            for(int i = 0; i < curStudentPosList.Count; i++)
            {
                ulong theId = curStudentPosList[i];
                if (theId != 0)
                    studentList.Add(theId);
            }

            if (!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, ConstantVal.miJingExploreNeedTiLi))
            {
                PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
            }
            else
            {

                RoleManager.Instance.DeProperty(PropertyIdType.Tili, -ConstantVal.miJingExploreNeedTiLi);

                Debug.Log("进入探险！");
                MapManager.Instance.EnterNewExplore(curExploreId, studentList);
            }


        }
        //MapManager.Instance.EnterFixedMap(curChoosedMapView.singleMapData.MapId);

    }


    void ShowAllCandidateStudent()
    {
        PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);
        candidateList.Clear();
        List<PeopleData> candidateShowPList = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.LianGong
                && p.studentStatusType != (int)StudentStatusType.AtExplore
                &&!curStudentPosList.Contains(p.onlyId))
            {
                candidateShowPList.Add(p);

            }
        }

        for (int i = 0; i < candidateShowPList.Count - 1; i++)
        {
            for (int j = 0; j < candidateShowPList.Count - 1 - i; j++)
            {
                //后面的战力高 则二者交换
                if (RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j + 1]) > RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j]))
                {
                    PeopleData temp = candidateShowPList[j];
                    candidateShowPList[j] = candidateShowPList[j + 1];
                    candidateShowPList[j + 1] = temp;

                }
            }
        }
        for (int i = 0; i < candidateShowPList.Count; i++)
        {
            ExplorePrepareStudentView view = PanelManager.Instance.OpenSingle<ExplorePrepareStudentView>(trans_chooseStudentGrid, candidateShowPList[i], this);
            candidateList.Add(view);
        }

    }


    /// <summary>
    /// 已上阵学生数量
    /// </summary>
    /// <returns></returns>
    int UpStudentCount()
    {
        int upNum = 0;
        for (int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong onlyId = curStudentPosList[i];
            if (onlyId != 0)
                upNum++;
        }
        return upNum;
    }

    //上阵弟子
    public void OnUpStudent(ExplorePrepareStudentView view)
    {
        PeopleData p = view.p;
        int posIndex = -1;
        for (int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong theOnlyId = curStudentPosList[i];
            if (theOnlyId == 0)
            {
                posIndex = i;
                break;
            }
        }
        if (posIndex == -1)
        {
            PanelManager.Instance.OpenFloatWindow("最多上阵" + curStudentPosList.Count + "个");
            return;
        }
         PrepareExplore(view.p, true);
    }
    /// <summary>
    /// 弟子上下阵
    /// </summary>
    /// <param name="view"></param>
    public void OnSuccessUpOrDownStudent(PeopleData p)
    {
        //已上阵 要下阵
        if (curStudentPosList.Contains(p.onlyId))
        {
            int index = curStudentPosList.IndexOf(p.onlyId);
            curStudentPosList[index] = 0;
            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent[index]);
        }
        else
        {
            for (int i = 0; i < curStudentPosList.Count; i++)
            {
                if (curStudentPosList[i] == 0)
                {
                    curStudentPosList[i] = p.onlyId;
                    SingleStudentView view = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_UpStudentParent[i], p);
                    view.showTips = true;
                    break;
                }
            }
        }
 
        ShowAllCandidateStudent();
    }



    ///// <summary>
    ///// 选择地图
    ///// </summary>
    //public void ChooseMap(SingleChooseExploreView view)
    //{
    //    curChoosedMapView = view;
    //    MapManager.Instance.curChoosedExploreId = view.singleExploreData.SettingId;
    //    for (int i = 0; i < allMapViewList.Count; i++)
    //    {
    //        SingleChooseExploreView theView = allMapViewList[i];
    //        if (theView.singleExploreData.SettingId == view.singleExploreData.SettingId)
    //        {
    //            theView.OnChoose(true);
    //        }
    //        else
    //        {
    //            theView.OnChoose(false);
    //        }
    //    }
    //}



    public override void Clear()
    {
        base.Clear();
    
        curStudentPosList.Clear();
        for (int i = 0; i < 4; i++)
        {
            curStudentPosList.Add(0);
        }
        for (int i = 0; i < trans_UpStudentParent.Count; i++)
        {
            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent[i]);
        }
        PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);

        ClearCertainParentAllSingle<WithoutKuangConsumeView>(trans_tiliGrid);
        //PanelManager.Instance.CloseAllSingle(trans_mapGrid);
        //allMapViewList.Clear();

    }
    
    ///// <summary>
    ///// 关闭退出
    ///// </summary>
    //void CloseThePanel()
    //{

    //}
}
