using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;


public class SingleStudentView : SingleViewBase
{
    public Button btn;
    public Image img_bg;
    public Image img_bgk;
    public Image img_yuanSu;//元素
    PeopleData theP;
    public PeopleData peopleData
    {
        get { 
            return theP; }
        set { 
            theP = value; }
    }
    public Text txt_name;
    public Text txt_lv;
    //public Transform trans_hpBar;
    //public Image img_hpBar;
    //public Text txt_hp;

    //public Text txt_energy;//精力（派遣
    //public Transform trans_tag;//派遣中
    public bool showTips;//显示tips
    
    public Portrait portrait;
    public Image img_icon;
    [Header("obj_nameBg")]
    public GameObject obj_nameBg;
    //public Transform trans_process;//弟子的进度
    //public Image img_processBar;//进度
    public FarmPrepareStudentView farmView;

    //[SerializeField]
    //public List<TalentSpritePair> peopletouxiangList = new List<TalentSpritePair>();
    //public static Dictionary<StudentTalent, Sprite> peopletouxiang;
    public override void Init(params object[] args)
    {
        base.Init(args);
        peopleData = args[0] as PeopleData;
        if (args.Length > 1)
        {
            farmView = args[1] as FarmPrepareStudentView;
        }
        else
        {
            farmView = null;
        }
        RegisterEvent(TheEventType.StudentStatusChange, OnStatusChange);
        RegisterEvent(TheEventType.RefreshPeopleShow, Refresh);
        RegisterEvent(TheEventType.ChangeStudentName, ShowName);
        RegisterEvent(TheEventType.ZhengRong, OnChangeFace);
        //RegisterEvent(TheEventType.QuanLiChanDanProcessing, ShowDanFarmProcess);
        //RegisterEvent(TheEventType.StudentRelaxProcess, StudentRelaxProcess);

        //addBtnListener(btn, () =>
        //{
        //    parentPanel.OnChoosedStudent(this);
        //});
        //img_bgk.gameObject.SetActive(false);
        if (img_bgk != null) {
            if (peopleData.isPlayer)
            {
                img_bgk.gameObject.SetActive(false);
            }
            else {
                if (!img_bgk.gameObject.activeInHierarchy) { img_bgk.gameObject.SetActive(true); }
            }
        }
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
        ShowPortrait();
    }

    public void ShowPortrait()
    {
        //    if (peopleData.portraitType == (int)PortraitType.ChangeFace
        //&& peopleData.portraitIndexList.Count > 0)
        //    {
        //        //掌门有头像
        //        if (peopleData.isPlayer
        //            && !string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang)
        //            && !string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0]))
        //        {
        //            //portrait.gameObject.SetActive(false);
        //            img_icon.gameObject.SetActive(true);
        //            string touXiangPath = RoleManager.Instance.TouXiangPath(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0].ToInt32());
        //            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + touXiangPath);
        //        }
        //        else
        //        {
        //            portrait.gameObject.SetActive(true);
        //            img_icon.gameObject.SetActive(false);
        //            portrait.Refresh(peopleData);
        //            img_icon.gameObject.SetActive(true);
        //        }

        //    }
        //    else
        //    {
        //        portrait.gameObject.SetActive(false);
        //        img_icon.gameObject.SetActive(true);

        //        //如果有enemysetting 则用enemysetting 的icon 如果没有 则用自己的icon 如果都没有 则用默认icon
        //        EnemySetting enemySetting = DataTable.FindEnemySetting(peopleData.enemySettingId);
        //        if (enemySetting != null)
        //        {
        //            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + enemySetting.SpecialPortrait);
        //        }
        //        else if (!string.IsNullOrWhiteSpace(peopleData.specialPortraitName))
        //        {
        //            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + peopleData.specialPortraitName);

        //        }
        //        else
        //        {
        //            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + ConstantVal.defaultPortraitName);

        //        }
        //    }
        StudentManager.Instance.SetTouxiang(img_icon, peopleData);
    }
    public virtual void RefreshShow()
    {
        string theName = this.name;
        ShowStudentKuang();
        ShowName();
        if(txt_lv!=null)
            ShowLv();
        //txt_name.color = CommonUtil.RarityColor((Rarity)(int)peopleData.studentRarity);
        //txt_name.ShowStudentTxtColor(PeopleData);

       // txt_energy.gameObject.SetActive(false);

        //trans_tag.gameObject.SetActive(false);
        //trans_process.gameObject.SetActive(false);
        //trans_hpBar.gameObject.SetActive(false);

        //if (peopleData.talent == (int)StudentTalent.LianGong
        //    || peopleData.studentType==(int)StudentType.Enemy
        //    || peopleData.isMyTeam)
        //{
        //    trans_hpBar.gameObject.SetActive(false);
        //    //SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
        //    //txt_hp.SetText(pro.num + "/" + pro.limit);
        //    //img_hpBar.fillAmount = pro.num / (float)pro.limit;

        //    //txt_energy.gameObject.SetActive(true);
        //    //txt_energy.SetText("精力：" + PeopleData.StudentCurEnergy + "%");

       
       
        //}
        ////炼丹师
        //else if(peopleData.talent == (int)StudentTalent.LianDan)
        //{

        //    trans_hpBar.gameObject.SetActive(false);
        //   // txt_energy.gameObject.SetActive(true);
        //    //txt_energy.SetText("精力：" + PeopleData.StudentCurEnergy + "%");

        //}
        //else
        //{

        //    trans_hpBar.gameObject.SetActive(false);
        //    //txt_energy.gameObject.SetActive(false);
        //   // txt_energy.SetText("精力：" + PeopleData.StudentCurEnergy + "%");

        //}

    }

    public void OnChangeFace(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        if (p == peopleData)
        {
            ShowPortrait();
        }
    }
    /// <summary>
    /// 显示等级
    /// </summary>
    public virtual void ShowLv()
    {
        if (farmView != null)
            txt_lv.gameObject.SetActive(false);
        else
        {
            txt_lv.gameObject.SetActive(true);
            if (peopleData.talent == (int)StudentTalent.LianGong)
            {

                 txt_lv.SetText((peopleData.trainIndex + 1) + "级" );
            }
            else
            {
                txt_lv.SetText("Lv." + peopleData.studentLevel);



            }
        }
    
    }

    public virtual void ShowStudentKuang()
    {
        if (img_bg != null)
        {
            img_bg.sprite = CommonUtil.StudentKuang(peopleData);
        }
        if (img_bgk != null)
        {
            if (peopleData.isPlayer)
            {
                img_bgk.gameObject.SetActive(false);
            }
            else
            {
                img_bgk.sprite = CommonUtil.StudentBgKuang(peopleData);
            }
        }
        if (peopleData.talent == (int)StudentTalent.LianGong)
        {
            if (img_yuanSu != null)
            {
                img_yuanSu.gameObject.SetActive(true);
                img_yuanSu.sprite = ConstantVal.YuanSuIcon((YuanSuType)peopleData.yuanSu);
            }
        }
        else
        {
            if (img_yuanSu != null)
            {
                img_yuanSu.gameObject.SetActive(false);
            }
        }

    }
    public void ShowName()
    {
        if(txt_name!=null)
            txt_name.SetText(peopleData.name);
        if (obj_nameBg != null)
            obj_nameBg.gameObject.SetActive(true);
    }


    ///// <summary>
    ///// 显示丹田进度
    ///// </summary>
    //public void ShowDanFarmProcess(object[] args)
    //{
    //    ulong onlyId = (ulong)args[0];
    //    float process = (float)args[1];

    //    if (!trans_process.gameObject.activeInHierarchy)
    //    {
    //        trans_process.gameObject.SetActive(true);
    //    }
    //    img_processBar.fillAmount = process;
    //}

    /// <summary>
    /// 刷新显示
    /// </summary>
    public void Refresh()
    {
    
       
        RefreshShow();
    }
    /// <summary>
    /// 状态改变
    /// </summary>
    public void OnStatusChange(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        if (p.onlyId == peopleData.onlyId)
        {
            peopleData = p;
            //RefreshShow();
        }
 
    }

    public override void Clear()
    {
        base.Clear();
        showTips = false;
    }



}
