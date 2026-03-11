using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Framework.Data;
using DG.Tweening;
using cfg;
using TooSimpleFramework.UI;
using Unity.VisualScripting;

public class StudentHandlePanel : PanelBase
{
    public PeopleData curChoosedP;

    public InfoBigStudentView curChoosedInfoBigStudentView;

    public Button btn_des;//稀有度介绍
    public Text txt_studentLimit;
    public Transform grid_infoBigStudentViewGrid;//大view父物体

    public Transform trans_allStudentGrid;//所有弟子
    public List<Button> btn_bigTagList;//大标签
    public List<GameObject> bigTagRedPointList;//大标签红点

    public List<Button> btn_smallTagList;//小标签
    public List<GameObject> smallTagRedPointList;//小标签红点
    public List<StudentTalent> smallTagTalentList;//小标签代表的弟子天赋
    List<PeopleData> curBigTagStudentList;//筛选出大标签的弟子
    List<PeopleData> curShowStudentList;//筛选出当前显示的弟子
    List<InfoPanelStudentView> curShowStudentViewList=new List<InfoPanelStudentView>();//显示的所有弟子view
    public Transform trans_smallTag;//小标签
    public int curChoosedBigTagIndex;//当前选的大tag

    public ScrollViewNevigation scrollViewNevigation;
    public Transform trans_sonPanelParent;
    #region 弟子详情

    #region 修炼界面

    //元神受损
    public Transform trans_yuanShenShouSun;//元神受损
    public Text txt_yuanShenShouSun;
    public Text txt_canEatTiaoXiDanNum;//可以吃调息丹的数量
    public Transform trans_noTiaoXiDan;//无调息丹
    public Transform grid_tiaoXiDan;
    public Button btn_jumpToBuyTiaoXiDan;//去买调息丹
    
    public TrainSetting curTrainSetting;
    //public SingleMapEventData singleMapEventData;//可能地图事件帮助修炼
    public Text txt_todayRemainEatXiuWeiDanNum;//今天可服用次数
    public Transform trans_xiuWeiDanParent;//修为丹父物体
    //public Image img_trainBarInXiuLianPanel;//修炼界面的bar
    public Text txt_curXiuWeiInXiuLianPanel;//修炼界面的当前修为
    //public Text txt_curJingJieInXiulianPanel;//当前境界
    //public Text txt_curBigPhaseNameInTrainPanel;//当前大境界
    public Text txt_nextJingJieInXiulianPanel;//下个境界
    public Transform trans_poJingDanParent;//破镜丹父物体
    public Transform trans_poJingDanGrid;//破镜丹格子
    public Text txt_poJingDan;//丹的情况
    public Text txt_successRate;//突破成功率
    public Transform grid_xiuWeiDan;//修为丹格子
    public Transform trans_noXiuWeiDan;//当前没有修为丹
    public Transform trans_reachLimit;//品质已达上限
    public Button btn_jumpToLianDanFang;//跳去炼丹房
    //public List<Button> btn_xiuWeiDanTagBtnList;//修为丹标签按钮
    //public List<GameObject> obj_xiuWeiDanTagObjList;//修为丹标签底图
    List<XiuWeiDanItemView> xiuWeiDanList = new List<XiuWeiDanItemView>();
    public Transform trans_usedUpMapEvent;//灵气已耗尽
    //public Transform trans_proAddFlyTxtParent;//飘字父物体
    public Button btn_addPoJingDan;//破境丹
    //public Transform trans_consumeGrid;//消耗
    public Button btn_breakThrough;//开始突破
    //public Button btn_quitXiuLianPanel;//退出修炼界面
    //public Button btn_mapEventXiuLian;//灵气充沛地修炼按钮
    //public Image img_predictBarInMapEventXiuLian;//灵气充沛地预测bar
    //public Transform trans_effectParent;
    //#region 曲线移动
    ////public Transform trans_startPoint;
    //public Transform trans_endPoint;
    //public Vector2Int height;//贝塞尔高度区间
    //public int resolution;//点数量
    //#endregionbtn_changeXingGe
    //bool startBreakThroughXiAnim = false;
    //float breakThroughXiAnimTimer = 0;
    //float breakThroughXiAnimTime = 1;
    public Button btn_talentTest;//天赋觉醒
    public Button btn_removeTalent;//移除天赋

    [Header("改变性格")]
    public Button btn_changeXingGe;//改变性格
    public Button btn_xiSui;//洗髓

    [Header("整容")]
    public Button btn_zhengRong;//整容

    #endregion
    //public Transform trans_nameGrid;
    public Text txt_studentName;
    public Button btn_changeName;
    public Transform trans_studentDetailPanel;
    public Text txt_yanZhi;//颜值
    public Text txt_xingGe;//性格

    [Header("显示弟子")]
    public Transform trans_showStudent;//显示的弟子
    [Header("显示弟子ICON")]
    public Image img_student_icon;

    [Header("进度条")]
    public Image img_expBar;
    public Text txt_exp;
    //public Button btn_breakThrough;//突破
    public Text txt_lv;//等级
    public Transform trans_proGrid;//属性
    //public Transform trans_cost;//突破消耗
    public Button btn_social;//社交
    public Button btn_closeStudentDetailPanel;//关闭详情
    //public Transform trans_skill;//技能
    //public Transform trans_skillGrid;//技能格子
    //public Transform trans_noSkill;//未领悟
    //public Text txt_SkillDes;//技能描述
    //public Transform trans_skillUpgradeNeedMatGrid;//功法所需材料
    //public Button btn_skillUpgrade;//功法升级
    //public GameObject obj_skillUpgradeRedPoint;//功法升级红点
    public Button btn_xueMai;//血脉
    public GameObject obj_xueMaiRedPoint;//血脉升级红点


    [Header("信息tag")]
    public Button btn_infoTag;//信息tag
    public Button btn_weaponTag;//武器tag
    public Button btn_socialTag;//社交tag
    public Button btn_skillTag;//技能tag
    //public Color txtColor_gray = new Color32(110, 89, 70, 255);//绿字

    public Color txtColor_gray = new Color32(73, 42, 35, 255);
    public Color txtColor_wirt = new Color32(255,255, 255,0);//白色
    public Transform trans_infoPanel;//信息
    //public Transform trans_equipPanel;//武器
    public Transform trans_socialPanel;//社交


    #region 装备面板
    //public Button btn_suoYaoTag;//索要
    //public Button btn_sendTag;//赠送
    //public Button btn_suoYao;//索要

   // public Transform trans_equipGrid;//装备格子
   // public Transform trans_suoYaoPanel;//索要面板
    //public Transform trans_sendPanel;//赠送面板
   // public Transform trans_sendEquipKnapsackGrid;//赠送格子

    #endregion

    #region 社交动态面板
    public Transform trans_socializationPanel;//社交动态面板
    public Button btn_haoGan;
    public Button btn_chouHen;
    public Button btn_record;

    public Transform trans_scroll;//scroll
    public Transform trans_grid;//认识的人

    public Transform trans_record;//社交动态
    public Transform trans_recordGrid;//动态格子
    #endregion
    #endregion

    public Button btn_left;
    public Button btn_right;

    StudentBigTag initTag;
    Sprite sprt_choose;
    Sprite sprt_unChoose;
    SingleStudentInfoTagType curInfoTagType;


    [Header("Big选中按钮")]
    public Sprite select_img;

    public Sprite unselect_img;
    public Color unselect_color = new Color32(185, 179, 169, 255);

    [Header("small选中按钮")]
    public Sprite s_select_img;

    public Sprite s_unselect_img;

    public Color select_color = new Color32(221, 215, 199, 255);

    public Text zhandouli;
    [Header("Baricon")]
    public Image Baricon;

    [Header("txt_curWork")]
    public Text txt_curWork;

    public Transform tarn_content;

    public Transform parent_xuemai;

    private float tagButtonTotalWidth = 887f;
    private float tagButtonDefaultWidth = 212f;
    public override void Init(params object[] args)
    {
        base.Init(args);
        if (args.Length > 0)
        {
            initTag = (StudentBigTag)args[0];
        }
        else
        {
            initTag = StudentBigTag.None;
        }
        addBtnListener(btn_des, () =>
        {
            PanelManager.Instance.OpenPanel<RarityDesPanel>(PanelManager.Instance.trans_layer2);
        });
          
        addBtnListener(btn_breakThrough, () =>
        {
            int curLevel = 0;
            if (curChoosedP.talent == (int)StudentTalent.LianGong)
                curLevel = curChoosedP.trainIndex;
            else
                curLevel = curChoosedP.studentLevel;
            if (curLevel >= StudentManager.Instance.GetStudentLevelLimit(curChoosedP))
            {
                PanelManager.Instance.OpenFloatWindow("潜力已达上限");
                return;
            }
            if (curChoosedP.talent != (int)StudentTalent.LianGong)
            {


                StudentUpgradeSetting setting = DataTable._studentUpgradeList[curChoosedP.studentLevel - 1];
                int needExp = setting.NeedExp.ToInt32();
                if (curChoosedP.studentCurExp < needExp)
                {
                    PanelManager.Instance.OpenFloatWindow("经验不够");
                    return;
                }
                //List<List<int>> matList = CommonUtil.SplitCfg(setting.NeedMat);
                //for (int i = 0; i < matList.Count; i++)
                //{
                //    List<int> singleMat = matList[i];
                //    int id = singleMat[0];
                //    int num = singleMat[1];
                //    if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                //    {
                //        PanelManager.Instance.OpenFloatWindow("材料不够");
                //        return;
                //    }
                //}
                //for (int i = 0; i < matList.Count; i++)
                //{
                //    List<int> singleMat = matList[i];
                //    int id = singleMat[0];
                //    int num = singleMat[1];
                //    ItemManager.Instance.LoseItem(id, (ulong)num);

                //}
            }
 
            StudentManager.Instance.BreakThrough(curChoosedP);
            PanelManager.Instance.CloseTaskGuidePanel();
            RefreshXiuWeiShow();
        });


        addBtnListener(btn_addPoJingDan, () =>
        {
            ShowAllPoJingDan();
        });
        addBtnListener(btn_jumpToLianDanFang, () =>
        {
            GameSceneManager.Instance.GoToScene(SceneType.Mountain);
            MountainPanel mountainPanel = GameObject.Find("MountainPanel").GetComponent<MountainPanel>();
            SingleFarmView farmView = mountainPanel.FindFarmViewById((int)DanFarmIdType.LianDanLu);
            if (farmView != null)
            {
                EventCenter.Broadcast(TheEventType.NevigateToMountainPos, farmView.GetComponent<RectTransform>());
                farmView.OnClickedFarm();
            }
            //PanelManager.Instance.ClosePanel(this);
            //if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType !=(int)SceneType.Mountain)
            //{

            //}
        });
        //for (int i = 0; i < btn_xiuWeiDanTagBtnList.Count; i++)
        //{
        //    int index = i;
        //    Button btn = btn_xiuWeiDanTagBtnList[i];
        //    addBtnListener(btn, () =>
        //    {
        //        //ShowAllXiuWeiDan(index + 1);

        //        //for (int j = 0; j < obj_xiuWeiDanTagObjList.Count; j++)
        //        //{
        //        //    GameObject obj = obj_xiuWeiDanTagObjList[j];
        //        //    if (index == j)
        //        //        obj.gameObject.SetActive(true);
        //        //    else
        //        //        obj.gameObject.SetActive(false);

        //        //}

        //    });

        //}

        addBtnListener(btn_talentTest, () =>
        {
            int cost = 1500;//基础是1500

            cost = Mathf.RoundToInt(cost * ConstantVal.GetValAddByRarity((Rarity)(int)curChoosedP.studentRarity));

            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)cost))
            {
                        ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
                return;
            }

            PanelManager.Instance.OpenPanel<TalentTestPanel>(PanelManager.Instance.trans_layer2, curChoosedP);
        });

        for(int i = 0; i < btn_bigTagList.Count; i++)
        {
            int index = i;
            Button btn = btn_bigTagList[i];
            addBtnListener(btn, () =>
             {
                 OnBigTagClick(index);
             });
        }

        for (int i = 0; i < btn_smallTagList.Count; i++)
        {
            int index = i;
            Button btn = btn_smallTagList[i];
            addBtnListener(btn, () =>
            {
                OnSmallTagClick(index);
            });
        }

        addBtnListener(btn_infoTag, () =>
         {
             ShowSingleStudentTypeInfo(SingleStudentInfoTagType.Info);
         });
        addBtnListener(btn_weaponTag, () =>
        {
            ShowSingleStudentTypeInfo(SingleStudentInfoTagType.Equip);
        });
        addBtnListener(btn_skillTag, () =>
        {
            ShowSingleStudentTypeInfo(SingleStudentInfoTagType.Skill);
        });
        addBtnListener(btn_social, () =>
        {
            if (curChoosedP.isPlayer)
            {
                PanelManager.Instance.OpenFloatWindow("掌门无法查看社交属性");
                return;
            }
            ShowSingleStudentTypeInfo(SingleStudentInfoTagType.Social);
            //PanelManager.Instance.OpenPanel<SocializationPanel>(PanelManager.Instance.trans_layer2,curChoosedP);
        });
 
        addBtnListener(btn_haoGan, () =>
        {
            btn_haoGan.GetComponent<Text>().color = txtColor_gray;
            btn_chouHen.GetComponent<Text>().color = Color.black;
            btn_record.GetComponent<Text>().color = Color.black;
            ShowHaoGan();
        });
        addBtnListener(btn_chouHen, () =>
        {
            btn_haoGan.GetComponent<Text>().color = Color.black;
            btn_chouHen.GetComponent<Text>().color = txtColor_gray;
            btn_record.GetComponent<Text>().color = Color.black;
            ShowHate();
        });
        addBtnListener(btn_record, () =>
        {
            btn_haoGan.GetComponent<Text>().color = Color.black;
            btn_chouHen.GetComponent<Text>().color = Color.black;
            btn_record.GetComponent<Text>().color = txtColor_gray;
            ShowRecord();
        });
        //addBtnListener(btn_changeName, () =>
        //{
        //    PanelManager.Instance.OpenPanel<ChangeNamePanel>(PanelManager.Instance.trans_layer2,ChangeNameType.StudentName,curChoosedP);
        //});
     
     
        //addBtnListener(btn_xueMai, () =>
        //{
        //    PanelManager.Instance.OpenPanel<XueMaiPanel>(parent_xuemai,curChoosedP);
        //});
        //addBtnListener(btn_equipUpgrade, () =>
        //{
        //    if (curChoosedP != null && curChoosedP.CurEquipItem != null)
        //    {
        //       // SingleDanFarmData farm = LianDanManager.Instance.FindBestEquipFarm();
        //        EquipmentManager.Instance.OnIntenseEquip(curChoosedP.CurEquipItem.OnlyId, null,curChoosedP);
        //    }

        //});
        addBtnListener(btn_closeStudentDetailPanel, () =>
        {
            trans_studentDetailPanel.gameObject.SetActive(false);
        });

        addBtnListener(btn_removeTalent, () =>
        {
        
        
        });

        addBtnListener(btn_xiSui, () =>
        {                return;

        });

        //addBtnListener(btn_zhengRong, () =>
        //{
        //    PanelManager.Instance.OpenPanel<ZhengRongPanel>(PanelManager.Instance.trans_layer2, curChoosedP);


        //});

        //addBtnListener(btn_changeXingGe, () =>
        //{
        //    return;

        //});
        btn_zhengRong.gameObject.SetActive(false);
        btn_changeXingGe.gameObject.SetActive(false);


        addBtnListener(btn_left, () =>
         {
             int curChoosedIndex = 0;
             for(int i = 0; i < curShowStudentViewList.Count; i++)
             {
                 PeopleData p = curShowStudentViewList[i].peopleData;
                 if (p.onlyId == curChoosedInfoBigStudentView.p.onlyId)
                 {
                     curChoosedIndex = i;
                     break;
                 }
             }
             if (curChoosedIndex > 0)
             {
                 OnChoosedStudent(curShowStudentViewList[curChoosedIndex - 1].peopleData);
                 ShowSingleStudentTypeInfo(curInfoTagType);
             }
              
         });
        addBtnListener(btn_right, () =>
        {
            int curChoosedIndex = 0;
            for (int i = 0; i < curShowStudentViewList.Count; i++)
            {
                PeopleData p = curShowStudentViewList[i].peopleData;
                if (p.onlyId == curChoosedInfoBigStudentView.p.onlyId)
                {
                    curChoosedIndex = i;
                    break;
                }
            }
            if (curChoosedIndex < curShowStudentList.Count-1)
            {
                OnChoosedStudent(curShowStudentViewList[curChoosedIndex + 1].peopleData);
                ShowSingleStudentTypeInfo(curInfoTagType);
            }

        });
        addBtnListener(btn_jumpToBuyTiaoXiDan, () =>
        {
            PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.LiBao);
        });
        RegisterEvent(TheEventType.HandleStudent, OnReceiveChooseStudentEvent);
        RegisterEvent(TheEventType.StudentBreakthroughSuccess, OnBreakThrough);
        RegisterEvent(TheEventType.FailStudentBreakThrough, OnBreakThrough);
        //RegisterEvent(TheEventType.SkillUpgrade, OnUpgradeSkill);

        RegisterEvent(TheEventType.OnGetStudentExp, OnGetStudentExp);
        RegisterEvent(TheEventType.EndowTalent, OnEndowTalent);
        RegisterEvent(TheEventType.RemoveTalent, OnRemoveTalent);

       // RegisterEvent(TheEventType.OnSendItem, OnSendItem);
        RegisterEvent(TheEventType.RefreshStudentRedPoint, OnRefreshRedPoint);
        RegisterEvent(TheEventType.OnRemoveStudent, OnRemoveStudent);
        RegisterEvent(TheEventType.ChangeStudentName, ShowName);
        RegisterEvent(TheEventType.ChangeXingGe, OnChangeXingGe);
        RegisterEvent(TheEventType.SuccessXiuLian, OnSuccessXiuLian);
        RegisterEvent(TheEventType.OnProcessXiuwei, OnProcessXiuWei);
        RegisterEvent(TheEventType.SuccessBreakThrough, OnSuccessBreakThrough);
        RegisterEvent(TheEventType.UsedBreakDan, OnUseBreakDan);
        RegisterEvent(TheEventType.FailStudentBreakThrough, OnFailStudentBreakthrough);
        RegisterEvent(TheEventType.UseTiaoXiDan, OnUseTiaoXiDan);
        RegisterEvent(TheEventType.ZhengRong, OnZhengRong);

        //OnBigTagClick(0);

        sprt_choose = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "btn_dizi_ui_Anniu1");
        sprt_unChoose = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "btn_dizi_ui_Anniu2");
        s_select_img = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "btn_select_img");
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        trans_studentDetailPanel.gameObject.SetActive(false);
        txt_studentLimit.SetText("手下数量："+RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count + "/" + RoleManager.Instance._CurGameInfo.studentData.MaxStudentNum);
        OnBigTagClick((int)initTag);
        InitRedPoint();
        ShowGuide();

        //trans_smallTag.gameObject.SetActive(true);
        //btn_smallTagList[0].onClick.Invoke();
        trans_smallTag.gameObject.SetActive(false);
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_studentTuPo)
        {
             //找可以突破的弟子来点击
            for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
            {
                PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                if (p.talent == (int)StudentTalent.LianGong)
                {
                    if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(p) )
                    {
                        //修武按钮
                        PanelManager.Instance.ShowTaskGuidePanel(btn_bigTagList[3].gameObject);
                        break;
                    }

                }else if (p.talent != (int)StudentTalent.None)
                {
                    if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(p) )
                    {
                        //生产按钮
                        PanelManager.Instance.ShowTaskGuidePanel(btn_bigTagList[2].gameObject);
                        break;
                    }
                }
            }
        }
        else if (TaskManager.Instance.guide_studentJueXing)
        {
            //找可以觉醒的弟子来点击
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
            {
                PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

                if (p.talent == (int)StudentTalent.None)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int expLimit = setting.NeedExp.ToInt32();
                    if(p.studentCurExp >= expLimit)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(btn_bigTagList[1].gameObject);
                    }
                 
                }
            }
        }
        //装备法器
        else if (TaskManager.Instance.guide_equipEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);

           
        }
        //学习功法
        else if (TaskManager.Instance.guide_studySkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
        }
        //装备功法
        else if (TaskManager.Instance.guide_equipSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
        }  
        //升级功法
        else if (TaskManager.Instance.guide_upgradeSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
        }
    }
    private void Update()
    {
        RefreshYuanShenShouSunTimeShow();
    }
    #region 修炼页面
    /// <summary>
    /// 使用修为丹
    /// </summary>
    public void OnUseXiuWeiDan(XiuWeiDanItemView view)
    {
        if (curChoosedP.talent == (int)StudentTalent.LianGong
            && view.GetItemData().setting.ItemType.ToInt32() == (int)ItemType.Dan)
            RoleManager.Instance.OnDanXiuLian(curChoosedP, view.GetItemData());
        else
            StudentManager.Instance.UseChunLingGuo(curChoosedP, view.GetItemData());

    }

    /// <summary>
    /// 使用调息丹
    /// </summary>
    public void OnUseTiaoXiDan(TiaoXiDanItemView view)
    {
        if (curChoosedP.talent == (int)StudentTalent.LianGong
            && view.GetItemData().setting.ItemType.ToInt32() == (int)ItemType.TiaoXiDan)
            StudentManager.Instance.UseTiaoXiDan(curChoosedP, view.GetItemData());
    }
    /// <summary>
    /// 使用调息丹完毕
    /// </summary>
    public void OnUseTiaoXiDan()
    {
        RefreshXiuWeiShow();

    }
    /// <summary>
    /// 显示所有破境丹
    /// </summary>
    void ShowAllPoJingDan()
    {
        //trans_xiuWeiDanParent.gameObject.SetActive(true);
        ClearCertainParentAllSingle<ItemView>(trans_poJingDanGrid);

        //List<ItemData> itemList = ItemManager.Instance.FindItemListByType(ItemType.Dan);
        TrainSetting setting = DataTable._trainList[curChoosedP.trainIndex];
        //判断有没有该丹
        int danId = setting.SuccessDanId.ToInt32();


        ItemData data = ItemManager.Instance.FindItemBySettingId(danId);
        if (data != null)
        {
            AddSingle<PoJingDanItemView>(trans_poJingDanGrid, data,this);

        }
        else
        {
            data = new ItemData();
            data.settingId = danId;
            data.count = 0;
            AddSingle<PoJingDanItemView>(trans_poJingDanGrid, data,this);

        }

    }
    /// <summary>
    /// 显示修炼界面
    /// </summary>
    public void  ShowXiuLianPanel()
    {

 
        RefreshXiuWeiShow();

    }

    /// <summary>
    /// 显示修炼
    /// </summary>
    void ShowXiuLian(bool regenerateDan=true)
    {
        btn_talentTest.gameObject.SetActive(false);
        btn_breakThrough.gameObject.SetActive(false);
        trans_yuanShenShouSun.gameObject.SetActive(false);
        //trans_xiuWeiDanParent.gameObject.SetActive(true);
        if (curChoosedP.talent != (int)StudentTalent.LianGong)
        {
            txt_todayRemainEatXiuWeiDanNum.gameObject.SetActive(false);
        }
        else
        {
            txt_todayRemainEatXiuWeiDanNum.gameObject.SetActive(true);

            txt_todayRemainEatXiuWeiDanNum.SetText("");

        }
        List<ItemData> itemList = new List<ItemData>();
        if (regenerateDan)
        {
            ClearCertainParentAllSingle<ItemView>(grid_xiuWeiDan);
            xiuWeiDanList = new List<XiuWeiDanItemView>();

            itemList = ItemManager.Instance.FindValidXiuWeiDan(curChoosedP);
            //排序
            //从大到小排序  
            for (int i = 0; i < itemList.Count - 1; i++)
            {
                for (int j = 0; j < itemList.Count - 1 - i; j++)
                {
                    //前面的小于后面的，则二者交换
                    if (itemList[j].setting.Param.ToInt32()
                        < itemList[j + 1].setting.Param.ToInt32())
                    {
                        var temp = itemList[j];
                        itemList[j] = itemList[j + 1];
                        itemList[j + 1] = temp;
                    }
                }
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                XiuWeiDanItemView view = AddSingle<XiuWeiDanItemView>(grid_xiuWeiDan, itemList[i], this);
                xiuWeiDanList.Add(view);
            }
        }
  
        
        if (xiuWeiDanList.Count > 0)
        {
            trans_reachLimit.gameObject.SetActive(false);
            trans_noXiuWeiDan.gameObject.SetActive(false);
        }
        else
        {    //如果已达上限
            if (StudentManager.Instance.IfPReachLevelLimit(curChoosedP))
            {
                trans_reachLimit.gameObject.SetActive(true);
                trans_noXiuWeiDan.gameObject.SetActive(false);
            }
            else
            {
                trans_reachLimit.gameObject.SetActive(false);

                trans_noXiuWeiDan.gameObject.SetActive(true);
                Text txt = trans_noXiuWeiDan.GetComponentInChildren<Text>();
                         txt.SetText(LanguageUtil.GetLanguageText((int)LanguageIdType.当前无任何修为丹));
                    btn_jumpToLianDanFang.gameObject.SetActive(true);

            }



        }
        //int giantLevel = RoleManager.Instance.GiantLevel(curChoosedP);
        //btn_xiuWeiDanTagBtnList[giantLevel - 1].onClick.Invoke();
    }

    void ShowAllXiuWeiDan()
    {

    }
    /// <summary>
    /// 显示突破
    /// </summary>
    void ShowTuPo( )
    {
        trans_xiuWeiDanParent.gameObject.SetActive(false);
        trans_yuanShenShouSun.gameObject.SetActive(false);

        btn_breakThrough.gameObject.SetActive(true);
        List<ItemData> tuPoMatList = new List<ItemData>();
        //新人弟子 天赋觉醒
        if (curChoosedP.talent == (int)StudentTalent.None)
        {
            btn_breakThrough.gameObject.SetActive(false);
            btn_talentTest.gameObject.SetActive(true);
            int cost = 1500;//基础是1500
            cost = Mathf.RoundToInt(cost * ConstantVal.GetValAddByRarity((Rarity)(int)curChoosedP.studentRarity));
            //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_cost, (int)ItemIdType.LingShi, cost, ConsumeType.Item);
            int danId = (int)ItemIdType.LingShi;
            ItemData item = new ItemData();
            item.settingId = danId;
            item.count = (ulong)cost;
            tuPoMatList.Add(item);

        }
        //练功弟子
        else if(curChoosedP.talent==(int)StudentTalent.LianGong
            ||curChoosedP.onlyId==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
        {
            int baseRate = curTrainSetting.SuccessRate.ToInt32();
            txt_successRate.SetText("");

            if (baseRate >= 100)
            {
                trans_poJingDanParent.gameObject.SetActive(false);
            }
            else
            {
                trans_poJingDanParent.gameObject.SetActive(true);
                //突破丹
                //ShowAllPoJingDan();
                //List<ItemData> itemList = ItemManager.Instance.FindItemListByType(ItemType.Dan);
                TrainSetting setting = DataTable._trainList[curChoosedP.trainIndex];
                //判断有没有该丹
                int danId = setting.SuccessDanId.ToInt32();
                ItemData item = new ItemData();
                item.settingId = danId;
                ItemData danData = ItemManager.Instance.FindItemBySettingId(danId);
                if (danData !=null)
                {
                    item.count = danData.count;

                }
                tuPoMatList.Add(item);

             }
            btn_talentTest.gameObject.SetActive(false);

        }
        //不是练功弟子
        else
        {
            trans_poJingDanParent.gameObject.SetActive(true);

            txt_poJingDan.SetText("");

            StudentUpgradeSetting setting = DataTable._studentUpgradeList[curChoosedP.studentLevel - 1];

            btn_breakThrough.gameObject.SetActive(true);
            btn_talentTest.gameObject.SetActive(false);

            List<List<int>> needMat = CommonUtil.SplitCfg(setting.NeedMat);
            for (int i = 0; i < needMat.Count; i++)
            {
                List<int> singleMat = needMat[i];

                int consumeId = singleMat[0];
                int consumeNum = singleMat[1];
                ItemData item = new ItemData();
                item.settingId = consumeId;
                item.count = (ulong)consumeNum;
                tuPoMatList.Add(item);
                //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_cost, consumeId, consumeNum, ConsumeType.Item);
            }
         }

        //trans_xiuWeiDanParent.gameObject.SetActive(true);
 
       

        //ItemData data = ItemManager.Instance.FindItemBySettingId(danId);
        //if (data != null)
        //{
        //    AddSingle<PoJingDanItemView>(trans_poJingDanGrid, data);

        //}
        //else
        //{
        //    data = new ItemData();
        //    data.settingId = danId;
        //    data.count = 0;
        //    AddSingle<PoJingDanItemView>(trans_poJingDanGrid, data);

        //}

        ////判定修为有没有满 没满级 可突破
        //if (trainIndex < DataTable._trainList.Count - 1)
        //{
        //    //修为没满不能突破
        //    if (RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei < xiuweiNeed)
        //    {

        //        trans_poJingDanParent.gameObject.SetActive(false);

        //    }
        //    //修为满了能突破
        //    else
        //    {
 
        //        ShowPoJingDanParent();

        //     }
        //}


    }

     
    /// <summary>
    /// 元神受损剩余时间显示
    /// </summary>
    void RefreshYuanShenShouSunTimeShow()
    {
        if (curChoosedP == null)
            return;
                  txt_yuanShenShouSun.SetText("");

    }
    /// <summary>
    /// 修为显示刷新 突破or修炼
    /// </summary>
    void RefreshXiuWeiShow(bool regenerateDan=true)
    {
        long lvLimit = StudentManager.Instance.GetStudentLevelLimit(curChoosedP);

        zhandouli.SetText(RoleManager.Instance.CalcZhanDouLi(curChoosedP).ToString());
        //弟子突破
        if (curChoosedP.talent != (int)StudentTalent.LianGong)
        {
            btn_xueMai.gameObject.SetActive(false);
            PanelManager.Instance.CloseAllPanel(parent_xuemai);

            txt_lv.SetText("Lv."+curChoosedP.studentLevel);
            if (curChoosedP.studentLevel + 1 <= lvLimit)
                txt_nextJingJieInXiulianPanel.SetText("Lv."+(curChoosedP.studentLevel + 1));
            else
                txt_nextJingJieInXiulianPanel.SetText("极限");

            if (curChoosedP.studentLevel < lvLimit)
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[curChoosedP.studentLevel - 1];
                int expLimit = setting.NeedExp.ToInt32();
                //int needLingShi = setting.needMat.ToInt32();
                img_expBar.fillAmount = curChoosedP.studentCurExp / (float)expLimit;
                txt_exp.SetText(curChoosedP.studentCurExp + "/" + expLimit);
                //txt_exp.gameObject.SetActive(true);


            }
            else
            {
                img_expBar.fillAmount = 1;
                txt_exp.gameObject.SetActive(false);
                //btn_breakThrough.gameObject.SetActive(false);
            }
        }
        //修武显示修为
        else
        {
            //btn_xueMai.gameObject.SetActive(true);
            btn_xueMai.gameObject.SetActive(false);
            PanelManager.Instance.CloseAllPanel(parent_xuemai);
            //btn_xueMai.onClick.Invoke();
            PanelManager.Instance.OpenPanel<XueMaiPanel>(parent_xuemai, curChoosedP);
            TrainSetting trainSetting = DataTable._trainList[curChoosedP.trainIndex];
    
            txt_exp.SetText(curChoosedP.curXiuwei + "/" + trainSetting.XiuWeiNeed);

    
            int lastXiuWeiNeed = 0;
            if (curChoosedP.trainIndex >= 1)
            {
                TrainSetting lastTrainSetting = DataTable._trainList[curChoosedP.trainIndex - 1];
                lastXiuWeiNeed = lastTrainSetting.XiuWeiNeed.ToInt32();
            }
            img_expBar.fillAmount = (curChoosedP.curXiuwei - (ulong)lastXiuWeiNeed) / (float)(trainSetting.XiuWeiNeed.ToInt32() - lastXiuWeiNeed);

            txt_lv.SetText("Lv."+(curChoosedP.trainIndex+1));
            if (curChoosedP.trainIndex + 1 <= lvLimit)
            {
   
            }
            else
                txt_nextJingJieInXiulianPanel.SetText("极限");
        }


        if (!StudentManager.Instance.CheckIfCanTuPo(curChoosedP))
        {
                         ShowXiuLian(regenerateDan);

        }
        else
        {
            ShowTuPo();
        }
        ////不能突破 只能修炼
        //if (curChoosedP.curXiuwei < curTrainSetting.xiuWeiNeed.ToUInt64())
        //{
        //    ////如果有事件 不显示
        //    //if (singleMapEventData != null)
        //    //{
        //    //    trans_xiuWeiDanParent.gameObject.SetActive(false);
        //    //    //btn_mapEventXiuLian.gameObject.SetActive(true);

        //    //}
        //    //else
        //    //{
        //    //    //btn_mapEventXiuLian.gameObject.SetActive(false);

        //    //}
        //    trans_xiuWeiDanParent.gameObject.SetActive(true);

        //    btn_breakThrough.gameObject.SetActive(false);
        //}
        ////可以突破
        //else
        //{
        //    trans_xiuWeiDanParent.gameObject.SetActive(false);

        //    ////能突破 如果有事件 则显示事件修炼
        //    //if (singleMapEventData != null)
        //    //{
        //    //    //btn_mapEventXiuLian.gameObject.SetActive(true);
        //    //    btn_breakThrough.gameObject.SetActive(false);
        //    //}
        //    //else
        //    //{



        //    //}
        //    //btn_mapEventXiuLian.gameObject.SetActive(false);
        //    btn_breakThrough.gameObject.SetActive(true);
        //    //ShowPoJingDanParent();
        //}
    }

    /// <summary>
    /// 使用了突破丹
    /// </summary>
    void OnUseBreakDan()
    {
        RefreshXiuWeiShow();
    }
    /// <summary>
    /// 突破失败
    /// </summary>
    void OnFailStudentBreakthrough()
    {
        RefreshXiuWeiShow();
    }
    //void  ShowPoJingDanParent()
    //{
    //    int baseRate = curTrainSetting.successRate.ToInt32();
    //    if (baseRate >= 100)
    //    {
    //        trans_poJingDanParent.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        trans_poJingDanParent.gameObject.SetActive(true);
    //        //突破丹
    //        ShowAllPoJingDan();

    //        txt_poJingDan.SetText("可服用丹药(" + RoleManager.Instance._CurGameInfo.playerPeople.curEatedDanNum + "/2)");
    //    }
    //}


    /// <summary>
    /// 成功修炼
    /// </summary>
    void OnSuccessXiuLian()
    {
        //if (singleMapEventData != null)
        //{
        //    singleMapEventData = null;
        //    trans_usedUpMapEvent.gameObject.SetActive(true);
        //    img_predictBarInMapEventXiuLian.gameObject.SetActive(false);
        //    //btn_xiuLian.gameObject.SetActive(false);

        //}
        //else
        //{

        //}
        bool regenerateDan = false;
        //用丹修炼
        for (int i = xiuWeiDanList.Count - 1; i >= 0; i--)
        {
            XiuWeiDanItemView view = xiuWeiDanList[i];
            ItemData data = view.GetItemData();
            if (ItemManager.Instance.CheckIfHaveItemBySettingId(data.settingId))
            {
                view.RefreshShow();
            }
            else
            {
                PanelManager.Instance.CloseSingle(view);
                regenerateDan = true;
            }
        }
        RefreshXiuWeiShow(regenerateDan);

        TrainSetting trainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex];
        txt_exp.SetText(RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei + "/" + trainSetting.XiuWeiNeed);
    }
    /// <summary>
    /// 修为进行中
    /// </summary>
    void OnProcessXiuWei()
    {
        //if (GameTimeManager.Instance.lastAddXiuWeiTimer == 0)
        //{
        //    img_trainBar.DOKill();
        //    img_trainBar.fillAmount = GameTimeManager.Instance.lastAddXiuWeiTimer / 6;

        //}
        //else     
        //    img_trainBar.DOFillAmount(GameTimeManager.Instance.lastAddXiuWeiTimer / 6f, 1f);


        RefreshXiuWeiShow();
    }

    void OnSuccessBreakThrough(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        //AddSingle<levelupp>(trans_effectParent);
        int curIndex = p.trainIndex;
        curTrainSetting = DataTable._trainList[curIndex];
        List<SinglePropertyData> proAdd = args[1] as List<SinglePropertyData>;
        long before = (long)args[2];
        long after = (long)args[3];
        //StartCoroutine(ShowTxtAddAnim(proAdd, before,after));
        //中境界突破成功 等级-1为trainindex
        if (curIndex % 10 == 0)
        {
 
            Debug.Log("中境界突破成功");
            //大境界突破成功
            if (curIndex % 30 == 0)
            {
                Debug.Log("大境界突破成功");

            }

            PanelManager.Instance.OpenPanel<BreakThroughResPanel>(PanelManager.Instance.trans_layer2, p, proAdd);
             
        }
    }

    #endregion

    public void LocateStudent(PeopleData p)
    {
        if (p.talent == (int)StudentTalent.LianGong)
        {
            btn_bigTagList[3].onClick.Invoke();

        }else if (p.talent != (int)StudentTalent.None)
        {
            btn_bigTagList[2].onClick.Invoke();

        }
        else if (p.talent == (int)StudentTalent.None)
        {
            btn_bigTagList[1].onClick.Invoke();

        }
        for(int i = 0; i < curShowStudentViewList.Count; i++)
        {
            if (curShowStudentViewList[i].peopleData.onlyId == p.onlyId)
            {
                PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, curShowStudentViewList[i].gameObject);
                break;
            }
        }
    }

    #region 红点
    void InitRedPoint()
    {
        for(int i = 0; i < bigTagRedPointList.Count; i++)
        {           
            RedPointManager.Instance.SetRedPointUI(bigTagRedPointList[i], RedPointType.MainPanel_Btn_Student_BigTag, i);
        }
        for (int i = 0; i < smallTagRedPointList.Count; i++)
        {
            RedPointManager.Instance.SetRedPointUI(smallTagRedPointList[i], RedPointType.MainPanel_Btn_Student_BigTag_SmallTag, (int)smallTagTalentList[i]);
        }
        if (curChoosedP != null)
        {
            //RedPointManager.Instance.SetRedPointUI(obj_skillUpgradeRedPoint, RedPointType.MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade, (int)(ulong)curChoosedP.onlyId);
            RedPointManager.Instance.SetRedPointUI(obj_xueMaiRedPoint, RedPointType.MainPanel_Btn_Student_InfoBigStudentView_XueMai, (int)(ulong)curChoosedP.onlyId);

        }


    }

    void OnRefreshRedPoint(object[] args)
    {
        InitRedPoint();
    }
    #endregion
  
    /// <summary>
    /// 移除弟子
    /// </summary>
    /// <param name="p"></param>
    void OnRemoveStudent()
    {
        OnBigTagClick(curChoosedBigTagIndex);
    }

    public void btn_color(Button btn_select,List<Button> btns) {
        for (int i = 0; i < btns.Count; i++) {
            Outline[] outlines = btns[i].GetComponentsInChildren<Outline>();
            if (btn_select == btns[i])
            {
                btn_select.GetComponent<Image>().sprite = s_select_img;
                btn_select.GetComponent<Image>().color = Color.white;
                btn_select.GetComponentInChildren<Text>().color = select_color;
                btn_select.GetComponentInChildren<Outline>().enabled = true;
                for (int j = 0;j< outlines.Length; j++)
                {
                    outlines[j].enabled = true;
                }
            }
            else {
                btns[i].GetComponent<Image>().color = txtColor_wirt;
                btns[i].GetComponentInChildren<Text>().color = txtColor_gray;
                for (int j = 0; j < outlines.Length; j++)
                {
                    outlines[j].enabled = false;
                }
            }
        }
    }
    public List<Button> btns = new List<Button>();
    #region 弟子详情
    /// <summary>
    /// 显示信息面板
    /// </summary>
    public void ShowStudentInfoPanel()
    {
        curInfoTagType = SingleStudentInfoTagType.Info;

        btn_color(btn_infoTag, btns);

        trans_infoPanel.gameObject.SetActive(true);
        tarn_content.gameObject.SetActive(true);
        //trans_equipPanel.gameObject.SetActive(false);
        trans_socialPanel.gameObject.SetActive(false);

        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);

        PanelManager.Instance.CloseAllSingle(trans_proGrid);
        //PanelManager.Instance.CloseAllSingle(trans_cost);

        ShowXiuLianPanel();

     
        if(curChoosedP.talent==(int)StudentTalent.LianGong
            || curChoosedP.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
        {
            Baricon.sprite = ConstantVal.BarIcon((YuanSuType)curChoosedP.yuanSu);
            for (int i = 0; i < curChoosedP.curBattleProList.Count; i++)
            {
                PropertySetting setting = DataTable.FindPropertySetting(curChoosedP.curBattleProList[i].id);
                if (setting.AlwaysShow=="1") 
                    PanelManager.Instance.OpenSingle<BigStudentSinglePropertyView>(trans_proGrid, curChoosedP.curBattleProList[i].id, curChoosedP.curBattleProList[i].num, (Quality)(int)curChoosedP.curBattleProList[i].quality);
            }
        }
        else
        {
            for (int i = 0; i < curChoosedP.propertyList.Count; i++)
            {
                PanelManager.Instance.OpenSingle<BigStudentSinglePropertyView>(trans_proGrid, (int)curChoosedP.propertyList[i].id, (int)curChoosedP.propertyList[i].num, (Quality)(int)curChoosedP.propertyList[i].quality);
            }
        }
      
      
        InitRedPoint();

        //觉醒
        if (TaskManager.Instance.guide_studentJueXing)
        {
            if (curChoosedP.talent == (int)StudentTalent.None)
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[curChoosedP.studentLevel - 1];
                int expLimit = setting.NeedExp.ToInt32();
                if (curChoosedP.studentCurExp >= expLimit)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(btn_talentTest.gameObject);
                }

            }
        }
        else if (TaskManager.Instance.guide_studentTuPo)
        {
            //找可以突破的弟子来点击

            if (curChoosedP.talent == (int)StudentTalent.LianGong)
            {
                if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(curChoosedP))
                {
                    //修武按钮
                    PanelManager.Instance.ShowTaskGuidePanel(btn_breakThrough.gameObject);
                }

            }
            else if (curChoosedP.talent != (int)StudentTalent.None)
            {
                if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(curChoosedP))
                {
                    //生产按钮
                    PanelManager.Instance.ShowTaskGuidePanel(btn_breakThrough.gameObject);
                }
            }

        }
        //功法
        else if (TaskManager.Instance.guide_studySkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_skillTag.gameObject);
        }
        //学习功法
        else if (TaskManager.Instance.guide_equipSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_skillTag.gameObject);
        }  
        //升级功法
        else if (TaskManager.Instance.guide_upgradeSkill)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_skillTag.gameObject);
        }    
        //强化法器
        else if (TaskManager.Instance.guide_intenseEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_weaponTag.gameObject);
        }
    }


    public void ShowCurWork(PeopleData p)
    {
        if (txt_curWork != null)
        {
            if (p.studentStatusType == (int)StudentStatusType.DanFarmWork
    || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
    || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
            {
                SingleDanFarmData singleDanFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmOnlyId];
                DanFarmSetting setting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
                txt_curWork.SetText(setting.Name + "驻守");
            }
            else if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {
                txt_curWork.SetText("正在秘境探险");
            }
            else if (p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                txt_curWork.SetText("已上阵");
            }
            else
            {
                txt_curWork.SetText("");
            }
        }
    }

    /// <summary>
    /// 显示技能面板
    /// </summary>
    public void ShowSkillPanel()
    {
        curInfoTagType = SingleStudentInfoTagType.Skill;

        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);
        PanelManager.Instance.CloseAllPanel(parent_xuemai);

        btn_color(btn_skillTag, btns);

        PanelManager.Instance.OpenPanel<SkillPanel>(trans_sonPanelParent, curChoosedP);
        
        trans_socialPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示装备面板
    /// </summary>
    public void ShowEquipPanel()
    {
        curInfoTagType = SingleStudentInfoTagType.Equip;

        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);
        PanelManager.Instance.CloseAllPanel(parent_xuemai);
        PanelManager.Instance.OpenPanel<EquipPanel>(trans_sonPanelParent, curChoosedP);

        btn_color(btn_weaponTag, btns);

        trans_socialPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示社交面板
    /// </summary>
    public void ShowSocialPanel()
    {
        curInfoTagType = SingleStudentInfoTagType.Social;

        

        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);

        btn_color(btn_social, btns);
        //trans_infoPanel.gameObject.SetActive(false);
        // trans_equipPanel.gameObject.SetActive(false);
        trans_socialPanel.gameObject.SetActive(true);
        btn_haoGan.onClick.Invoke();
    }

    /// <summary>
    /// 显示有好感的人
    /// </summary>
    void ShowHaoGan()
    {
        trans_scroll.gameObject.SetActive(true);
        trans_record.gameObject.SetActive(false);
        ClearCertainParentAllSingle<SingleHaoGanDuStudentView>(trans_grid);

        for (int i = 0; i < curChoosedP.socializationData.knowPeopleList.Count; i++)
        {
            PeopleData knowP = StudentManager.Instance.FindStudent(curChoosedP.socializationData.knowPeopleList[i]);
            if (StudentManager.Instance.FindHaoGanDu(curChoosedP, knowP) >= 0)
            {
                AddSingle<SingleHaoGanDuStudentView>(trans_grid, curChoosedP, knowP);
            }
        }

    }


    /// <summary>
    /// 显示有仇的人
    /// </summary>
    void ShowHate()
    {
        trans_scroll.gameObject.SetActive(true);
        trans_record.gameObject.SetActive(false);

        ClearCertainParentAllSingle<SingleHaoGanDuStudentView>(trans_grid);

        for (int i = 0; i < curChoosedP.socializationData.knowPeopleList.Count; i++)
        {
            PeopleData knowP = StudentManager.Instance.FindStudent(curChoosedP.socializationData.knowPeopleList[i]);
            if (StudentManager.Instance.FindHaoGanDu(curChoosedP, knowP) < 0)
            {
                AddSingle<SingleHaoGanDuStudentView>(trans_grid, curChoosedP, knowP);
            }
        }

    }

    /// <summary>
    /// 显示记录
    /// </summary>
    void ShowRecord()
    {
        trans_scroll.gameObject.SetActive(false);
        trans_record.gameObject.SetActive(true);

        ClearCertainParentAllSingle<SingleSocializationRecordVew>(trans_recordGrid);
        for (int i = curChoosedP.socializationData.socialRecordList.Count - 1; i >= 0; i--)
        {
            SocializationRecordData recordData = curChoosedP.socializationData.socialRecordList[i];
            AddSingle<SingleSocializationRecordVew>(trans_recordGrid, recordData);
        }
    }
    #endregion

    /// <summary>
    /// 顶部ui
    /// </summary>
    void UpViewShow()
    {
        if (curChoosedP == null)
            return;
        ShowName();


        //PanelManager.Instance.CloseAllSingle(trans_showStudent);

        //PanelManager.Instance.OpenSingle<SingleStudentView>(trans_showStudent, curChoosedP);
        StudentManager.Instance.SetTouxiang(img_student_icon, curChoosedP);
        ShowCurWork(curChoosedP);
    }
    /// <summary>
    /// 显示姓名
    /// </summary>
    void ShowName()
    {
        txt_studentName.SetText(curChoosedP.name);
        //ClearCertainParentAllSingle<NameWordView>(trans_nameGrid);
        //float singleWidth = 0;
        //for (int i = 0; i < curChoosedP.name.Length; i++)
        //{
        //    char word = ((string)curChoosedP.name)[i];
        //    NameWordView view= AddSingle<NameWordView>(trans_nameGrid, word);
        //    singleWidth = view.GetComponent<RectTransform>().sizeDelta.x;
        //}
        //btn_changeName.transform.localPosition = new Vector2(trans_nameGrid.localPosition.x + curChoosedP.name.Length* singleWidth/2+singleWidth/4, trans_nameGrid.localPosition.y);
    }
    /// <summary>
    /// 弟子得到经验值
    /// </summary>
    public void OnGetStudentExp()
    {
        if(curChoosedP != null)
        {
            UpViewShow();
            RefreshXiuWeiShow(false);
            if (curChoosedBigTagIndex==(int)SingleStudentInfoTagType.Info)
            {
                ShowStudentInfoPanel();
            }
        }
       
    
    }

    //RegisterEvent(TheEventType.SkillUpgrade, OnUpgradeSkill);

    ///// <summary>
    ///// 技能升级成功
    ///// </summary>
    //public void OnUpgradeSkill(object[] param)
    //{
    //    PeopleData p = param[0] as PeopleData;
      
    //        UpViewShow();
    //        if (trans_infoPanel.gameObject.activeInHierarchy)
    //        {
    //            ShowStudentInfoPanel();
    //        }
        

    //}
    /// <summary>
    /// 突破成功
    /// </summary>
    public void OnBreakThrough(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        if (p.onlyId == curChoosedP.onlyId)
        {
            curChoosedP = p;
            curTrainSetting = DataTable._trainList[p.trainIndex];
            UpViewShow();
            if (trans_infoPanel.gameObject.activeInHierarchy)
            {
                ShowStudentInfoPanel();
            }
        }

    }
    ///// <summary>
    ///// 赠送装备
    ///// </summary>
    //public void OnSendItem(object[] param)
    //{
    //    PeopleData p = param[0] as PeopleData;
    //    if (p.onlyId == curChoosedP.onlyId)
    //    {
    //        curChoosedP = p;

    //        OnBigTagClick(curChoosedBigTagIndex);
            

    //        UpViewShow();
    //        if (trans_infoPanel.gameObject.activeInHierarchy)
    //        {
    //            ShowStudentInfoPanel();
    //        }
    //        if (trans_sendPanel.gameObject.activeInHierarchy)
    //        {
    //            ShowSendPanel();
    //        }
    //    }

    //}

    public void OnReceiveChooseStudentEvent(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        UpViewShow();
        if (trans_infoPanel.gameObject.activeInHierarchy)
        {
            ShowStudentInfoPanel();
        }
    }

    public void OnClickedStudent(PeopleData p)
    {
        for(int i=0;i< curShowStudentViewList.Count; i++)
        {
            InfoPanelStudentView view = curShowStudentViewList[i];
            if (view.peopleData.onlyId == p.onlyId)
                view.OnChoose(true);
            else
                view.OnChoose(false);

        }
        OnChoosedStudent(p);
       // trans_allStudentGrid.GetComponent<RectTransform>().ForceUpdateRectTransforms();
        LayoutRebuilder.ForceRebuildLayoutImmediate(trans_allStudentGrid.GetComponent<RectTransform>());

        if (TaskManager.Instance.guide_studentTuPo)
        {
            if (curChoosedP.talent == (int)StudentTalent.LianGong)
            {
                if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(curChoosedP))
                {
                    //修武按钮
                    PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
                }
            }
        }
        else if(TaskManager.Instance.guide_studentJueXing)
        {
            if (curChoosedP.talent == (int)StudentTalent.None)
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[curChoosedP.studentLevel - 1];
                int expLimit = setting.NeedExp.ToInt32();
                if (curChoosedP.studentCurExp >= expLimit)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
                }
            }
        }

     
    }

    public void OnStudentInfoClickStudent(PeopleData p)
    {
        trans_studentDetailPanel.gameObject.SetActive(true);
        curChoosedP = p;
        curTrainSetting = DataTable._trainList[p.trainIndex];
        UpViewShow();
        ShowStudentInfoPanel();
  
        if (TaskManager.Instance.guide_equipEquip)
        {
            if (curChoosedP.talent == (int)StudentTalent.LianGong)
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_weaponTag.gameObject);

            }
        }
    }

    /// <summary>
    /// 选择了弟子
    /// </summary>
    public void OnChoosedStudent(PeopleData p)
    {
        curChoosedP = p;
        curTrainSetting = DataTable._trainList[p.trainIndex];
        UpViewShow();
        ClearCertainParentAllSingle<SingleViewBase>(grid_infoBigStudentViewGrid);
        curChoosedInfoBigStudentView = AddSingle<InfoBigStudentView>(grid_infoBigStudentViewGrid, p, this);
        for (int i = 0; i < curShowStudentViewList.Count; i++)
        {
            InfoPanelStudentView view = curShowStudentViewList[i];
            if (view.peopleData.onlyId == p.onlyId)
            {
                view.OnChoose(true);
            }
            else
            {
                view.OnChoose(false);
            }
        }
        if(curChoosedP.talent==(int)StudentTalent.LianGong
            || curChoosedP.isPlayer)
        {
            btn_weaponTag.gameObject.SetActive(true);
            btn_skillTag.gameObject.SetActive(true);
        }  
        else
        {
            btn_weaponTag.gameObject.SetActive(false);
            btn_skillTag.gameObject.SetActive(false);

        }
        AdjustTagButtonWidths();
        //觉醒
       if (TaskManager.Instance.guide_studentJueXing)
        {


            if (p.talent == (int)StudentTalent.None)
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                int expLimit = setting.NeedExp.ToInt32();
                if (p.studentCurExp >= expLimit)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
                }

            }

        }
        else if (TaskManager.Instance.guide_studentTuPo)
        {
            //找可以突破的弟子来点击
            if (p.talent == (int)StudentTalent.LianGong)
            {
                if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(p))
                {
                    //修武按钮
                    PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
                }

            }
            else if (p.talent != (int)StudentTalent.None)
            {
                if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(p))
                {
                    //生产按钮
                    PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
                }
            }

        }
        //强化法器
        else if (TaskManager.Instance.guide_intenseEquip)
        {
            PanelManager.Instance.ShowTaskGuidePanel(curChoosedInfoBigStudentView.btn_info.gameObject);
        }
    }
    
    /// <summary>
    /// 大标签
    /// </summary>
    void OnBigTagClick(int index)
    {
        PanelManager.Instance.CloseTaskGuidePanel();

        for (int i = 0; i < btn_bigTagList.Count; i++)
        {
            Button btn = btn_bigTagList[i];
            if (i == index)
            {
                btn.GetComponent<Image>().sprite = select_img;
                btn.GetComponentInChildren<Text>().color = txtColor_gray;
            }
            else
            {
                btn.GetComponent<Image>().sprite = unselect_img;
                btn.GetComponentInChildren<Text>().color = unselect_color;//185,179,169,255
                //btn.GetComponentInChildren<OutlineEx>().OutlineWidth = 3;
            }

        }
        //筛选出大标签的弟子
        curBigTagStudentList = StudentManager.Instance.FindBigTagStudentList((StudentBigTag)index);
        curChoosedBigTagIndex = index;
        
        //有小标签
        if (index == 2)
        {
            curShowStudentList = curBigTagStudentList;
            ShowAllStudent();
            trans_smallTag.gameObject.SetActive(true);
        }
        else
        {
            btn_smallTagList[0].onClick.Invoke();
            trans_smallTag.gameObject.SetActive(false);
        }

        if (curShowStudentViewList.Count > 0)
        {
            int clickIndex = 0;
            if (curChoosedP != null)
            {
                for(int i = 0; i < curShowStudentViewList.Count; i++)
                {
                    PeopleData theP = curShowStudentViewList[i].peopleData;
                    if (theP.onlyId == curChoosedP.onlyId)
                    {
                        clickIndex = i;
                        break;
                    }
                }
            }
            curShowStudentViewList[clickIndex].btn.onClick.Invoke();
        }
        else
        {
            ClearCertainParentAllSingle<SingleViewBase>(grid_infoBigStudentViewGrid);
            curChoosedInfoBigStudentView = null;
            curChoosedP = null;
            curTrainSetting = null;
        }
    }

    /// <summary>
    /// 点击了小标签
    /// </summary>
    void OnSmallTagClick(int index)
    {
        PanelManager.Instance.CloseTaskGuidePanel();
        for(int i = 0; i < btn_smallTagList.Count; i++)
        {
            Button btn=btn_smallTagList[i];
            Outline[] outlines = btn.GetComponentsInChildren<Outline>();
            if (index == i)
            {
                //btn.GetComponent<Image>().sprite = select_img;
                btn.GetComponentInChildren<Text>().color = new Color32(209,203,187,255);
                for (int j = 0; j < outlines.Length; j++)
                {
                    outlines[j].enabled = true;
                }
            }
            else
            {
                //btn.GetComponent<Image>().sprite = unselect_img;
                btn.GetComponentInChildren<Text>().color = txtColor_gray;
                for (int j = 0; j < outlines.Length; j++)
                {
                    outlines[j].enabled = false;
                }
            }
        }

        StudentTalent talent= smallTagTalentList[index];
        //点第一个小标签
        curShowStudentList = StudentManager.Instance.FindTalentStudentList(curBigTagStudentList, talent);
        if (curChoosedBigTagIndex == 0)
        {
            curShowStudentList.Insert(0,RoleManager.Instance._CurGameInfo.playerPeople);
        }
        ShowAllStudent();

    }

    public void ShowAllStudent()
    {
         ClearCertainParentAllSingle<SingleViewBase>(trans_allStudentGrid);

        //排序

        curShowStudentViewList.Clear();
        //trans_chooseStudent.gameObject.SetActive(true);
        List<PeopleData> showStudentList = new List<PeopleData>();

        //排序筛选 
        for (int i = 0; i < curShowStudentList.Count; i++)
        {
            PeopleData p = curShowStudentList[i];
            showStudentList.Add(p);
        }
        for (int i = 0; i < showStudentList.Count - 1; i++)
        {
            for (int j = 0; j < showStudentList.Count - 1 - i; j++)
            {
                //后面的有红点，则二者交换
                if (RedPointManager.Instance.GetRedPointVal(RedPointType.MainPanel_Btn_Student_InfoBigStudentView, (int)(ulong)showStudentList[j + 1].onlyId))
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
                //后面的更厉害，则二者交换
                if ( RoleManager.Instance.CalcZhanDouLi(showStudentList[j + 1])>
                    RoleManager.Instance.CalcZhanDouLi(showStudentList[j]))
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
                //后面的品级更高，则二者交换
                if (showStudentList[j + 1].studentRarity >
                    showStudentList[j].studentRarity)
                {
                    PeopleData temp = showStudentList[j];
                    showStudentList[j] = showStudentList[j + 1];
                    showStudentList[j + 1] = temp;
                }
            }
        }
        if(curChoosedBigTagIndex== (int)StudentBigTag.LianGong)
        {
            for (int i = 0; i < showStudentList.Count - 1; i++)
            {
                for (int j = 0; j < showStudentList.Count - 1 - i; j++)
                {
                    //后面的战斗力更高，则二者交换
                    if (RoleManager.Instance.CalcZhanDouLi(showStudentList[j + 1]) >
                        RoleManager.Instance.CalcZhanDouLi(showStudentList[j]))
                    {
                        PeopleData temp = showStudentList[j];
                        showStudentList[j] = showStudentList[j + 1];
                        showStudentList[j + 1] = temp;
                    }
                }
            }
        }
        //掌门第一个
        if (curChoosedBigTagIndex == (int)StudentBigTag.None)
        {
            for (int i = 0; i < showStudentList.Count - 1; i++)
            {
                for (int j = 0; j < showStudentList.Count - 1 - i; j++)
                {
                    //后面的战斗力更高，则二者交换
                    if (showStudentList[j + 1].onlyId==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
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
            InfoPanelStudentView view = AddSingle<InfoPanelStudentView>(trans_allStudentGrid, showStudentList[i],this);
            curShowStudentViewList.Add(view);
        }

        if (TaskManager.Instance.guide_studentTuPo)
        {
            //找可以突破的弟子来点击
            for (int i = 0; i < curShowStudentViewList.Count; i++)
            {
                PeopleData p = curShowStudentViewList[i].peopleData;
                if (p.talent == (int)StudentTalent.LianGong)
                {
                    if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(p))
                    {
                        //修武按钮
                         PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, curShowStudentViewList[i].btn.gameObject);
                        break;
                    }

                }
                else if (p.talent != (int)StudentTalent.None)
                {
                    if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(p))
                    {
                        //生产按钮
                        PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, curShowStudentViewList[i].btn.gameObject);
                        break;
                    }
                }
            }
        }
        if (TaskManager.Instance.guide_studentJueXing)
        {
            //找可以觉醒的弟子来点击
            for (int i = 0; i < curShowStudentViewList.Count; i++)
            {
                PeopleData p = curShowStudentViewList[i].peopleData;

                if (p.talent == (int)StudentTalent.None)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int expLimit = setting.NeedExp.ToInt32();
                    if (p.studentCurExp >= expLimit)
                    {
                        PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, curShowStudentViewList[i].btn.gameObject);
                    }

                }
            }
        }
    }
    /// <summary>
    /// 移除了天赋
    /// </summary>
    /// <param name="args"></param>
    void OnRemoveTalent(object[] args)
    {
        //如果标签页是新人则再次点击标签页 以刷新grid显示 否则
        OnBigTagClick(curChoosedBigTagIndex);

        //刷新当前弟子
        UpViewShow();
        if (trans_infoPanel.gameObject.activeInHierarchy)
        {
            ShowStudentInfoPanel();
        }
        //else if (trans_equipPanel.gameObject.activeInHierarchy)
        //{
        //    ShowEquipPanel();

        //}
    }

    /// <summary>
    /// 赋予了天赋
    /// </summary>
    /// <param name="args"></param>
    void OnEndowTalent(object[] args)
    {
        //如果标签页是新人则再次点击标签页 以刷新grid显示 否则
        PeopleData p = args[0] as PeopleData;
        ClickCertainStudent(p);
        //if (p.talent ==(int)StudentTalent.LianGong)
        //{
        //    OnBigTagClick((int)StudentBigTag.LianGong);

        //}
        //else
        //{
        //    OnBigTagClick((int)StudentBigTag.Product);
        //    int index = smallTagTalentList.IndexOf((StudentTalent)(int)p.talent);
        //    OnSmallTagClick(index);
        //}


        //刷新当前弟子
        UpViewShow();
        if (trans_infoPanel.gameObject.activeInHierarchy)
        {
            ShowStudentInfoPanel();
        }
    }
    /// <summary>
    /// 点击特定弟子
    /// </summary>
    public void ClickCertainStudent(PeopleData p)
    {
        if (p.talent == (int)StudentTalent.LianGong)
        {
            OnBigTagClick((int)StudentBigTag.LianGong);
        }
        else
        {
            OnBigTagClick((int)StudentBigTag.Product);
            int index = smallTagTalentList.IndexOf((StudentTalent)(int)p.talent);
            OnSmallTagClick(index);
        }
        for(int i = 0; i < curShowStudentViewList.Count; i++)
        {
            InfoPanelStudentView theView = curShowStudentViewList[i];
            if (theView.peopleData.onlyId == p.onlyId)
                theView.btn.onClick.Invoke();
        }
    }
    /// <summary>
    /// 改变性格
    /// </summary>
    /// <param name="args"></param>
    void OnChangeXingGe()
    {
   

        // 只需要刷新顶部展示与信息面板，无需重新构建整个列表（避免选中状态闪烁）
        UpViewShow();

        // 如果当前正处于信息标签页，不必整页重建，避免性能浪费，只保证性格文本已更新
        // ShowStudentInfoPanel 会重新生成属性条等，如果后续性格影响排序再考虑调用
        if (trans_infoPanel.gameObject.activeInHierarchy)
        {
            // 保险策略：性格可能影响某些显示（如属性加成触发），此处仍执行刷新
            ShowStudentInfoPanel();
        }
    }
    /// <summary>
    /// 整容
    /// </summary>
    /// <param name="args"></param>
    void OnZhengRong(object[] args)
    {
        //如果标签页是新人则再次点击标签页 以刷新grid显示 否则
        OnBigTagClick(curChoosedBigTagIndex);

        //刷新当前弟子
        UpViewShow();
        if (trans_infoPanel.gameObject.activeInHierarchy)
        {
            ShowStudentInfoPanel();
        }
    }
    /// <summary>
    /// 自动选择能天赋测试的弟子
    /// </summary>
    public void AutoClickCanTalentTestStudent()
    {
        OnBigTagClick((int)StudentBigTag.New);

        for(int i = 0; i < curShowStudentViewList.Count; i++)
        {
            InfoPanelStudentView view = curShowStudentViewList[i];
            if (view.peopleData.studentCurExp >= 120)
            {
                PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn.gameObject,false);
                break;
            }
        }
    }
    /// <summary>
    /// 自动选择能天赋测试的弟子
    /// </summary>
    public void AutoChooseStudent(ulong onlyId)
    {
        OnBigTagClick((int)StudentBigTag.None);

        for (int i = 0; i < curShowStudentViewList.Count; i++)
        {
            InfoPanelStudentView view = curShowStudentViewList[i];
            if (view.peopleData.onlyId == onlyId)
            {
                PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn.gameObject, false);
                view.btn.onClick.Invoke();
                //view.btn_info.onClick.Invoke();
                break;
            }
     
            
        }
    }
    /// <summary>
    /// 自动选择能突破的弟子
    /// </summary>
    public void AutoClickCanTuPoStudent()
    {
        StudentBigTag bigTag = StudentBigTag.None;
        PeopleData choosedP = null;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent != (int)StudentTalent.None)
            {
                if (p.talent == (int)StudentTalent.LianGong)
                {
                    if (p.trainIndex < DataTable._trainList.Count-1)
                    {
                        int needXiuWei = DataTable._trainList[p.trainIndex].XiuWeiNeed.ToInt32();
                        if (p.curXiuwei >= (ulong)needXiuWei)
                        {
                            choosedP = p;
                            bigTag = StudentBigTag.LianGong;
                            break;
                        }
                    }
                }
                else
                {
                    if (p.studentLevel > 0 && p.studentLevel < DataTable._studentUpgradeList.Count)
                    {
                        StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                        int needExp = setting.NeedExp.ToInt32();
                        if (p.studentCurExp >= needExp)
                        {
                            choosedP = p;
                            bigTag = StudentBigTag.Product;
                            break;
                        }
                    }
                }
      
            }
        }
        if (choosedP != null)
        {
            OnBigTagClick((int)bigTag);
            for (int i = 0; i < curShowStudentViewList.Count; i++)
            {
                InfoPanelStudentView view = curShowStudentViewList[i];
                if (view.peopleData.onlyId == choosedP.onlyId)
                {
                    PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, view.btn.gameObject,false);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 选择某类
    /// </summary>
    public void ShowSingleStudentTypeInfo(SingleStudentInfoTagType type)
    {
        switch (type)
        {
            case SingleStudentInfoTagType.Info:
                ShowStudentInfoPanel();
                break;
            case SingleStudentInfoTagType.Skill:
                tarn_content.gameObject.SetActive(false);
                ShowSkillPanel();
                break;
            case SingleStudentInfoTagType.Equip:
                tarn_content.gameObject.SetActive(false);
                ShowEquipPanel();
                break;
            case SingleStudentInfoTagType.Social:
                tarn_content.gameObject.SetActive(false);
                if (curChoosedP.isPlayer)
                {
                    ShowStudentInfoPanel();

                }
                else
                {
                    ShowSocialPanel();
                }
                break;
        }
    }

   

    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_showStudent);
        PanelManager.Instance.CloseAllSingle(trans_proGrid);
        ClearCertainParentAllSingle<SingleViewBase>(grid_tiaoXiDan);
        ClearCertainParentAllSingle<SingleViewBase>(grid_xiuWeiDan);
        ClearCertainParentAllSingle<SingleViewBase>(grid_infoBigStudentViewGrid);
        PanelManager.Instance.CloseAllPanel(trans_sonPanelParent);
        PanelManager.Instance.CloseAllPanel(parent_xuemai);
        curChoosedP = null;
        curTrainSetting = null;
    }

    public override void OnClose()
    {
        base.OnClose();
        TaskManager.Instance.guide_studentJueXing = false;
        TaskManager.Instance.guide_studentTuPo = false;
        PanelManager.Instance.CloseTaskGuidePanel();
        //这里可能出现引导强化血脉
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex == 10)
        {
            if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString()).ToInt32() == 0)
            {
                PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.LiLian));
                TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString());

            }
        }
        else if (RoleManager.Instance.CandidateYuanSuNum() > RoleManager.Instance._CurGameInfo.playerPeople.curUnlockedYuanSuList.Count)
        {
            //引导解锁元素
            if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.ChangeYuanSu).ToString()).ToInt32() == 0)
            {
                TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.ChangeYuanSu).ToString());
                PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.ChangeYuanSu));
            }
        }
    }

    /// <summary>
    /// 调整信息标签按钮宽度
    /// 当有按钮隐藏时，可见按钮均分总宽度887
    /// 当所有按钮都显示时，使用默认宽度212
    /// </summary>
    void AdjustTagButtonWidths()
    {
        List<Button> tagButtons = new List<Button> { btn_infoTag, btn_weaponTag, btn_skillTag, btn_social };
        int visibleCount = 0;
        foreach (var btn in tagButtons)
        {
            if (btn.gameObject.activeSelf)
                visibleCount++;
        }

        float buttonWidth = visibleCount == tagButtons.Count ? tagButtonDefaultWidth : tagButtonTotalWidth / visibleCount;

        foreach (var btn in tagButtons)
        {
            if (btn.gameObject.activeSelf)
            {
                RectTransform rect = btn.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(buttonWidth, rect.sizeDelta.y);
            }
        }
    }
}

/// <summary>
/// 大标签
/// </summary>
public enum StudentBigTag
{
    None=0,//全部
    New=1,//新人
    Product=2,//生产
    LianGong=3,//修武
    End=4,
}

public enum SingleStudentInfoTagType
{
    None=0,
    Info=1,
    Skill=2,
    Equip=3,
    Social=4,
}
//public enum StudentSmallTag
//{
    
//}