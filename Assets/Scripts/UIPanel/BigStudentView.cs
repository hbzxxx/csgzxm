using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 大的弟子展示界面
/// </summary>
public class BigStudentView : SingleViewBase
{
    public PeopleData p;

    public Transform trans_proGrid;//属性面板

    public Transform trans_studentViewParent;

    public SingleStudentView singleStudentView;
    public Text txt_rarity;
    public Transform trans_rarity;//天赋稀有度
    public Text txt_talent;//天赋
    public Text txt_lv;//等级
    public Text txt_xingGe;//性格
    public Text txt_curWork;
    public Text txt_zhanDouLi;//战斗力
    public Image img_rarityBg;//稀有度背景
    public Text txt_yanZhi;//颜值

    //public Image img_touxiang;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        RegisterEvent(TheEventType.StopZuoZhenStudent, OnStopZuoZhenStudent);
        RegisterEvent(TheEventType.StudentPrepareTeam, OnPrepareTeam);
        RegisterEvent(TheEventType.StudentPrepareExplore, OnPrepareExplore);
        RegisterEvent(TheEventType.StudentBreakthroughSuccess, OnBreakThrough);
        RegisterEvent(TheEventType.FailStudentBreakThrough, OnBreakThrough);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        PanelManager.Instance.CloseAllSingle(trans_studentViewParent);

        ShowStudentView();
        string talentTxt = "";

        //设置头像
        //StudentManager.Instance.SetTouxiang(img_touxiang, p);

        talentTxt=StudentManager.Instance.TalentNameByTalent((StudentTalent)(int)p.talent);

        if (p.talent == (int)StudentTalent.LianGong)
        {
            string yuanSuName = ConstantVal.YuanSuName((YuanSuType)p.yuanSu);
            talentTxt += yuanSuName;
        }

        if (txt_talent != null)
            txt_talent.SetText(talentTxt);

        if (p.talent ==(int)StudentTalent.None)
        {
            if(trans_rarity!=null)
                trans_rarity.gameObject.SetActive(false);
        }
        else
        {
            if (trans_rarity != null)
                trans_rarity.gameObject.SetActive(true);
            if(img_rarityBg!=null)
                img_rarityBg.ShowStudentRarityBgImg(p.talentRarity);

            if (txt_rarity != null)
            {
                txt_rarity.SetText(ConstantVal.QualityName(p.talentRarity) + "级");
                txt_rarity.color = CommonUtil.RarityBlackColor((Rarity)(int)p.talentRarity);
                txt_talent.color = Color.white;
            }
        }

        ShowCurWork();
        ShowLV();
       
    }
    public virtual void ShowStudentView()
    {
        singleStudentView = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_studentViewParent, p);
    }
 

    public virtual void ShowLV()
    {
        if(trans_proGrid!=null)
            PanelManager.Instance.CloseAllSingle(trans_proGrid);
        int levelLimit = StudentManager.Instance.GetStudentLevelLimit(p);

        if (p.talent == (int)StudentTalent.LianGong)
        {
            if (txt_zhanDouLi != null)
            {
                txt_zhanDouLi.SetText("战斗力：" + RoleManager.Instance.CalcZhanDouLi(p).ToString());
                txt_zhanDouLi.gameObject.SetActive(true);
            }

            //显示战斗属性
            //for (int i = 0; i < p.curBattleProList.Count; i++)
            //{
            //    SinglePropertyData pro = p.curBattleProList[i];
            //    if (pro.id != (int)PropertyIdType.MpNum)
            //        PanelManager.Instance.OpenSingle<SinglePropertyView>(trans_proGrid, pro.id, pro.num, (Quality)(int)pro.quality);
            //}
            if (txt_lv != null)
            {
                txt_lv.gameObject.SetActive(true);
                TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                TrainSetting limitSetting = DataTable._trainList[levelLimit];
                txt_lv.SetText((p.trainIndex+1).ToString());
            }
      
        }
        else
        {
            SinglePropertyData pro = null;

            if (txt_lv != null)
            {
                if (p.talent == (int)StudentTalent.None)
                {
                    txt_lv.gameObject.SetActive(false);
                }
                else
                {
                    txt_lv.gameObject.SetActive(true);
                    txt_lv.SetText("Lv." + p.studentLevel + "/" + levelLimit);
                    pro = p.propertyList[0];
                }
            }

            //if (p.talent != (int)StudentTalent.LianGong)
            //{

            //}
            //for (int i = 0; i < p.propertyList.Count; i++)
            //{
            //    SinglePropertyData pro = p.propertyList[i];
            //    if (pro.id != (int)PropertyIdType.MpNum)
            //        PanelManager.Instance.OpenSingle<BigStudentSinglePropertyView>(trans_proGrid, p.propertyList[i].id, p.propertyList[i].num, (Quality)(int)p.propertyList[i].quality);
            //}
            //
            if (txt_zhanDouLi != null)
            {
                if (p.talent != (int)StudentTalent.None)
                {
                    pro = p.propertyList[0];
                    PropertySetting propertySetting = DataTable.FindPropertySetting(pro.id);
                    txt_zhanDouLi.SetText(propertySetting.Name + ":" + pro.num);
                }
                else
                {
                    txt_zhanDouLi.SetText("未觉醒");

                }
                txt_zhanDouLi.gameObject.SetActive(true);
            }
        }
    }
    public void OnBreakThrough(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {
            ShowLV();
        }
    }
    public void ShowCurWork()
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

        //student.StudentStatusType = ;
        //student.ZuoZhenDanFarmIndex = curChoosedDanFarmData.Index;

    }


    /// <summary>
    /// 停止坐镇弟子
    /// </summary>
    public void OnStopZuoZhenStudent(object[] args)
    {
        ShowCurWork();
    }
    /// <summary>
    /// 准备上阵
    /// </summary>
    /// <param name="args"></param>
    public void OnPrepareTeam(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if(theP.onlyId== p.onlyId)
        {
            ShowCurWork();
        }
    }

    /// <summary>
    /// 准备探索
    /// </summary>
    /// <param name="args"></param>
    public void OnPrepareExplore(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == p.onlyId)
        {
            ShowCurWork();
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_studentViewParent);
        if (trans_proGrid!=null)
            PanelManager.Instance.CloseAllSingle(trans_proGrid);
    }

}
