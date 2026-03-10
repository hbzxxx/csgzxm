using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBigStudentView : BigStudentView
{
    public Button btn;
    public Button btn_remove;//逐出
    public Button btn_info;//信息
    public StudentHandlePanel parentPanel;
    public float smallHeight = 300;
    public float bigHeight = 400;
    public RectTransform rect;
    public GameObject obj_redPoint;
    public GameObject obj_redPointInfo;

    //public Transform trans_tupo;//突破
    //public Transform trans_skill;//技能
    //public Transform trans_equip;//装备
    //public Transform trans_consume;//消耗
    //public Button btn_breakThrough;//突破
    //public Button btn_talentTest;//天赋测试
 
    public Transform trans_tuPoEffectParent;
    public float xiTimer = 0;
    public float xiTime = 1;
    public bool startXi = false;

    public Button btn_detail;//属性详情

    public Image img_touxiang;

    public Text role_name;
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as StudentHandlePanel;
        addBtnListener(btn, () =>
        {
            parentPanel.OnClickedStudent(p);
        });
        addBtnListener(btn_info, () =>
         {
             parentPanel.OnStudentInfoClickStudent(p);
         });
        addBtnListener(btn_remove, () =>
        {
            if (p.isPlayer)
            {
                PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.不能将自己逐出宗门));
            }
            PanelManager.Instance.OpenCommonHint("确定将" + p.name + LanguageUtil.GetLanguageText((int)LanguageIdType.逐出宗门) + "吗？", () =>
            {                    StudentManager.Instance.RemoveStudent(p);

                 
             }, null
            );

        });
  
        addBtnListener(btn_detail, () =>
         {
             if (p.talent != (int)StudentTalent.None)
             {
                 if (p.talent == (int)StudentTalent.LianGong)
                 {
                     PanelManager.Instance.OpenPanel<DetailPropertyPanel>(PanelManager.Instance.trans_layer2, p);
                 }
                 else
                 {
                     PanelManager.Instance.OpenPanel<ProductPropertyDesPanel>(PanelManager.Instance.trans_layer2, p);

                 }
             }
         });

        RegisterEvent(TheEventType.RefreshStudentRedPoint, OnRefreshRedPoint);
        RegisterEvent(TheEventType.StudentBreakthroughSuccess, OnSuccessBreakThrough);
        RegisterEvent(TheEventType.FailStudentBreakThrough, OnFailBreakThrough);
        RegisterEvent(TheEventType.OnSendItem, OnSendItem);
        RegisterEvent(TheEventType.OnSuoYao, OnSuoYao);

    }

    void ConfirmRemoveStudent()
    {

    }

    public override void OnOpenIng()
    {
        //base.OnOpenIng();

        //设置头像
        StudentManager.Instance.SetTouxiang(img_touxiang, p);
        OnRedPointShow();
        ShowTupo();
        ShowLV();
        role_name.SetText(p.name);
        //gameObject.name = p.onlyId.ToString();
    }

    private void Update()
    {
        if (startXi)
        {
            xiTimer += Time.deltaTime;
            if (xiTimer >= xiTime)
            {
                if (p.talent != (int)StudentTalent.LianGong)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];

                    startXi = false;
                    List<List<int>> matList = CommonUtil.SplitCfg(setting.NeedMat);
                    for (int i = 0; i < matList.Count; i++)
                    {
                        List<int> singleMat = matList[i];
                        int id = singleMat[0];
                        int num = singleMat[1];
                        if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)num))
                        {
                            PanelManager.Instance.OpenFloatWindow("材料不够");
                            return;
                        }
                    }
                    for (int i = 0; i < matList.Count; i++)
                    {
                        List<int> singleMat = matList[i];
                        int id = singleMat[0];
                        int num = singleMat[1];
                        ItemManager.Instance.LoseItem(id, (ulong)num);

                    }
                }
          
                StudentManager.Instance.BreakThrough(p);
                startXi = false;
            }
        }
    }

  

    /// <summary>
    /// 成功突破
    /// </summary>
    void OnSuccessBreakThrough(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {
            PanelManager.Instance.OpenSingle<dujie_dizidujiechenggong>(trans_tuPoEffectParent, trans_tuPoEffectParent.position);
       
        }
        OnRedPointShow();
        ShowTupo();
    }

    void OnFailBreakThrough(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {
            PanelManager.Instance.OpenSingle<dizidujie_dujieshibai>(trans_tuPoEffectParent, trans_tuPoEffectParent.position);

        }
        OnRedPointShow();
        ShowTupo();
    }
    /// <summary>
    /// 赠送装备
    /// </summary>
    public void OnSendItem(object[] param)
    {
        PeopleData theP = param[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {

            OnRedPointShow();
            ShowTupo();
        }

    }
    /// <summary>
    /// 索要装备
    /// </summary>
    public void OnSuoYao(object[] param)
    {
        PeopleData theP = param[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {

            OnRedPointShow();
            ShowTupo();
        }

    }
    /// <summary>
    /// 开始突破
    /// </summary>
    void StartTuPo()
    {
        xiTimer = 0;
        startXi = true;
        PanelManager.Instance.OpenSingle<dizidujie_xishou>(trans_tuPoEffectParent, trans_tuPoEffectParent.position);
    }


 

    /// <summary>
    /// 显示突破
    /// </summary>
    void ShowTupo()
    {

        //弟子突破
        if (p.talent != (int)StudentTalent.LianGong)
        {
            //trans_skill.gameObject.SetActive(false);
            //trans_equip.gameObject.SetActive(false);
            //PanelManager.Instance.CloseAllSingle(trans_consume);

            StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
            int expLimit = setting.NeedExp.ToInt32();


            if (p.studentCurExp >= expLimit)
            {
                //trans_consume.gameObject.SetActive(true);

                //可以突破
                if (p.talent == (int)StudentTalent.None)
                {
                    //btn_breakThrough.gameObject.SetActive(false);
                    //btn_talentTest.gameObject.SetActive(true);
                    int cost = 1500;//基础是1500
                    cost = Mathf.RoundToInt(cost * ConstantVal.GetValAddByRarity((Rarity)(int)p.studentRarity));
                    //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_consume, (int)ItemIdType.LingShi, cost, ConsumeType.Item);
                    //if (ItemManager.Instance.CheckIfHaveItemBySettingId((int)ItemIdType.TianJiTianFu))
                    //{
                    //    btn_tianJiTalentTest.gameObject.SetActive(true);
                    //}
                    //else
                    //{
                    //    btn_tianJiTalentTest.gameObject.SetActive(false);
                    //}
                }
                else
                {
                    if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(p))
                    {
                        //btn_breakThrough.gameObject.SetActive(true);
                        //btn_talentTest.gameObject.SetActive(false);
                        //btn_tianJiTalentTest.gameObject.SetActive(false);
                        List<List<int>> needMat = CommonUtil.SplitCfg(setting.NeedMat);
                        for (int i = 0; i < needMat.Count; i++)
                        {
                            List<int> singleMat = needMat[i];

                            int consumeId = singleMat[0];
                            int consumeNum = singleMat[1];

                            //PanelManager.Instance.OpenSingle<SingleConsumeView>(trans_consume, consumeId, consumeNum, ConsumeType.Item);
                        }
                    }
                    else
                    {
                        //btn_breakThrough.gameObject.SetActive(true);
                       // btn_talentTest.gameObject.SetActive(false);
                        //btn_tianJiTalentTest.gameObject.SetActive(false);
                    }

                }


            }
            else
            {
                //trans_consume.gameObject.SetActive(false);
               // btn_breakThrough.gameObject.SetActive(false);
               // btn_talentTest.gameObject.SetActive(false);
                //btn_tianJiTalentTest.gameObject.SetActive(false);

            }

        }
        //修武显示修为
        else
        {
            //PanelManager.Instance.CloseAllSingle(trans_skill);
            //trans_skill.gameObject.SetActive(true);
            //PanelManager.Instance.CloseAllSingle(trans_equip);
            //trans_equip.gameObject.SetActive(true);

            //if (p.curEquipItem != null)
            //{
            //    PanelManager.Instance.OpenSingle<WithCountItemView>(trans_equip, p.curEquipItem);
            //}
            if (p.allSkillData.equippedSkillIdList.Count > 1)
            {
                //SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1],
                //    p.allSkillData);
                //SingleSkillView view = PanelManager.Instance.OpenSingle<SingleSkillView>(trans_skill, singleSkillData, SkillViewType.Show);

            }
            else
            {

            }



            //btn_talentTest.gameObject.SetActive(false);
            ////btn_tianJiTalentTest.gameObject.SetActive(false);

            //if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(p)
            //    && StudentManager.Instance.FindStudentAtCangKuTarget(p) != StudentGoCangKuNeedType.EatPoJingDan)
            //{
            //    btn_breakThrough.gameObject.SetActive(true);

            //}
            //else
            //{
            //    btn_breakThrough.gameObject.SetActive(false);
            //}
 
        }
    }

    #region 红点
    /// <summary>
    /// 红点
    /// </summary>
    /// <param name="node"></param>
    public void OnRedPointShow()
    {
        //RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_Student_InfoBigStudentView, (int)(ulong)p.onlyId);
        //RedPointManager.Instance.SetRedPointUI(obj_redPointInfo, RedPointType.MainPanel_Btn_Student_InfoBigStudentViewInfo, (int)(ulong)p.onlyId);

    }
    void OnRefreshRedPoint(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (p == theP)
            OnRedPointShow();
    }
    #endregion
    public void OnChoose(bool choose)
    {
        if (choose)
        {
            //trans_btns.gameObject.SetActive(true);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, bigHeight);

            if (TaskManager.Instance.guide_studentTuPo)
            {
                //找可以突破的弟子来点击

                if (p.talent == (int)StudentTalent.LianGong)
                {
                    if (StudentManager.Instance.CheckIfLianGongStudentCanBreakThrough(p))
                    {
                        //修武按钮
                        PanelManager.Instance.ShowTaskGuidePanel(btn_info.gameObject);
                    }

                }
                else if (p.talent != (int)StudentTalent.None)
                {
                    if (StudentManager.Instance.CheckIfProductStudentCanBreakThrough(p))
                    {
                        //生产按钮
                        PanelManager.Instance.ShowTaskGuidePanel(btn_info.gameObject);
                    }
                }

            }
            else if (TaskManager.Instance.guide_studentJueXing)
            {
                //找可以觉醒的弟子来点击
                if (p.talent == (int)StudentTalent.None)
                {
                    StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];
                    int expLimit = setting.NeedExp.ToInt32();
                    if (p.studentCurExp >= expLimit)
                    {
                        PanelManager.Instance.ShowTaskGuidePanel(btn_info.gameObject);
                    }
                }

            }

        }
        else
        {
            //trans_btns.gameObject.SetActive(false);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, smallHeight);
        }
    }
    public override void Clear()
    {
        //base.Clear();
        if (trans_proGrid != null)
            PanelManager.Instance.CloseAllSingle(trans_proGrid);
        //trans_btns.gameObject.SetActive(false);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, smallHeight);
        //PanelManager.Instance.CloseAllSingle(trans_skill);
       // PanelManager.Instance.CloseAllSingle(trans_equip);
        //PanelManager.Instance.CloseAllSingle(trans_consume);
        PanelManager.Instance.CloseAllSingle(trans_tuPoEffectParent);
        startXi = false;
    }
}
