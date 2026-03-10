
using DG.Tweening;
using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SingleFarmView : SingleViewBase
{
    public SingleDanFarmData singleDanFarmData;
    public DanFarmSetting danFarmSetting;

    public Image icon;
    public Image img_range;//范围
    public LongPressAndClickBtnView pressBtn;//长按
    public Transform trans_lock;
    public Image img_longPressProcess;//长按进度
    bool longPress = false;
    float longPressTimer = 0;
    public float longPressMaxTime = 2;
    public Transform trans_moving;//移动的父物体
    public Button btn_confirmMoving;//确定移动
    public Button btn_cancelMoving;//取消移动
    bool moveAvailable;//移动是否合法
    bool choosed = false;
    //public Image img_lock;
    //public Button btn_skeLock;
    public Transform trans_empty;
    public Transform trans_content;//有内容
    public Transform trans_rebuilding;//升级中
    public Image img_rebuildProcess;//正在升级的进度
    public Text txt_rebuilding;//正在升级/建造
    public Transform trans_accomplishImmediately;//直接完成
    public Text txt_accomplishImmediatelyTiliNeed;//直接完成体力需求
    public Button btn_accomplishImmediately;//直接完成
    public Transform trans_valid;//升级完毕可以使用
    public Text txt_name;//建筑名
    //public Transform trans_common;//不需要材料
    //public Transform trans_needMat;//需要材料

    public Button btn_content;//内容按钮
     public Button btn_detail;//详情
    public Button btn_zone;//全力
    public Transform trans_remainZoneTime;
    public Text txt_remainZoneTime;//剩余全力
    //public Button btn_info;//信息

    //public Text txt_efficient;//效率
    public Transform trans_zuoZhenStudentPosParent;//坐镇弟子
    public List<Transform> trans_zuoZhenStudentGridList;//坐镇弟子
    public List<Transform> trans_zuoZhenContentList;//坐镇弟子
    public List<Button> studentStopZuoZhenBtnList;//坐镇弟子取消坐镇按钮
    public List<Button> studentPosBtnList;//弟子坐镇+按钮
    public GameObject obj_fire;//全力


    public Image img_produceBar;//产出bar
    public Transform trans_flyAnimParent;//产出飞物品

    public Button btn_buildNew;//建新的


    public int curChoosedStudentPos;//当前选择的弟子位


    public MountainPanel parentPanel;

  

    //弟子坐镇位置坐标
    public List<Transform> trans_zuoZhenParentList;//坐镇弟子父物体
    //public List<Vector2> zuoZhenPosYiPai = new List<Vector2> {new Vector2(5.3f,18.2f),
    //new Vector2(44.3f,18.2f),
    // new Vector2(79.1f,18.2f),
    // new Vector2(166.5f,18.2f)};
    ////四角
    //public List<Vector2> zuoZhenPosSiJiao = new List<Vector2> {new Vector2(0,21.4f),
    //new Vector2(125,21.4f),
    // new Vector2(0,155.2f),
    // new Vector2(175,155.2f)};

    public GameObject obj_pause;

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        parentPanel = args[1] as MountainPanel;
        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);

        RegisterEvent(TheEventType.StartFarmBuild, StartFarmBuild);
        RegisterEvent(TheEventType.UpgradeDanFarm, OnUpgrade);
        RegisterEvent(TheEventType.DanFarmRebuildProcess, OnDanFarmRebuildProcess);
        RegisterEvent(TheEventType.StopDanFarmRebuildProcess, OnStopDanFarmRebuildProcess);
        RegisterEvent(TheEventType.DanFarmProduceProcess, OnProducingProduct);
        RegisterEvent(TheEventType.DanFarmProduceAItem, OnProduceAItem);
        RegisterEvent(TheEventType.OnZuoZhenStudent, OnZuoZhen);
        RegisterEvent(TheEventType.QuanLiDanFarm, OnQuanLiDanFarm);
        RegisterEvent(TheEventType.StopDanFarmRebuildProcess, OnStopDanFarmRebuildProcess);
        RegisterEvent(TheEventType.EndQuanLiDanFarm, OnStopQuanLiDanFarm);
        RegisterEvent(TheEventType.RemoveDanFarm, OnRemoveDanFarm);
        RegisterEvent(TheEventType.StopZuoZhenStudent, OnSuccessStopZuoZhenStudent);
        RegisterEvent(TheEventType.StartMatDanFarmProduce, StartDanFarmProduce);
        RegisterEvent(TheEventType.StopMatDanFarmProduce, StopDanFarmProduce);
        RegisterEvent(TheEventType.PropertyDecrease,QuanLiShow);
        RegisterEvent(TheEventType.PropertyAdd, QuanLiShow);
        RegisterEvent(TheEventType.UnlockDanFarm, OnUnlockedDanFarm);
        RegisterEvent(TheEventType.ShowUnlockFarmPosStatus, ShowUnlockFarmPosStatus);
        RegisterEvent(TheEventType.OnClickedSingleFarm, OnReceivedClickedFarm);
        RegisterEvent(TheEventType.Moving_overlapRes, OverlapRes);
        RegisterEvent(TheEventType.QuitBuildingMode, OnQuitBuildingMode);
        RegisterEvent(TheEventType.OnMountainScrollMove, OnMountainScrollMove);
        RegisterEvent(TheEventType.ChangeLianDanStatus, OnChangeWorkStatus);
        #region 装备制造相关
        //RegisterEvent(TheEventType.MakeEquipProcessing, ShowEquipProcessChange);

        #endregion
        //addBtnListener(btn_upgrade, () =>
        //{
        //    parentPanel.OnOpenUpgradeDanFarmPanel(singleDanFarmData);
        //});  

        addBtnListener(btn_buildNew, () =>
        {
            if (singleDanFarmData.IsEmpty)
            {
                OnBuildNewClick();
                //parentPanel.OnOpenNewDanFarmPanel(singleDanFarmData.Index);
            }
        });
       

        for (int i = 0; i < studentPosBtnList.Count; i++)
        {
            Button btn = studentPosBtnList[i];
            int posIndex = i;
    
        }

    
        addBtnListener(btn_zone, () =>
        {

            if (LianDanManager.Instance.JudgeIfCanQuanLi(singleDanFarmData))
            {
                int validEmptyCount = 0;
                int validIndex = -1;
                bool haveFreeStudent=false;
                for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
                {
                   if(singleDanFarmData.PosUnlockStatusList[i]
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
                if(validEmptyCount>0
                && haveFreeStudent)
                {
                                         LianDanManager.Instance.OnQuanLiFarm(singleDanFarmData);

                }
                else
                {
                    LianDanManager.Instance.OnQuanLiFarm(singleDanFarmData);

                    //PanelManager.Instance.OpenCommonHint("开启全力将极大提升产出速度，确定开启吗？", () =>
                    //{
                    //}, null);
                }
             
    
            }

        });


 

        addBtnListener(btn_accomplishImmediately, () =>
        {
            if(trans_rebuilding.gameObject.activeInHierarchy
            &&RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count>0
            && RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList[0] == singleDanFarmData.SettingId)
            {
                int single = ConstantVal.tiliReviveMinute * 60/2;
                int needTiLi = Mathf.CeilToInt(singleDanFarmData.RemainTime / (float)single);
                PanelManager.Instance.OpenCommonHint("确定使用" + needTiLi + "点体力加速完成吗？", () =>
                 {
                     if (RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, needTiLi))
                     {
                         RoleManager.Instance.DeProperty(PropertyIdType.Tili, -needTiLi);
                         //立即完成
                         singleDanFarmData.RemainTime = 0;
                     }
                     else
                     {
                         PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
                     }
                 },null);
            
            }

        });
        pressBtn.callBack = OnLongPress;
        pressBtn.endCallBack = OnFinishLongPress;
        pressBtn.dragAction = OnDragging;
        pressBtn.clickCallBack = BeClicked;
        //取消移动
        addBtnListener(btn_cancelMoving, () =>
        {
            transform.localPosition = new Vector2(singleDanFarmData.LocalPos[0], singleDanFarmData.LocalPos[1]);
            EventCenter.Broadcast(TheEventType.OnClickedSingleFarm, null);
        });

        addBtnListener(btn_confirmMoving, () =>
        {
            if (moveAvailable)
            {
                singleDanFarmData.LocalPos = transform.localPosition;
                EventCenter.Broadcast(TheEventType.OnClickedSingleFarm, null);

            }
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (singleDanFarmData.SettingId == 70001) return;
        if(singleDanFarmData.LocalPos != Vector2.zero)
        transform.localPosition = singleDanFarmData.LocalPos;
        if (!singleDanFarmData.Unlocked)
        {
            ShowUnlockedStatus();
        }
        else if(singleDanFarmData.IsEmpty)
        {
            ShowEmpty();
        }
        else
        {
            ShowContent();
        }
        if (danFarmSetting != null)
        {
            if (danFarmSetting.Type == "5")
            {
                img_range.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.RangeIconName);
                img_range.SetNativeSize();
            }
            img_range.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + ConstantVal.bg_lingshu_5);
        }

    }
    private void Update()
    {
        if (longPress)
        {
            longPressTimer += Time.deltaTime;
            img_longPressProcess.fillAmount = longPressTimer / longPressMaxTime;

            //建造模式
            if (longPressTimer >= longPressMaxTime)
            {
                OnFinishLongPress();
                BuildingManager.Instance.EnterOnlyMovingBuildingMode(singleDanFarmData);
                choosed = true;
                BeClicked();
            }
        }
    }
    void OnQuitBuildingMode()
    {
        choosed = false;
        trans_moving.gameObject.SetActive(false);
        img_range.color = new Color(0, 0, 0, 0);
    }
    /// <summary>
    /// 是否冲突结果
    /// </summary>
    /// <param name="args"></param>
    void OverlapRes(object[] args)
    {
        SingleDanFarmData data = args[1] as SingleDanFarmData;
        if (data == singleDanFarmData)
        {
            bool overlap = (bool)args[0];
            if (overlap)
            {
                img_range.color = ConstantVal.unAvailableRed;
            }
            else
            {
                img_range.color = ConstantVal.availableGreen;
            }
            moveAvailable = !overlap;
        }
    
    }
    /// <summary>
    /// 拖动
    /// </summary>
    void OnDragging(PointerEventData data)
    {
        if (BuildingManager.Instance.curMode == MountainMode.Building
            && choosed)
        {
            transform.localPosition = data.position - new Vector2(Screen.width / 2, Screen.height / 2) - (Vector2)parentPanel.trans_content.localPosition;  //new Vector2(Screen.width/2,Screen.height/2);
                                                                                                                                                            //山门的所有ui是否重叠

            //Debug.Log("当前是否重叠：" + RectTransToScreenPos(rt, null).Overlaps(RectTransToScreenPos(TargetRt, null)));
            EventCenter.Broadcast(TheEventType.Moving_overlapSearch, img_range.GetComponent<RectTransform>(),singleDanFarmData);
        }
  

    }

    /// <summary>
    /// 长按
    /// </summary>
    void OnLongPress()
    {
        if (BuildingManager.Instance.curMode != MountainMode.Building)
        {
            longPress = true;
            img_longPressProcess.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 结束长按
    /// </summary>
    void OnFinishLongPress()
    {
       
            longPress = false;
            longPressTimer = 0;
        img_longPressProcess.fillAmount = 0;

        img_longPressProcess.gameObject.SetActive(false);
        
    }

    public void OnClickedFarm()
    {
        PanelManager.Instance.CloseTaskGuidePanel();

        LianDanManager.Instance.OnClickedFarm(singleDanFarmData);



    }
    /// <summary>
    /// 被点击
    /// </summary>
    public void BeClicked()
    {
        EventCenter.Broadcast(TheEventType.OnClickedSingleFarm, singleDanFarmData);

    }
    /// <summary>
    /// 接收点击建筑
    /// </summary>
    void OnReceivedClickedFarm(object[] args)
    {
        if (BuildingManager.Instance.curMode == MountainMode.Building)
        {
            if (args == null)
            {
                transform.localPosition = new Vector2(singleDanFarmData.LocalPos[0], singleDanFarmData.LocalPos[1]);

                trans_moving.gameObject.SetActive(false);
                img_range.color = new Color(0, 0, 0, 0);
                choosed = false;
            }
            else
            {
                SingleDanFarmData data = args[0] as SingleDanFarmData;

                if (data == this.singleDanFarmData)
                {
                    trans_moving.gameObject.SetActive(true);
                    transform.SetAsLastSibling();
                    EventCenter.Broadcast(TheEventType.Moving_overlapSearch, img_range.GetComponent<RectTransform>(),singleDanFarmData);

                    choosed = true;
                }
                else
                {
                    transform.localPosition = new Vector2(singleDanFarmData.LocalPos[0], singleDanFarmData.LocalPos[1]);

                    trans_moving.gameObject.SetActive(false);
                    img_range.color = new Color(0, 0, 0, 0);

                    choosed = false;
                }
            }
        }
        else
        {
            SingleDanFarmData data = args[0] as SingleDanFarmData;
            if(data==this.singleDanFarmData)
            OnClickedFarm();
        }
    
    }

    void OnMountainScrollMove(object[] args)
    {
        Transform landTrans = (Transform)args[0];
        ShowPos();
    }
    /// <summary>
    /// 显示位置 如果这里出现出框bug 那就是锚点设置有问题content的锚点为x0.5y1
    /// </summary>
    public void ShowPos()
    {
        if (!choosed)
            return;
        //世界坐标右上角
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,
         Mathf.Abs(-Camera.main.transform.position.z)));
        //世界坐标左边界
        float leftBorder = Camera.main.transform.position.x - (cornerPos.x - Camera.main.transform.position.x);
        //世界坐标右边界
        float rightBorder = cornerPos.x;
        //世界坐标上边界
        float topBorder = cornerPos.y;
        //世界坐标下边界
        float downBorder = Camera.main.transform.position.y - (cornerPos.y - Camera.main.transform.position.y);



        //index从0到3分别为 左下 左上 右上 右下的世界坐标
        Vector3[] corners = new Vector3[4];
        transform.GetComponent<RectTransform>().GetWorldCorners(corners);
        //width = rightBorder - leftBorder;
        //height = topBorder - downBorder;

        Vector3 leftDownVec = corners[0];//左下
        Vector3 leftUpVec = corners[1];//左上
        Vector3 rightUpVec = corners[2];//右上
        Vector3 rightDownVec = corners[3];//右下

        float myWidth = rightUpVec.x - leftUpVec.x;
        float myHeight = leftUpVec.y - leftDownVec.y;

        if (leftUpVec.y >= topBorder)
        {
            Debug.Log("到达上边界");
            transform.position = new Vector3(transform.position.x, topBorder - myHeight / 2, 0);
        }
        //下
        if (leftDownVec.y <= downBorder)
        {
            Debug.Log("到达下边界");

            transform.position = new Vector3(transform.position.x, downBorder + myHeight / 2, 0);

        }
        //左
        if (leftDownVec.x <= leftBorder)
        {
            Debug.Log("到达左边界");

            transform.position = new Vector3(leftBorder + myWidth / 2, transform.position.y, 0);
        }
        //右
        if (rightDownVec.x >= rightBorder)
        {
            Debug.Log("到达右边界");
            transform.position = new Vector3(rightBorder - myWidth / 2, transform.position.y, 0);

        }
        EventCenter.Broadcast(TheEventType.Moving_overlapSearch, img_range.GetComponent<RectTransform>(), singleDanFarmData);
    }

    /// <summary>
    /// 点击建造新田
    /// </summary>
    void OnBuildNewClick()
    {
        PanelManager.Instance.OpenPanel<NewDanFarmBuildPanel>(PanelManager.Instance.trans_layer2);

    }


    /// <summary>
    /// 显示未解锁状态
    /// </summary>
    public void ShowUnlockedStatus()
    {
        trans_empty.gameObject.SetActive(false);
        trans_content.gameObject.SetActive(false);
        if (ZongMenManager.Instance.JudgeIfCanUnlockFarm())
        {
            trans_lock.gameObject.SetActive(true);

            //img_lock.gameObject.SetActive(false);
            //btn_skeLock.gameObject.SetActive(true);
        }
        else
        {
            trans_lock.gameObject.SetActive(false);

            //img_lock.gameObject.SetActive(true);
            //btn_skeLock.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示空
    /// </summary>
    public void ShowEmpty()
    {
        trans_lock.gameObject.SetActive(false);
        trans_content.gameObject.SetActive(false);
        trans_empty.gameObject.SetActive(true);

    }

    /// <summary>
    /// 显示内容
    /// </summary>
    public void ShowContent()
    {
        switch ((DanFarmStatusType)singleDanFarmData.Status)
        {
            case DanFarmStatusType.Idling:
                ShowIdleDanFarm();

                break;
            case DanFarmStatusType.Working:
                ShowWorkingDanFarm();

                break;
            case DanFarmStatusType.Building:
                trans_empty.gameObject.SetActive(false);
                trans_lock.gameObject.SetActive(false);
                trans_content.gameObject.SetActive(true);
                trans_rebuilding.gameObject.SetActive(true);
                trans_valid.gameObject.SetActive(false);
                img_rebuildProcess.fillAmount = (singleDanFarmData.RebuildTotalTime - singleDanFarmData.RemainTime) / (float)singleDanFarmData.RebuildTotalTime;
                if (RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count > 0
                    && RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList[0] != singleDanFarmData.SettingId)
                {
                    txt_rebuilding.SetText("等待队列");
                    trans_accomplishImmediately.gameObject.SetActive(false);
                }
                else
                {

                }
                break;
            case DanFarmStatusType.Upgrading:
                trans_empty.gameObject.SetActive(false);
                trans_lock.gameObject.SetActive(false);
                trans_content.gameObject.SetActive(true);

                trans_rebuilding.gameObject.SetActive(true);
                trans_valid.gameObject.SetActive(false);
                img_rebuildProcess.fillAmount = (singleDanFarmData.RebuildTotalTime - singleDanFarmData.RemainTime) / (float)singleDanFarmData.RebuildTotalTime;
                if (RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count > 0
             && RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList[0] != singleDanFarmData.SettingId)
                {
                    txt_rebuilding.SetText("等待队列");
                    trans_accomplishImmediately.gameObject.SetActive(false);
                }
                else
                {

                }
                break;
        }

    }


    /// <summary>
    /// 显示idle
    /// </summary>
    void ShowIdleDanFarm()
    {


        ShowAppearance();
        trans_lock.gameObject.SetActive(false);
        trans_empty.gameObject.SetActive(false);
        trans_rebuilding.gameObject.SetActive(false);
        trans_content.gameObject.SetActive(true);
        trans_valid.gameObject.SetActive(true);
        //txt_name.SetText(danFarmSetting.name);

        //txt_des.gameObject.SetActive(false);
        obj_fire.gameObject.SetActive(false);
        //txt_efficient.gameObject.SetActive(false);
        ShowZuoZhenStudentStatus();

        img_produceBar.gameObject.SetActive(false);

        //炼器房
        if (danFarmSetting.Id.ToInt32() == (int)DanFarmIdType.EquipMake)
        {
            //trans_makeEquip.gameObject.SetActive(false);

            //if (RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData != null)
            //{
            //    ShowEquipMake(RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData);
            //}
            //else
            //{
            //    trans_makeEquip.gameObject.SetActive(false);
            //    img_produceBar.gameObject.SetActive(false);
            //}
        }
        if (singleDanFarmData.NeedForeItemId != 0)
        {
            obj_pause.gameObject.SetActive(true);
        }
        else
        {
            obj_pause.gameObject.SetActive(false);

        }
        if(danFarmSetting.Type.ToInt32()==(int)DanFarmType.SpecialJiaCheng)
        {
          
                trans_zuoZhenStudentPosParent.gameObject.SetActive(false);
            
        }
        else
        {
            
                trans_zuoZhenStudentPosParent.gameObject.SetActive(true);
          
        }
        QuanLiShow();

    }

    /// <summary>
    /// 显示外观
    /// </summary>
    void ShowAppearance()
    {
        icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.mountainUIPath + danFarmSetting.UiName);
        icon.SetNativeSize();
    }

    /// <summary>
    /// 显示坐镇弟子情况
    /// </summary>
    void ShowZuoZhenStudentStatus()
    {
        for(int i = 0; i < trans_zuoZhenParentList.Count; i++)
        {
            //if (singleDanFarmData.DanFarmType == (int)DanFarmType.CangJingGe)
            //{
            //    trans_zuoZhenParentList[i].localPosition = zuoZhenPosYiPai[i];
            //}
            //else
            //{
            //    trans_zuoZhenParentList[i].localPosition = zuoZhenPosSiJiao[i];

            //}
        }

        //坐镇弟子情况
        for (int i = 0; i < trans_zuoZhenStudentGridList.Count; i++)
        {
            Transform theTrans = trans_zuoZhenStudentGridList[i];
            PanelManager.Instance.CloseAllSingle(theTrans);

        }

        for (int i = 0; i < singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            bool status = singleDanFarmData.PosUnlockStatusList[i];
            Transform content = trans_zuoZhenContentList[i];
            //未解锁
            if (!status)
            {
                content.gameObject.SetActive(false);
            }
            else
            {
                content.gameObject.SetActive(true);
                ulong onlyId = singleDanFarmData.ZuoZhenStudentIdList[i];
                //判断有没有弟子
                if (onlyId != 0)
                {
                    PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                    PanelManager.Instance.OpenSingle<ZuoZhenDiZiView>(trans_zuoZhenStudentGridList[i], p);
                    studentStopZuoZhenBtnList[i].gameObject.SetActive(true);
                    studentPosBtnList[i].gameObject.SetActive(false);
                }
                else
                {
                    studentStopZuoZhenBtnList[i].gameObject.SetActive(false);
                    studentPosBtnList[i].gameObject.SetActive(true);
                }
            }
        }

    }

    /// <summary>
    /// 显示工作中
    /// </summary>
    void ShowWorkingDanFarm()
    {
        danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);


        ShowAppearance();

        trans_empty.gameObject.SetActive(false);
        trans_lock.gameObject.SetActive(false);
        trans_content.gameObject.SetActive(true);

        trans_rebuilding.gameObject.SetActive(false);
        trans_valid.gameObject.SetActive(true);

        //txt_name.SetText(danFarmSetting.name);

  
        //txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");

        ShowZuoZhenStudentStatus();
        QuanLiShow();
        if (singleDanFarmData.OpenQuanLi)
            obj_fire.gameObject.SetActive(true);
        else
            obj_fire.gameObject.SetActive(false);

        img_produceBar.gameObject.SetActive(true);

        obj_pause.gameObject.SetActive(false);
        ////炼器房
        //if (danFarmSetting.id.ToInt32() == (int)DanFarmIdType.EquipMake)
        //{
        //    ShowEquipMake(RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData);
        //    //trans_makeEquip.gameObject.SetActive(true);
        //}

        if (danFarmSetting.Type.ToInt32() == (int)DanFarmType.SpecialJiaCheng)
        {

            trans_zuoZhenStudentPosParent.gameObject.SetActive(false);

        }
        else
        {

            trans_zuoZhenStudentPosParent.gameObject.SetActive(true);

        }
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
            trans_remainZoneTime.gameObject.SetActive(true);
            txt_remainZoneTime.SetText("剩余" + remainWeek + "周");
        }
        else
        {
            trans_remainZoneTime.gameObject.SetActive(false);
        }

    }
    /// <summary>
    /// 升级
    /// </summary>
    void OnUpgrade(object[] param)
    {
        SingleDanFarmData theData = param[0] as SingleDanFarmData;
        if (theData.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = theData;
            OnOpenIng();
            transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 开始建造
    /// </summary>
    void StartFarmBuild(object[] param)
    {
        SingleDanFarmData data = param[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            ShowContent();
        }

    }

    /// <summary>
    /// 建造过程
    /// </summary>
    void OnDanFarmRebuildProcess(object[] param)
    {
        SingleDanFarmData data = param[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            float process = (data.RebuildTotalTime - data.RemainTime) / (float)data.RebuildTotalTime;
            if (trans_rebuilding.gameObject.activeInHierarchy)
            {
                img_rebuildProcess.DOFillAmount(process, 1f);
                if(RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count>0
                    && RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList[0] != singleDanFarmData.SettingId)
                {
                    txt_rebuilding.SetText("等待队列");
                    trans_accomplishImmediately.gameObject.SetActive(false);
                }
                else
                {
                    string remain= GameTimeManager.Instance.GetYearMonthWeekByDay(data.RemainTime);
                    txt_rebuilding.SetText("剩余" + remain);
                    
                        trans_accomplishImmediately.gameObject.SetActive(true);
                        //1点体力能减这么多天
                        int single = ConstantVal.tiliReviveMinute * 60 / 2;
                        int needTiLi =Mathf.CeilToInt(data.RemainTime / (float)single);
                        txt_accomplishImmediatelyTiliNeed.SetText("-" + needTiLi);

                    
                }
                //txt_rebuilding.SetText("建造中");
                //如果前面有队列 显示等待建造

            }
            else
            {
                trans_rebuilding.gameObject.SetActive(true);
                img_rebuildProcess.DOKill();
                img_rebuildProcess.fillAmount = process;
            }
        }

    }

    /// <summary>
    /// 停止建造
    /// </summary>
    void OnStopDanFarmRebuildProcess(object[] param)
    {
        SingleDanFarmData data = param[0] as SingleDanFarmData;

        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            if (data.Status == (int)DanFarmStatusType.Working)
                ShowWorkingDanFarm();
            else
                ShowIdleDanFarm();

        }
        //其它丹房弟子加成效率可能会影响
        else
        {
            //txt_des.SetText("X" + LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData) + "/月");
            //txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");
        }
    }

    /// <summary>
    /// 正在产出产品
    /// </summary>
    void OnProducingProduct(object[] param)
    {
    
        SingleDanFarmData theData = param[0] as SingleDanFarmData;
        if (theData.OnlyId == singleDanFarmData.OnlyId)
        {
            if (danFarmSetting != null &&
           danFarmSetting.Id == "10006")
            {

            }
            if (singleDanFarmData.DanFarmWorkType !=(int) DanFarmWorkType.Special)
            {
                singleDanFarmData = theData;

                int processDay = (singleDanFarmData.ProcessSpeed - singleDanFarmData.ProcessDanTimer+1);
                float process = processDay / (float)singleDanFarmData.ProcessSpeed;
                if (processDay == 1)
                {
                    img_produceBar.DOKill();
                    img_produceBar.fillAmount = 0;

                }
                img_produceBar.DOFillAmount(process, 1f);
            }
            QuanLiRemainTimeShow();
        }
    }

    /// <summary>
    /// 产出了产品 飞物品
    /// </summary>
    void OnProduceAItem(object[] param)
    {
        SingleDanFarmData theData = param[0] as SingleDanFarmData;
        if (theData.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = theData;
            int num = LianDanManager.Instance.CalcDanPrice(singleDanFarmData);
            ItemData data = new ItemData();
            data.settingId = theData.ProductSettingId;
            data.count = (ulong)num;
            Vector2 pos = (Vector2)trans_flyAnimParent.transform.position;
            if (singleDanFarmData.OpenQuanLi)
            {
                trans_flyAnimParent.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                trans_flyAnimParent.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            }
            PanelManager.Instance.OpenSingle<GetItemFlyUpAnimView>(trans_flyAnimParent, trans_flyAnimParent.transform.position, data);
        }
    }

    /// <summary>
    /// 坐镇弟子
    /// </summary>
    void OnZuoZhen(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            int posIndex = (int)args[1];

            PanelManager.Instance.CloseAllSingle(trans_zuoZhenStudentGridList[posIndex]);
            singleDanFarmData = data;

            PeopleData p = StudentManager.Instance.FindStudent(singleDanFarmData.ZuoZhenStudentIdList[posIndex]);
            PanelManager.Instance.OpenSingle<ZuoZhenDiZiView>(trans_zuoZhenStudentGridList[posIndex], p);

            studentPosBtnList[posIndex].gameObject.SetActive(false);
            studentStopZuoZhenBtnList[posIndex].gameObject.SetActive(true);
            // btn_stopZuoZhen.gameObject.SetActive(true);
        }
        //txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");
        QuanLiShow();
    }
    #region 全力产丹
    /// <summary>
    /// 全力产丹
    /// </summary>
    void OnQuanLiDanFarm(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = data;
            obj_fire.gameObject.SetActive(true);
            //txt_des.SetText("X" + LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData) + "/月");
            //txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");
            btn_zone.gameObject.SetActive(false);
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
            //坐镇弟子情况
            //PanelManager.Instance.CloseAllSingle(trans_zuoZhenStudentGrid);
            //txt_efficient.SetText("效率：" + "+" + LianDanManager.Instance.CalcDanFarmEfficient(singleDanFarmData) + "%");
            //btn_zone.gameObject.SetActive(true);
            obj_fire.gameObject.SetActive(false);
        }
        QuanLiShow();

    }

    #endregion
    /// <summary>
    /// 移除丹房
    /// </summary>
    void OnRemoveDanFarm(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = data;
            OnOpenIng();
            PanelManager.Instance.CloseSingle(this);

        }
    }
    /// <summary>
    /// 取消坐镇
    /// </summary>
    /// <param name="args"></param>
    void OnSuccessStopZuoZhenStudent(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = data;

        }
        OnOpenIng();

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
            singleDanFarmData = data;
            ShowWorkingDanFarm();
            //进度显示
            OnProducingProduct(new object[] { data });
        }
    }

    /// <summary>
    /// 停止产出需要材料的产品
    /// </summary>
    /// <param name="args"></param>
    public void StopDanFarmProduce(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = data;
            ShowIdleDanFarm();
            //img_produceBar.DOKill();
            //img_produceBar.
        }
    }
    ///// <summary>
    ///// 取消坐镇
    ///// </summary>
    ///// <param name="p"></param>
    ///// <returns></returns>
    //bool OnStopZuoZhen(PeopleData p)
    //{

    //    PanelManager.Instance.OpenCommonHint("确定取消" + p.Name + "坐镇该丹房吗？", () =>
    //    {
    //        LianDanManager.Instance.StopZuoZhen(p.OnlyId);
    //        EventCenter.Broadcast(TheEventType.CloseStopZuoZhenStudentChoosePanel);
    //    }, null);

    //    return false;
    //}



    #region  装备制造相关
 
 
     


    #endregion

    /// <summary>
    /// 解锁了丹田
    /// </summary>
    void OnUnlockedDanFarm(object[] args)
    {
        SingleDanFarmData data = args[0] as SingleDanFarmData;
        if (data.OnlyId == singleDanFarmData.OnlyId)
        {
            singleDanFarmData = data;

        }
        OnOpenIng();
    }

    /// <summary>
    /// 显示是否可解锁
    /// </summary>
    void ShowUnlockFarmPosStatus()
    {
        if (!singleDanFarmData.Unlocked)
            ShowUnlockedStatus();
    
    }

    void OnChangeWorkStatus(object[] args)
    {
        SingleDanFarmData farmData = args[0] as SingleDanFarmData;
        if (farmData == singleDanFarmData)
            ShowContent();
    }

 
    public override void Clear()
    {
        base.Clear();
        //坐镇弟子情况
        for (int i = 0; i < trans_zuoZhenStudentGridList.Count; i++)
        {
            Transform theTrans = trans_zuoZhenStudentGridList[i];
            PanelManager.Instance.CloseAllSingle(theTrans);

        }
        trans_moving.gameObject.SetActive(false);
        img_longPressProcess.gameObject.SetActive(false);
        img_longPressProcess.fillAmount = 0;
        choosed = false;
        img_range.color = new Color32(0, 0, 0, 0);
        longPress = false;
        longPressTimer = 0;
    }
}
