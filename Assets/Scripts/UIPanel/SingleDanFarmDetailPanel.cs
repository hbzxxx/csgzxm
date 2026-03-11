using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;
 
public class SingleDanFarmDetailPanel : PanelBase
{
    public SingleDanFarmData singleDanFarmData;
    public DanFarmSetting danFarmSetting;
    public Image img_productIcon;
    public Text txt_des;

    public Button btn_upgrade;
    public Transform trans_studentGrid;//弟子位
    public Text txt_curLevel;//当前等级
    public Text txt_curLevel2;//当前等级显示在红底
    public Text txt_efficient;//效率
    public Transform trans_efficientBar;//效率bar
    public Image img_efficientBar;//效率bar
    public Image img_farm;//建筑icon
    //public Transform grid_name;//建筑名
    public Image img_name;//名字
    public Text txt_name;//名字
    public Text txt_jiBie;//级别
    public Button btn_remove;//拆除丹房
    
    public Transform trans_sonParent;

    #region 炼丹炉
    public Transform trans_needMat;
    public Button btn_chooseProduct;
    //public Button btn_cancelProduct;//取消炼制
    //public Button btn_jiaSuLianDan;//源力加速
    //public Text txt_jiaSuLianDanNeedd;//源力加速需要
    public Transform trans_remainBar;
    public Image img_remain;
    //public Text txt_remain;
    //public Button btn_receive;

    public Transform trans_studentProGrid;//弟子属性
    #endregion
    //弟子位相关
    // public List<ulong> curStudentPosList = new List<ulong>();//学生位
    public List<Transform> trans_UpStudentParentList;//上阵学生的位置
    public List<Transform> trans_UpStudentLockList;//上阵弟子锁
    public List<Text> txt_upStudentPosTxtList;//上阵弟子解锁状态
    public Button btn_openChooseStudent;//打开学生选择界面
    public List<Button> btn_openChooseStudentList;//打开弟子选择界面List
    //public Transform trans_chooseStudent;//选择学生
    public Transform trans_chooseStudentGrid;//选择学生格子
    //public Button btn_closeChooseStudent;//关闭选择学生界面
    public List<FarmPrepareStudentView> farmPrepareStudentViewList = new List<FarmPrepareStudentView>();

    public ScrollViewNevigation scrollViewNevigation;


    #region 加速
    public Button btn_zone;
    public Transform trans_remainZoneTime;
    public Text txt_remainZoneTime;

    #endregion

    public Button btn_startProduct;//开始生产
    public Button btn_stopProduct;//停止生产
    public Text txt_needForeItem;//需要前置物品
    public Transform trans_needForeItemGrid;//前置物品是什么

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        addBtnListener(btn_upgrade, () =>
        {
            PanelManager.Instance.OpenPanel<DanFarmUpgradePanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);
        });

        addBtnListener(btn_chooseProduct, () =>
        {
            //if (singleDanFarmData.ProductItemList.Count > 0)
            //{
            //    PanelManager.Instance.OpenFloatWindow("您还有未收获的丹，请先收获");
            //    return;
            //}
            PanelManager.Instance.CloseAllPanel(trans_sonParent);
                PanelManager.Instance.OpenPanel<ChooseFarmProductMatPanel>(trans_sonParent, singleDanFarmData);
        });

        addBtnListener(btn_openChooseStudent, () =>
        {
            PanelManager.Instance.CloseAllPanel(trans_sonParent);

            ShowStudentChoose();
        });

        ////收获
        //addBtnListener(btn_receive, () =>
        // {
        //     List<int> settingIdList = new List<int>();
        //     List<ulong> numList = new List<ulong>();
        //     for(int i = 0; i < singleDanFarmData.ProductItemList.Count; i++)
        //     {
        //         settingIdList.Add(singleDanFarmData.ProductItemList[i].SettingId);
        //         numList.Add(singleDanFarmData.ProductItemList[i].Count);
        //     }
        //     ItemManager.Instance.GetItemWithAwardPanel(settingIdList, numList);
        //     singleDanFarmData.ProductItemList.Clear();
        //     ShowCurProduct();
        // });

        for(int i = 0; i < btn_openChooseStudentList.Count; i++)
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
        addBtnListener(btn_remove, () =>
        {
            if(singleDanFarmData.DanFarmWorkType==(int)DanFarmWorkType.NeedMat
            && singleDanFarmData.Status == (int)DanFarmStatusType.Working)
            {
                PanelManager.Instance.OpenFloatWindow("请等待生产完成");
                return;
            }
            PanelManager.Instance.OpenCommonHint("确定拆除吗？", () =>
            {
                PanelManager.Instance.ClosePanel(this);
                LianDanManager.Instance.OnRemoveADanFarm(singleDanFarmData.OnlyId);

            }, null);
        });

        addBtnListener(btn_zone, () =>
        {

            if (LianDanManager.Instance.JudgeIfCanQuanLi(singleDanFarmData))
            {
                int validEmptyCount = 0;
                int validIndex = -1;
                bool haveFreeStudent = false;
                for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
                {
                    if (singleDanFarmData.PosUnlockStatusList[i]
                     && singleDanFarmData.ZuoZhenStudentIdList[i] == 0)
                    {
                        validEmptyCount++;
                        validIndex = i;
                        break;
                    }
                }
                //判断是否有空闲弟子没坐进来
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

                    if (p.studentStatusType == (int)StudentStatusType.None)
                    {

                        haveFreeStudent = true;
                    }
                }
                if (validEmptyCount > 0
                && haveFreeStudent)
                {
                                      LianDanManager.Instance.OnQuanLiFarm(singleDanFarmData);

                }
                else
                {
                    LianDanManager.Instance.OnQuanLiFarm(singleDanFarmData);

    
                }


            }

        });
        addBtnListener(btn_stopProduct, () =>
        {
            LianDanManager.Instance.OnStopProduct(singleDanFarmData);
        });
        addBtnListener(btn_startProduct, () =>
        {
            LianDanManager.Instance.OnStartProduct(singleDanFarmData);
        });
        RegisterEvent(TheEventType.EndQuanLiDanFarm, OnStopQuanLiDanFarm);

        RegisterEvent(TheEventType.PropertyDecrease, QuanLiShow);
        RegisterEvent(TheEventType.PropertyAdd, QuanLiShow);

        RegisterEvent(TheEventType.UpgradeDanFarm, OnUpgradeDanFarmStart);

        RegisterEvent(TheEventType.OnZuoZhenStudent, OnSuccessUpOrDownStudent);
        RegisterEvent(TheEventType.StopZuoZhenStudent, OnSuccessUpOrDownStudent);
        RegisterEvent(TheEventType.StartMatDanFarmProduce, StartDanFarmProduce);
        RegisterEvent(TheEventType.DanFarmProduceProcess, OnDanfarmProcess);
        RegisterEvent(TheEventType.StopMatDanFarmProduce, OnStopProduceNeedMatItem);
        RegisterEvent(TheEventType.ChangeLianDanStatus, OnChangeStatus);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();



        //ClearCertainParentAllSingle<NameWordView>(grid_name);
        //List<char> wordList = new List<char>();
        //for(int i = 0; i < danFarmSetting.name.Length; i++)
        //{
        //    char word = danFarmSetting.name[i];
        //    wordList.Add(word);
        //}
        //if(singleDanFarmData.TalentType==(int)StudentTalent.CaiKuang
        //    ||singleDanFarmData.TalentType== (int)StudentTalent.ZaoHua
        //    || singleDanFarmData.TalentType == (int)StudentTalent.JingWen)
        //{
        //    txt_jiBie.gameObject.SetActive(true);
        //    string str = (wordList[0].ToString() + wordList[1].ToString());
        //    txt_jiBie.SetText(str);
        //    wordList.RemoveAt(0);
        //    wordList.RemoveAt(0);
        //}
        //else
        //{
        //    txt_jiBie.gameObject.SetActive(false);
             
        //}
        ////LianDanManager.Instance.farm
        //for (int i = 0; i < wordList.Count; i++)
        //{
        //    AddSingle<NameWordView>(grid_name, wordList[i]);
        //}

        txt_name.SetText(danFarmSetting.Name);
        //if(!string.IsNullOrWhiteSpace(danFarmSetting.rarity))
        //{
        //    txt_jiBie.SetText(ConstantVal.QualityName(danFarmSetting.rarity.ToInt32()));
        //}
        //else
        //{
        //    txt_jiBie.SetText("");
        //}

        img_farm.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.InsideUIName);
        img_farm.SetNativeSize();
        //trans_chooseStudent.gameObject.SetActive(false);

        List<int> unlockLevelList = LianDanManager.Instance.StudentPosUnlockLevelList(danFarmSetting);
        for (int i=0;i< txt_upStudentPosTxtList.Count; i++)
        {
            txt_upStudentPosTxtList[i].SetText(unlockLevelList[i] + "级解锁");
        }

        ShowProductInfo();

        ShowUpgradeStatus();
        ShowAllStudent();
        ShowStudentChoose();
        GuideChooseStudentZuoZhen();

        //炼丹炉有丹直接跳转界面
        if (singleDanFarmData.TalentType == (int)StudentTalent.LianJing)
        {
            if(singleDanFarmData.ProductItemList.Count>0)
            btn_chooseProduct.onClick.Invoke();
        }
        QuanLiShow();
    }


    /// <summary>
    /// 显示是否全力
    /// </summary>
    void QuanLiShow()
    {
        //如果不能全力
        if (!LianDanManager.Instance.JudgeIfCanQuanLi(singleDanFarmData))
        {
            btn_zone.GetComponent<Image>().material = PanelManager.Instance.mat_grey;
            //btn_zone.gameObject.SetActive(false);

        }
        //正在加速
        else if (singleDanFarmData.OpenQuanLi)
        {
            btn_zone.GetComponent<Image>().material = PanelManager.Instance.mat_grey;

            //btn_zone.gameObject.SetActive(false);
        }
        //可以加速但没加速
        else
        {
            if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
                btn_zone.gameObject.SetActive(true);
            btn_zone.GetComponent<Image>().material = null;

        }
        if (singleDanFarmData.DanFarmWorkType != (int)DanFarmWorkType.Common)
            btn_zone.gameObject.SetActive(false);
        QuanLiRemainTimeShow();

    }
    /// <summary>
    /// 全力剩余时间显示
    /// </summary>
    void QuanLiRemainTimeShow()
    {
        if (singleDanFarmData.OpenQuanLi)
        {
            int remainWeek = singleDanFarmData.QuanliRemainTime / 7;
            //trans_remainZoneTime.gameObject.SetActive(true);
            txt_remainZoneTime.SetText("剩余" + remainWeek + "周");
        }
        else
        {
           // trans_remainZoneTime.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 停止全力产丹
    /// </summary>
    /// <param name="args"></param>
    void OnStopQuanLiDanFarm(object[] args)
    {
        SingleDanFarmData theData = args[0] as SingleDanFarmData;
        if (theData.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = theData;

        }
        QuanLiShow();

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
                 //导航到第一个有天赋的弟子
                for(int i=0;i< farmPrepareStudentViewList.Count; i++)
                {
                    FarmPrepareStudentView view = farmPrepareStudentViewList[i];
                    if(view.p.talent== (int)TaskManager.Instance.chooseValidStudentZuoZhenTalent)
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
                        //StartCoroutine(yieldLocateScroll(view.btn_up.gameObject));
                        //scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                        //PanelManager.Instance.ShowTaskGuidePanel(view.btn_up.gameObject);
                        break;
                    }
                }
            }
            TaskManager.Instance.chooseValidStudentZuoZhen = false;
        }
        else if (TaskManager.Instance.danFarmUpgrade
       && TaskManager.Instance.danFarmUpgradeOnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);
        }
        else if (TaskManager.Instance.guide_lianDan
   && singleDanFarmData.SettingId==(int)DanFarmIdType.LianDanLu)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_chooseProduct.gameObject);
        }
    }

 
    private void Update()
    {
    }
    /// <summary>
    /// 显示产出基本信息
    /// </summary>
    void ShowProductInfo()
    {
        if (danFarmSetting.WorkType.ToInt32() == (int)DanFarmWorkType.Common)
        {
            txt_efficient.gameObject.SetActive(true);
            ClearCertainParentAllSingle<SingleViewBase>(trans_needForeItemGrid);
            trans_needMat.gameObject.SetActive(false);
            img_productIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + Framework.Data.DataTable.FindItemSetting(singleDanFarmData.ProductSettingId).UiName);
            
            if (singleDanFarmData.Status == (int)DanFarmStatusType.Idling)
            {
                txt_des.SetText("");
                txt_efficient.SetText("");
                img_efficientBar.fillAmount = 0;
            }
            else
            {
                txt_des.SetText("X" + LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData) + "/月");
                txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");
                img_efficientBar.fillAmount = (LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) / (LianDanManager.Instance.SingleFarmDataUnlockedStudentPosCount(singleDanFarmData) * 100f));
                trans_efficientBar.gameObject.SetActive(true);
            }


            if (singleDanFarmData.NeedForeItemId>0
                &&singleDanFarmData.Status == (int)DanFarmStatusType.Working)
            {
                btn_stopProduct.gameObject.SetActive(true);
                btn_startProduct.gameObject.SetActive(false);
                txt_needForeItem.SetText("");
                AddSingle<SingleConsumeView>(trans_needForeItemGrid, singleDanFarmData.NeedForeItemId, LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData), ConsumeType.Item);
            }
            else
            {
      
                if (singleDanFarmData.NeedForeItemId > 0)
                {
                    AddSingle<SingleConsumeView>(trans_needForeItemGrid, singleDanFarmData.NeedForeItemId, LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData), ConsumeType.Item);

                    btn_stopProduct.gameObject.SetActive(false);
                    btn_startProduct.gameObject.SetActive(true);
                    //我主动停的
                    if (!singleDanFarmData.HandleStop)
                    {
                        ItemSetting needSetting = DataTable.FindItemSetting(singleDanFarmData.NeedForeItemId);
                        txt_needForeItem.SetText(needSetting.Name + "不足,暂停生产");
                    }
                    //不是我主动停的
                    else
                    {
                        txt_needForeItem.SetText("");
                    }
                }
                else
                {
                    btn_stopProduct.gameObject.SetActive(false);
                    btn_startProduct.gameObject.SetActive(false);
                }
            }
        }
        else
        {

            trans_needMat.gameObject.SetActive(true);

            txt_efficient.gameObject.SetActive(false);
            trans_efficientBar.gameObject.SetActive(false);
            ShowCurProduct();
        }
    }
    
    /// <summary>
    /// 显示升级信息
    /// </summary>
    public void ShowUpgradeStatus()
    {
        txt_curLevel.SetText("Lv"+singleDanFarmData.CurLevel);
        txt_curLevel2.SetText(singleDanFarmData.CurLevel.ToString());
    }

    void OnUpgradeDanFarmStart(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;

        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ClosePanel(this);
        }
    }

    #region 弟子相关
    /// <summary>
    /// 显示所有弟子
    /// </summary>
    public void ShowAllStudent()
    {
        for(int i = 0; i < singleDanFarmData.PosUnlockStatusList.Count; i++)
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



        List<PeopleData> pList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        int index = 0;
        for(int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
        {
            ulong theId = singleDanFarmData.ZuoZhenStudentIdList[i];
            if (theId > 0)
            {
                SingleStudentView view= AddSingle<SingleStudentView>(trans_UpStudentParentList[i], pList[index]);
                //view.obj_nameBg.gameObject.SetActive(false);
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
    }


    /// <summary>
    /// 显示弟子选择
    /// </summary>
    void ShowStudentChoose()
    {
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
        farmPrepareStudentViewList.Clear();
        //trans_chooseStudent.gameObject.SetActive(true);
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
                    else if (showStudentList[j].propertyList != null && showStudentList[j].propertyList.Count > 0 
                        && showStudentList[j + 1].propertyList != null && showStudentList[j + 1].propertyList.Count > 0
                        && showStudentList[j].propertyList[0].num <= showStudentList[j + 1].propertyList[0].num)
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
        for(int i=0;i< singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            if (singleDanFarmData.PosUnlockStatusList[i])
            {
                unlockedCount++;
            }
        }
        int zuoZhenCount = 0;
        for(int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
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
                SingleDanFarmData studentFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);//  RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];
                DanFarmSetting studentFarmSetting = DataTable.FindDanFarmSetting(studentFarmData.SettingId);
                DanFarmSetting curSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
                if (studentFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
                    && p.studentStatusType == (int)DanFarmStatusType.Working) 
                { 
                    PanelManager.Instance.OpenCommonHint("正在" + studentFarmSetting.Name + "工作，请等待工作完成。", null, null);
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
            else if(p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                PanelManager.Instance.OpenCommonHint("已上阵，请先下阵。", null, null);
            }
            else if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {
                PanelManager.Instance.OpenFloatWindow("正在"+LanguageUtil.GetLanguageText((int)LanguageIdType.秘境));
                return;
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
            for(int i = 0; i < farmPrepareStudentViewList.Count; i++)
            {
                farmPrepareStudentViewList[i].RefreshShow();
            }
            ShowProductInfo();
            ShowAllStudent();
            ShowStudentChoose();
        }
    }

    #endregion


    void OnDanfarmProcess(object[] args)
    {
        SingleDanFarmData farmData = args[0] as SingleDanFarmData;
        if (farmData.OnlyId == singleDanFarmData.OnlyId)
        {
            ShowCurProduct();
        }
    }
    /// <summary>
    /// 停止产出
    /// </summary>
    /// <param name="args"></param>
    void OnStopProduceNeedMatItem(object[] args)
    {
        SingleDanFarmData farmData = args[0] as SingleDanFarmData;
        if (farmData.OnlyId == singleDanFarmData.OnlyId)
        {
            ShowCurProduct();
        }
    }
    /// <summary>
    /// 显示当前产出 (需要材料的物品
    /// </summary>
    void ShowCurProduct()
    {
        if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.NeedMat)
        {
            btn_chooseProduct.gameObject.SetActive(true);

            //正在产丹
            if (singleDanFarmData.Status == (int)DanFarmStatusType.Working)
            {
                //防止报错 正常不会走到这里 但不知道为什么有玩家走到这里
                if (singleDanFarmData.ProductSettingId == 0)
                {
                    singleDanFarmData.Status = (int)DanFarmStatusType.Idling;
                    ShowCurProduct();
                }
                else
                {
                    img_productIcon.gameObject.SetActive(true);

                    txt_des.SetText((singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) + "/" + singleDanFarmData.ProductTotalNum);
                    //btn_cancelProduct.gameObject.SetActive(true);
            
                    //txt_jiaSuLianDanNeedd.SetText("-" + LianDanManager.Instance.JiaSuLianDanNeedTili(singleDanFarmData));
                    //btn_jiaSuLianDan.gameObject.SetActive(true);
                    img_productIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + Framework.Data.DataTable.FindItemSetting(singleDanFarmData.ProductSettingId).UiName);
                     trans_remainBar.gameObject.SetActive(true);
                    img_remain.gameObject.SetActive(true);
                    img_remain.fillAmount = (singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) / (float)singleDanFarmData.ProductTotalNum;
                   // btn_receive.gameObject.SetActive(false);
                }

     
            }
            else
            {
                if (singleDanFarmData.ProductItemList.Count <= 0)
                {
                    //btn_receive.gameObject.SetActive(false);
                    trans_remainBar.gameObject.SetActive(false);
                    txt_des.SetText("当前空闲");
                    img_productIcon.gameObject.SetActive(false);

                }
                else
                {
                    //btn_receive.gameObject.SetActive(true);
                    btn_chooseProduct.gameObject.SetActive(true);
                    trans_remainBar.gameObject.SetActive(true);
                    if ((float)singleDanFarmData.ProductTotalNum == 0)
                        img_remain.fillAmount = 1;
                    else
                    {
                        img_remain.fillAmount = (singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) / (float)singleDanFarmData.ProductTotalNum;

                    }
                    txt_des.SetText((singleDanFarmData.ProductTotalNum - singleDanFarmData.ProductRemainNum) + "/" + singleDanFarmData.ProductTotalNum);

                    img_productIcon.gameObject.SetActive(true);

                }
                //btn_cancelProduct.gameObject.SetActive(false);
                //btn_jiaSuLianDan.gameObject.SetActive(false);
            }
        }
       
    }

    /// <summary>
    /// 开始产出需要材料的产品
    /// </summary>
    /// <param name="args"></param>
    public void StartDanFarmProduce(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            ShowCurProduct();
        }
    }


    void OnChangeStatus(object[] args)
    {
        SingleDanFarmData theData = args[0] as SingleDanFarmData;
        if (theData == singleDanFarmData)
        {
            ShowProductInfo();
        }
    }

    public override void Clear()
    {
        base.Clear();
        for (int i = 0; i < trans_UpStudentParentList.Count; i++)
        {
            Transform trans = trans_UpStudentParentList[i];
            PanelManager.Instance.CloseAllSingle(trans);
        }
        ClearCertainParentAllSingle<SingleViewBase>(trans_needForeItemGrid);
    }

    public override void OnClose()
    {
        base.OnClose();
        TaskManager.Instance.chooseValidStudentZuoZhen = false;
        TaskManager.Instance.danFarmUpgrade = false;
        PanelManager.Instance.CloseTaskGuidePanel();
        PanelManager.Instance.CloseAllPanel(trans_sonParent);
    }
}
