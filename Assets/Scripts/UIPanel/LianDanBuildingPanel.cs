 using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LianDanBuildingPanel : PanelBase
{
    //public Transform grid_name;//建筑名
    public Text txt_name;
    public Text txt_curLevel;//当前等级

    public Button btn_remove;//拆
    public Image img_farm;
    DanFarmSetting danFarmSetting;

    public Button btn_upgrade;//升级
    public Button btn_studentPage;
    public Button btn_gemPage;

    public Transform trans_studentInfluenceGrid;//弟子属性影响

    #region 宝石
    public Transform trans_gemPage;//页面

    public Button btn_startMakeGem;//开始炼制
    public int curChoosedGemPictureItemId;//当前选的宝石id
    public List<Text> gemQualityRateTxtList;//宝石质量
    public SingleDanFarmData singleDanFarmData;
    public List<GemPictureView> gemPictureViewList = new List<GemPictureView>();
    public Transform trans_pictureGrid;//图纸页面
    public List<Button> gemTagBtnList;
    public List<Image> gemTagBtnBottomImgList;

    public Button btn_makeTag;//做宝石页面
    public Button btn_compositeTag;//合成宝石页面

    public Transform trans_makeGemParent;//做宝石
    public Transform trans_sonParent;//合成宝石

    #endregion

    #region 弟子页面

    public Transform trans_studentPage;//弟子页面

    //弟子位相关
    public Transform trans_studentProGrid;

    // public List<ulong> curStudentPosList = new List<ulong>();//学生位
    public List<Transform> trans_UpStudentParentList;//上阵学生的位置
    public List<Transform> trans_UpStudentLockList;//上阵弟子锁
    public List<Button> btn_openChooseStudentList;//打开弟子选择界面List
    //public Transform trans_chooseStudent;//选择学生
    public Transform trans_chooseStudentGrid;//选择学生格子
    //public Button btn_closeChooseStudent;//关闭选择学生界面
    public List<FarmPrepareStudentView> farmPrepareStudentViewList = new List<FarmPrepareStudentView>();
    #endregion

    public ScrollViewNevigation scrollViewNevigation;
    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        EventCenter.Register(TheEventType.LianDanProcess, OnShowLianDanProcess);
        EventCenter.Register(TheEventType.HarvestDan, OnShowLianDanProcess);

        //RegisterEvent(TheEventType.LianDanMoneyFull, OnShowMoneyDanFull);

        RegisterEvent(TheEventType.OnZuoZhenStudent, OnSuccessUpOrDownStudent);
        RegisterEvent(TheEventType.StopZuoZhenStudent, OnSuccessUpOrDownStudent);
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

        addBtnListener(btn_startMakeGem, () =>
        {
            LianDanManager.Instance.StartMakeGem(curChoosedGemPictureItemId, singleDanFarmData);
            PanelManager.Instance.CloseTaskGuidePanel();
            TaskManager.Instance.guide_makeGem = false;
        });

        addBtnListener(btn_studentPage, () =>
         {
             ShowStudentPage();
         });
        addBtnListener(btn_gemPage, () =>
        {
            ShowGemPage();
        });

        for(int i=0;i< gemTagBtnList.Count; i++)
        {
            int index = i;
            Button btn = gemTagBtnList[i];
            addBtnListener(btn, () =>
             {
                 OnGemTagClicked(index);
             });
        }
        addBtnListener(btn_remove, () =>
        {
         
            PanelManager.Instance.OpenCommonHint("确定拆除吗？", () =>
            {
                PanelManager.Instance.ClosePanel(this);
                LianDanManager.Instance.OnRemoveADanFarm(singleDanFarmData.OnlyId);

            }, null);
        });
        RegisterEvent(TheEventType.UpgradeDanFarm, OnUpgradeDanFarmStart);

        addBtnListener(btn_makeTag, OnMakeGemTagClicked);
        addBtnListener(btn_compositeTag, OnCompositeTagClicked);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        //ClearCertainParentAllSingle<NameWordView>(grid_name);
        //for (int i = 0; i < danFarmSetting.name.Length; i++)
        //{
        //    char word = danFarmSetting.name[i];
        //    AddSingle<NameWordView>(grid_name, word);
        //}
        txt_name.SetText(danFarmSetting.Name);
        ShowUpgradeStatus();

        img_farm.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.UiName);
        img_farm.SetNativeSize();
        btn_studentPage.onClick.Invoke();
        GuideChooseStudentZuoZhen();
        ShowGuide();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_makeGem)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_makeTag.gameObject);
        }
    }

    void OnUpgradeDanFarmStart(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;

        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ClosePanel(this);
        }
    }
    /// <summary>
    /// 显示建筑升级状况
    /// </summary>
    void ShowUpgradeStatus()
    {
        txt_curLevel.SetText("Lv" + singleDanFarmData.CurLevel);

    }

    public void GuideChooseStudentZuoZhen()
    {
        //放弟子
        if (TaskManager.Instance.chooseValidStudentZuoZhen
            && TaskManager.Instance.chooseValidStudentZuoZhenFarmOnlyId == singleDanFarmData.OnlyId)
        {
            if (TaskManager.Instance.chooseValidStudentZuoZhenTalent != StudentTalent.None)
            {
                string talentName = StudentManager.Instance.TalentNameByTalent(TaskManager.Instance.chooseValidStudentZuoZhenTalent);
                PanelManager.Instance.OpenFloatWindow("请点击具有" + talentName + "天赋的弟子，使其驻守");
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
                PanelManager.Instance.OpenFloatWindow("请点击任意弟子，使其驻守");

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
        }        //升级
        else if (TaskManager.Instance.danFarmUpgrade
             && TaskManager.Instance.danFarmUpgradeOnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);
        }

    }

    /// <summary>
    /// 显示宝石页面
    /// </summary>
    void ShowGemPage()
    {
        trans_gemPage.gameObject.SetActive(true);
        trans_studentPage.gameObject.SetActive(false);
        //ShowGemPanel();
        //btn_makeTag.onClick.Invoke();
    }

    /// <summary>
    /// 做宝石
    /// </summary>
    void OnMakeGemTagClicked()
    {
        ShowGemPage();

        trans_makeGemParent.gameObject.SetActive(true);
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
        OnGemTagClicked(0);

    }

    /// <summary>
    /// 点击合成
    /// </summary>
    void OnCompositeTagClicked()
    {
        ShowGemPage();

        trans_makeGemParent.gameObject.SetActive(false);
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
        PanelManager.Instance.OpenPanel<GemCompositePanel>(trans_sonParent,singleDanFarmData);
    }
    /// <summary>
    /// 显示弟子页面
    /// </summary>
    void ShowStudentPage()
    {
        trans_gemPage.gameObject.SetActive(false);

        trans_studentPage.gameObject.SetActive(true);
        ShowAllStudent();
        ShowStudentChoose();

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
                AddSingle<SingleStudentView>(trans_UpStudentParentList[i], pList[index]);
                index++;
            }
        }

        ClearCertainParentAllSingle<SinglePropertyView>(trans_studentProGrid);
        List<int> proIdList = new List<int>();
        List<int> proNumList = new List<int>();
        for (int i = 0; i < pList.Count; i++)
        {
            PeopleData p = pList[i];
            if (p.talent != singleDanFarmData.TalentType)
                continue;
            for (int j = 0; j < p.propertyIdList.Count; j++)
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

        for (int i = 0; i < proNumList.Count; i++)
        {
            BigStudentSinglePropertyView singlePropertyView = AddSingle<BigStudentSinglePropertyView>(trans_studentProGrid, proIdList[i], proNumList[i], Quality.None);
        }

        int proNum = 0;
        List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            
                for (int j = 0; j < p.propertyIdList.Count; j++)
                {
                    int theId = p.propertyIdList[j];
                    if (theId == (int)PropertyIdType.LianShi)
                    {
                        proNum += p.propertyList[j].num;
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
                    PanelManager.Instance.OpenCommonHint("该"+LanguageUtil.GetLanguageText((int)LanguageIdType.弟子)+"正在" + studentFarmSetting.Name + "工作，请等待工作完成", null, null);
                }
                else
                {
                    PanelManager.Instance.OpenCommonHint("该"+LanguageUtil.GetLanguageText((int)LanguageIdType.弟子)+"正在" + studentFarmSetting.Name + "驻守，是否将它换到这个" + curSetting.Name + "?", () =>
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
                PanelManager.Instance.OpenCommonHint("正在" + LanguageUtil.GetLanguageText((int)LanguageIdType.秘境) + "，请等待探险完成", null, null);
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

    ///// <summary>
    ///// 显示宝石面板
    ///// </summary>
    //public void ShowGemPanel()
    //{



    //    ////ClearCertainParentAllSingle<SingleConsumeView>(trans_gemConsumeGrid);
    //    ////品质
    //    //int proVal = LianDanManager.Instance.GetTotalLianShiProNum();
    //    //List<int> weightList = LianDanManager.Instance.GetGemQualityWeightList(proVal);
    //    //gemQualityRateTxtList[0].SetText("白色  " + weightList[0] + "%");
    //    //gemQualityRateTxtList[1].SetText("绿色  " + weightList[1] + "%");
    //    //gemQualityRateTxtList[2].SetText("蓝色  " + weightList[2] + "%");
    //    //gemQualityRateTxtList[3].SetText("紫色  " + weightList[3] + "%");
    //    //gemQualityRateTxtList[4].SetText("金色  " + weightList[4] + "%");


    //}

    /// <summary>
    /// 某个标签点击了
    /// </summary>
    void OnGemTagClicked(int index)
    {
        for(int i = 0; i < gemTagBtnBottomImgList.Count; i++)
        {
            Image img = gemTagBtnBottomImgList[i];
            if (i == index)
            {
                img.gameObject.SetActive(true);
            }
            else
            {
                img.gameObject.SetActive(false);
            }
        }

        ClearCertainParentAllSingle<GemPictureView>(trans_pictureGrid);
        List<int> settingIdList = unlockedGemPictureId((Rarity)(index + 1));
        gemPictureViewList.Clear();
        for (int i = 0; i < settingIdList.Count; i++)
        {
            ItemData item = new ItemData();
            item.settingId = settingIdList[i];
            GemPictureView view =AddSingle<GemPictureView>(trans_pictureGrid, item, this);
            gemPictureViewList.Add(view);
        }
        if (gemPictureViewList.Count > 0)
        {
            gemPictureViewList[0].btn.onClick.Invoke();
            btn_startMakeGem.gameObject.SetActive(true);
        }
        else
        {
            curChoosedGemPictureItemId = 0;
            btn_startMakeGem.gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 某个等级解锁了的宝石图纸
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public List<int> unlockedGemPictureId(Rarity rarity)
    {
        RoleManager.Instance._CurGameInfo.LianDanData.MaxGemRarity = 5;
        // if (RoleManager.Instance._CurGameInfo.LianDanData == null)
        // {
        //     Debug.LogWarning("[LianDanBuildingPanel] LianDanData 为空，请检查数据初始化");
        //     return new List<int>();
        // }
        if ((int)rarity <= RoleManager.Instance._CurGameInfo.LianDanData.MaxGemRarity)
        {
            return DataTable.FindRarityPoSuiGem(rarity);
        }
        return new List<int>();
    }
    /// <summary>
    /// 显示选择的宝石
    /// </summary>
    public void ShowChoosedGem(int settingId)
    {
        curChoosedGemPictureItemId = settingId;
       
        for (int i=0;i< gemPictureViewList.Count; i++)
        {
            GemPictureView view = gemPictureViewList[i];
            if (view.GetItemData().settingId == settingId)
                view.OnChoosed(true);
            else
                view.OnChoosed(false);
        }

        if (TaskManager.Instance.guide_makeGem)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_startMakeGem.gameObject);
        }
        //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_gemConsumeGrid, (int)ItemIdType.LingShi, num, ConsumeType.Item);
    }

    IEnumerator yieldShowGuide()
    {
        yield return null;
        //SingleNPCData curGuideNPCData = FindNPCByOnlyId(curGuideNPCOnlyId);
        //if (curGuideNPCData == null)
        //    return;
        //SingleTaskProtoData curTaskData = FindTaskById(curGuideNPCData, curGuideNPCData.CurTaskId);
        //if (curTaskData == null)
        //    return;
        TaskManager.Instance.ShowGuide();

    }


    /// <summary>
    /// 显示炼丹进度
    /// </summary>
    public void OnShowLianDanProcess()
    {
        //PanelManager.Instance.CloseAllSingle(trans_danGrid);
        //ShowMoneyDan();
    }





    public override void Clear()
    {
        base.Clear();
        curChoosedGemPictureItemId = 0;
        PanelManager.Instance.CloseAllSingle(trans_studentProGrid);
        PanelManager.Instance.CloseAllPanel(trans_sonParent);

        EventCenter.Remove(TheEventType.LianDanProcess, OnShowLianDanProcess);
        EventCenter.Remove(TheEventType.HarvestDan, OnShowLianDanProcess);
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.chooseValidStudentZuoZhen = false;
        TaskManager.Instance.danFarmUpgrade = false;
        TaskManager.Instance.guide_makeGem = false;


    }
}

/// <summary>
/// 状态
/// </summary>
public enum DanFarmStatusType
{
    None=0,
    Working=1,
    Building=2,//建造中
    Upgrading = 3,//升级中
    Idling=4,//待机中
}