using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Framework.Data;

public class TalentTestPanel : PanelBase
{
    public Transform trans_testing;//测试中
    public Transform trans_testBar;//进度条
    public Image img_testBar;//测试进度条
    public Text txt_testRes;//结果
    public Button btn_closeRes;//关闭结果

    public PeopleData curTestPeople;//当前测试的弟子
    //public bool tianJi = false;//天级觉醒

    public Transform trans_studentGrid;//弟子格子

    public Transform trans_candidateTalent;//备选天赋
    public Transform talentGrid;//备选天赋格子
    public Button btn_adRegenerateTalent;//重新生天赋
    public Transform trans_regenerateNeedMatGrid;//重新生成天赋需要的材料
    public Button btn_tianJiRegenerateTalent;//必得天级天赋
    public Text txt_remainTianJiMatNum;//剩余天级晶石

    public Button btn_testBtn;//测试按钮
    public Text txt_remainZhuanHunMatNum;//剩余转魂丹数量

    public Text txt_remainNum;//剩余次数
    public float processBackSpeed;//测试回退速度
    public float clickAddPercent;//点击增加百分比
    public int curParse;//当前阶段
    public List<float> functionalPercentList = new List<float> { 0.3f, 0.6f, 1f }; //有用的位置
    public List<string> parseIdleAnimNameList = new List<string> { "1", "2", "3" };//点击
    public List<string> parseChangeAnimNameList = new List<string> { "1to2", "2to3", "4boom" };//转换

    public Spine.Unity.SkeletonGraphic ske_test;
    public Transform trans_boomEffectParent;//爆炸父物体 
    public Transform trans_boomEffectPos;//爆炸位置

    public float generateSingleTalentTimer;//生成单个天赋计时器
    public float generateSingleTalentTime=0.5f;//生成单个天赋时间
    public bool waitToGenerateSingleTalent;//等待生成单个天赋

    public List<SingleTalentView> talentViewList = new List<SingleTalentView>();//生成的天赋选择
    #region 曲线移动
    //public Transform trans_startPoint;
    public Transform trans_endPoint;
    public float height;
    public int resolution;//点数量
    public float moveSpeed;//天赋移动速度

    #endregion

    public AudioClipType beforeAudioClipType;
    public Button btn_des;
    public bool tianJi;//是否天级觉醒
    public override void Init(params object[] args)
    {
        base.Init(args);
        curTestPeople = args[0] as PeopleData;
        //if (args.Length >= 2)
        //{
        //    tianJi = (bool)args[1];
        //}
        //else
        //{
        //    tianJi = false;
        //}
        waitToGenerateSingleTalent = false;
        addBtnListener(btn_closeRes, () =>
        {
           
           
        });

        //防止玩家凹存档
        addBtnListener(btn_adRegenerateTalent, () =>
        {
       
        });
        //防止玩家凹存档
        addBtnListener(btn_tianJiRegenerateTalent, () =>
        {
            if (ConstantVal.talentTestMaxADNum - curTestPeople.changeTalentNum <= 0)
            {
                PanelManager.Instance.OpenFloatWindow("已达最大改命次数");
                return;
            }

            OnTianJiRdmExecuted();
        });
        addBtnListener(btn_testBtn, () =>
        {
            img_testBar.DOKill();
            float before = img_testBar.fillAmount;

            img_testBar.DOFillAmount(before + clickAddPercent, 0.1f).OnComplete(() =>
            {
                for (int i = 0; i < functionalPercentList.Count; i++)
                {
                    float theParse = functionalPercentList[i];
                    if (img_testBar.fillAmount >= theParse
                    &&curParse<=i)
                    {
                        //变化
                        curParse++;

                        //炸
                        if (curParse >= 3)
                        {
                            curParse = 3;
                            ske_test.AnimationState.SetAnimation(0, parseChangeAnimNameList[curParse-1], false);
                            trans_testBar.gameObject.SetActive(false);
                            btn_testBtn.gameObject.SetActive(false);
                            generateSingleTalentTimer = 0;
                            waitToGenerateSingleTalent = true;
                        }
                        else
                        {
                            ske_test.AnimationState.SetAnimation(0, parseChangeAnimNameList[curParse-1], false);
                   
                        }
                        //炸
                        AddSingle<MagicSoftExplosionGreen>(trans_boomEffectParent, trans_boomEffectPos.localPosition);
               
                    }
                    else
                    {
                        if (curParse < 3)
                        {
                            ske_test.AnimationState.SetAnimation(0, parseIdleAnimNameList[curParse], false);

                        }

                    }
                }
            });


        });
        addBtnListener(btn_des, () =>
        {
            string str = "";
            for (int i = 1; i < 9; i++)
            {
                 str += StudentManager.Instance.TalentNameByTalent((StudentTalent)i)+":"+ ConstantVal.TalentFunction((StudentTalent)i) + "\n";

            }
            PanelManager.Instance.OpenPanel<NoTransparentTipsPanel>(PanelManager.Instance.trans_layer2, str, (Vector2)btn_des.transform.position);

        });
        RegisterEvent(TheEventType.Appear3Talent, OnAppear3Talent);

        RegisterEvent(TheEventType.EndowTalent, OnEndowTalent);
        RegisterEvent(TheEventType.OnADRerdm, OnADReRdmExecuted);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        //RefreshStudentShow();
        OnStartTest(curTestPeople);
        beforeAudioClipType = AuditionManager.Instance.curAudioClipType;
        AuditionManager.Instance.PlayBGM(AudioClipType.Train);
    }
    private void Update()
    {
        if (waitToGenerateSingleTalent)
        {
            generateSingleTalentTimer += Time.deltaTime;
            if (generateSingleTalentTimer >= generateSingleTalentTime)
            {
                //if (tianJi
                //    &&ItemManager.Instance.CheckIfCangKuItemEnough((int)ItemIdType.TianJiTianFu,1))
                //{
                //    tianJi = false;
                //    ItemManager.Instance.LoseItem((int)ItemIdType.TianJiTianFu, 1);
                //}
                StudentManager.Instance.OnTalentTest(curTestPeople, true,tianJi);
                waitToGenerateSingleTalent = false;
            }
        }
     
    }
    private void FixedUpdate()
    {
        img_testBar.fillAmount -= processBackSpeed;
        
    }

    /// <summary>
    /// 测试中
    /// </summary>
    public void OnStartTest(PeopleData p)
    {
        trans_testing.gameObject.SetActive(false);
        trans_candidateTalent.gameObject.SetActive(false);
        ske_test.gameObject.SetActive(true);
        ske_test.AnimationState.SetAnimation(0, parseIdleAnimNameList[0], false);

        curTestPeople = p;
        trans_testing.gameObject.SetActive(true);
        btn_closeRes.gameObject.SetActive(false);
        txt_testRes.gameObject.SetActive(false);
        trans_testBar.gameObject.SetActive(true);
        
        img_testBar.DOKill();
        img_testBar.fillAmount = 0;
        curParse = 0;
        trans_testBar.gameObject.SetActive(true);
        btn_testBtn.gameObject.SetActive(true);
        txt_remainNum.SetText("(剩余次数" + (ConstantVal.talentTestMaxADNum - curTestPeople.changeTalentNum) + "/" + ConstantVal.talentTestMaxADNum + "次)");
        //img_testBar.DOFillAmount(1, 5).OnComplete(() =>
        //{
        //    StudentManager.Instance.OnTalentTest(p);
        //    //txt_testRes.gameObject.SetActive(true);

        //});
    }

    /// <summary>
    /// 成功赋予了天赋
    /// </summary>
    /// <param name="args"></param>
    public void OnEndowTalent(object[] args)
    {
        string res = "";
        PeopleData p = args[0] as PeopleData;
        if (p.onlyId == curTestPeople.onlyId)
        {
            curTestPeople = p;
       
            res = "觉醒了天赋——" + StudentManager.Instance.TalentNameByTalent((StudentTalent)(int)p.talent);
        }
        trans_testBar.gameObject.SetActive(false);
        txt_testRes.SetText(res);
        txt_testRes.gameObject.SetActive(true);
        btn_closeRes.gameObject.SetActive(true);
               PanelManager.Instance.ClosePanel(this);

        //ChatManager.Instance.AddChat(p, data.content);
    }

    /// <summary>
    /// 出现3个天赋
    /// </summary>
    void OnAppear3Talent(object[] args)
    {
        trans_candidateTalent.gameObject.SetActive(true);
        //btn_adRegenerateTalent.gameObject.SetActive(true);  刷新按钮

        ClearCertainParentAllSingle<SingleConsumeView>(trans_regenerateNeedMatGrid);
        //if (DataTable.FindItemSetting((int)ItemIdType.ZhuanHunDan) != null)
        //{
        //    AddSingle<SingleConsumeView>(trans_regenerateNeedMatGrid, ItemIdType.ZhuanHunDan, 1, ConsumeType.Item);

        //    txt_remainZhuanHunMatNum.SetText("（剩余" + ItemManager.Instance.FindItemCount((int)ItemIdType.ZhuanHunDan) + ")");
        //}

        if (ItemManager.Instance.CheckIfHaveItemBySettingId((int)ItemIdType.TianJiTianFu))
        {
            //btn_tianJiRegenerateTalent.gameObject.SetActive(true);//天级刷新
            txt_remainTianJiMatNum.SetText("（剩余" + ItemManager.Instance.FindItemCount((int)ItemIdType.TianJiTianFu) + ")");

        }
        else
        {
            btn_tianJiRegenerateTalent.gameObject.SetActive(false);
        }
        ClearCurTalent();

        List<StudentTalent> talentList = args[0] as List<StudentTalent>;
        List<Quality> qualityList = args[1] as List<Quality>;
        List<YuanSuType> yuanSuList = args[2] as List<YuanSuType>;

        Quality max = Quality.None;
        int maxIndex = -1;
        //排序
        for(int i = 0; i < talentList.Count; i++)
        {
            StudentTalent theTalent = talentList[i];
            Quality theQuality = qualityList[i];
            if (max <= theQuality)
            {
                max = theQuality;
                maxIndex = i;
            }
        }
        var tmp = talentList[1];
        talentList[1] = talentList[maxIndex];
        talentList[maxIndex] = tmp;

        var tmp2 = qualityList[1];
        qualityList[1] = qualityList[maxIndex];
        qualityList[maxIndex] = tmp2;

        var tmp3 = yuanSuList[1];
        yuanSuList[1] = yuanSuList[maxIndex];
        yuanSuList[maxIndex] = tmp3;

        for (int i = 0; i < talentList.Count; i++)
        {
            SingleTalentView view= AddSingle<SingleTalentView>(talentGrid, talentList[i],qualityList[i], yuanSuList[i],this);
            talentViewList.Add(view);
        }
      
    }

    /// <summary>
    /// 选择特定天赋
    /// </summary>
    /// <param name="studentTalent"></param>
    public void OnChoosedCertainTalent(SingleTalentView singleTalentView)
    {
        for(int i = 0; i < talentViewList.Count; i++)
        {
            SingleTalentView theView = talentViewList[i];
            if (theView == singleTalentView)
            {
                theView.OnChoose(true);
            }
            else
            {
                theView.OnChoose(false);
            }
        }
        return;
 
    }

    /// <summary>
    /// 滑动了天赋
    /// </summary>
    /// <param name="singleTalentView"></param>
    public void OnSlidedTalent(SingleTalentView singleTalentView)
    {
        btn_adRegenerateTalent.gameObject.SetActive(false);
        btn_tianJiRegenerateTalent.gameObject.SetActive(false);
        Vector3 pos = singleTalentView.trans_effectStartPos.position;
        //先收回所有天赋
        ClearCurTalent();
        FlyTalentView view= AddSingle<FlyTalentView>(trans_boomEffectParent, pos,singleTalentView.quality);
        CommonUtil.LocalBezierMove(view.transform, view.transform.localPosition, trans_endPoint.localPosition,resolution, height, () =>{
            view.OnBoom();
            
            StudentManager.Instance.EndowTalentToStudent(curTestPeople, singleTalentView.talent, singleTalentView.quality,singleTalentView.yuanSu);
        });
    }

    /// <summary>
    /// 看广告重新随机
    /// </summary>
    void OnADReRdmExecuted()
    {
        tianJi = false;

        //反作弊存档：
        curTestPeople.lastAppearTalentList.Clear();
        curTestPeople.lastAppearTalentQualityList.Clear();
        curTestPeople.lastAppearYuanSuList.Clear();
        curTestPeople.changeTalentNum++;
        ClearCurTalent();

        OnStartTest(curTestPeople);
    }

    /// <summary>
    /// 天级晶石重新随机
    /// </summary>
    void OnTianJiRdmExecuted()
    {
        return;
 
    }
    /// <summary>
    /// 清除当前天赋
    /// </summary>
    void ClearCurTalent()
    {
   
        ClearCertainParentAllSingle<SingleTalentView>(talentGrid);
        talentViewList.Clear();

    }
    public override void Clear()
    {
        base.Clear();
        tianJi = false;
        PanelManager.Instance.CloseAllSingle(trans_boomEffectParent);
        ClearCurTalent();
    }

    public override void OnClose()
    {
        base.OnClose();
        AuditionManager.Instance.PlayBGM(beforeAudioClipType);

    }
}
