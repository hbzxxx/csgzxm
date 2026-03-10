 using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMakeBuildingPanel : PanelBase
{
    public Image img_farm;

    public Transform trans_curMakingEquip;//当前正在做的装备
    public Transform trans_curMakingEquipGrid;//当前正在做的装备父物体
    public Image img_process;//进度
    public Text txt_process;

    public Transform trans_sonPanelParent;//子界面

    //public Transform trans_studentGrid;//炼器师
    //public List<SingleStudentView> studentViewList = new List<SingleStudentView>();//所有学生

    public Button btn_studentPageTag;//弟子页面
    public Button btn_makeEquipTag;//铸器
    //public Button btn_IntenseEquipTag;//炼器
    //public Button btn_dissolveTag;//分解tag

    public SingleDanFarmData singleDanFarmData;
    DanFarmSetting danFarmSetting;
    public Text txt_curLevel;//当前等级
    public Text txt_curLevel2;//当前等级

    public Button btn_upgrade;//升级

    public Transform trans_studentPage;//弟子页面
  
    //弟子位相关
    public Transform trans_studentProGrid;
    //弟子属性影响
    public Transform trans_studentInfluenceGrid;
    // public List<ulong> curStudentPosList = new List<ulong>();//学生位
    public List<Transform> trans_UpStudentParentList;//上阵学生的位置
    public List<Transform> trans_UpStudentLockList;//上阵弟子锁
    public List<Button> btn_openChooseStudentList;//打开弟子选择界面List
    //public Transform trans_chooseStudent;//选择学生
    public Transform trans_chooseStudentGrid;//选择学生格子
    //public Button btn_closeChooseStudent;//关闭选择学生界面
    public List<FarmPrepareStudentView> farmPrepareStudentViewList = new List<FarmPrepareStudentView>();

    public ScrollViewNevigation scrollViewNevigation;

    public Button btn_remove;//拆除

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        //addBtnListener(btn_equipMake, () =>
        //{
        //    if (RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData != null)
        //    {
        //        PanelManager.Instance.OpenFloatWindow("正在炼制装备，请等待炼制完成");
        //        return;
        //    }
        //    PanelManager.Instance.OpenPanel<EquipMakePanel>(PanelManager.Instance.trans_layer2);
        //});

        addBtnListener(btn_makeEquipTag, OnShowEquipMake);
        //addBtnListener(btn_IntenseEquipTag, OnShowEquipIntense);
        //addBtnListener(btn_dissolveTag, OnShowEquipDissolve);

        addBtnListener(btn_upgrade, () =>
        {
            DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
            List<int> totalLevelList = CommonUtil.SplitCfgOneDepth(danFarmSetting.UpgradeRent);
            if (singleDanFarmData.CurLevel < totalLevelList.Count)
            {
                PanelManager.Instance.OpenPanel<DanFarmUpgradePanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);

            }
            else
            {
                PanelManager.Instance.OpenFloatWindow("当前已达最大等级");
            }

            //PanelManager.Instance.OpenPanel<DanFarmUpgradePanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);
         });

        for (int i = 0; i < btn_openChooseStudentList.Count; i++)
        {
            Button btn = btn_openChooseStudentList[i];
            int index = i;

            addBtnListener(btn, () =>
            {
                bool unlock = singleDanFarmData.PosUnlockStatusList[index];
                ulong onlyId = singleDanFarmData.ZuoZhenStudentIdList[index];
                if (unlock)
                {
                    if (onlyId > 0)
                    {
                        LianDanManager.Instance.StopZuoZhen(onlyId);

                    }
                }
            });

        }
        addBtnListener(btn_studentPageTag, () =>
        {
            OnShowStudentPage();
        });


        addBtnListener(btn_remove, () =>
        {
            if (RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData != null)
            {
                PanelManager.Instance.OpenFloatWindow("请等待制造完成");
                return;
            }
            PanelManager.Instance.OpenCommonHint("确定拆除吗？", () =>
            {
                PanelManager.Instance.ClosePanel(this);
                LianDanManager.Instance.OnRemoveADanFarm(singleDanFarmData.OnlyId);

            }, null);
        });
        RegisterEvent(TheEventType.OnZuoZhenStudent, OnSuccessUpOrDownStudent);
        RegisterEvent(TheEventType.StopZuoZhenStudent, OnSuccessUpOrDownStudent);
        //EventCenter.Register(TheEventType.StartMakeEquip, CloseThePanel);
        RegisterEvent(TheEventType.UpgradeDanFarm, OnUpgradeDanFarmStart);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        img_farm.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.UiName);
        img_farm.SetNativeSize();
        //显示当前正在做的装备和进度 
        if (RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData != null)
        {
            btn_makeEquipTag.onClick.Invoke();
        }
        //如果当前没有做装备
        else
        {
            btn_studentPageTag.onClick.Invoke();
        }
        ShowAllStudent();
        ShowUpgradeStatus();
        GuideChooseStudentZuoZhen();
        AuditionManager.Instance.PlayVoice(AudioClipType.TieJiangPu);
    }
    void OnUpgradeDanFarmStart(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;

        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ClosePanel(this);
        }
    }

    public void GuideChooseStudentZuoZhen()
    {
        //放弟子
        if (TaskManager.Instance.chooseValidStudentZuoZhen
            && TaskManager.Instance.chooseValidStudentZuoZhenFarmOnlyId==singleDanFarmData.OnlyId)
        {
            if (TaskManager.Instance.chooseValidStudentZuoZhenTalent != StudentTalent.None)
            {
                string talentName = StudentManager.Instance.TalentNameByTalent(TaskManager.Instance.chooseValidStudentZuoZhenTalent);
                 //导航到第一个有天赋的弟子
                for (int i = 0; i < farmPrepareStudentViewList.Count; i++)
                {
                    FarmPrepareStudentView view = farmPrepareStudentViewList[i];
                    if (view.p.talent == (int)TaskManager.Instance.chooseValidStudentZuoZhenTalent)
                    {
                        PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn_up.gameObject);

                        break;
                    }
                }
            }
            //导航到空闲弟子
            else
            {
 
                for (int i = 0; i < farmPrepareStudentViewList.Count; i++)
                {
                    FarmPrepareStudentView view = farmPrepareStudentViewList[i];
                    if (view.p.studentStatusType == (int)StudentStatusType.None)
                    {
                        PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn_up.gameObject);

                        break;
                    }
                }
            }
            TaskManager.Instance.chooseValidStudentZuoZhen = false;
        }
        //升级
       else if (TaskManager.Instance.danFarmUpgrade
            && TaskManager.Instance.danFarmUpgradeOnlyId == singleDanFarmData.OnlyId)
        {       
            PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);      
         }
        //造装备
        else if (TaskManager.Instance.guide_makeEquip)
        {
            if (RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData == null)
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_makeEquipTag.gameObject);
            }
        }
        ////强化装备
        //else if (TaskManager.Instance.guide_intenseEquip)
        //{
        //    if (RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData == null)
        //    {
        //        PanelManager.Instance.ShowTaskGuidePanel(btn_IntenseEquipTag.gameObject);
        //    }
        //}
    }

    /// <summary>
    /// 显示建筑升级状况
    /// </summary>
    void ShowUpgradeStatus()
    {
        txt_curLevel.SetText("Lv" + singleDanFarmData.CurLevel);
        txt_curLevel2.SetText(singleDanFarmData.CurLevel.ToString());
    }

    /// <summary>
    /// 显示弟子页面
    /// </summary>
    void OnShowStudentPage()
    {
        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);
        ShowStudentChoose();
    }

    /// <summary>
    /// 显示铸器
    /// </summary>
    void OnShowEquipMake()
    {
        //如果正在修器 则不能点
        PanelManager.Instance.CloseTaskGuidePanel();

        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);

        btn_makeEquipTag.GetComponent<Image>().color = ConstantVal.color_choosed;
        //btn_IntenseEquipTag.GetComponent<Image>().color = Color.white;
        //btn_dissolveTag.GetComponent<Image>().color = Color.white;

        //显示正在炼制的装备
        PanelManager.Instance.OpenPanel<EquipMakePanel>(trans_sonPanelParent,singleDanFarmData);


    }
 
 

    #region 弟子相关
    /// <summary>
    /// 显示所有学生
    /// </summary>
    public void ShowAllStudent()
    {
        for (int i = 0; i < singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            bool unlock = singleDanFarmData.PosUnlockStatusList[i];
            if (unlock)
            {
                trans_UpStudentLockList[i].gameObject.SetActive(false);
            }
            else
            {
                trans_UpStudentLockList[i].gameObject.SetActive(true);
            }
            Transform trans = trans_UpStudentParentList[i];
            ClearCertainParentAllSingle<SingleStudentView>(trans);
        }
        //炼器师
        List<SingleStudentView> studentViewList = new List<SingleStudentView>();

        List<PeopleData> pList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        int index = 0;
        for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
        {
            ulong theId = singleDanFarmData.ZuoZhenStudentIdList[i];
            if (theId > 0)
            {
                SingleStudentView view= AddSingle<SingleStudentView>(trans_UpStudentParentList[i], pList[index]);
                view.obj_nameBg.gameObject.SetActive(false);
                index++;
            }
        }

        ClearCertainParentAllSingle<SinglePropertyView>(trans_studentProGrid);
        List<int> proIdList = new List<int>();
        List<int> proNumList = new List<int>();
        for(int i = 0; i < pList.Count; i++)
        {
            PeopleData p = pList[i];
            if (p.talent != singleDanFarmData.TalentType)
                continue;
            for(int j = 0; j < p.propertyIdList.Count; j++)
            {
                int theId = p.propertyIdList[j];
                int theNum = p.propertyList[j].num;
                if (!proIdList.Contains(theId))
                {
                    proIdList.Add(theId);
                    proNumList.Add(0);
                }
                int theindex = proIdList.IndexOf(theId);
                proNumList[theindex] += theNum;
            }
        }

        for(int i=0; i < proNumList.Count; i++)
        {
            BigStudentSinglePropertyView singlePropertyView = AddSingle<BigStudentSinglePropertyView>(trans_studentProGrid, proIdList[i], proNumList[i], Quality.None);
        }
        //弟子属性加成

        int proNum = 0;
        List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            if (p.talent == (int)StudentTalent.DuanZhao)
            {
                for (int j = 0; j < p.propertyIdList.Count; j++)
                {
                    int theId = p.propertyIdList[j];
                    if (theId == (int)PropertyIdType.ShiWu)
                    {
                        proNum += p.propertyList[j].num;
                    }
                }
            }

        }
        ClearCertainParentAllSingle<SingleStudentInfluenceRateView>(trans_studentInfluenceGrid);
        CommonUtil.ShowStudentInfulenceRateByTotalPro(trans_studentInfluenceGrid, proNum);
    }

    /// <summary>
    /// 显示弟子选择
    /// </summary>
    void ShowStudentChoose()
    {
        farmPrepareStudentViewList.Clear();
        trans_studentPage.gameObject.SetActive(true);
        ClearCertainParentAllSingle<FarmPrepareStudentView>(trans_chooseStudentGrid);
        List<PeopleData> showStudentList = new List<PeopleData>();

        //排序筛选 
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            //在这个房间的不会显示
            if (singleDanFarmData.ZuoZhenStudentIdList.Contains(p.onlyId))
            {
                continue;
            }

            showStudentList.Add(p);

        }
        for (int i = 0; i < showStudentList.Count - 1; i++)
        {
            for (int j = 0; j < showStudentList.Count - 1 - i; j++)
            {
                //后面的未坐镇，前面的坐镇则二者交换
                if (showStudentList[j + 1].studentStatusType != (int)StudentStatusType.DanFarmQuanLi
                    && showStudentList[j + 1].studentStatusType != (int)StudentStatusType.DanFarmWork
                   && showStudentList[j + 1].studentStatusType != (int)StudentStatusType.DanFarmRelax
                   && (showStudentList[j].studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                   || showStudentList[j].studentStatusType == (int)StudentStatusType.DanFarmWork
                   || showStudentList[j].studentStatusType == (int)StudentStatusType.DanFarmRelax)
                   )
                {
                    PeopleData temp = showStudentList[j];
                    showStudentList[j] = showStudentList[j + 1];
                    showStudentList[j + 1] = temp;

                }
            }
        }
        for (int i = 0; i < showStudentList.Count - 1; i++)
        {
            for (int j = 0; j < showStudentList.Count - 1 - i; j++)
            {
                //后面的有天赋，则二者交换
                if (showStudentList[j + 1].talent == singleDanFarmData.TalentType)
                {
                    //如果前面的没天赋或者后面的天赋更高 则二者交换
                    bool exchange = false;
                    if (showStudentList[j].talent != singleDanFarmData.TalentType)
                    {
                        exchange = true;
                    }
                    else if (showStudentList[j].propertyList[0].num <= showStudentList[j + 1].propertyList[0].num)
                    {
                        exchange = true;

                    }
                    if (exchange)
                    {
                        PeopleData temp = showStudentList[j];
                        showStudentList[j] = showStudentList[j + 1];
                        showStudentList[j + 1] = temp;
                    }


                }
            }
        }


        for (int i = 0; i < showStudentList.Count; i++)
        {
            PeopleData p = showStudentList[i];
            FarmPrepareStudentView view = AddSingle<FarmPrepareStudentView>(trans_chooseStudentGrid, p, this);
            farmPrepareStudentViewList.Add(view);
        }
    }

    /// <summary>
    /// 弟子驻守
    /// </summary>
    public void OnUpStudent(FarmPrepareStudentView farmPrepareStudentView)
    {
        int choosedPosIndex = 0;
        int unlockedCount = 0;
        for (int i = 0; i < singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            if (singleDanFarmData.PosUnlockStatusList[i])
            {
                unlockedCount++;
            }
        }
        int zuoZhenCount = 0;
        for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
        {
            if (singleDanFarmData.ZuoZhenStudentIdList[i] > 0)
                zuoZhenCount++;
        }
        if (zuoZhenCount >= unlockedCount)
        {
            PanelManager.Instance.OpenFloatWindow("无空位");
            return;
        }
        else
        {
            for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
            {
                if (singleDanFarmData.ZuoZhenStudentIdList[i] == 0)
                {
                    choosedPosIndex = i;
                    break;
                }
            }
         }

        if (!singleDanFarmData.ZuoZhenStudentIdList.Contains(farmPrepareStudentView.p.onlyId))
        {
            PeopleData p = farmPrepareStudentView.p;

            //如果正在工作
            if (p.studentStatusType == (int)StudentStatusType.DanFarmWork
                || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                 || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
            {
                SingleDanFarmData studentFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];
                DanFarmSetting studentFarmSetting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);
                DanFarmSetting curSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
                if (studentFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
                    && p.studentStatusType == (int)DanFarmStatusType.Working)
                {
                    PanelManager.Instance.OpenCommonHint( "正在" + studentFarmSetting.Name + "工作，请等待工作完成。", null, null);
                }
                else
                {
                    PanelManager.Instance.OpenCommonHint("正在" + studentFarmSetting.Name + "驻守，是否将它换到这个" + curSetting.Name + "?", () =>
                    {
                        LianDanManager.Instance.StopZuoZhen(p.onlyId);
                        LianDanManager.Instance.ZuoZhen(p, singleDanFarmData, choosedPosIndex);
                        EventCenter.Broadcast(TheEventType.CloseZuoZhenStudentChoosePanel);

                    }, () =>
                    {

                    });
                }

            }
            //是否在秘境探险
            else if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {
                PanelManager.Instance.OpenCommonHint("正在" + LanguageUtil.GetLanguageText((int)LanguageIdType.秘境) + "探险，请等待探险完成。", null, null);
            }   
            //是否在上阵
            else if (p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                PanelManager.Instance.OpenCommonHint("已上阵，请先下阵。", null, null);
            }
            else
            {
                LianDanManager.Instance.ZuoZhen(p, singleDanFarmData, choosedPosIndex);
            }

        }

    }

    void OnSuccessUpOrDownStudent(object[] args)
    {
        SingleDanFarmData farm = args[0] as SingleDanFarmData;
        if (farm.OnlyId == singleDanFarmData.OnlyId)
        {
            for (int i = 0; i < farmPrepareStudentViewList.Count; i++)
            {
                farmPrepareStudentViewList[i].RefreshShow();
            }
            ShowAllStudent();
            ShowStudentChoose();

        }
    }

    #endregion
    public void CloseThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }

    public override void Clear()
    {
        base.Clear();
        //EventCenter.Remove(TheEventType.StartMakeEquip, CloseThePanel);
        PanelManager.Instance.CloseAllSingle(trans_curMakingEquipGrid);
    }

    public override void OnClose()
    {
        base.OnClose();
        if (trans_sonPanelParent.childCount > 0)
        {
           PanelManager.Instance.ClosePanel(trans_sonPanelParent.GetChild(0).GetComponent<PanelBase>());
        }
        TaskManager.Instance.chooseValidStudentZuoZhen = false;
        TaskManager.Instance.danFarmUpgrade = false;
        TaskManager.Instance.guide_makeEquip = false;
        TaskManager.Instance.guide_intenseEquip = false;
        PanelManager.Instance.CloseTaskGuidePanel();
    }


}
