using DG.Tweening;
using Framework.Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class LiLianPanel : PanelBase
{
    #region 弟子上阵

    public List<Image> haoGanProcessImgList;//好感度进度图
    public List<ulong> curStudentPosList = new List<ulong>();//学生位
    public List<GameObject> studentPosBgList;//背景
    public List<Transform> trans_UpStudentParent;//上阵学生的位置
    public List<Button> posBtnList;//位置按钮

    public List<LiLianPrepareStudentView> candidateList = new List<LiLianPrepareStudentView>();

    public Button btn_openChooseStudent;//打开学生选择界面

    public Transform trans_chooseStudent;//选择学生
    public Transform trans_chooseStudentGrid;//选择学生格子
    public Button btn_closeChooseStudent;//关闭选择学生界面

    public Button btn_start;//开始历练

    public Transform trans_relationShip;
    public Image img_relationShipBar;
    public Image img_haoGanDuIcon;//好感度icon
    public Text txt_relationShipLabel;

    public Button btn_haoGanDes;//好感描述
    #endregion

    public Transform trans_liLianProcess;//历练进程
    public List<SkeletonGraphic> xiaoRenSkeList;//小人
    public SkeletonGraphic bgSke;//背景spine
    public int liLianId;//历练id
    public LiLianSetting liLianSetting;
    public int totalMoveIndex;//总段数
    public int curMoveIndex;//当前走到第几段了
    public Image img_process;//进程
    public Transform trans_flyItemAnimParent;//飞物品
    SingleLiLianData liLianData;//历练数据
    public Transform trans_eventGrid;//事件

    public bool startYieldShowFlyAnim = false;
    float yieldShowFlyAnimTime = 1f;//延迟飞
    float yieldShowFlyAnimTimer = 0;//延迟飞
    int yieldHaoGanAdd = 0;

    #region 操作模块
    public PressBtnView btn_fly;
    public Rigidbody2D rb1;
    public Rigidbody2D rb2;
    public float vAdd;
    public float yanChiTime;
    public bool startFlyUltraAward;//开始飞额外物品
    public float flyUltraAwardTimer = 0;
    public float flyUltraAwardTime;//多久生成一个
    public Transform trans_ultraAwardUpPos;//上部
    public Transform trans_ultraAwardDownPos;//下部
    public Transform trans_ultraAwardParent;//额外物品父物体

    List<ItemData> possibleUltraAwardItemList;//额外物品可能的
    List<int> possibleUltraAwardWeightList;//额外可能物品的权重
    #endregion

    #region 结算
    public Transform trans_jieSuan;//结算
    public Transform trans_jieSuanRelationShip;//结算关系
    //public Text txt_jieSuanHaoGan;//结算好感
    public Text txt_jieSuanHaoGanAddNum;//结算好感增加数量
    public Transform trans_jieSuanHaoGanAddNumDownPos;
    public Transform trans_jieSuanHaoGanAddNumUpPos;
    public List<Image> jieSuanhaoGanProcessImgList;//好感度进度图

    public Image img_jieSuanRelationShipImgBar;//结算好感度
    public Transform trans_jieSuanPGrid;//结算人物格子
    public Transform trans_jieSuanItemGrid;//结算物品格子
    public Button btn_closeJieSuan;//退出
    public int handleEventCurIndex = 0;
    public int handleEventTotalIndex = 0;//事件下标
    public List<ItemData> curHandleEventItemList;//当前事件的物品
    public int curHandleEventHaoGanAdd;//当前事件的好感

     public Button btn_ADJieSuan;//广告结算

    #region 结算奖励相关
 

    #endregion

    #endregion


    public override void Init(params object[] args)
    {
        base.Init(args);
        liLianId = (int)args[0];
        liLianSetting = DataTable.FindLiLianSetting(liLianId);

        List<List<int>> ultraAward = CommonUtil.SplitCfg(ConstantVal.liLianUltraPossibleItem);
        possibleUltraAwardItemList = new List<ItemData>();
        possibleUltraAwardWeightList = new List<int>();
        for (int i = 0; i < ultraAward.Count; i++)
        {
            List<int> single = ultraAward[i];
            ItemData data = new ItemData();
            data.settingId = single[0];
            data.count = (ulong)single[1];
            possibleUltraAwardWeightList.Add(single[2]);
            possibleUltraAwardItemList.Add(data);
        }


        addBtnListener(btn_start, () =>
         {
             OnLiLianStart();
         });
        for(int i = 0; i < posBtnList.Count; i++)
        {
            int index = i;
            Button btn = posBtnList[i];
            addBtnListener(btn, () =>
            {
                OnDownStudent(index);
            });
        }
        addBtnListener(btn_closeJieSuan, () =>
        {
            LiLianManager.Instance.OnGetAward(false);
            PanelManager.Instance.ClosePanel(this);

            //第一次结算要引导到升级点
            if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.QiangHuaXueMai).ToString()).ToInt32() == 0)
            {
                PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.QiangHuaXueMai));
                TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.QiangHuaXueMai).ToString());

            }
        });

        addBtnListener(btn_ADJieSuan, () =>
        {
            PanelManager.Instance.ClosePanel(this);

            ADManager.Instance.WatchAD(ADType.JieSuanLiLian);
            //第一次结算要引导到升级点
            if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.QiangHuaXueMai).ToString()).ToInt32() == 0)
            {
                PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.QiangHuaXueMai));
                TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.QiangHuaXueMai).ToString());

            }
        });
        btn_fly.callBack = OnFly;

        addBtnListener(btn_haoGanDes, () =>
        {
            PanelManager.Instance.OpenPanel<HaoGanDesPanel>(PanelManager.Instance.trans_layer2);
        });
     }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_liLianProcess.gameObject.SetActive(false);
        trans_chooseStudent.gameObject.SetActive(true);
        trans_jieSuan.gameObject.SetActive(false);
        ShowAllCandidateStudent();
        RefreshRelationShipShow();
        
    }


    /// <summary>
    /// 飞
    /// </summary>
    void OnFly()
    {
        rb1.AddForce(new Vector2(0, vAdd));
        //rb1.velocity += new Vector2(0, vAdd);
    }


    /// <summary>
    /// 历练开始
    /// </summary>
    public void OnLiLianStart()
    {
        flyUltraAwardTimer = 0;
        startFlyUltraAward = true;
        bool haveStudent = false;
        List<PeopleData> pList = new List<PeopleData>();

        for (int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong onlyId = curStudentPosList[i];
            if (onlyId > 0)
            {
                pList.Add(StudentManager.Instance.FindStudent(onlyId));
                haveStudent = true;
            }
        }
        if (pList.Count == 0)
        {
            PanelManager.Instance.OpenFloatWindow("您至少选择一个"+LanguageUtil.GetLanguageText((int)LanguageIdType.弟子)+"进行历练");
            return;
        }
        else if (pList.Count == 1)
        {
             ReallyGo(pList);


        }
        else
        {
            ReallyGo(pList);
        }

    }

    void ReallyGo(List<PeopleData> pList)
    {
        if (!GameTimeManager.Instance.connectedToFuWuQiTime)
        {
            PanelManager.Instance.OpenFloatWindow("该功能必须在联网情况下进行");
            return;
        }

        // 使用新的矿洞挖掘玩法
        if (LiLianManager.Instance.useMineMode)
        {
            // 不关闭历练面板，直接打开挖矿面板
            if (LiLianManager.Instance.isLiLian(liLianId))
            {
                PanelManager.Instance.OpenPanel<MinePanel>(PanelManager.Instance.trans_layer2, pList, liLianId);
                return;
            }
        }

        // 旧的历练玩法
        bgSke.skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.liLianFolderPath + liLianSetting.Bg);
        bgSke.Initialize(true);
        bgSke.AnimationState.SetAnimation(0, "animation", true);

        liLianData = LiLianManager.Instance.OnGenerate(liLianId, pList);
        if (pList[0].gender == (int)Gender.Male)
        {
            xiaoRenSkeList[0].skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + ConstantVal.liLianMaleSke);
        }
        else
        {
            xiaoRenSkeList[0].skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + ConstantVal.liLianFemaleSke);

        }
        //只有一个人
        if (pList.Count == 1)
        {
            xiaoRenSkeList[1].gameObject.SetActive(false);
        }
        else
        {
            xiaoRenSkeList[1].gameObject.SetActive(true);
            if (pList[1].gender == (int)Gender.Male)
            {
                xiaoRenSkeList[1].skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + ConstantVal.liLianMaleSke);
            }
            else
            {
                xiaoRenSkeList[1].skeletonDataAsset = ResourceManager.Instance.GetObj<SkeletonDataAsset>(ConstantVal.battlePeoplePath + ConstantVal.liLianFemaleSke);

            }
        }
        for(int i = 0; i < pList.Count; i++)
        {
            xiaoRenSkeList[i].Initialize(true);
            xiaoRenSkeList[i].AnimationState.SetAnimation(0, "feixing", true);
        }
       
        trans_chooseStudent.gameObject.SetActive(false);
        trans_liLianProcess.gameObject.SetActive(true);
        curMoveIndex = 1;
        totalMoveIndex = liLianData.eventStrList.Count;
        img_process.DOKill();
        img_process.fillAmount = 0;
        TweenMoving();
        TaskManager.Instance.GetDailyAchievement(TaskType.LiLian, "1");
    }

    void TweenMoving()
    {
        if (curMoveIndex > totalMoveIndex)
        {
            //结算面板
            JieSuan();
        }
        else
        {
            for (int i = 0; i < xiaoRenSkeList.Count; i++)
            {
                if(xiaoRenSkeList[i].gameObject.activeInHierarchy)
                xiaoRenSkeList[i].AnimationState.SetAnimation(0, "feixing", true);
            }
            int moveTime = RandomManager.Next(8, 10);
            img_process.DOFillAmount(curMoveIndex / (float)totalMoveIndex, moveTime).OnComplete(() =>
            {
                handleEventCurIndex = 0;
                handleEventTotalIndex = 0;
                curMoveIndex++;

                if (LiLianManager.Instance.curParticipatedPList.Count == 2)
                {
                    AddSingle<SingleLiLianDesView>(trans_eventGrid, LiLianDesStr(liLianData.eventStrList[curMoveIndex - 2]));
                }

                curHandleEventItemList = new List<ItemData>();
                 ItemData data = liLianData.itemList[curMoveIndex-2];
                if (data != null)
                {
                    curHandleEventItemList.Add(data);
                     handleEventTotalIndex++;
                }
                int haoGanAdd = liLianData.haoGanList[curMoveIndex - 2];
                if (haoGanAdd > 0)
                {
                    curHandleEventHaoGanAdd = haoGanAdd;
                    handleEventTotalIndex++;
                }

                HandleEvent();
            });
        }

    }
    string LiLianDesStr(string content)
    {
        if (LiLianManager.Instance.curParticipatedPList.Count == 2)
        {
            content = content.Replace("[a]", LiLianManager.Instance.curParticipatedPList[0].name);
            content = content.Replace("[b]", LiLianManager.Instance.curParticipatedPList[1].name);

        }
        else
        {
            content = "";
        }

        return content;
    }
    /// <summary>
    /// 处理事件
    /// </summary>
    void HandleEvent()
    {
        if (handleEventCurIndex < handleEventTotalIndex)
        {
            bgSke.timeScale = 0;
            startYieldShowFlyAnim = true;
            yieldShowFlyAnimTimer = 0;

            for (int i = 0; i < xiaoRenSkeList.Count; i++)
            {
                xiaoRenSkeList[i].AnimationState.SetAnimation(0, "jingzhi", true);
            }
        }
        else
        {
            TweenMoving();
        }

    }

    /// <summary>
    /// 结算
    /// </summary>
    void JieSuan()
    {
        AuditionManager.Instance.PlayVoice(AudioClipType.MakeFinish);

        for (int i = 0; i < xiaoRenSkeList.Count; i++)
        {
            SkeletonGraphic ske = xiaoRenSkeList[i];
            if(ske.gameObject.activeInHierarchy)
            xiaoRenSkeList[i].AnimationState.SetAnimation(0, "jingzhi", true);
        }

        if (LiLianManager.Instance.curParticipatedPList.Count == 2)
        {
            int curHaoGan = 0;
            PeopleData p1 = LiLianManager.Instance.curParticipatedPList[0];
            PeopleData p2 = LiLianManager.Instance.curParticipatedPList[1];

            int index = p1.socializationData.knowPeopleList.IndexOf(p2.onlyId);
            curHaoGan = p1.socializationData.haoGanDu[index];


     

            AddSingle<LiLianShangZhenStudent>(trans_jieSuanPGrid, LiLianManager.Instance.curParticipatedPList[0]);
            AddSingle<LiLianShangZhenStudent>(trans_jieSuanPGrid, LiLianManager.Instance.curParticipatedPList[1]);
           

            int beforeHaoGan = curHaoGan - LiLianManager.Instance.totalHaoGanAdd;

            string str = "";
       
            str = ReplaceStr(str, p2);
 
            DialogManager.Instance.CreateDialog(new List<DialogData> { new DialogData(p1, str) }, () =>
            {
                trans_jieSuan.gameObject.SetActive(true);

                trans_jieSuanRelationShip.gameObject.SetActive(true);

                img_jieSuanRelationShipImgBar.DOKill();

                if (beforeHaoGan < 0 && curHaoGan < 0)
                {
                    RefreshJieSuanHaoGanIconShow(curHaoGan);
                    img_jieSuanRelationShipImgBar.color = Color.red;
                    img_jieSuanRelationShipImgBar.fillAmount = Mathf.Abs(beforeHaoGan) / (float)100;
                    //txt_jieSuanHaoGan.text = "仇恨值";
                    img_jieSuanRelationShipImgBar.DOFillAmount(Mathf.Abs(curHaoGan) / (float)100, 1f);
                }
                else if (beforeHaoGan < 0 && curHaoGan >= 0)
                {
                    RefreshJieSuanHaoGanIconShow(beforeHaoGan);

                    img_jieSuanRelationShipImgBar.color = Color.red;
                    img_jieSuanRelationShipImgBar.fillAmount =Mathf.Abs(beforeHaoGan) / (float)100;
                    //txt_jieSuanHaoGan.text = "仇恨值";
                    img_jieSuanRelationShipImgBar.DOFillAmount(0 / (float)100, .5f).OnComplete(() =>
                    {
                        RefreshJieSuanHaoGanIconShow(curHaoGan);

                        img_jieSuanRelationShipImgBar.color = Color.green;
                        //txt_jieSuanHaoGan.text = "好感度";
                        img_jieSuanRelationShipImgBar.DOFillAmount(Mathf.Abs(curHaoGan) / (float)100, .5f);
                    });
                }
                else
                {
                    RefreshJieSuanHaoGanIconShow(curHaoGan);

                    img_jieSuanRelationShipImgBar.color = Color.green;
                    img_jieSuanRelationShipImgBar.fillAmount = Mathf.Abs(beforeHaoGan) / (float)100;
                    //txt_jieSuanHaoGan.text = "好感度";
                    img_jieSuanRelationShipImgBar.DOFillAmount(Mathf.Abs(curHaoGan) / (float)100, 1f);
                }
                txt_jieSuanHaoGanAddNum.DOKill();
                txt_jieSuanHaoGanAddNum.SetText("+" + (curHaoGan - beforeHaoGan));
                txt_jieSuanHaoGanAddNum.transform.localPosition = trans_jieSuanHaoGanAddNumDownPos.transform.localPosition;
                txt_jieSuanHaoGanAddNum.transform.DOLocalMoveY(trans_jieSuanHaoGanAddNumUpPos.transform.localPosition.y, 1f);
                ShowJieSuan();

            });
        }
        else
        {
            trans_jieSuan.gameObject.SetActive(true);

            trans_jieSuanRelationShip.gameObject.SetActive(false);
            ShowJieSuan();
        }

    }
    /// <summary>
    /// 刷新结算好感的icon
    /// </summary>
    void RefreshJieSuanHaoGanIconShow(int haoGanDu)
    {
        if (haoGanDu >= 0)
        {
            img_jieSuanRelationShipImgBar.color = Color.green;
            jieSuanhaoGanProcessImgList[0].gameObject.SetActive(true);
            jieSuanhaoGanProcessImgList[1].gameObject.SetActive(true);
            jieSuanhaoGanProcessImgList[2].sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.haoGanXin3);
            jieSuanhaoGanProcessImgList[2].gameObject.SetActive(true);
            if (haoGanDu < 30)
            {
                jieSuanhaoGanProcessImgList[0].DOFade(0.2f, 0);
                jieSuanhaoGanProcessImgList[1].DOFade(0.2f, 0);
                jieSuanhaoGanProcessImgList[2].DOFade(0.2f, 0);
            }
            else if (haoGanDu < 60)
            {
                jieSuanhaoGanProcessImgList[0].DOFade(1f, 0);
                jieSuanhaoGanProcessImgList[1].DOFade(0.2f, 0);
                jieSuanhaoGanProcessImgList[2].DOFade(0.2f, 0);
            }
            else if (haoGanDu < 90)
            {
                jieSuanhaoGanProcessImgList[0].DOFade(1f, 0);
                jieSuanhaoGanProcessImgList[1].DOFade(1f, 0);
                jieSuanhaoGanProcessImgList[2].DOFade(0.2f, 0);
            }
            else
            {
                jieSuanhaoGanProcessImgList[0].DOFade(1f, 0);
                jieSuanhaoGanProcessImgList[1].DOFade(1f, 0);
                jieSuanhaoGanProcessImgList[2].DOFade(1f, 0);
            }
        }
        else
        {
            jieSuanhaoGanProcessImgList[0].gameObject.SetActive(false);
            jieSuanhaoGanProcessImgList[1].gameObject.SetActive(false);
            jieSuanhaoGanProcessImgList[2].sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.chouHenIcon);
            if (haoGanDu > -90)
            {
                jieSuanhaoGanProcessImgList[2].DOFade(.2f, 0);
            }
            else
            {
                jieSuanhaoGanProcessImgList[2].DOFade(1f, 0);
            }
             img_relationShipBar.color = Color.red;
        }
        jieSuanhaoGanProcessImgList[2].SetNativeSize();

    }
    void ShowJieSuan()
    {
        GetAwardItemView view1= AddSingle<GetAwardItemView>(trans_jieSuanItemGrid, LiLianManager.Instance.totalItemData);
        view1.StartAnim();
       LiLianManager.Instance.receivedItemList = ItemManager.Instance.CombineItemList(LiLianManager.Instance.receivedItemList);
        for(int i = 0; i < LiLianManager.Instance.receivedItemList.Count; i++)
        {
            GetAwardItemView view= AddSingle<GetAwardItemView>(trans_jieSuanItemGrid, LiLianManager.Instance.receivedItemList[i]);
            view.StartAnim();
        }
    }

    public string ReplaceStr(string str,PeopleData p)
    {
        string res = str.Replace("[a]",p.name);
        return res;
    }
    private void Update()
    {

        if (startYieldShowFlyAnim)
        {
            yieldShowFlyAnimTimer += Time.deltaTime;
            if (yieldShowFlyAnimTimer >= yieldShowFlyAnimTime)
            {
                bgSke.timeScale = 1;

                startYieldShowFlyAnim = false;
                //飞物品
                if (handleEventCurIndex < curHandleEventItemList.Count)
                {
                    GetItemFlyUpAnimView view = AddSingle<GetItemFlyUpAnimView>(trans_flyItemAnimParent, trans_flyItemAnimParent.position, curHandleEventItemList[handleEventCurIndex]);

                }
                else
                {
                    if (yieldHaoGanAdd > 0)
                    {
                        ItemData data = new ItemData();
                        data.settingId = (int)ItemIdType.LingShi;
                        data.count = (ulong)yieldHaoGanAdd;
                        GetItemFlyUpAnimView view = AddSingle<GetItemFlyUpAnimView>(trans_flyItemAnimParent, trans_flyItemAnimParent.position, data);
                    }
                }
                //TweenMoving();
                
                handleEventCurIndex++;
                HandleEvent();
            }
        }
        //飞行过程中生成奖励物品
        else
        {
            if (startFlyUltraAward
                && !startYieldShowFlyAnim
                && curMoveIndex<totalMoveIndex)
            {
                flyUltraAwardTimer += Time.deltaTime;
                if (flyUltraAwardTimer >= flyUltraAwardTime)
                {
                    int yLeft = (int)trans_ultraAwardDownPos.localPosition.y;
                    int yRight = (int)trans_ultraAwardUpPos.localPosition.y;
                    int posY = RandomManager.Next(yLeft, yRight);

                    int index = CommonUtil.GetIndexByWeight(possibleUltraAwardWeightList);
                    ItemData data = possibleUltraAwardItemList[index];

                    AddSingle<LiLianUltraAwardView>(trans_ultraAwardParent,new Vector2(trans_ultraAwardDownPos.localPosition.x,posY), data,this);
                    flyUltraAwardTimer = 0;
                }
            }
        }
   
    }

    /// <summary>
    /// 历练捡东西
    /// </summary>
    /// <param name="view"></param>
    public void OnGetUltraItem(LiLianUltraAwardView view)
    {
        //AddSingle<LiLianUltraAwardView>(trans_ultraAwardParent, new Vector2(view.transform.localPosition.x,view.transform.localPosition.y));
        //ItemData item = new ItemData();
        //item.settingId = 10001;
        //item.count = 1;
        LiLianManager.Instance.receivedItemList.Add(view.item);
        GetItemFlyUpAnimView view2 = AddSingle<GetItemFlyUpAnimView>(trans_flyItemAnimParent, view.transform.position, view.item);
        
        //ItemManager.Instance.CombineItemList(receivedItemList)
    }
    #region 弟子上阵
    /// <summary>
    /// 历练上阵
    /// </summary>
    public void OnUpStudent(LiLianPrepareStudentView view)
    {
        PeopleData p = view.peopleData;
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
            PanelManager.Instance.OpenFloatWindow("最多上阵" + curStudentPosList.Count + "个"+LanguageUtil.GetLanguageText((int)LanguageIdType.弟子)+"");
            return;
        }

        //在posindex上增加弟子
        curStudentPosList[posIndex] = p.onlyId;
        studentPosBgList[posIndex].gameObject.SetActive(false);
        LiLianShangZhenStudent upView = PanelManager.Instance.OpenSingle<LiLianShangZhenStudent>(trans_UpStudentParent[posIndex], p);
        ShowAllCandidateStudent();
        RefreshRelationShipShow();

    }

    /// <summary>
    /// 弟子下阵
    /// </summary>
    /// <param name="posIndex"></param>
    public void OnDownStudent(int posIndex)
    {
        if (curStudentPosList[posIndex] != 0)
        {
            curStudentPosList[posIndex] = 0;
            studentPosBgList[posIndex].gameObject.SetActive(true);

            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent[posIndex]);
            ShowAllCandidateStudent();
            RefreshRelationShipShow();
        }
    }

    /// <summary>
    /// 友好度显示
    /// </summary>
    public void RefreshRelationShipShow()
    {
        ulong onlyId1 = 0;
        ulong onlyId2 = 0;

        onlyId1 = curStudentPosList[0];
        onlyId2 = curStudentPosList[1];

        if (onlyId1 == 0 || onlyId2 == 0)
        {
            trans_relationShip.gameObject.SetActive(false);
        }
        else
        {
            PeopleData p1 = StudentManager.Instance.FindStudent(onlyId1);
            PeopleData p2= StudentManager.Instance.FindStudent(onlyId2);
            if (!p1.socializationData.knowPeopleList.Contains(onlyId2))
            {
                trans_relationShip.gameObject.SetActive(false);
            }
            else
            {
                trans_relationShip.gameObject.SetActive(true);
               
                int index = p1.socializationData.knowPeopleList.IndexOf(onlyId2);
                int haoGanDu = p1.socializationData.haoGanDu[index];

                if (haoGanDu >= 0)
                {
                    txt_relationShipLabel.SetText("好感度");
                    img_relationShipBar.color = Color.green;
                    haoGanProcessImgList[0].gameObject.SetActive(true);
                    haoGanProcessImgList[1].gameObject.SetActive(true);
                    haoGanProcessImgList[2].sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.haoGanXin3);
                    haoGanProcessImgList[2].gameObject.SetActive(true);
                    if (haoGanDu < 30)
                    {
                        haoGanProcessImgList[0].DOFade(0.2f, 0);
                        haoGanProcessImgList[1].DOFade(0.2f, 0);
                        haoGanProcessImgList[2].DOFade(0.2f, 0);
                    }
                    else if (haoGanDu < 60)
                    {
                        haoGanProcessImgList[0].DOFade(1f, 0);
                        haoGanProcessImgList[1].DOFade(0.2f, 0);
                        haoGanProcessImgList[2].DOFade(0.2f, 0);
                    }
                    else if (haoGanDu < 90)
                    {
                        haoGanProcessImgList[0].DOFade(1f, 0);
                        haoGanProcessImgList[1].DOFade(1f, 0);
                        haoGanProcessImgList[2].DOFade(0.2f, 0);
                    }
                    else
                    {
                        haoGanProcessImgList[0].DOFade(1f, 0);
                        haoGanProcessImgList[1].DOFade(1f, 0);
                        haoGanProcessImgList[2].DOFade(1f, 0);
                    }
                }
                else
                {
                    haoGanProcessImgList[0].gameObject.SetActive(false);
                    haoGanProcessImgList[1].gameObject.SetActive(false);
                    haoGanProcessImgList[2].sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.chouHenIcon);
                    if (haoGanDu >-90)
                    {
                        haoGanProcessImgList[2].DOFade(.2f, 0);
                    }
                    else
                    {
                        haoGanProcessImgList[2].DOFade(1f, 0);
                    }
                    txt_relationShipLabel.SetText("仇恨值");
                    img_relationShipBar.color = Color.red;
                }
                haoGanProcessImgList[2].SetNativeSize();
                img_haoGanDuIcon.sprite = ConstantVal.HaoGanIcon(haoGanDu, (Gender)(int)p1.gender, (Gender)(int)p2.gender);

                int fillVal = Mathf.Abs(haoGanDu);
                img_relationShipBar.fillAmount = fillVal / 100f;
             }
        }
    }


    void ShowAllCandidateStudent()
    {
        PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);
        candidateList.Clear();
        List<PeopleData> candidateShowPList = new List<PeopleData>();

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (!curStudentPosList.Contains(p.onlyId))
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
            LiLianPrepareStudentView view = PanelManager.Instance.OpenSingle<LiLianPrepareStudentView>(trans_chooseStudentGrid, candidateShowPList[i], this);
            candidateList.Add(view);
        }

    }
    #endregion

    public override void Clear()
    {
        base.Clear();

        curStudentPosList.Clear();
        for (int i = 0; i < 2; i++)
        {
            curStudentPosList.Add(0);
            studentPosBgList[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < trans_UpStudentParent.Count; i++)
        {
            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent[i]);
        }
        trans_jieSuan.gameObject.SetActive(false);
        PanelManager.Instance.CloseAllSingle(trans_flyItemAnimParent);

        PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);

        ClearCertainParentAllSingle<GetAwardItemView>(trans_jieSuanItemGrid);
        ClearCertainParentAllSingle<LiLianShangZhenStudent>(trans_jieSuanPGrid);
        ClearCertainParentAllSingle<SingleLiLianDesView>(trans_eventGrid);
        ClearCertainParentAllSingle<LiLianUltraAwardView>(trans_ultraAwardParent);
        startFlyUltraAward = false;
    }
}
