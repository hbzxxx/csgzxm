 using DG.Tweening;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class TrainPanel : PanelBase
{

    #region 顶端描述
    public Text txt_TopJingJieDes;
    public Text txt_TopRankDes;
    public Text txt_TopZongMenLevelDes;
    #endregion
    #region  人物属性界面
    public GameObject obj_redPoint;
    public Text txt_uid;
     public Image img_headIcon;
    public Portrait portrait;
    public Transform trans_train;//训练面板

    public Text txt_curXiuWei;//当前修为
    public Image img_trainBar;

    public Text txt_bigPhaseName;//大境界
    public Text txt_centerPhaseName;//中境界
    public Text txt_smallPhaseName;//小境界

    public Text txt_pName;//名字
    public Button btn_changeZhangMenName;//改掌门名
    public Text txt_zongMenName;//宗门名
    public Button btn_changeZongMenName;//改宗门名
    public Image img_mpBar;//蓝条
    public Transform trans_proGrid;
    public Button btn_detailProperty;//详细属性
    public Button btn_openAudio;//开启音效
    public Button btn_closeAudio;//关闭音效
    public Button btn_lunTan;//论坛
    public Button btn_addQQ;//一键加群
    public Button btn_clearRubbish;//清理垃圾

    public Button btn_xueMai;//血脉
    #endregion


    TrainSetting curTrainSetting;



    //public 
    ulong xiuweiNeed;//需要多少修为
    int trainIndex;//当前训练等级




    public Transform trans_xiuLianPanel;//修炼界面


    //#region 突破成功界面
    //public Transform trans_breakThroughProChangePanel;//突破成功属性改变面板
    //public Transform trans_breakThroughProChangeGrid;//突破成功属性改变格子
    //public Transform trans_newXueMaiUpgrade;//血脉上限增加
    //public Transform trans_newYuanSuStudy;//新的元素可以学习
    //public Button btn_closeBreakThroughResPanel;//关闭突破结算面板
    //#endregion

    public AudioClipType beforeAudioBGMType;//之前是什么音效

    public Text txt_roleId;

    public Button btn_rank;//排行榜

    public Button btn_touXiang;
    public Button btn_touXiangKuang;
    public Button btn_duiHuanMa;//兑换码

    public override void Init(params object[] args)
    {
        base.Init(args);
        //if (args.Length > 0)
        //    singleMapEventData = args[0] as SingleMapEventData;
        //else
        //    singleMapEventData = null;
        trainIndex = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex;

        curTrainSetting = DataTable._trainList[trainIndex];
        //addBtnListener(btn_breakThrough, OnBreakThrough);
        //RegisterEvent(TheEventType.SuccessBreakThrough, OnSuccessBreakThrough);
        //RegisterEvent(TheEventType.UsedBreakDan, ShowXiuLianPanel);
        //RegisterEvent(TheEventType.FailBreakThrough, OnFailBreakThrough);
        RegisterEvent(TheEventType.ChangeZhangMenName, ShowName);
        RegisterEvent(TheEventType.ChangeZongMenName, ShowName);
        //RegisterEvent(TheEventType.RefreshZhangMenRedPoint, RefreshRedPoint);

        //addBtnListener(btn_closeBreakThroughResPanel, () =>
        // {
        //     PanelManager.Instance.CloseAllSingle(trans_breakThroughProChangeGrid);
        //     trans_breakThroughProChangePanel.gameObject.SetActive(false);


        //     if (RoleManager.Instance.CandidateYuanSuNum() > RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count)
        //     {
        //         //引导解锁元素
        //         if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.ChangeYuanSu).ToString()).ToInt32() == 0)
        //         {
        //             TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.ChangeYuanSu).ToString());
        //             PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.ChangeYuanSu));
        //         }
        //     }
        //     //这里可能出现引导去历练
        //     else if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex == 10)
        //     {
        //         if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString()).ToInt32() == 0)
        //         {
        //             PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.LiLian));
        //             TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString());
        //         }
        //     }
        // });

        //addBtnListener(btn_xiuLian, () =>
        //{
        //    ShowXiuLianPanel();
     
        //});
        //addBtnListener(btn_quitXiuLianPanel, () =>
        //{
        //    trans_xiuLianPanel.gameObject.SetActive(false);
        //    ShowPropertyPanel();
        //});


  

        addBtnListener(btn_changeZhangMenName, () =>
        {
            PanelManager.Instance.OpenPanel<ChangeNamePanel>(PanelManager.Instance.trans_layer2, ChangeNameType.ZhangMenName);
        });
        addBtnListener(btn_changeZongMenName, () =>
        {
            PanelManager.Instance.OpenPanel<ChangeNamePanel>(PanelManager.Instance.trans_layer2, ChangeNameType.ZongMenName);
        });
 
        addBtnListener(btn_openAudio, () =>
        {
            AuditionManager.Instance.OnMuteVoice(false);
            btn_openAudio.gameObject.SetActive(false);
            btn_closeAudio.gameObject.SetActive(true);
        });
        addBtnListener(btn_closeAudio, () =>
        {
            AuditionManager.Instance.OnMuteVoice(true);
            btn_openAudio.gameObject.SetActive(true);
            btn_closeAudio.gameObject.SetActive(false);
        });
        addBtnListener(btn_lunTan, () =>
        {
            Application.OpenURL("taptap://taptap.com/app?app_id=230065&source=outer");

        });

        addBtnListener(txt_TopJingJieDes.GetComponent<Button>(), () =>
        {
            TopDesPanel topDesPanel = PanelManager.Instance.OpenPanel<TopDesPanel>(PanelManager.Instance.trans_layer2);
            topDesPanel.ShowJingJie();

        });
        addBtnListener(txt_TopRankDes.GetComponent<Button>(), () =>
        {
            TopDesPanel topDesPanel = PanelManager.Instance.OpenPanel<TopDesPanel>(PanelManager.Instance.trans_layer2);
            topDesPanel.ShowRank();

        });
        addBtnListener(btn_addQQ, () =>
        {
            AddQQManager.Instance.OnJoinQQGroup();
        });
        addBtnListener(btn_clearRubbish, () => 
        {
            CommonUtil.DeleteAllFile(ConstantVal.GetDownLoadRubbishPath());
            CommonUtil.DeleteAllFile(ConstantVal.GetDownLoadRubbishPath2());

            PanelManager.Instance.OpenFloatWindow("清理成功");
        });

        //addBtnListener(btn_xueMai, () =>
        //{
        //    PanelManager.Instance.OpenPanel<XueMaiPanel>(PanelManager.Instance.trans_layer2, RoleManager.Instance._CurGameInfo.playerPeople);
        //});
        addBtnListener(btn_detailProperty, () =>
        {
            PanelManager.Instance.OpenPanel<DetailPropertyPanel>(PanelManager.Instance.trans_layer2, RoleManager.Instance._CurGameInfo.playerPeople);
        });

        addBtnListener(txt_uid.GetComponent<Button>(), () =>
         {
             OnCopyUID();
         });

        addBtnListener(btn_rank, () =>
         {
             if (Game.Instance.isLogin)
                 PanelManager.Instance.OpenPanel<RoleRangePanel>(PanelManager.Instance.trans_layer2);
             else
                 PanelManager.Instance.OpenFloatWindow("未连接到服务器，请重启游戏登录后查看");
         });

        addBtnListener(btn_touXiang, () =>
         {
             PanelManager.Instance.OpenPanel<TouXiangChangePanel>(PanelManager.Instance.trans_layer2);
         });
        addBtnListener(btn_duiHuanMa, () =>
         {
             PanelManager.Instance.OpenPanel<DuiHuanMaPanel>(PanelManager.Instance.trans_layer2);
         });
        RegisterEvent(TheEventType.ChangeTouXiang, OnChangeTouXiang);
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_uid.SetText(RoleManager.Instance._CurGameInfo.TheGuid);
        txt_roleId.SetText(RoleManager.Instance._CurGameInfo.roleId.ToString());
        //ShowPropertyPanel();
        //trans_xiuWeiDanParent.gameObject.SetActive(false);
        //trans_breakThroughProChangePanel.gameObject.SetActive(false);
        //trans_xiuLianPanel.gameObject.SetActive(false);
        beforeAudioBGMType = AuditionManager.Instance.curAudioClipType;
        AuditionManager.Instance.PlayBGM(AudioClipType.Train);

        //音效已关闭
        if (AuditionManager.Instance.muteAudio)
        {
            btn_openAudio.gameObject.SetActive(true);
            btn_closeAudio.gameObject.SetActive(false);
        }
        else
        {
            btn_openAudio.gameObject.SetActive(false);
            btn_closeAudio.gameObject.SetActive(true);
        }
        ShowTopDes();

        RefreshPortraitShow();

        ShowName();
    }
    void RefreshPortraitShow()
    {
        //掌门有头像
        //if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang)
        //    && !string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0]))
        //{

        //    //string touXiangPath = RoleManager.Instance.TouXiangPath(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0].ToInt32());
        //    //img_headIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + touXiangPath);
        //}
        //else
        //{
        //    //portrait.gameObject.SetActive(true);
        //    //img_headIcon.gameObject.SetActive(false);
        //    //portrait.Refresh(RoleManager.Instance._CurGameInfo.playerPeople);

        //}
        portrait.gameObject.SetActive(false);
        StudentManager.Instance.SetTouxiang(img_headIcon, RoleManager.Instance._CurGameInfo.playerPeople);
    }
    /// <summary>
    /// 更换头像
    /// </summary>
    void OnChangeTouXiang()
    {
        RefreshPortraitShow();
    }
    //void RefreshRedPoint()
    //{
    //    RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_ZhangMen_XueMai, 0);
    //}

    void OnCopyUID()
    {
        TextEditor te = new TextEditor();
        te.text = txt_uid.text;
        te.SelectAll();
        te.Copy();
        PanelManager.Instance.OpenFloatWindow("已复制到剪贴板");
    }

    /// <summary>
    /// 顶端描述
    /// </summary>
    void ShowTopDes()
    {
         txt_TopRankDes.SetText(ConstantVal.MatchRankName(RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel));
        txt_TopZongMenLevelDes.SetText(RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName + "·Lv" + RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel);
    }
    public void ShowName()
    {

        txt_pName.SetText(RoleManager.Instance._CurGameInfo.playerPeople.name);
        txt_zongMenName.SetText(RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName);
    }
    ///// <summary>
    ///// 显示属性界面
    ///// </summary>
    //void ShowPropertyPanel()
    //{
    //    int lastXiuWeiNeed = 0;

    //    if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex >= 1)
    //    {
    //        TrainSetting lastTrainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex - 1];
    //        lastXiuWeiNeed = lastTrainSetting.xiuWeiNeed.ToInt32();
    //    }
    //    img_trainBar.fillAmount = (RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei - (ulong)lastXiuWeiNeed)
    /// (float)(curTrainSetting.xiuWeiNeed.ToInt32() - lastXiuWeiNeed);
    //    txt_curXiuWei.SetText(RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei + "/" + curTrainSetting.xiuWeiNeed);

    //    txt_bigPhaseName.SetText(curTrainSetting.bigPhaseName);
    //    txt_centerPhaseName.SetText(curTrainSetting.centerPhaseName);
    //    txt_smallPhaseName.SetText(curTrainSetting.smallPhaseName);

    //    ShowName();
    //     SinglePropertyData singlePropertyData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, RoleManager.Instance._CurGameInfo.playerPeople);
    //    img_mpBar.fillAmount = singlePropertyData.num / (float)singlePropertyData.limit;

    //    PanelManager.Instance.CloseAllSingle(trans_proGrid);
    //    List<SinglePropertyData> proList = RoleManager.Instance.GetShowProList(RoleManager.Instance._CurGameInfo.playerPeople);
    //    AddSingle<ZhanDouLiPropertyView>(trans_proGrid, RoleManager.Instance._CurGameInfo.playerPeople);
    //    for (int i = 0; i < proList.Count; i++)
    //    {
    //        SinglePropertyData thePro = proList[i];
    //        int theId = thePro.id;
    //        int theNum = thePro.num;
    //        if (thePro.id != (int)PropertyIdType.MpNum)
    //        {
    //            SinglePropertyView proView = PanelManager.Instance.OpenSingle<SinglePropertyView>(trans_proGrid, theId, theNum, (Quality)(int)thePro.quality);

    //        }
    //    }
    //}

    ///// <summary>
    ///// 失败
    ///// </summary>
    //void OnFailBreakThrough(object[] args)
    //{
    //    int offset = (int)args[0];
    //    int rateAdd = (int)args[1];
    //    AddSingle<TuPoShiBaiSingle>(trans_effectParent, offset, rateAdd);

    //    ShowXiuLianPanel();
    //}



 

 


    ////显示文字动画
    //IEnumerator ShowTxtAddAnim(List<SinglePropertyData> proAdd ,long before,long after)
    //{
    //    //飘字
    //    for (int i = 0; i < proAdd.Count; i++)
    //    {
    //        PropertySetting setting = DataTable.FindPropertySetting(proAdd[i].Id);
    //        string showNum = RoleManager.Instance.GetPropertyShow(proAdd[i].Id, proAdd[i].Num);
    //        AddSingle<FadeTxtAnimView>(trans_proAddFlyTxtParent, setting.name + "+" + showNum);
    //        yield return new WaitForSeconds(.2f);
    //    }
    //    yield return new WaitForSeconds(1f);

    //    PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);
    //}

 


  
    ///// <summary>
    ///// 成功突破
    ///// </summary>
    //void OnSuccessBreakThrough(object[] args)
    //{
    //    //AddSingle<levelupp>(trans_effectParent);
    //    int curIndex = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex;
    //    curTrainSetting = DataTable._trainList[curIndex];
    //    List<SinglePropertyData> proAdd = args[0] as List<SinglePropertyData>;
    //    long before = (long)args[1];
    //    long after = (long)args[2];
    //    //StartCoroutine(ShowTxtAddAnim(proAdd, before,after));
    //    //中境界突破成功 等级-1为trainindex
    //    if (curIndex % 10 == 0)
    //    {
    //        PanelManager.Instance.CloseAllSingle(trans_breakThroughProChangeGrid);

    //        Debug.Log("中境界突破成功");
    //        //大境界突破成功
    //        if (curIndex % 30 == 0)
    //        {
    //            Debug.Log("大境界突破成功");

    //        }


    //        List<int> proIdList = new List<int>();
    //        for(int i = 0; i < proAdd.Count; i++)
    //        {
    //            SinglePropertyData pro = proAdd[i];
    //            int id = pro.Id;
    //            int num = pro.Num;

    //            int curNum= RoleManager.Instance.FindMyProperty(id).num;
    //            int beforeNum = curNum - num;

    //            string curNumShow = RoleManager.Instance.GetPropertyShow(id, curNum);
    //            string beforeNumShow = RoleManager.Instance.GetPropertyShow(id, beforeNum);
    //            PanelManager.Instance.OpenSingle<PropertyCompareView>(trans_breakThroughProChangeGrid, id, beforeNumShow, curNumShow);
    //        }

    //        trans_breakThroughProChangePanel.gameObject.SetActive(true);
    //        int beforeTrainIndex = curIndex - 1;
    //        if(XueMaiManager.Instance.limitLevel(beforeTrainIndex)< XueMaiManager.Instance.limitLevel(curIndex))
    //        {
    //            trans_newXueMaiUpgrade.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            trans_newXueMaiUpgrade.gameObject.SetActive(false);
    //        }
    //        if(RoleManager.Instance.CandidateYuanSuNum(beforeTrainIndex)< RoleManager.Instance.CandidateYuanSuNum(curIndex))
    //        {
    //            trans_newYuanSuStudy.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            trans_newYuanSuStudy.gameObject.SetActive(false);
    //        }
    //    }
    //    //ShowXiuLianPanel();
    //    ShowTopDes();
    //}
    private void Update()
    {
        //if (startBreakThroughXiAnim)
        //{
        //    breakThroughXiAnimTimer += Time.deltaTime;
        //    if (breakThroughXiAnimTimer >= breakThroughXiAnimTime)
        //    {
        //        startBreakThroughXiAnim = false;
        //        ReallyBreakThrough();
        //    }
        //}
    }
    ///// <summary>
    ///// 动画播放完了 突破
    ///// </summary>
    //void ReallyBreakThrough()
    //{
    //    btn_breakThrough.gameObject.SetActive(true);
    //    btn_quitXiuLianPanel.gameObject.SetActive(true);
    //    //当前等级小于上限 可以突破
    //    if (trainIndex < DataTable._trainList.Count - 1)
    //    {
  
             
    //        TrainSetting trainSetting = DataTable._trainList[trainIndex + 1];
    //        List<List<int>> proChange = CommonUtil.SplitCfg(trainSetting.proChange);

    //        List<SinglePropertyData> addedProList = new List<SinglePropertyData>();//加了哪些属性

    //        RoleManager.Instance.OnBreakThrough();
    //    }
    //    else
    //    {
    //        PanelManager.Instance.OpenFloatWindow("您已登峰造极");
    //    }

    //}
    ///// <summary>
    ///// 开始突破
    ///// </summary>
    //void OnBreakThrough()
    //{
    //    int trainIndex = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex;
    //    //当前等级小于上限 可以突破
    //    if (trainIndex < DataTable._trainList.Count - 1)
    //    {
    //        startBreakThroughXiAnim = true;
    //        breakThroughXiAnimTimer = 0;
    //        btn_breakThrough.gameObject.SetActive(false);
    //        btn_quitXiuLianPanel.gameObject.SetActive(false);
    //        AddSingle<dujie_xishou>(trans_effectParent,trans_endPoint.transform.localPosition);
 
    //    }
    //    else
    //    {
    //        PanelManager.Instance.OpenFloatWindow("您已登峰造极");
    //    }
    //}




    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_effectParent);
        //startBreakThroughXiAnim = false;
       // tranList.Clear();
       // PanelManager.Instance.CloseAllSingle(trans_trainGridParent);
       // singlePropertyViewList.Clear();
       // singlePropertyIdList.Clear();
       //  PanelManager.Instance.CloseAllSingle(trans_propertyGrid);
       
    }


    public override void OnClose()
    {
        base.OnClose();
        //PanelManager.Instance.CloseAllSingle(trans_proAddFlyTxtParent);
        //PanelManager.Instance.CloseAllSingle(trans_breakThroughProChangeGrid);
        //singleMapEventData = null;
        AuditionManager.Instance.PlayBGM(beforeAudioBGMType);
     
    }
}
